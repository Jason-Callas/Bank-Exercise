namespace Bank.Shared.Commands {

	public class ChangeDailyTransferLimit :
		ICommand {

		public Guid AccountId { get; set; }

		public decimal Amount { get; set; }

		// Can the currency of a bank account really change after the account is created? Sure,
		// a customer can deposit or transfer different currency but chances are it will be exchanged
		// first which implies the Currency value should be at the Account level.
		public string Currency { get; set; } = default!;

	}

}
