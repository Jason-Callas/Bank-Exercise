namespace Bank.Shared.Events {

	public abstract class BaseEvent<T> :
		IEvent<T> {

		public BaseEvent(DateTime? timestamp = null) {
			TimestampUtc = timestamp ?? DateTime.UtcNow;
		}

		public DateTime TimestampUtc { get; protected set; }

	}

}
