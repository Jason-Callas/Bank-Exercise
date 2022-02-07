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
			if (string.IsNullOrWhiteSpace(name)) {
				throw new ArgumentNullException(nameof(name));
			}

			if (name.Length > 30) {
				throw new ArgumentOutOfRangeException(nameof(name), "The length of the value must be 30 characters or less.");
			}

			Id = id;
			CustomerName = name;
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

		public string CustomerName { get; protected set; }

		public Money OverdraftLimit { get; protected set; }

		public Money DailyWireTransferLimit { get; protected set; }

	}

}
