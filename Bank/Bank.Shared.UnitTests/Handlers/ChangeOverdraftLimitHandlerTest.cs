namespace Bank.Shared.UnitTests.Handlers {

	using System;
	using System.Threading;
	using System.Threading.Tasks;
	using Bank.Shared.Commands;
	using Bank.Shared.Domain.Entities;
	using Bank.Shared.Domain.ValueObjects;
	using Bank.Shared.Exceptions;
	using Bank.Shared.Handlers;
	using Bank.Shared.Repositories;
	using Bank.Shared.UnitTests.Fixtures;
	using FluentAssertions;
	using Moq;
	using Xunit;

	[Trait("Type", "Unit")]
	[Trait("Category", "Handler")]
	public class ChangeOverdraftLimitHandlerTest :
		IClassFixture<AccountDataFixture> {

		private readonly AccountDataFixture _dataFixture;

		public ChangeOverdraftLimitHandlerTest(AccountDataFixture dataFixture) {
			_dataFixture = dataFixture ?? throw new ArgumentNullException(nameof(dataFixture));
		}

		[Fact()]
		[Trait("Class", nameof(ChangeOverdraftLimitHandler))]
		[Trait("Method", nameof(ChangeOverdraftLimitHandler.Handle))]
		public void When_NullCommandIsPassedToHandler_Expect_ExceptionToBeThrown() {
			// ** Arrange

			var mockRepository = new Mock<IAccountRepository>();

			var handler = new ChangeOverdraftLimitHandler(mockRepository.Object);

			// ** Act

			Func<Task> act = async () => await handler.Handle(default, new CancellationToken());

			// ** Assert

			act.Should()
				.ThrowAsync<ArgumentNullException>();
		}

		[Fact()]
		[Trait("Class", nameof(ChangeOverdraftLimitHandler))]
		[Trait("Method", nameof(ChangeOverdraftLimitHandler.Handle))]
		public void When_CommandWithIdThatDoesNotExistsIsPassedToHandler_Expect_ExceptionToBeThrown() {
			// ** Arrange

			var command = new ChangeOverdraftLimit() {
				Id = Guid.NewGuid()
			};

			var mockRepository = new Mock<IAccountRepository>();

			var handler = new ChangeOverdraftLimitHandler(mockRepository.Object);

			// ** Act

			Func<Task> act = async () => await handler.Handle(command, new CancellationToken());

			// ** Assert

			act.Should()
				.ThrowAsync<AccountNotExistException>();
		}


		[Fact()]
		[Trait("Class", nameof(ChangeOverdraftLimitHandler))]
		[Trait("Method", nameof(ChangeOverdraftLimitHandler.Handle))]
		public void When_CommandWithNegativeAmountIsPassedToHandler_Expect_ExceptionToBeThrown() {
			// ** Arrange

			var command = new ChangeOverdraftLimit() {
				Id = Guid.NewGuid(),
				Amount = -120m
			};

			var mockRepository = new Mock<IAccountRepository>();

			var handler = new ChangeOverdraftLimitHandler(mockRepository.Object);

			// ** Act

			Func<Task> act = async () => await handler.Handle(command, new CancellationToken());

			// ** Assert

			act.Should()
				.ThrowAsync<ArgumentOutOfRangeException>();
		}

		[Fact()]
		[Trait("Class", nameof(ChangeOverdraftLimitHandler))]
		[Trait("Method", nameof(ChangeOverdraftLimitHandler.Handle))]
		public void When_CommandWithIdThatExistsIsPassedToHandler_Expect_AccountToBeUpdated() {
			// ** Arrange

			var expectedAccount = _dataFixture.GetNewAccount();

			var expectedLimit = new Money(325m, "USD");

			var command = new ChangeOverdraftLimit() {
				Id = Guid.NewGuid(),
				Amount = expectedLimit.Amount,
				Currency = expectedLimit.Currency
			};

			var mockRepository = new Mock<IAccountRepository>();

			mockRepository.Setup(x => x.GetByIdAsync(It.IsAny<Guid>()))
				.ReturnsAsync(expectedAccount);

			var updateWasCalled = false;
			mockRepository.Setup(x => x.UpdateAsync(It.IsAny<Account>()))
				.Callback<Account>(x => updateWasCalled = true);

			var handler = new ChangeOverdraftLimitHandler(mockRepository.Object);

			// ** Act

			Func<Task> act = () => handler.Handle(command, new CancellationToken());

			// ** Assert

			act.Should()
				.NotThrowAsync();

			updateWasCalled.Should()
				.BeTrue();
		}

	}

}
