namespace Bank.Shared.Domain.Entities {
	using Ardalis.GuardClauses;
	using Bank.Shared.Domain.ValueObjects;
	using Bank.Shared.Events;

	public class Account :
		IAggregateRoot, IEntity<Guid> {

		// Do the methods for use-cases take in the Command objects directly or do the Command Handlers flatten the
		// objects to individual arguments?? (i.e. should this static method be CreateAsync(CreateAccount account) )
		public static async Task<Account> CreateAsync(Guid id, string name) {
			// Do I do validation here since I am calling the ctor which has it???

			var account = new Account(id, name);

			await Bus.RaiseEventAsync<AccountCreated>(new AccountCreated(account));

			return account;
		}

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
		}

		public async Task SetOverdraftLimit(Money limit) {
			// Need to check if we can change limit if the current balance will violate the limit. In other words, is the previous limit honored.
			// Should be create custom guard to cover Money value object ???
			// I also wonder if Negative check should throw ArgumentOutOfRangeException as opposed to ArgumentException

			Guard.Against.Negative(limit.Amount, nameof(limit));

			OverdraftLimit = limit;

			await Bus.RaiseEventAsync<AccountOverdraftLimitChanged>(new AccountOverdraftLimitChanged(this));
		}

		public async Task SetDailyWireTransferLimit(Money limit) {
			if (limit.Amount < 0) {
				throw new ArgumentOutOfRangeException(nameof(limit.Amount), "Value must be equal to or greater than 0.");
			}

			DailyWireTransferLimit = limit;

			await Bus.RaiseEventAsync<AccountDailyWireTransferLimitChanged>(new AccountDailyWireTransferLimitChanged(this));
		}

		public Guid Id { get; protected set; }

		public string CustomerName { get; protected set; }

		public Money OverdraftLimit { get; protected set; }

		public Money DailyWireTransferLimit { get; protected set; }

	}

}
