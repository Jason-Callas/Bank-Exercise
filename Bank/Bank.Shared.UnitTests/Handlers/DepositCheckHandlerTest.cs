namespace Bank.Shared.UnitTests.Handlers {

	using System;
	using System.Threading;
	using System.Threading.Tasks;
	using Bank.Shared.Commands;
	using Bank.Shared.Domain.Entities;
	using Bank.Shared.Exceptions;
	using Bank.Shared.Handlers;
	using Bank.Shared.UnitTests.Fixtures;
	using FluentAssertions;
	using Linedata.Foundation.Domain;
	using Linedata.Foundation.Domain.EventSourcing;
	using Moq;
	using Xunit;

	[Trait("Type", "Unit")]
	[Trait("Category", "Handler")]
	public class DepositCheckHandlerTest :
		IClassFixture<AccountDataFixture> {

		private readonly AccountDataFixture _dataFixture;

		public DepositCheckHandlerTest(AccountDataFixture dataFixture) {
			_dataFixture = dataFixture ?? throw new ArgumentNullException(nameof(dataFixture));
		}

		[Fact()]
		[Trait("Class", nameof(DepositCheckHandler))]
		[Trait("Method", nameof(DepositCheckHandler.Handle))]
		public async Task When_NullCommandIsPassedToHandler_Expect_ExceptionToBeThrown() {
			// ** Arrange

			var mockRepository = new Mock<IEventSourcedRepository<Account>>();

			var handler = new DepositCheckHandler(mockRepository.Object);

			// ** Act

			Func<Task> act = async () => await handler.Handle(default, new CancellationToken());

			// ** Assert

			await act.Should()
				.ThrowAsync<ArgumentNullException>();
		}

		[Fact()]
		[Trait("Class", nameof(DepositCheckHandler))]
		[Trait("Method", nameof(DepositCheckHandler.Handle))]
		public async Task When_CommandWithIdThatDoesNotExistsIsPassedToHandler_Expect_ExceptionToBeThrown() {
			// ** Arrange

			var command = new DepositCheck(Guid.NewGuid(), 25m, "USD");

			var mockRepository = new Mock<IEventSourcedRepository<Account>>();

			var handler = new DepositCheckHandler(mockRepository.Object);

			// ** Act

			Func<Task> act = async () => await handler.Handle(command, new CancellationToken());

			// ** Assert

			await act.Should()
				.ThrowAsync<AccountNotExistException>();
		}

		[Fact()]
		[Trait("Class", nameof(DepositCheckHandler))]
		[Trait("Method", nameof(DepositCheckHandler.Handle))]
		public async Task When_CommandWithIdThatExistsIsPassedToHandler_Expect_AccountToBeUpdated() {
			// ** Arrange

			var expectedAccount = _dataFixture.GetNewAccount();

			var command = new DepositCheck(Guid.NewGuid(), 325m, _dataFixture.DefaultCurrency);

			var mockRepository = new Mock<IEventSourcedRepository<Account>>();

			mockRepository.Setup(x => x.FindAsync(It.IsAny<Identity>(), null))
				.ReturnsAsync(expectedAccount);

			var updateWasCalled = false;
			mockRepository.Setup(x => x.SaveAsync(It.IsAny<Account>(), null))
				.Callback<Account, Metadata?>((a, m) => updateWasCalled = true);

			var handler = new DepositCheckHandler(mockRepository.Object);

			// ** Act

			Func<Task> act = async () => { await handler.Handle(command, new CancellationToken()); };

			// ** Assert

			await act.Should()
				.NotThrowAsync();

			updateWasCalled.Should()
				.BeTrue();
		}

	}

}
