namespace Bank.Shared.Events {

	using Ardalis.GuardClauses;
	using Bank.Shared.Domain.ValueObjects;

	public class AccountCashWithdrawalRejected :
		EventBase<Guid> {

		public AccountCashWithdrawalRejected(Guid aggregateId, Money amount, string? reason = null) : base(aggregateId) {
			Amount = Guard.Against.Null(amount);
			Reason = reason;
		}

		public Money Amount { get; protected set; }

		public string? Reason { get; protected set; }

	}

}
