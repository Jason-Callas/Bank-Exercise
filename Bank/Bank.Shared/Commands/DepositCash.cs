namespace Bank.Shared.Commands {

	public class DepositCash :
		ICommand {

		public DepositCash(Guid accountId, decimal amount, string currency) {
			AccountId = accountId;
			Amount = amount;
			Currency = currency;
		}

		public Guid AccountId { get; protected set; }

		public decimal Amount { get; protected set; }

		public string Currency { get; protected set; }

	}

}
