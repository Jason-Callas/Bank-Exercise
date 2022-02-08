namespace Bank.Shared.Events {

	using Bank.Shared.Domain.Entities;

	public class AccountDailyWireTransferLimitChanged :
		AccountBaseEvent {

		public AccountDailyWireTransferLimitChanged(Account account, DateTime? timestamp = null) : base(account, timestamp) {
		}

	}

}
