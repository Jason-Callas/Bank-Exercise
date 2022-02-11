namespace Bank.Shared.Domain.Entities {

	internal class WithdrawCashAccountTransaction :
		AccountTransaction {

		public WithdrawCashAccountTransaction(decimal amount, DateTime timestamp, bool isSuccessful, bool wasTransferred = false) : base(amount, timestamp, wasTransferred, isSuccessful) {
		}

		public override decimal ApplicableAmount => Amount;

	}

}
