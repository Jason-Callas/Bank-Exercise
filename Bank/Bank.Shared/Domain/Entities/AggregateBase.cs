using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bank.Shared.Events;

namespace Bank.Shared.Domain.Entities {

	public abstract class AggregateBase<TId> :
		IEntity<TId> {

		public TId Id { get; protected set; }

		private void ApplyEvent(IEvent<TId> @event) {
			if (!UncommittedEvents.Any(e => Equals(e.Id, @event.Id))) {
				((dynamic)this).Apply((dynamic)@event);
			}
		}

		protected void RaiseEvent<TEvent>(TEvent @event)
			where TEvent : IEvent<TId> {

			ApplyEvent(@event);

			UncommittedEvents.Add(@event);
		}

		// Unsure if this is needed since the list is publicly exposed
		public void ClearUncommittedEvents() {
			UncommittedEvents.Clear();
		}

		public ICollection<IEvent<TId>> UncommittedEvents = new List<IEvent<TId>>();

	}

}
