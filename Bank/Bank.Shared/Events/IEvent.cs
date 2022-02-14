namespace Bank.Shared.Events {

	public interface IEvent<TAggregateId> {

		Guid Id { get; }

		TAggregateId AggregateId { get; }

		// Seems to be need for when we write event to store
		string EventName { get; }
	}

}
