namespace Bank.Shared.Domain.Entities {

	using Bank.Shared.Extensions;
	using NodaTime;

	internal class DepositCheckAccountTransaction :
		CreditAccountTransaction {

		public DepositCheckAccountTransaction(decimal amount, Instant when) : base(amount, when, isSuccessful: true) {
		}

		public override decimal ApplicableAmount => HasCleared ? Amount : 0m;

		public bool HasCleared {
			get {
				// Checks are available the "next" business day
				return IsSuccessful && SystemClock.Instance.GetCurrentInstant() >= When.NextBusinessDay();
			}
		}

	}

}
