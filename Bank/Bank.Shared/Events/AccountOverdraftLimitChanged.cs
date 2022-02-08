namespace Bank.Shared.Events {

	using Bank.Shared.Domain.Entities;

	public class AccountOverdraftLimitChanged :
		AccountBaseEvent {

		public AccountOverdraftLimitChanged(Account account, DateTime? timestamp = null) : base(account, timestamp) {
		}

	}

}
