namespace Bank.Shared.Domain.Entities {

	using NodaTime;

	internal class DepositCheckAccountTransaction :
		CreditAccountTransaction {

		public DepositCheckAccountTransaction(decimal amount, Instant when) : base(amount, when, isSuccessful: true) {
		}

		public override decimal ApplicableAmount => HasCleared ? Amount : 0m;

		public bool HasCleared {
			get {
				// Checks are avilable the next day
				//   Think I need to review requirements since it mentions M-F/9-5...not sure how that fits in.

				return IsSuccessful && SystemClock.Instance.GetCurrentInstant() >= When.Plus(Duration.FromDays(1));
			}
		}

	}

}
