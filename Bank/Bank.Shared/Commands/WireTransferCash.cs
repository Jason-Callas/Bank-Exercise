namespace Bank.Shared.Commands {

	public class WireTransferCash :
		ICommand {

		public WireTransferCash(Guid accountId, decimal amount, string currency) {
			AccountId = accountId;
			Amount = amount;
			Currency = currency;
		}

		public Guid AccountId { get; protected set; }

		public decimal Amount { get; protected set; }

		public string Currency { get; protected set; }

	}

}
