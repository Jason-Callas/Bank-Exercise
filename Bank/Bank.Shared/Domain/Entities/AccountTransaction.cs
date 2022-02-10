namespace Bank.Shared.Domain.Entities {

	internal class AccountTransaction {

		public AccountTransaction(decimal amount, DateTime timestamp, bool wasTransferred = false) {
			Amount = amount;
			Timestamp = timestamp;
			WasTransferred = wasTransferred;
		}

		// Since the transaction is created as a result of an event at the account level,
		// and is immutable, I do NOT think we actually need an Id

		public decimal Amount { get; }

		public DateTime Timestamp { get; }

		public bool WasTransferred { get; }

	}

}
