namespace Bank.Shared.IntegrationTests.Fixtures {

	using EventStore.Client;

	public class EventStoreFixture {

		// We can share client across a given application
		private static EventStoreClient? s_client = default;

		public EventStoreClient GetClient() {
			if (s_client == null) {
				var settings = EventStoreClientSettings
					.Create("esdb://127.0.0.1:2113?tls=false&keepAliveTimeout=10000&keepAliveInterval=10000");

				s_client = new EventStoreClient(settings);
			}

			return s_client;
		}

	}

}
