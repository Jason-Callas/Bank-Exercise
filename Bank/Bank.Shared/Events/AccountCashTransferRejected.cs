namespace Bank.Shared.Events {

	using Ardalis.GuardClauses;
	using Bank.Shared.Domain.ValueObjects;

	public class AccountCashTransferRejected :
		EventBase<Guid> {

		public AccountCashTransferRejected(Guid aggregateId, Money amount, string? reason = null) : base(aggregateId) {
			Amount = Guard.Against.Null(amount);
			Reason = reason;
		}

		public Money Amount { get; }

		public string? Reason { get; }

	}

}
