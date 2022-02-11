namespace Bank.Shared.Domain.Entities {

	internal class WithdrawCashAccountTransaction :
		AccountTransaction {

		public WithdrawCashAccountTransaction(decimal amount, bool isSuccessful, bool wasTransferred = false) : base(amount, wasTransferred, isSuccessful) {
		}

		public override decimal ApplicableAmount => Amount;

	}

}
