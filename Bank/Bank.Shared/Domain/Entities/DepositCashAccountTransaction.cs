namespace Bank.Shared.Domain.Entities {

	using NodaTime;

	internal class DepositCashAccountTransaction :
		CreditAccountTransaction {

		public DepositCashAccountTransaction(decimal amount, Instant when) : base(amount, when, isSuccessful: true) {
		}

	}

}
