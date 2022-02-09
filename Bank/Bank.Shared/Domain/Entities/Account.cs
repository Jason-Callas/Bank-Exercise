﻿namespace Bank.Shared.Domain.Entities {
	using Ardalis.GuardClauses;
	using Bank.Shared.Domain.ValueObjects;
	using Bank.Shared.Events;

	public class Account :
		AggregateBase<Guid>, IAggregateRoot{

		public Account(Guid id, string name) {
			// Need to research this more. Some "guard" libraries support checking string length but others (this one for example, put length check in
			// the realm of validation which _seems_ to NOT be the same as "guard" checks...

			// At this time, I am unsure if I should extend it (as mentioned https://github.com/ardalis/GuardClauses/issues/69), or use a different library,
			// or leave the explicit "validation" check.

			Guard.Against.NullOrWhiteSpace(name, nameof(name));

			if (name.Length > 30) {
				throw new ArgumentOutOfRangeException(nameof(name), "The length of the value must be 30 characters or less.");
			}

			Id = id;
			CustomerName = name;

			// The USD currency value should probably be injected instead of hard-coded... maybe even use a constant (for now)
			OverdraftLimit = new Money(0m, "USD");
			DailyWireTransferLimit = new Money(0m, "USD");

			RaiseEvent(new AccountCreated(id, name));
		}

		internal void Apply(AccountCreated @event) {
			// Not sure I like this repeat of code from what is in ctor
			//   I _guess_ I could modify the ctor to create a faux event and pass it down to here....
			//
			// That approach (in the ctor and other methods) **would** address the issue of "where does the command get applied"
			// question that was already discussed...

			Id = @event.AggregateId;
			CustomerName = @event.CustomerName;
		}

		internal void Apply(AccountOverdraftLimitChanged @event) {
			OverdraftLimit = @event.Limit;
		}

		private void Apply(AccountDailyWireTransferLimitChanged evt) {
			DailyWireTransferLimit = evt.Limit;
		}

		public void SetOverdraftLimit(Money limit) {
			Guard.Against.Null(limit, nameof(limit));

			// Should we create custom guard to cover Money value object ???
			// I also wonder if Negative check should throw ArgumentOutOfRangeException as opposed to ArgumentException

			Guard.Against.Negative(limit.Amount, nameof(limit));
			Guard.Against.Null(limit.Currency, nameof(limit.Currency));

			// Need to check if we can change limit if the current balance will violate the limit. In other words, is the previous limit honored.

			OverdraftLimit = limit;

			RaiseEvent(new AccountOverdraftLimitChanged(Id, limit));
		}

		public void SetDailyWireTransferLimit(Money limit) {
			Guard.Against.Null(limit, nameof(limit));

			Guard.Against.Negative(limit.Amount, nameof(limit));
			Guard.Against.Null(limit.Currency, nameof(limit.Currency));

			DailyWireTransferLimit = limit;

			RaiseEvent(new AccountDailyWireTransferLimitChanged(Id, limit));
		}

		public string CustomerName { get; protected set; }

		public Money OverdraftLimit { get; protected set; }

		public Money DailyWireTransferLimit { get; protected set; }

	}

}
