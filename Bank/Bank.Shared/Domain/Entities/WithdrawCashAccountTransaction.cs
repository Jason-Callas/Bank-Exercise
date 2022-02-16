namespace Bank.Shared.Domain.Entities {

	internal class WithdrawCashAccountTransaction :
		AccountTransaction {

		public WithdrawCashAccountTransaction(decimal amount, bool isSuccessful) : base(amount, isSuccessful) {
		}

	}

}
