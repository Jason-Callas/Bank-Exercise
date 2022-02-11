namespace Bank.Shared.Domain.Entities {

	internal class DepositCashAccountTransaction :
		AccountTransaction {

		public DepositCashAccountTransaction(decimal amount, DateTime timestamp, bool wasTransferred = false) : base(amount, timestamp, wasTransferred, isSuccessful: true) {
		}

		public override decimal ApplicableAmount => Amount;

	}

}
