namespace Bank.Shared.IntegrationTests.Fixtures {
	using Bank.Shared.Domain.Entities;
	using Linedata.Foundation.Domain.EventSourcing;
	using Linedata.Foundation.EventStorage;
	using Linedata.Foundation.EventStorage.EventStore;
	using Microsoft.Extensions.Configuration;
	using Microsoft.Extensions.DependencyInjection;

	public class EventStoreFixture {

		private static ServiceProvider GetProvider() {
			// ** Event Store only works with DI so we are forced to go this route

			var services = new ServiceCollection();

			var configuration = new ConfigurationBuilder()
				.AddJsonFile("appsettings.json")
				.AddJsonFile("appsettings.Development.json", true)
				.Build();

			services.AddEventStorage(configuration);
			services.AddEventSourcing(builder => {
				builder.WithPrefixedCamelCaseStreamNames("bank");

				builder.ForAggregate<Account>();
			});

			return services.BuildServiceProvider();
		}

		public IEventStoreConnectionFactory GetConnectionFactory() {
			var provider = GetProvider();

			var x = provider.GetRequiredService<IEventSourcedRepositoryFactory>();

			return provider.GetRequiredService<IEventStoreConnectionFactory>();
		}

		public IEventSourcedRepositoryFactory GetRepositoryFactory() {
			var provider = GetProvider();

			return provider.GetRequiredService<IEventSourcedRepositoryFactory>();
		}

	}

}
