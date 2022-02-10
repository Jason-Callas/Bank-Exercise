namespace Bank.Shared.Events {

	using Ardalis.GuardClauses;
	using Bank.Shared.Domain.ValueObjects;

	public class AccountCashDeposited :
		EventBase<Guid> {

		public AccountCashDeposited(Guid aggregateId, Money amount) : base(aggregateId) {
			Amount = Guard.Against.Null(amount);
		}

		public Money Amount { get; protected set; }

	}

}
