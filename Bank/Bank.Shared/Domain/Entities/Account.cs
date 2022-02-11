namespace Bank.Shared.Domain.Entities {

	using Ardalis.GuardClauses;
	using Bank.Shared.Domain.ValueObjects;
	using Bank.Shared.Events;
	using Bank.Shared.Exceptions;

	public class Account :
		AggregateBase<Guid>, IAggregateRoot{

		public Account(Guid id, string name, string currency) {
			// Need to research this more. Some "guard" libraries support checking string length but others (this one for example, put length check in
			// the realm of validation which _seems_ to NOT be the same as "guard" checks...

			// At this time, I am unsure if I should extend it (as mentioned https://github.com/ardalis/GuardClauses/issues/69), or use a different library,
			// or leave the explicit "validation" check.

			Guard.Against.NullOrWhiteSpace(name, nameof(name));

			if (name.Length > 30) {
				throw new ArgumentException(nameof(name), "The length of the value must be 30 characters or less.");
			}

			Guard.Against.NullOrWhiteSpace(currency, nameof(currency));

			if (currency.Length != 3) {
				throw new ArgumentException(nameof(currency), "The length of the value must be 3 characters.");
			}

			RaiseEvent(new AccountCreated(id, name, currency));
		}

		internal void Apply(AccountCreated @event) {
			Id = @event.AggregateId;
			CustomerName = @event.CustomerName;
			Currency = @event.Currency;

			OverdraftLimit = new Money(0m, Currency);
			DailyWireTransferLimit = new Money(0m, Currency);
		}

		internal void Apply(AccountOverdraftLimitChanged @event) {
			OverdraftLimit = @event.Limit;
		}

		internal void Apply(AccountDailyWireTransferLimitChanged @event) {
			DailyWireTransferLimit = @event.Limit;
		}

		internal void Apply(AccountCashDeposited @event) {
			Transactions.Add(new DepositCashAccountTransaction(@event.Amount.Amount, @event.TimestampUtc));
		}

		internal void Apply(AccountCheckDeposited @event) {
			Transactions.Add(new DepositCheckAccountTransaction(@event.Amount.Amount, @event.TimestampUtc));
		}

		internal void Apply(AccountCashWithdrawn @event) {
			Transactions.Add(new WithdrawCashAccountTransaction(@event.Amount.Amount, @event.TimestampUtc, true));
		}

		internal void Apply(AccountCashWithdrawalRejected @event) {
			Transactions.Add(new WithdrawCashAccountTransaction(@event.Amount.Amount, @event.TimestampUtc, false));
		}

		private void ValidateCurrencyOrThrow(Money value, string actionLabel) {
			if (!string.Equals(value.Currency, Currency, StringComparison.OrdinalIgnoreCase)) {
				throw new InvalidCurrencyException($"Unable to accept {actionLabel} due to currency mismatch. Account is configured for '{Currency}' but new value is '{value.Currency}'.");
			}
		}

		private bool IsAccountBlocked() {
			// ** The logic for this is as follows
			// **   1. Start with an unblocked account
			// **   2. Cycle through transactions and toggle flag as needed
			// **       a. Rejected debits will set the account as blocked
			// **       b. Cash credits on blocked account will immediately unblock account
			// **       c. Check credits will unblock a blocked account if the funds are available after the blocking

			var lastBlockingTransaction = default(AccountTransaction);

			// Transaction order DOES matter

			foreach (var trans in Transactions.OrderBy(t => t.TimestampUtc).ToArray()) {
				if (trans is DepositCashAccountTransaction && lastBlockingTransaction is not null) {
					lastBlockingTransaction = null;
				}

				// Purposely leaving the successful check separate from the one or many type checks
				if (!trans.IsSuccessful) {
					if (trans is WithdrawCashAccountTransaction) {
						lastBlockingTransaction = trans;
					}
				}
			}

			return (lastBlockingTransaction is not null);
		}

		private decimal GetAvailableBalance() {
			// Transaction order does NOT matter
			return Transactions.Where(t => t.IsSuccessful).Select(t => t.ApplicableAmount).Sum();
		}

		private DebitApproval IsDebitAllowed(decimal amount) {
			// ** Need to check/calculate if Blocked

			var availableBalance = GetAvailableBalance();
			if (availableBalance < amount) {
				if (OverdraftLimit.Amount == 0m) {
					return DebitApproval.InsufficientFunds;
				}

				var availableFunds = availableBalance + OverdraftLimit.Amount;
				if (availableFunds < amount) {
					return DebitApproval.OverdraftExceeded;
				}
			}

			return DebitApproval.Approved;
		}

		public void SetOverdraftLimit(Money limit) {
			Guard.Against.Null(limit, nameof(limit));

			// Should we create custom guard to cover Money value object ???
			// I also wonder if Negative check should throw ArgumentOutOfRangeException as opposed to ArgumentException

			Guard.Against.Negative(limit.Amount, nameof(limit));
			Guard.Against.Null(limit.Currency, nameof(limit.Currency));

			ValidateCurrencyOrThrow(limit, "Overdraft Limit");

			RaiseEvent(new AccountOverdraftLimitChanged(Id, limit));
		}

		public void SetDailyWireTransferLimit(Money limit) {
			Guard.Against.Null(limit, nameof(limit));

			Guard.Against.Negative(limit.Amount, nameof(limit));
			Guard.Against.Null(limit.Currency, nameof(limit.Currency));

			ValidateCurrencyOrThrow(limit, "Daily Wire Transfer Limit");

			RaiseEvent(new AccountDailyWireTransferLimitChanged(Id, limit));
		}

		public void DepositCash(Money amount) {
			Guard.Against.Null(amount, nameof(amount));

			Guard.Against.Negative(amount.Amount, nameof(amount));
			Guard.Against.Null(amount.Currency, nameof(amount.Currency));

			ValidateCurrencyOrThrow(amount, "Cash Deposit");

			RaiseEvent(new AccountCashDeposited(Id, amount));
		}

		public void DepositCheck(Money amount) {
			Guard.Against.Null(amount, nameof(amount));

			Guard.Against.Negative(amount.Amount, nameof(amount));
			Guard.Against.Null(amount.Currency, nameof(amount.Currency));

			ValidateCurrencyOrThrow(amount, "Check Deposit");

			RaiseEvent(new AccountCheckDeposited(Id, amount));
		}

		public void WithdrawCash(Money amount) {
			Guard.Against.Null(amount, nameof(amount));

			Guard.Against.Negative(amount.Amount, nameof(amount));
			Guard.Against.Null(amount.Currency, nameof(amount.Currency));

			ValidateCurrencyOrThrow(amount, "Cash Withdrawl");

			var approval = IsDebitAllowed(amount.Amount);

			if (approval == DebitApproval.Approved) {
				// ** Let withdrawal happen
				RaiseEvent(new AccountCashWithdrawn(Id, amount));
			}
			else {
				// ** Do NOT let withdrawal happen

				var reason = default(string);

				switch (approval) {
					case DebitApproval.AccountBlocked:
						reason = "Account is currently blocked from any withdrawals.";
						break;
					case DebitApproval.InsufficientFunds:
						reason = "Account does not have sufficient funds.";
						break;
					case DebitApproval.OverdraftExceeded:
						reason = "Account does not have sufficient funds and withdrawal exceeded overdraft limit.";
						break;
				}

				RaiseEvent(new AccountCashWithdrawalRejected(Id, amount, reason));

				// ** An argument can be made that there should **also* be an event to "block" the account. However, that _could_
				// ** imply that there _then_ needs to be an event that "unblocks" it. While that is easy to do if a **cash** deposit
				// ** is made, how does that get handled when a deposited **check** becomes available the next day.
				// **
				// ** One solution would be a timer that performs checks but that is not feasible. Another possibility would
				// ** would be to add another check
			}
		}

		private string CustomerName { get; set; }

		private string Currency { get; set; }

		private Money OverdraftLimit { get; set; }

		private Money DailyWireTransferLimit { get; set; }

		private ICollection<AccountTransaction> Transactions { get; set; } = new List<AccountTransaction>();

	}

}
