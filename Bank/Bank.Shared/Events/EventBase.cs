namespace Bank.Shared.Events {

	public abstract class EventBase<TAggregateId> :
		IEvent<TAggregateId> {

		public EventBase() {
			Id = Guid.NewGuid();
			TimestampUtc = DateTime.UtcNow;
		}

		public EventBase(TAggregateId aggregateId) : this() {
			AggregateId = aggregateId;
		}

		public EventBase(TAggregateId aggregateId, DateTime timestamp) : this(aggregateId) {
			TimestampUtc = timestamp;
		}

		public Guid Id { get; protected set; }

		public TAggregateId AggregateId { get; private set; }


		public DateTime TimestampUtc { get; protected set; }

	}

}
