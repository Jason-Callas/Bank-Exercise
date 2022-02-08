namespace Bank.Shared.Events {

	using Ardalis.GuardClauses;
	using Bank.Shared.Domain.Entities;

	public abstract class AccountBaseEvent :
		BaseEvent<Account> {

		protected AccountBaseEvent(Account account, DateTime? timestamp = null) : base(timestamp) {
			Account = Guard.Against.Null(account);

			Account = account;
		}

		public Account Account { get; protected set; }

	}

}
