namespace Bank.Shared.Events {

	using Ardalis.GuardClauses;
	using Bank.Shared.Domain.ValueObjects;
	using NodaTime;

	public class AccountCashTransferred :
		AccountTransaction {

		public AccountCashTransferred(Guid aggregateId, Money amount, Instant when) : base(aggregateId, when) {
			Amount = Guard.Against.Null(amount);
		}

		public Money Amount { get; }

	}

}
