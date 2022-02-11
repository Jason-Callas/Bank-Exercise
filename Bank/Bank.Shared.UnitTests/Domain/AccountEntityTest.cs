namespace Bank.Shared.Domain.UnitTests {

	using System;
	using FluentAssertions;
	using FluentAssertions.NodaTime;
	using Bank.Shared.Domain.Entities;
	using Bank.Shared.Domain.ValueObjects;
	using Xunit;
	using Bank.Shared.Events;
	using Bank.Shared.UnitTests.Fixtures;
	using Bank.Shared.Exceptions;
	using NodaTime;

	[Trait("Type", "Unit")]
	[Trait("Category", "Entity")]
	public class AccountEntityTest :
		IClassFixture<AccountDataFixture> {

		private readonly AccountDataFixture _dataFixture;

		public AccountEntityTest(AccountDataFixture dataFixture) {
			_dataFixture = dataFixture ?? throw new ArgumentNullException(nameof(dataFixture));
		}

		[Fact()]
		[Trait("Class", nameof(Account))]
		[Trait("Method", "ctor")]
		public void When_CreateAccountWithValidParametersIsCalled_Expect_CommandToBeCompleted() {
			// ** Arrange

			var expectedEvents = new IEvent<Guid>[] {
				new AccountCreated(_dataFixture.DefaultAccountId, _dataFixture.DefaultCustomerName, _dataFixture.DefaultCurrency)
			};

			// ** Act

			var account = new Account(_dataFixture.DefaultAccountId, _dataFixture.DefaultCustomerName, _dataFixture.DefaultCurrency);

			// ** Assert

			account.GetUncommittedEvents().Should()
				.BeEquivalentTo(expectedEvents, options =>
					options
						.RespectingRuntimeTypes()
						.Excluding(x => x.Id)
				);
		}

		[Fact()]
		[Trait("Class", nameof(Account))]
		[Trait("Method", "ctor")]
		public void When_CreateAccountWithInvalidParametersIsCalled_Expect_ExceptionToBeThrown() {
			// ** Arrange

			// ** Act

			Action act = () => new Account(_dataFixture.DefaultAccountId, "Joe Dirt with a name that is too long to fit in the property but I need more words...", _dataFixture.DefaultCurrency);

			// ** Assert

			// Pretty sure I need to review/adjust validation handling and the types of exceptions thrown
			act.Should()
				.Throw<ArgumentException>();
		}

		[Fact()]
		[Trait("Class", nameof(Account))]
		[Trait("Method", nameof(Account.SetOverdraftLimit))]
		public void When_AccountOverdraftLimitIsChangedToValidValue_Expect_CommandToBeCompleted() {
			// ** Arrange

			var account = new Account(_dataFixture.DefaultAccountId, _dataFixture.DefaultCustomerName, _dataFixture.DefaultCurrency);
			account.ClearUncommittedEvents();

			var newLimit = new Money(300m, _dataFixture.DefaultCurrency);

			var expectedEvents = new IEvent<Guid>[] {
				new AccountOverdraftLimitChanged(_dataFixture.DefaultAccountId, newLimit)
			};

			// ** Act

			account.SetOverdraftLimit(newLimit);

			// ** Assert

			account.GetUncommittedEvents().Should()
				.BeEquivalentTo(expectedEvents, options =>
					options
						.RespectingRuntimeTypes()
						.Excluding(x => x.Id)
				);
		}

		[Fact()]
		[Trait("Class", nameof(Account))]
		[Trait("Method", nameof(Account.SetOverdraftLimit))]
		public void When_AccountOverdraftLimitIsChangedToZero_Expect_CommandToBeCompleted() {
			// ** Arrange

			var account = new Account(_dataFixture.DefaultAccountId, _dataFixture.DefaultCustomerName, _dataFixture.DefaultCurrency);
			account.ClearUncommittedEvents();

			var newLimit = new Money(0m, _dataFixture.DefaultCurrency);

			var expectedEvents = new IEvent<Guid>[] {
				new AccountOverdraftLimitChanged(_dataFixture.DefaultAccountId, newLimit)
			};

			// ** Act

			account.SetOverdraftLimit(newLimit);

			// ** Assert

			account.GetUncommittedEvents().Should()
				.BeEquivalentTo(expectedEvents, options =>
					options
						.RespectingRuntimeTypes()
						.Excluding(x => x.Id)
				);
		}

		[Fact()]
		[Trait("Class", nameof(Account))]
		[Trait("Method", nameof(Account.SetOverdraftLimit))]
		public void When_AccountOverdraftLimitIsChangedToNegativeValue_Expect_ExceptionToBeThrown() {
			// ** Arrange

			var account = new Account(_dataFixture.DefaultAccountId, _dataFixture.DefaultCustomerName, _dataFixture.DefaultCurrency);
			account.ClearUncommittedEvents();

			var newLimit = new Money(-250m, _dataFixture.DefaultCurrency);        // negative value not allowed

			// ** Act

			Action act = () => account.SetOverdraftLimit(newLimit);

			// ** Assert

			act.Should()
				.Throw<ArgumentException>();
		}

		[Fact()]
		[Trait("Class", nameof(Account))]
		[Trait("Method", nameof(Account.SetOverdraftLimit))]
		public void When_AccountOverdraftLimitIsChangedWithDifferentCurrencyThanAccount_Expect_ExceptionToBeThrown() {
			// ** Arrange

			var account = new Account(_dataFixture.DefaultAccountId, _dataFixture.DefaultCustomerName, _dataFixture.DefaultCurrency);
			account.ClearUncommittedEvents();

			var newLimit = new Money(200m, "USD");        // Account is set to GSB

			// ** Act

			Action act = () => account.SetOverdraftLimit(newLimit);

			// ** Assert

			act.Should()
				.Throw<InvalidCurrencyException>();
		}

		[Fact()]
		[Trait("Class", nameof(Account))]
		[Trait("Method", nameof(Account.SetDailyWireTransferLimit))]
		public void When_AccountDailyWireTransferLimitIsChangedToValidValue_Expect_CommandToBeCompleted() {
			// ** Arrange

			var account = new Account(_dataFixture.DefaultAccountId, _dataFixture.DefaultCustomerName, _dataFixture.DefaultCurrency);
			account.ClearUncommittedEvents();

			var newLimit = new Money(300m, _dataFixture.DefaultCurrency);

			var expectedEvents = new IEvent<Guid>[] {
				new AccountDailyWireTransferLimitChanged(_dataFixture.DefaultAccountId, newLimit)
			};

			// ** Act

			account.SetDailyWireTransferLimit(newLimit);

			// ** Assert

			account.GetUncommittedEvents().Should()
				.BeEquivalentTo(expectedEvents, options =>
					options
						.RespectingRuntimeTypes()
						.Excluding(x => x.Id)
				);
		}

		[Fact()]
		[Trait("Class", nameof(Account))]
		[Trait("Method", nameof(Account.SetDailyWireTransferLimit))]
		public void When_AccountDailyWireTransferLimitIsChangedToZero_Expect_CommandToBeCompleted() {
			// ** Arrange

			var account = new Account(_dataFixture.DefaultAccountId, _dataFixture.DefaultCustomerName, _dataFixture.DefaultCurrency);
			account.ClearUncommittedEvents();

			var newLimit = new Money(0m, _dataFixture.DefaultCurrency);

			var expectedEvents = new IEvent<Guid>[] {
				new AccountDailyWireTransferLimitChanged(_dataFixture.DefaultAccountId, newLimit)
			};

			// ** Act

			account.SetDailyWireTransferLimit(newLimit);

			// ** Assert

			account.GetUncommittedEvents().Should()
				.BeEquivalentTo(expectedEvents, options =>
					options
						.RespectingRuntimeTypes()
						.Excluding(x => x.Id)
				);
		}

		[Fact()]
		[Trait("Class", nameof(Account))]
		[Trait("Method", nameof(Account.SetDailyWireTransferLimit))]
		public void When_AccountDailyWireTransferLimitIsChangedToNegativeValue_Expect_ExceptionToBeThrown() {
			// ** Arrange

			var account = new Account(_dataFixture.DefaultAccountId, _dataFixture.DefaultCustomerName, _dataFixture.DefaultCurrency);
			account.ClearUncommittedEvents();

			var newLimit = new Money(-250m, _dataFixture.DefaultCurrency);        // negative value not allowed

			// ** Act

			Action act = () => account.SetDailyWireTransferLimit(newLimit);

			// ** Assert

			act.Should()
				.Throw<ArgumentException>();
		}

		[Fact()]
		[Trait("Class", nameof(Account))]
		[Trait("Method", nameof(Account.SetDailyWireTransferLimit))]
		public void When_AccountDailyWireTransferLimitIsChangedWithDifferentCurrencyThanAccount_Expect_ExceptionToBeThrown() {
			// ** Arrange

			var account = new Account(_dataFixture.DefaultAccountId, _dataFixture.DefaultCustomerName, _dataFixture.DefaultCurrency);
			account.ClearUncommittedEvents();

			var newLimit = new Money(200m, "USD");        // Account is set to GSB

			// ** Act

			Action act = () => account.SetDailyWireTransferLimit(newLimit);

			// ** Assert

			act.Should()
				.Throw<InvalidCurrencyException>();
		}

		[Fact()]
		[Trait("Class", nameof(Account))]
		[Trait("Method", nameof(Account.DepositCash))]
		public void When_CashIsDepositedIntoAccount_Expect_CommandToBeCompleted() {
			// ** Arrange

			var account = new Account(_dataFixture.DefaultAccountId, _dataFixture.DefaultCustomerName, _dataFixture.DefaultCurrency);
			account.ClearUncommittedEvents();

			var deposit = new Money(1000m, _dataFixture.DefaultCurrency);

			var expectedEvents = new IEvent<Guid>[] {
				new AccountCashDeposited(_dataFixture.DefaultAccountId, deposit)
			};

			// ** Act

			account.DepositCash(deposit);

			// ** Assert

			account.GetUncommittedEvents().Should()
				.BeEquivalentTo(expectedEvents, options =>
					options
						.RespectingRuntimeTypes()
						.Excluding(x => x.Id)
				);
		}

		[Fact()]
		[Trait("Class", nameof(Account))]
		[Trait("Method", nameof(Account.DepositCash))]
		public void When_MultipleCashDepositsAreMadeToAccount_Expect_CommandsToBeCompleted() {
			// ** Arrange

			var account = new Account(_dataFixture.DefaultAccountId, _dataFixture.DefaultCustomerName, _dataFixture.DefaultCurrency);
			account.ClearUncommittedEvents();

			var deposit1 = new Money(1000m, _dataFixture.DefaultCurrency);
			var deposit2 = new Money(500m, _dataFixture.DefaultCurrency);

			var expectedEvents = new IEvent<Guid>[] {
				new AccountCashDeposited(_dataFixture.DefaultAccountId, deposit1),
				new AccountCashDeposited(_dataFixture.DefaultAccountId, deposit2)
			};

			// ** Act

			account.DepositCash(deposit1);
			account.DepositCash(deposit2);

			// ** Assert

			account.GetUncommittedEvents().Should()
				.BeEquivalentTo(expectedEvents, options =>
					options
						.RespectingRuntimeTypes()
						.Excluding(x => x.Id)
				);
		}

		[Fact()]
		[Trait("Class", nameof(Account))]
		[Trait("Method", nameof(Account.DepositCash))]
		public void When_CashIsDepositedIntoAccountButWithDifferentCurrency_Expect_ExceptionToBeThrown() {
			// ** Arrange

			var account = new Account(_dataFixture.DefaultAccountId, _dataFixture.DefaultCustomerName, _dataFixture.DefaultCurrency);
			account.ClearUncommittedEvents();

			var deposit = new Money(275m, "USD");        // Account is set to GSB

			// ** Act

			Action act = () => account.DepositCash(deposit);

			// ** Assert

			act.Should()
				.Throw<InvalidCurrencyException>();
		}

		[Fact()]
		[Trait("Class", nameof(Account))]
		[Trait("Method", nameof(Account.DepositCheck))]
		public void When_CheckIsDepositedIntoAccount_Expect_CommandToBeCompleted() {
			// ** Arrange

			var account = new Account(_dataFixture.DefaultAccountId, _dataFixture.DefaultCustomerName, _dataFixture.DefaultCurrency);
			account.ClearUncommittedEvents();

			var deposit = new Money(1000m, _dataFixture.DefaultCurrency);

			var expectedEvents = new IEvent<Guid>[] {
				new AccountCheckDeposited(_dataFixture.DefaultAccountId, deposit, SystemClock.Instance.GetCurrentInstant())
			};

			// ** Act

			account.DepositCheck(deposit);

			// ** Assert

			account.GetUncommittedEvents().Should()
				.BeEquivalentTo(expectedEvents, options =>
					options
						.RespectingRuntimeTypes()
						.Excluding(x => x.Id)
						// To cover the difference in "deposited on"
						.Using<Instant>(ctx => ctx.Subject.Should().BeCloseTo(ctx.Expectation, TimeSpan.FromMilliseconds(200))).WhenTypeIs<Instant>()
				);
		}

		[Fact()]
		[Trait("Class", nameof(Account))]
		[Trait("Method", nameof(Account.DepositCheck))]
		public void When_MultipleCheckDepositsAreMadeToAccount_Expect_CommandsToBeCompleted() {
			// ** Arrange

			var account = new Account(_dataFixture.DefaultAccountId, _dataFixture.DefaultCustomerName, _dataFixture.DefaultCurrency);
			account.ClearUncommittedEvents();

			var deposit1 = new Money(1000m, _dataFixture.DefaultCurrency);
			var deposit2 = new Money(500m, _dataFixture.DefaultCurrency);

			var expectedEvents = new IEvent<Guid>[] {
				new AccountCheckDeposited(_dataFixture.DefaultAccountId, deposit1, SystemClock.Instance.GetCurrentInstant()),
				new AccountCheckDeposited(_dataFixture.DefaultAccountId, deposit2, SystemClock.Instance.GetCurrentInstant())
			};

			// ** Act

			account.DepositCheck(deposit1);
			account.DepositCheck(deposit2);

			// ** Assert

			account.GetUncommittedEvents().Should()
				.BeEquivalentTo(expectedEvents, options =>
					options
						.RespectingRuntimeTypes()
						.Excluding(x => x.Id)
						// To cover the difference in "deposited on"
						.Using<Instant>(ctx => ctx.Subject.Should().BeCloseTo(ctx.Expectation, TimeSpan.FromMilliseconds(200))).WhenTypeIs<Instant>()
				);
		}

		[Fact()]
		[Trait("Class", nameof(Account))]
		[Trait("Method", nameof(Account.DepositCheck))]
		public void When_CheckIsDepositedIntoAccountButWithDifferentCurrency_Expect_ExceptionToBeThrown() {
			// ** Arrange

			var account = new Account(_dataFixture.DefaultAccountId, _dataFixture.DefaultCustomerName, _dataFixture.DefaultCurrency);
			account.ClearUncommittedEvents();

			var deposit = new Money(275m, "USD");        // Account is set to GSB

			// ** Act

			Action act = () => account.DepositCheck(deposit);

			// ** Assert

			act.Should()
				.Throw<InvalidCurrencyException>();
		}

		[Fact()]
		[Trait("Class", nameof(Account))]
		[Trait("Method", nameof(Account.WithdrawCash))]
		public void When_CashIsWithdrawnFromAccountButBalanceAndOverdrawLimitIsZero_Expect_RequestToFail() {
			// ** Arrange

			var account = new Account(_dataFixture.DefaultAccountId, _dataFixture.DefaultCustomerName, _dataFixture.DefaultCurrency);
			account.SetOverdraftLimit(new Money(0m, _dataFixture.DefaultCurrency));
			account.ClearUncommittedEvents();

			var withdrawal = new Money(100m, _dataFixture.DefaultCurrency);

			var expectedEvents = new IEvent<Guid>[] {
				new AccountCashWithdrawalRejected(_dataFixture.DefaultAccountId, withdrawal, "Account does not have sufficient funds.")
			};

			// ** Act

			account.WithdrawCash(withdrawal);

			// ** Assert

			account.GetUncommittedEvents().Should()
				.BeEquivalentTo(expectedEvents, options =>
					options
						.RespectingRuntimeTypes()
						.Excluding(x => x.Id)
				);
		}

		[Fact()]
		[Trait("Class", nameof(Account))]
		[Trait("Method", nameof(Account.WithdrawCash))]
		public void When_CashIsWithdrawnFromAccountThatHasEnoughFunds_Expect_RequestToSucceed() {
			// ** Arrange

			var account = new Account(_dataFixture.DefaultAccountId, _dataFixture.DefaultCustomerName, _dataFixture.DefaultCurrency);
			account.DepositCash(new Money(200m, _dataFixture.DefaultCurrency));
			account.SetOverdraftLimit(new Money(0m, _dataFixture.DefaultCurrency));
			account.ClearUncommittedEvents();

			var withdrawal = new Money(100m, _dataFixture.DefaultCurrency);

			var expectedEvents = new IEvent<Guid>[] {
				new AccountCashWithdrawn(_dataFixture.DefaultAccountId, withdrawal)
			};

			// ** Act

			account.WithdrawCash(withdrawal);

			// ** Assert

			account.GetUncommittedEvents().Should()
				.BeEquivalentTo(expectedEvents, options =>
					options
						.RespectingRuntimeTypes()
						.Excluding(x => x.Id)
				);
		}

		[Fact()]
		[Trait("Class", nameof(Account))]
		[Trait("Method", nameof(Account.WithdrawCash))]
		public void When_CashIsWithdrawnFromAccountThatHasEnoughFundsDueToOverdraftLimit_Expect_RequestToSucceed() {
			// ** Arrange

			var account = new Account(_dataFixture.DefaultAccountId, _dataFixture.DefaultCustomerName, _dataFixture.DefaultCurrency);
			account.DepositCash(new Money(75m, _dataFixture.DefaultCurrency));
			account.SetOverdraftLimit(new Money(100m, _dataFixture.DefaultCurrency));
			account.ClearUncommittedEvents();

			var withdrawal = new Money(100m, _dataFixture.DefaultCurrency);

			var expectedEvents = new IEvent<Guid>[] {
				new AccountCashWithdrawn(_dataFixture.DefaultAccountId, withdrawal)
			};

			// ** Act

			account.WithdrawCash(withdrawal);

			// ** Assert

			var x = account.GetUncommittedEvents();

			account.GetUncommittedEvents().Should()
				.BeEquivalentTo(expectedEvents, options =>
					options
						.RespectingRuntimeTypes()
						.Excluding(x => x.Id)
				);
		}

		[Fact()]
		[Trait("Class", nameof(Account))]
		[Trait("Method", nameof(Account.WithdrawCash))]
		public void When_CashIsWithdrawnFromAccountThatExceedsOverdraftLimit_Expect_RequestToFail() {
			// ** Arrange

			var account = new Account(_dataFixture.DefaultAccountId, _dataFixture.DefaultCustomerName, _dataFixture.DefaultCurrency);
			account.DepositCash(new Money(25m, _dataFixture.DefaultCurrency));
			account.SetOverdraftLimit(new Money(50m, _dataFixture.DefaultCurrency));
			account.ClearUncommittedEvents();

			var withdrawal = new Money(100m, _dataFixture.DefaultCurrency);

			var expectedEvents = new IEvent<Guid>[] {
				new AccountCashWithdrawalRejected(_dataFixture.DefaultAccountId, withdrawal, "Account does not have sufficient funds and withdrawal exceeded overdraft limit.")
			};

			// ** Act

			account.WithdrawCash(withdrawal);

			// ** Assert

			account.GetUncommittedEvents().Should()
				.BeEquivalentTo(expectedEvents, options =>
					options
						.RespectingRuntimeTypes()
						.Excluding(x => x.Id)
				);
		}

		[Fact()]
		[Trait("Class", nameof(Account))]
		[Trait("Method", nameof(Account.WithdrawCash))]
		public void When_CashIsWithdrawnFromAccountWhereBalanceIsNotEnoughDueToPendingCheck_Expect_RequestToFail() {
			// ** Arrange

			var account = new Account(_dataFixture.DefaultAccountId, _dataFixture.DefaultCustomerName, _dataFixture.DefaultCurrency);
			account.SetOverdraftLimit(new Money(0m, _dataFixture.DefaultCurrency));
			account.DepositCash(new Money(75m, _dataFixture.DefaultCurrency));
			account.DepositCheck(new Money(75m, _dataFixture.DefaultCurrency));
			account.ClearUncommittedEvents();

			var withdrawal = new Money(100m, _dataFixture.DefaultCurrency);

			var expectedEvents = new IEvent<Guid>[] {
				new AccountCashWithdrawalRejected(_dataFixture.DefaultAccountId, withdrawal, "Account does not have sufficient funds.")
			};

			// ** Act

			account.WithdrawCash(withdrawal);

			// ** Assert

			account.GetUncommittedEvents().Should()
				.BeEquivalentTo(expectedEvents, options =>
					options
						.RespectingRuntimeTypes()
						.Excluding(x => x.Id)
				);
		}

		[Fact()]
		[Trait("Class", nameof(Account))]
		[Trait("Method", nameof(Account.WithdrawCash))]
		public void When_CashIsWithdrawnFromAccountWithEnoughFundsDuetoClearedCheck_Expect_RequestToFail() {
			// ** Arrange

			var checkDepositedOn = SystemClock.Instance.GetCurrentInstant().Minus(Duration.FromDays(2));

			var reviveEvents = new IEvent<Guid>[] {
				new AccountCreated(_dataFixture.DefaultAccountId, _dataFixture.DefaultCustomerName, _dataFixture.DefaultCurrency),
				new AccountCashDeposited(_dataFixture.DefaultAccountId, new Money(75m, _dataFixture.DefaultCurrency)),
				new AccountCheckDeposited(_dataFixture.DefaultAccountId, new Money(75m, _dataFixture.DefaultCurrency), checkDepositedOn)
			};

			var account = new Account(reviveEvents);
			account.ClearUncommittedEvents();

			var withdrawal = new Money(100m, _dataFixture.DefaultCurrency);

			var expectedEvents = new IEvent<Guid>[] {
				new AccountCashWithdrawn(_dataFixture.DefaultAccountId, withdrawal)
			};

			// ** Act

			account.WithdrawCash(withdrawal);

			// ** Assert

			account.GetUncommittedEvents().Should()
				.BeEquivalentTo(expectedEvents, options =>
					options
						.RespectingRuntimeTypes()
						.Excluding(x => x.Id)
				);
		}

	}

}
