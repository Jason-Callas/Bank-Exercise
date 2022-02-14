namespace Bank.Shared.Domain.Entities {

	using Bank.Shared.Events;

	public abstract class AggregateBase<TId> :
		IEntity<TId> {

		private readonly ICollection<IEvent<TId>> _uncommittedEvents = new List<IEvent<TId>>();

		public AggregateBase(IEnumerable<IEvent<TId>>? events = null) {
			if (events is not null) {
				foreach (var @event in events) {
					ApplyEvent(@event);
				}
			}
		}

		private void ApplyEvent(IEvent<TId> @event) {
			if (!_uncommittedEvents.Any(e => Equals(e.Id, @event.Id))) {
				((dynamic)this).Apply((dynamic)@event);
			}
		}

		protected void RaiseEvent<TEvent>(TEvent @event)
			where TEvent : IEvent<TId> {

			ApplyEvent(@event);

			_uncommittedEvents.Add(@event);
		}

		// Unsure if this is needed since the list is publicly exposed
		public void ClearUncommittedEvents() {
			_uncommittedEvents.Clear();
		}

		public IEnumerable<IEvent<TId>> GetUncommittedEvents() {
			return _uncommittedEvents.AsEnumerable();
		}

		public TId Id { get; protected set; }

	}

}
