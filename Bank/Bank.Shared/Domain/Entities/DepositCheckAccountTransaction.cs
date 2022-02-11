namespace Bank.Shared.Domain.Entities {

	internal class DepositCheckAccountTransaction :
		AccountTransaction {

		public DepositCheckAccountTransaction(decimal amount, DateTime timestamp, bool wasTransferred = false) : base(amount, timestamp, wasTransferred) {
		}

		// Checks are avilable the next day
		//   Think I need to review requirements since it mentions M-F/9-5...not sure how that fits in.
		public override decimal ApplicableAmount => DateTime.UtcNow >= TimestampUtc.AddDays(1) ? Amount : 0m;

	}

}
