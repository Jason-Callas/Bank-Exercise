namespace Bank.Shared.Events {

	using Ardalis.GuardClauses;

	public class AccountCreated :
		EventBase<Guid> {

		public AccountCreated(Guid aggregateId, string customerName) : base(aggregateId) {
			CustomerName = Guard.Against.NullOrWhiteSpace(customerName, nameof(customerName));
		}

		public string CustomerName { get; protected set; }

	}

}
