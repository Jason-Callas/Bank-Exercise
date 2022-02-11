namespace Bank.Shared.Events {

	using Ardalis.GuardClauses;
	using Bank.Shared.Domain.ValueObjects;

	public class AccountCashWithdrawn :
		EventBase<Guid> {

		public AccountCashWithdrawn(Guid aggregateId, Money amount) : base(aggregateId) {
			Amount = Guard.Against.Null(amount);
		}

		public Money Amount { get; protected set; }

	}

}
