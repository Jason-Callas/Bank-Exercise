namespace Bank.Shared.Domain.Entities {

	internal class AccountTransaction {

		public AccountTransaction(decimal amount, DateTime timestamp, bool wasTransferred = false, bool wasCheck = false) {
			Amount = amount;
			Timestamp = timestamp;
			WasTransferred = wasTransferred;
			WasCheck = wasCheck;
		}

		// Since the transaction is created as a result of an event at the account level,
		// and is immutable, I do NOT think we actually need an Id

		public decimal Amount { get; }

		public DateTime Timestamp { get; }

		// This is kinda horrible...really should not be using individual flags to indicate different use cases as
		// it forces us to have to check combinations. Better approach would be to add enum flag to indicate type of
		// transaction.

		public bool WasTransferred { get; }

		public bool WasCheck { get; }

	}

}
