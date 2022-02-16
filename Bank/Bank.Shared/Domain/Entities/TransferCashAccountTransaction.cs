namespace Bank.Shared.Domain.Entities {

	using NodaTime;

	internal class TransferCashAccountTransaction :
		DebitAccountTransaction {

		public TransferCashAccountTransaction(decimal amount, Instant when, bool isSuccessful) : base(amount, when, isSuccessful) {
		}

	}

}
