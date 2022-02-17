namespace Bank.Shared.Events {

	using Ardalis.GuardClauses;
	using Bank.Shared.Domain.ValueObjects;
	using NodaTime;

	public class AccountCashDeposited :
		AccountTransaction {

		public AccountCashDeposited(Guid aggregateId, Money amount, Instant when) : base(aggregateId, when) {
			Amount = Guard.Against.Null(amount);
		}

		public Money Amount { get; }

	}

}
