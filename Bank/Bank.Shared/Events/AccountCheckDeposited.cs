namespace Bank.Shared.Events {

	using Ardalis.GuardClauses;
	using Bank.Shared.Domain.ValueObjects;

	public class AccountCheckDeposited :
		EventBase<Guid> {

		public AccountCheckDeposited(Guid aggregateId, Money amount) : base(aggregateId) {
			Amount = Guard.Against.Null(amount);
		}

		public Money Amount { get; protected set; }

	}

}
