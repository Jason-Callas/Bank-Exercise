namespace Bank.Shared.Events {

	public interface IEvent<TAggregateId> {

		Guid Id { get; }

		TAggregateId AggregateId { get; }

		DateTime TimestampUtc { get; }

	}

}
