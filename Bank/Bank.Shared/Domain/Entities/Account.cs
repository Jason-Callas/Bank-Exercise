namespace Bank.Shared.Domain.Entities {

	using Bank.Shared.Domain.ValueObjects;
	using Bank.Shared.Events;

	public class Account :
		IAggregateRoot, IEntity<Guid> {

		public static async Task<Account> CreateAsync(Guid id, string name) {
			// Do I do validation here ???

			var account = new Account(id, name);

			await Bus.RaiseEventAsync<AccountCreated>(new AccountCreated(account));

			return account;
		}

		public Account(Guid id, string name) {
			// Do I do validation here too or let each data type do its own validation with the ValueObject???

			Id = id;
			FullName = new FullName(name);
		}

		public Guid Id { get; protected set; }

		public FullName FullName { get; protected set; }

	}

}
