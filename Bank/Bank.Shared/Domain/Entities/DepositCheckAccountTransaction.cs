using NodaTime;

namespace Bank.Shared.Domain.Entities {

	internal class DepositCheckAccountTransaction :
		AccountTransaction {

		public DepositCheckAccountTransaction(decimal amount, Instant depositedOn, bool wasTransferred = false) : base(amount, wasTransferred) {
			DepositedOn = depositedOn;
		}

		// Checks are avilable the next day
		//   Think I need to review requirements since it mentions M-F/9-5...not sure how that fits in.
		public override decimal ApplicableAmount => SystemClock.Instance.GetCurrentInstant() >= DepositedOn.Plus(Duration.FromDays(1)) ? Amount : 0m;

		public Instant DepositedOn { get; protected set; }
	}

}
