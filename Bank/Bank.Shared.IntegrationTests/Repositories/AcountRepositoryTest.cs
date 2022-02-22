namespace Bank.Shared.IntegrationTests.Repositories {

	using System.Threading.Tasks;
	using Bank.Shared.Domain.Entities;
	using Bank.Shared.Domain.ValueObjects;
	using Bank.Shared.IntegrationTests.Fixtures;
	using FluentAssertions;
	using Xunit;

	public class AcountRepositoryTest :
		IClassFixture<EventStoreFixture>,
		IClassFixture<AccountDataFixture> {

		private readonly EventStoreFixture _eventStoreFixture;

		private readonly AccountDataFixture _dataFixture;

		public AcountRepositoryTest(EventStoreFixture eventStoreFixture, AccountDataFixture dataFixture) {
			_eventStoreFixture = eventStoreFixture;
			_dataFixture = dataFixture;
		}

		[Fact()]
		public async Task When_NewAccountIsWrittenToStore_Expect_DataToBePersisted() {
			// ** Arrange

			var factory = _eventStoreFixture.GetRepositoryFactory();
			var repo = factory.GetRepository<Account>();

			var account = new Account(_dataFixture.DefaultAccountId, _dataFixture.DefaultCustomerName, _dataFixture.DefaultCurrency);

			// Reset
			//await repo.DeleteAsync(account.Id);

			// ** Act

			await repo.SaveAsync(account);

			// ** Assert
		}

		[Fact()]
		public async Task When_ExistingAccountIsReadFromStore_Expect_AccountToBeReturned() {
			// ** Arrange

			var factory = _eventStoreFixture.GetRepositoryFactory();
			var repo = factory.GetRepository<Account>();

			var expectedAccount = new Account(_dataFixture.DefaultAccountId, _dataFixture.DefaultCustomerName, _dataFixture.DefaultCurrency);
			//await repo.DeleteAsync(expectedAccount.Id);
			await repo.SaveAsync(expectedAccount);

			// ** Act

			var reconstitutedAccount = await repo.FindAsync(_dataFixture.DefaultAccountId);

			// ** Assert

			reconstitutedAccount.Should()
				.BeEquivalentTo(expectedAccount);
		}

		[Fact()]
		public async Task When_ExistingAccountWithNewTransactionIsWrittenToStore_Expect_DataToBePersisted() {
			// ** Arrange

			var factory = _eventStoreFixture.GetRepositoryFactory();
			var repo = factory.GetRepository<Account>();

			var expectedAccount = new Account(_dataFixture.DefaultAccountId, _dataFixture.DefaultCustomerName, _dataFixture.DefaultCurrency);
			//await repo.DeleteAsync(expectedAccount.Id);
			await repo.SaveAsync(expectedAccount);

			expectedAccount.TakeEvents();

			// ** Act

			expectedAccount.DepositCash(new Money(450m, _dataFixture.DefaultCurrency));
			await repo.SaveAsync(expectedAccount);

			var reconstitutedAccount = await repo.FindAsync(_dataFixture.DefaultAccountId);

			// ** Assert

			reconstitutedAccount.Should()
				.BeEquivalentTo(expectedAccount);
		}

	}

}
