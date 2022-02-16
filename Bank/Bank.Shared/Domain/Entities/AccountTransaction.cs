namespace Bank.Shared.Domain.Entities {

	using NodaTime;

	internal abstract class AccountTransaction {

		public AccountTransaction(decimal amount, Instant when, bool isSuccessful = true) {
			Amount = amount;
			When = when;
			IsSuccessful = isSuccessful;
		}

		// Since the transaction is created as a result of an event at the account level,
		// and is immutable, I do not THINK we actually need an Id

		public decimal Amount { get; }

		public virtual decimal ApplicableAmount {
			get {
				return Amount;
			}
		}

		public Instant When { get; }

		public bool IsSuccessful { get; }

	}

}
