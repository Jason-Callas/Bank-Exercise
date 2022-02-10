﻿namespace Bank.Shared.Domain.Entities {

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

		internal void Apply(AccountDailyWireTransferLimitChanged evt) {
			DailyWireTransferLimit = evt.Limit;
		}

		public void SetOverdraftLimit(Money limit) {
			Guard.Against.Null(limit, nameof(limit));

			// Should we create custom guard to cover Money value object ???
			// I also wonder if Negative check should throw ArgumentOutOfRangeException as opposed to ArgumentException

			Guard.Against.Negative(limit.Amount, nameof(limit));
			Guard.Against.Null(limit.Currency, nameof(limit.Currency));

			if (!string.Equals(limit.Currency, Currency, StringComparison.OrdinalIgnoreCase)) {
				throw new InvalidCurrencyException($"Unable to set Overdraft Limit due to currency mismatch. Account is configured for '{Currency}' but new limit value is '{limit.Currency}'.");
			}

			RaiseEvent(new AccountOverdraftLimitChanged(Id, limit));
		}

		public void SetDailyWireTransferLimit(Money limit) {
			Guard.Against.Null(limit, nameof(limit));

			Guard.Against.Negative(limit.Amount, nameof(limit));
			Guard.Against.Null(limit.Currency, nameof(limit.Currency));

			if (!string.Equals(limit.Currency, Currency, StringComparison.OrdinalIgnoreCase)) {
				throw new InvalidCurrencyException($"Unable to set Overdraft Limit due to currency mismatch. Account is configured for '{Currency}' but new limit value is '{limit.Currency}'.");
			}

			RaiseEvent(new AccountDailyWireTransferLimitChanged(Id, limit));
		}

		private string CustomerName { get; set; }

		private string Currency { get; set; }

		private Money OverdraftLimit { get; set; }

		private Money DailyWireTransferLimit { get; set; }

	}

}
