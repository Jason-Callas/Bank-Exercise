using NodaTime;

namespace Bank.Shared.Domain.Entities {

	internal class TransferCashAccountTransaction :
		AccountTransaction {

		public TransferCashAccountTransaction(decimal amount, LocalDate transferredOn, bool isSuccessful) : base(amount, isSuccessful) {
			TransferredOn = transferredOn;
		}

		public LocalDate TransferredOn { get; }

	}

}
