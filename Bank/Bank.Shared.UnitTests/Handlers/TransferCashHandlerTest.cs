﻿namespace Bank.Shared.UnitTests.Handlers {

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
	public class TransferCashHandlerTest :
		IClassFixture<AccountDataFixture> {

		private readonly AccountDataFixture _dataFixture;

		public TransferCashHandlerTest(AccountDataFixture dataFixture) {
			_dataFixture = dataFixture ?? throw new ArgumentNullException(nameof(dataFixture));
		}

		[Fact()]
		[Trait("Class", nameof(WireTransferCashHandler))]
		[Trait("Method", nameof(WireTransferCashHandler.Handle))]
		public async Task When_NullCommandIsPassedToHandler_Expect_ExceptionToBeThrown() {
			// ** Arrange

			var mockRepository = new Mock<IAccountRepository>();

			var handler = new WireTransferCashHandler(mockRepository.Object);

			// ** Act

			Func<Task> act = async () => await handler.Handle(default, new CancellationToken());

			// ** Assert

			await act.Should()
				.ThrowAsync<ArgumentNullException>();
		}

		[Fact()]
		[Trait("Class", nameof(WireTransferCashHandler))]
		[Trait("Method", nameof(WireTransferCashHandler.Handle))]
		public async Task When_CommandWithIdThatDoesNotExistsIsPassedToHandler_Expect_ExceptionToBeThrown() {
			// ** Arrange

			var command = new WireTransferCash(Guid.NewGuid(), 25m, "USD");

			var mockRepository = new Mock<IAccountRepository>();

			var handler = new WireTransferCashHandler(mockRepository.Object);

			// ** Act

			Func<Task> act = async () => await handler.Handle(command, new CancellationToken());

			// ** Assert

			await act.Should()
				.ThrowAsync<AccountNotExistException>();
		}

		[Fact()]
		[Trait("Class", nameof(WireTransferCashHandler))]
		[Trait("Method", nameof(WireTransferCashHandler.Handle))]
		public async Task When_CommandWithIdThatExistsIsPassedToHandler_Expect_AccountToBeUpdated() {
			// ** Arrange

			var expectedAccount = _dataFixture.GetNewAccount();

			var command = new WireTransferCash(Guid.NewGuid(), 325m, _dataFixture.DefaultCurrency);

			var mockRepository = new Mock<IAccountRepository>();

			mockRepository.Setup(x => x.GetByIdAsync(It.IsAny<Guid>()))
				.ReturnsAsync(expectedAccount);

			var updateWasCalled = false;
			mockRepository.Setup(x => x.UpdateAsync(It.IsAny<Account>()))
				.Callback<Account>(x => updateWasCalled = true);

			var handler = new WireTransferCashHandler(mockRepository.Object);

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
