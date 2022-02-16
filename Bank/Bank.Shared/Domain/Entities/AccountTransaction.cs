namespace Bank.Shared.Domain.Entities {

	internal abstract class AccountTransaction {

		public AccountTransaction(decimal amount, bool isSuccessful = true) {
			Amount = amount;
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

		public bool IsSuccessful { get; }

	}

}
