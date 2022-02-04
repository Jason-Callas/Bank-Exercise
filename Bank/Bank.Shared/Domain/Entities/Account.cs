namespace Bank.Shared.Domain.Entities {

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
			// This is not really an issue in the ctor since any exception here will result in
			// a null object reference being returned to the caller (i.e. new Account(blah, blah) will fail)
			// depending on just the value objects doing validation can result in an invalid Entity.
			//
			// Consider the following scenario
			//
			//   * Request comes in to update two properties
			//   * Instance of Account is created from store or cache
			//   * Method is called that performs update where each Value Object does its own validation
			//            this.SomeProperty = new SomeValueObject(value1);          this works...
			//            this.SomeProperty2 = new SomeOtherValueObject(value2);    this throws exception...
			//     The entity instance is now potentially in an invalid state since SomeProperty was updated but SomeProperty2 was not.

			// So now I have to duplicate the validation rules...hope I got the length check correct... easy mistake to make...
			if (string.IsNullOrWhiteSpace(name)) {
				throw new ArgumentNullException(nameof(name));
			}

			if (name.Length > 30) {
				throw new ArgumentOutOfRangeException(nameof(name), "The length of the value must be 30 characters or less.");
			}

			Id = id;
			CustomerName = new FullName(name);
		}

		public async Task SetOverdraftLimit(Money limit) {
			if (limit.Amount < 0) {
				throw new ArgumentOutOfRangeException(nameof(limit.Amount), "Value must be equal to or greater than 0.");
			}

			// Need to check if we can change limit if the current balance will violate the limit. In other words, is the previous limit honored.

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

		public FullName CustomerName { get; protected set; }

		public Money OverdraftLimit { get; protected set; }

		public Money DailyWireTransferLimit { get; protected set; }

	}

}
