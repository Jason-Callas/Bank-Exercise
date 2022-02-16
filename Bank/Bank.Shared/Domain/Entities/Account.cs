namespace Bank.Shared.Domain.Entities {

	using Ardalis.GuardClauses;
	using Bank.Shared.Domain.ValueObjects;
	using Bank.Shared.Events;
	using Bank.Shared.Exceptions;
	using NodaTime;

	public class Account :
		AggregateBase<Guid>, IAggregateRoot{

		public Account(Guid id, string name, string currency) : base() {
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

		// Really, really hate the idea of exposing this ctor simply for purposes of unit testing or _maybe_ reconstituting entuty
		// from store. There HAS to be a "cleaner" way...
		public Account(IEnumerable<IEvent<Guid>> events) : base(events) {
		}

		internal void Apply(AccountCreated @event) {
			Id = @event.AggregateId;
			CustomerName = @event.CustomerName;
			Currency = @event.Currency;

			OverdraftLimit = new Money(0m, Currency);
			DailyWireTransferLimit = new Money(0m, Currency);
		}

		internal void Apply(AccountOverdraftLimitChanged @event) {
			ValidateAggregateEventOrThrow(@event);

			OverdraftLimit = @event.Limit;
		}

		internal void Apply(AccountDailyWireTransferLimitChanged @event) {
			ValidateAggregateEventOrThrow(@event);

			DailyWireTransferLimit = @event.Limit;
		}

		internal void Apply(AccountCashDeposited @event) {
			ValidateAggregateEventOrThrow(@event);

			Transactions.Add(new DepositCashAccountTransaction(@event.Amount.Amount));
		}

		internal void Apply(AccountCheckDeposited @event) {
			ValidateAggregateEventOrThrow(@event);

			Transactions.Add(new DepositCheckAccountTransaction(@event.Amount.Amount, @event.DepositedOn));
		}

		internal void Apply(AccountCashWithdrawn @event) {
			ValidateAggregateEventOrThrow(@event);

			Transactions.Add(new WithdrawCashAccountTransaction(@event.Amount.Amount, true));
		}

		internal void Apply(AccountCashTransferred @event) {
			ValidateAggregateEventOrThrow(@event);

			Transactions.Add(new TransferCashAccountTransaction(@event.Amount.Amount, @event.TransferredOn, true));
		}

		internal void Apply(AccountCashWithdrawalRejected @event) {
			ValidateAggregateEventOrThrow(@event);

			Transactions.Add(new WithdrawCashAccountTransaction(@event.Amount.Amount, false));
		}

		internal void Apply(AccountCashTransferRejected @event) {
			ValidateAggregateEventOrThrow(@event);

			// Should a failed transfer _look_ like a withdrawal?? Maybe the term "withdraw" should be changed to "debit"
			Transactions.Add(new WithdrawCashAccountTransaction(@event.Amount.Amount, false));
		}

		private void ValidateCurrencyOrThrow(Money value, string actionLabel) {
			if (!string.Equals(value.Currency, Currency, StringComparison.OrdinalIgnoreCase)) {
				throw new InvalidCurrencyException($"Unable to accept {actionLabel} due to currency mismatch. Account is configured for '{Currency}' but new value is '{value.Currency}'.");
			}
		}

		private void ValidateAggregateEventOrThrow(IEvent<Guid> @event) {
			if (@event.AggregateId != Id) {
				throw new InvalidAccountException($"Cannot process '{@event.GetType().Name}' event. Associated aggregate id '{@event.AggregateId}' does not match entity id '{Id}'.");
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

			foreach (var trans in Transactions.ToArray()) {
				// If we are currently blocked then perform some checks to see if we should unblock
				if (lastBlockingTransaction is not null && trans.IsSuccessful) {
					if (
						trans is DepositCashAccountTransaction ||
						trans is DepositCheckAccountTransaction && ((DepositCheckAccountTransaction)trans).HasCleared
						) {
						lastBlockingTransaction = null;
					}
				}

				// Purposely leaving the successful check separate from the one or many type checks
				if (!trans.IsSuccessful) {
					lastBlockingTransaction = trans;
				}
			}

			return (lastBlockingTransaction is not null);
		}

		private decimal GetTransferTotalOnDate(LocalDate? date = null) {
			if (Transactions is null) {
				return 0m;
			}

			if (!date.HasValue) {
				// Heavens forbid NodaTime offered something as convenient as DateTime.UtcNow.Date....

				date = SystemClock.Instance.GetCurrentInstant().InUtc().Date;
			}

			return Transactions
				.OfType<TransferCashAccountTransaction>()
				.Where(t => t.TransferredOn == date.Value && t.IsSuccessful)
				.Select(t => t.ApplicableAmount)
				.DefaultIfEmpty(0m)
				.Sum();
		}

		private decimal GetTransferTotalToday() {
			return GetTransferTotalOnDate(SystemClock.Instance.GetCurrentInstant().InUtc().Date);
		}

		private decimal GetAvailableBalance() {
			var totalCredits = Transactions
				.Where(t => t is DepositCashAccountTransaction || t is DepositCheckAccountTransaction)
				.Where(t => t.IsSuccessful)
				.Select(t => t.ApplicableAmount)
				.Sum();

			var totalDebits = Transactions
				.Where(t => t is WithdrawCashAccountTransaction || t is TransferCashAccountTransaction)
				.Where(t => t.IsSuccessful)
				.Select(t => t.ApplicableAmount)
				.Sum();

			return totalCredits - totalDebits;
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

		private DebitApproval IsDebitViaTransferAllowed(decimal amount) {
			var result = IsDebitAllowed(amount);

			// No need to do additional checks if already not allowed
			if (result != DebitApproval.Approved) {
				return result;
			}

			if (DailyWireTransferLimit.Amount < GetTransferTotalToday() + amount) {
				return DebitApproval.DailyTransferExceeded;
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

			RaiseEvent(new AccountCheckDeposited(Id, amount, SystemClock.Instance.GetCurrentInstant()));
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

		public void TransferCash(Money amount) {
			Guard.Against.Null(amount, nameof(amount));

			Guard.Against.Negative(amount.Amount, nameof(amount));
			Guard.Against.Null(amount.Currency, nameof(amount.Currency));

			ValidateCurrencyOrThrow(amount, "Transfer Cash");

			var approval = IsDebitViaTransferAllowed(amount.Amount);

			if (approval == DebitApproval.Approved) {
				var today = SystemClock.Instance.GetCurrentInstant().InUtc().Date;

				// ** Let transfer happen
				RaiseEvent(new AccountCashTransferred(Id, amount, today));
			}
			else {
				// ** Do NOT let transfer happen

				var reason = default(string);

				switch (approval) {
					case DebitApproval.AccountBlocked:
						reason = "Account is currently blocked from any debits.";
						break;
					case DebitApproval.InsufficientFunds:
						reason = "Account does not have sufficient funds.";
						break;
					case DebitApproval.OverdraftExceeded:
						reason = "Account does not have sufficient funds and debit exceeded overdraft limit.";
						break;
					case DebitApproval.DailyTransferExceeded:
						reason = "Cannot transfer funds in amounts that total greater than daily limit.";
						break;
				}

				RaiseEvent(new AccountCashTransferRejected(Id, amount, reason));
			}
		}

		private string CustomerName { get; set; }

		private string Currency { get; set; }

		private Money OverdraftLimit { get; set; }

		private Money DailyWireTransferLimit { get; set; }

		private ICollection<AccountTransaction> Transactions { get; set; } = new List<AccountTransaction>();

	}

}
