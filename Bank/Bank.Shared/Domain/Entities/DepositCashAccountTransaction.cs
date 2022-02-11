namespace Bank.Shared.Domain.Entities {

	internal class DepositCashAccountTransaction :
		AccountTransaction {

		public DepositCashAccountTransaction(decimal amount, bool wasTransferred = false) : base(amount, wasTransferred, isSuccessful: true) {
		}

		public override decimal ApplicableAmount => Amount;

	}

}
