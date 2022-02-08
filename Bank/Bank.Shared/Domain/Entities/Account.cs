namespace Bank.Shared.Domain.Entities {
	using Ardalis.GuardClauses;
	using Bank.Shared.Domain.ValueObjects;
	using Bank.Shared.Events;

	public class Account :
		IAggregateRoot, IEntity<Guid> {

		public static Account? Load(IEnumerable<AccountBaseEvent> events) {
			Guard.Against.Null(events, nameof(events));

			if (!events.Any()) {
				return default;
			}

			// Do not really like this approach of calling a static method but I am in the position
			// of NOT having an instance of Account initially...with the ctor expecting an id and name
			// argument. But that data is within the AccountCreated event and I do not like the idea of
			// explicitly processing that here.
			//
			// This approach will most likely change once I read up more on using events as "source of truth".

			// This approach (using a static method) seems wrong but running with it as first attempt

			var account = new Account();

			foreach (var evt in events) {
				if (evt is AccountCreated) {
					account.Apply((AccountCreated)evt);
				}
				else if (evt is AccountOverdraftLimitChanged) {
					account.Apply((AccountOverdraftLimitChanged)evt);
				}
				else if (evt is AccountDailyWireTransferLimitChanged) {
					account.Apply((AccountDailyWireTransferLimitChanged)evt);
				}
				else {
					throw new NotImplementedException("Account domain event is not supported");
				}
			}

			return account;
		}

		// Do the methods for use-cases take in the Command objects directly or do the Command Handlers flatten the
		// objects to individual arguments?? (i.e. should this static method be CreateAsync(CreateAccount account) )
		public static async Task<Account> CreateAsync(Guid id, string name) {
			// Do I do validation here since I am calling the ctor which has it???

			var account = new Account(id, name);

			await Bus.RaiseEventAsync<AccountCreated>(new AccountCreated(account));

			return account;
		}

		protected static Account ReplayEvent(AccountBaseEvent evt, Account? account) {


			return account;
		}

		// Used for event replay
		private Account() {
			CustomerName = String.Empty;
			OverdraftLimit = new Money(0m, "USD");
			DailyWireTransferLimit = new Money(0m, "USD");
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

		private void Apply(AccountCreated evt) {
			Guard.Against.Null(evt);
			Guard.Against.Null(evt.Account, nameof(evt.Account));

			// Not sure I like this repeat of code from what is in ctor
			//   I _guess_ I could modify the ctor to create a faux event and pass it down to here....
			//
			// That approach (in the ctor and other methods) **would** address the issue of "where does the command get applied"
			// question that was already discussed...

			Id = evt.Account.Id;
			CustomerName = evt.Account.CustomerName;
		}

		private void Apply(AccountOverdraftLimitChanged evt) {
			Guard.Against.Null(evt);
			Guard.Against.Null(evt.Account, nameof(evt.Account));

			OverdraftLimit = evt.Account.OverdraftLimit;
		}

		private void Apply(AccountDailyWireTransferLimitChanged evt) {
			Guard.Against.Null(evt);
			Guard.Against.Null(evt.Account, nameof(evt.Account));

			DailyWireTransferLimit = evt.Account.DailyWireTransferLimit;
		}

		public async Task SetOverdraftLimit(Money limit) {
			Guard.Against.Null(limit, nameof(limit));

			// Should we create custom guard to cover Money value object ???
			// I also wonder if Negative check should throw ArgumentOutOfRangeException as opposed to ArgumentException

			Guard.Against.Negative(limit.Amount, nameof(limit));
			Guard.Against.Null(limit.Currency, nameof(limit.Currency));

			// Need to check if we can change limit if the current balance will violate the limit. In other words, is the previous limit honored.

			OverdraftLimit = limit;

			await Bus.RaiseEventAsync<AccountOverdraftLimitChanged>(new AccountOverdraftLimitChanged(this));
		}

		public async Task SetDailyWireTransferLimit(Money limit) {
			Guard.Against.Null(limit, nameof(limit));

			Guard.Against.Negative(limit.Amount, nameof(limit));
			Guard.Against.Null(limit.Currency, nameof(limit.Currency));

			DailyWireTransferLimit = limit;

			await Bus.RaiseEventAsync<AccountDailyWireTransferLimitChanged>(new AccountDailyWireTransferLimitChanged(this));
		}

		public Guid Id { get; protected set; }

		public string CustomerName { get; protected set; }

		public Money OverdraftLimit { get; protected set; }

		public Money DailyWireTransferLimit { get; protected set; }

	}

}
