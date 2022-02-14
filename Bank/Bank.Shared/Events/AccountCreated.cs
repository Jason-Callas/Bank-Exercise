namespace Bank.Shared.Events {

	using Ardalis.GuardClauses;

	public class AccountCreated :
		EventBase<Guid> {

		public AccountCreated(Guid aggregateId, string customerName, string currency) : base(aggregateId) {
			CustomerName = Guard.Against.NullOrWhiteSpace(customerName, nameof(customerName));
			Currency = Guard.Against.NullOrWhiteSpace(currency, nameof(currency)); ;
		}

		public string CustomerName { get; }

		public string Currency { get; }

	}

}
