namespace Bank.Shared.UnitTests.Handlers {

	using System;
	using System.Threading;
	using System.Threading.Tasks;
	using Bank.Shared.Domain.Entities;
	using Bank.Shared.Handlers;
	using Bank.Shared.Queries;
	using Bank.Shared.UnitTests.Fixtures;
	using FluentAssertions;
	using Linedata.Foundation.Domain;
	using Linedata.Foundation.Domain.EventSourcing;
	using Moq;
	using Xunit;

	[Trait("Type", "Unit")]
	[Trait("Category", "Handler")]
	public class QueryAccountByIdHandlerTest :
		IClassFixture<AccountDataFixture> {

		private readonly AccountDataFixture _dataFixture;

		public QueryAccountByIdHandlerTest(AccountDataFixture dataFixture) {
			_dataFixture = dataFixture ?? throw new ArgumentNullException(nameof(dataFixture));
		}

		[Fact()]
		[Trait("Class", nameof(QueryAccountByIdHandler))]
		[Trait("Method", nameof(QueryAccountByIdHandler.Handle))]
		public async Task When_NullCommandIsPassedToHandler_Expect_ExceptionToBeThrown() {
			// ** Arrange

			var mockRepository = new Mock<IEventSourcedRepository<Account>>();

			var handler = new QueryAccountByIdHandler(mockRepository.Object);

			// ** Act

			Func<Task> act = async () => await handler.Handle(default, new CancellationToken());

			// ** Assert

			await act.Should()
				.ThrowAsync<ArgumentNullException>();
		}

		[Fact()]
		[Trait("Class", nameof(QueryAccountByIdHandler))]
		[Trait("Method", nameof(QueryAccountByIdHandler.Handle))]
		public async Task When_QueryWithIdThatDoesNotExistsIsPassedToHandler_Expect_NullToBeReturned() {
			// ** Arrange

			var query = new GetAccountById(Guid.NewGuid());

			var mockRepository = new Mock<IEventSourcedRepository<Account>>();

			var getByIdWasCalled = false;
			mockRepository.Setup(x => x.FindAsync(It.IsAny<Identity>(), null))
				.Callback<Identity, Func<Metadata, Task>?>((i, m) => getByIdWasCalled = true);

			var handler = new QueryAccountByIdHandler(mockRepository.Object);

			// ** Act

			var account = await handler.Handle(query, new CancellationToken());

			// ** Assert

			account.Should()
				.BeNull();

			getByIdWasCalled.Should()
				.BeTrue();
		}

		[Fact()]
		[Trait("Class", nameof(QueryAccountByIdHandler))]
		[Trait("Method", nameof(QueryAccountByIdHandler.Handle))]
		public async Task When_QueryWithIdThatExistsIsPassedToHandler_Expect_AccountToBeReturned() {
			// ** Arrange

			var expectedAccount = _dataFixture.GetNewAccount();

			var query = new GetAccountById(Guid.Parse(expectedAccount.Id.ToString()));

			var mockRepository = new Mock<IEventSourcedRepository<Account>>();

			mockRepository.Setup(x => x.FindAsync(It.IsAny<Identity>(), null))
				.ReturnsAsync(expectedAccount);

			var handler = new QueryAccountByIdHandler(mockRepository.Object);

			// ** Act

			var account = await handler.Handle(query, new CancellationToken());

			// ** Assert

			account.Should()
				.BeEquivalentTo(expectedAccount);
		}

	}

}
