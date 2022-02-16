namespace Bank.Shared.Domain.Entities {

	internal enum DebitApproval {

		Approved,

		AccountBlocked,

		InsufficientFunds,

		OverdraftExceeded,

		DailyTransferExceeded

	}

}
