namespace Bank.Shared.UnitTests.Handlers {

	using System;
	using System.Threading;
	using System.Threading.Tasks;
	using Bank.Shared.Commands;
	using Bank.Shared.Domain.Entities;
	using Bank.Shared.Handlers;
	using Bank.Shared.Repositories;
	using FluentAssertions;
	using Moq;
	using Xunit;

	public class CreateAccountHandlerTest {

		[Fact()]
		[Trait("Class", nameof(CreateAccountHandler))]
		[Trait("Method", nameof(CreateAccountHandler.Handle))]
		public void When_NullCommandIsPassedToHandler_Expect_ExceptionToBeThrown() {
			// ** Arrange

			var mockRepository = new Mock<IRepository<Account>>();

			var handler = new CreateAccountHandler(mockRepository.Object);

			// ** Act

			Func<Task> act = async () => await handler.Handle(default, new CancellationToken());

			// ** Assert

			act.Should()
				.ThrowAsync<ArgumentNullException>();
		}

		[Fact()]
		[Trait("Class", nameof(CreateAccountHandler))]
		[Trait("Method", nameof(CreateAccountHandler.Handle))]
		public void When_ValidCommandIsPassedToHandler_Expect_AccountToBeCreated() {
			// ** Arrange

			var command = new CreateAccount() {
				Name = "Joe Dirt"
			};

			var mockRepository = new Mock<IRepository<Account>>();

			var createWasCalled = false;
			mockRepository.Setup(x => x.CreateAsync(It.IsAny<Account>()))
				.Callback<Account>(x => createWasCalled = true);

			var handler = new CreateAccountHandler(mockRepository.Object);

			// ** Act

			Func<Task> act = async () => await handler.Handle(command, new CancellationToken());

			// ** Assert

			act.Should()
				.NotThrowAsync();

			createWasCalled.Should()
				.BeTrue();
		}

	}

}
