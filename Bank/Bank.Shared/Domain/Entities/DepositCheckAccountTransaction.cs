using NodaTime;

namespace Bank.Shared.Domain.Entities {

	internal class DepositCheckAccountTransaction :
		AccountTransaction {

		public DepositCheckAccountTransaction(decimal amount, Instant depositedOn) : base(amount, isSuccessful: true) {
			DepositedOn = depositedOn;
		}

		public override decimal ApplicableAmount => HasCleared ? Amount : 0m;

		public Instant DepositedOn { get; }

		public bool HasCleared {
			get {
				// Checks are avilable the next day
				//   Think I need to review requirements since it mentions M-F/9-5...not sure how that fits in.

				return IsSuccessful && SystemClock.Instance.GetCurrentInstant() >= DepositedOn.Plus(Duration.FromDays(1));
			}
		}

	}

}
