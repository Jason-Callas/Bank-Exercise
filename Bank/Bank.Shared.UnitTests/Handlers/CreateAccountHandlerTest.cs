namespace Bank.Shared.UnitTests.Handlers {

	using System;
	using System.Threading;
	using System.Threading.Tasks;
	using Bank.Shared.Commands;
	using Bank.Shared.Domain.Entities;
	using Bank.Shared.Handlers;
	using Bank.Shared.Repositories;
	using Bank.Shared.UnitTests.Fixtures;
	using FluentAssertions;
	using Moq;
	using Xunit;

	[Trait("Type", "Unit")]
	[Trait("Category", "Handler")]
	public class CreateAccountHandlerTest :
		IClassFixture<AccountDataFixture> {

		private readonly AccountDataFixture _dataFixture;

		public CreateAccountHandlerTest(AccountDataFixture dataFixture) {
			_dataFixture = dataFixture ?? throw new ArgumentNullException(nameof(dataFixture));
		}

		[Fact()]
		[Trait("Class", nameof(CreateAccountHandler))]
		[Trait("Method", nameof(CreateAccountHandler.Handle))]
		public async Task When_NullCommandIsPassedToHandler_Expect_ExceptionToBeThrown() {
			// ** Arrange

			var mockRepository = new Mock<IAccountRepository>();

			var handler = new CreateAccountHandler(mockRepository.Object);

			// ** Act

			Func<Task> act = async () => await handler.Handle(default, new CancellationToken());

			// ** Assert

			await act.Should()
				.ThrowAsync<ArgumentNullException>();
		}

		[Fact()]
		[Trait("Class", nameof(CreateAccountHandler))]
		[Trait("Method", nameof(CreateAccountHandler.Handle))]
		public async Task When_ValidCommandIsPassedToHandler_Expect_AccountToBeCreated() {
			// ** Arrange

			var command = new CreateAccount() {
				Name = "Joe Dirt",
				Currency = _dataFixture.DefaultCurrency
			};

			var mockRepository = new Mock<IAccountRepository>();

			var createWasCalled = false;
			mockRepository.Setup(x => x.CreateAsync(It.IsAny<Account>()))
				.Callback<Account>(x => createWasCalled = true);

			var handler = new CreateAccountHandler(mockRepository.Object);

			// ** Act

			await handler.Handle(command, new CancellationToken());

			// ** Assert

			createWasCalled.Should()
				.BeTrue();
		}

	}

}
