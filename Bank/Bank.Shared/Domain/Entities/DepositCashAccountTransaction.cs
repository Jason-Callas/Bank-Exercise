namespace Bank.Shared.Domain.Entities {

	internal class DepositCashAccountTransaction :
		AccountTransaction {

		public DepositCashAccountTransaction(decimal amount) : base(amount, isSuccessful: true) {
		}

	}

}
