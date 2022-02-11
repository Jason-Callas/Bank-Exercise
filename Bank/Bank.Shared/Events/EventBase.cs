namespace Bank.Shared.Events {

	public abstract class EventBase<TAggregateId> :
		IEvent<TAggregateId> {

		public EventBase() {
			Id = Guid.NewGuid();
		}

		public EventBase(TAggregateId aggregateId) : this() {
			AggregateId = aggregateId;
		}

		public Guid Id { get; protected set; }

		public TAggregateId AggregateId { get; private set; }


		// This property exists to work around a Fluent Assertions object graph comparison issue. In essence, the
		// Should().BeEquivalentTo() method is only comparing properties of the derived types and NOT the type of
		// the complex element in the collection. In addition, only the properties of the expected object are being
		// check and not is what is only on the actual side.
		//
		// More information can be found https://github.com/fluentassertions/fluentassertions/issues/798 and
		// https://github.com/fluentassertions/fluentassertions/pull/1704

		public virtual string EventName {
			get {
				return GetType().Name;
			}
		}

	}

}
