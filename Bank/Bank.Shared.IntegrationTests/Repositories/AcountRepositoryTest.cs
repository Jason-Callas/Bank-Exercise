
using System;
using System.Threading.Tasks;
using Bank.Shared.Domain.Entities;
using Bank.Shared.Domain.ValueObjects;
using Bank.Shared.IntegrationTests.Fixtures;
using Bank.Shared.Repositories;
using FluentAssertions;
using Xunit;

namespace Bank.Shared.IntegrationTests.Repositories {
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

			var client = _eventStoreFixture.GetClient();

			var repo = new AccountRepository(client);

			var account = new Account(_dataFixture.DefaultAccountId, _dataFixture.DefaultCustomerName, _dataFixture.DefaultCurrency);

			// Reset
			await repo.DeleteAsync(account.Id);

			// ** Act

			await repo.CreateAsync(account);

			// ** Assert
		}

		[Fact()]
		public async Task When_ExistingAccountIsReadFromStore_Expect_AccountToBeReturned() {
			// ** Arrange

			var client = _eventStoreFixture.GetClient();

			var repo = new AccountRepository(client);

			var expectedAccount = new Account(_dataFixture.DefaultAccountId, _dataFixture.DefaultCustomerName, _dataFixture.DefaultCurrency);
			await repo.DeleteAsync(expectedAccount.Id);
			await repo.CreateAsync(expectedAccount);

			// ** Act

			var reconstitutedAccount = await repo.GetByIdAsync(_dataFixture.DefaultAccountId);

			// ** Assert

			reconstitutedAccount.Should()
				.BeEquivalentTo(expectedAccount);
		}

		[Fact()]
		public async Task When_ExistingAccountWithNewTransactionIsWrittenToStore_Expect_DataToBePersisted() {
			// ** Arrange

			var client = _eventStoreFixture.GetClient();

			var repo = new AccountRepository(client);

			var expectedAccount = new Account(_dataFixture.DefaultAccountId, _dataFixture.DefaultCustomerName, _dataFixture.DefaultCurrency);
			await repo.DeleteAsync(expectedAccount.Id);
			await repo.CreateAsync(expectedAccount);

			expectedAccount.ClearUncommittedEvents();

			// ** Act

			expectedAccount.DepositCash(new Money(450m, _dataFixture.DefaultCurrency));
			await repo.UpdateAsync(expectedAccount);

			var reconstitutedAccount = await repo.GetByIdAsync(_dataFixture.DefaultAccountId);

			// ** Assert

			reconstitutedAccount.Should()
				.BeEquivalentTo(expectedAccount);
		}

	}

}
