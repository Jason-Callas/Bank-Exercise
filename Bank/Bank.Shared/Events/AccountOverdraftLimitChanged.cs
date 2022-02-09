namespace Bank.Shared.Events {

	using Ardalis.GuardClauses;
	using Bank.Shared.Domain.ValueObjects;

	public class AccountOverdraftLimitChanged :
		EventBase<Guid> {

		public AccountOverdraftLimitChanged(Guid aggregateId, Money newLimit) : base(aggregateId) {
			Limit = Guard.Against.Null(newLimit);
		}

		public Money Limit { get; protected set; }

	}

}
