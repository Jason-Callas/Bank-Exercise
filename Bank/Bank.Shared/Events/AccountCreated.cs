namespace Bank.Shared.Events {

	using Bank.Shared.Domain.Entities;

	public class AccountCreated :
		AccountBaseEvent {

		public AccountCreated(Account account, DateTime? timestamp = null) : base(account, timestamp) {
		}

	}

}
