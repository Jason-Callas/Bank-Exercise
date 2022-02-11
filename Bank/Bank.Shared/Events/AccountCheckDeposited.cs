namespace Bank.Shared.Events {

	using Ardalis.GuardClauses;
	using Bank.Shared.Domain.ValueObjects;
	using NodaTime;

	public class AccountCheckDeposited :
		EventBase<Guid> {

		public AccountCheckDeposited(Guid aggregateId, Money amount, Instant depositedOn) : base(aggregateId) {
			Amount = Guard.Against.Null(amount);
			DepositedOn = depositedOn;
		}

		public Money Amount { get; protected set; }

		public Instant DepositedOn { get; set; }

	}

}
