namespace Bank.Shared.IntegrationTests.Repositories {
	using Bank.Shared.IntegrationTests.Fixtures;
	using Bank.Shared.Repositories;
	using EventStore.Client;
	using Xunit;

	public class AcountRepositoryTest :
		IClassFixture<EventStoreFixture> {

		private readonly EventStoreFixture _eventStoreFixture;

		public AcountRepositoryTest(EventStoreFixture eventStoreFixture) {
			_eventStoreFixture = eventStoreFixture;
		}

		public void When_Expect() {
			// ** Arrange

			var client = _eventStoreFixture.GetClient();

			var repo = new AccountRepository(client);

			// ** Act

			// ** Assert
		}

	}

}
