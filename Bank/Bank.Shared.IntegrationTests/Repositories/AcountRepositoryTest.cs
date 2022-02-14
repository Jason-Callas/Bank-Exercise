
using System;
using System.Threading.Tasks;
using Bank.Shared.Domain.Entities;
using Bank.Shared.IntegrationTests.Fixtures;
using Bank.Shared.Repositories;
using EventStore.Client;
using Xunit;

namespace Bank.Shared.IntegrationTests.Repositories {
	public class AcountRepositoryTest :
		IClassFixture<EventStoreFixture> {

		private readonly EventStoreFixture _eventStoreFixture;

		public AcountRepositoryTest(EventStoreFixture eventStoreFixture) {
			_eventStoreFixture = eventStoreFixture;
		}

		[Fact()]
		public async Task When_Expect() {
			// ** Arrange

			var client = _eventStoreFixture.GetClient();

			var repo = new AccountRepository(client);

			var account = new Account(Guid.Empty, "John Doe", "USD");

			await repo.DeleteAsync(account.Id);

			// ** Act

			await repo.CreateAsync(account);

			// ** Assert
		}

		[Fact()]
		public async Task When_Expec2t() {
			// ** Arrange

			var client = _eventStoreFixture.GetClient();

			var repo = new AccountRepository(client);

			// ** Act

			var account = await repo.GetByIdAsync(Guid.Empty);

			// ** Assert
		}

	}

}
