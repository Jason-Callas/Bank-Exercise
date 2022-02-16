namespace Bank.Shared.Domain.Entities {

	using NodaTime;

	internal class WithdrawCashAccountTransaction :
		DebitAccountTransaction {

		public WithdrawCashAccountTransaction(decimal amount, Instant when, bool isSuccessful) : base(amount, when, isSuccessful) {
		}

	}

}
