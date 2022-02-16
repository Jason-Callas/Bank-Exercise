namespace Bank.Shared.Events {

	using Ardalis.GuardClauses;
	using Bank.Shared.Domain.ValueObjects;
	using NodaTime;

	public class AccountCashTransferred :
		EventBase<Guid> {

		public AccountCashTransferred(Guid aggregateId, Money amount, LocalDate transferredOn) : base(aggregateId) {
			Amount = Guard.Against.Null(amount);
			TransferredOn = transferredOn;
		}

		public Money Amount { get; }

		public LocalDate TransferredOn { get; }

	}

}
