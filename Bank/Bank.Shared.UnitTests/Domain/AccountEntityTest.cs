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

		// ** Account Creation ********************************************************************************

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

		// ** Overdraft Limit ********************************************************************************

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

		// ** Wire Transfer Limit ********************************************************************************

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

		// ** Cash Deposit ********************************************************************************

		[Fact()]
		[Trait("Class", nameof(Account))]
		[Trait("Method", nameof(Account.DepositCash))]
		public void When_CashIsDepositedIntoAccount_Expect_CommandToBeCompleted() {
			// ** Arrange

			var account = new Account(_dataFixture.DefaultAccountId, _dataFixture.DefaultCustomerName, _dataFixture.DefaultCurrency);
			account.ClearUncommittedEvents();

			var deposit = new Money(1000m, _dataFixture.DefaultCurrency);

			var expectedEvents = new IEvent<Guid>[] {
				new AccountCashDeposited(_dataFixture.DefaultAccountId, deposit, SystemClock.Instance.GetCurrentInstant())
			};

			// ** Act

			account.DepositCash(deposit);

			// ** Assert

			account.GetUncommittedEvents().Should()
				.BeEquivalentTo(expectedEvents, options =>
					options
						.RespectingRuntimeTypes()
						.Excluding(x => x.Id)
						.Using<Instant>(ctx => ctx.Subject.Should().BeCloseTo(ctx.Expectation, Duration.FromMilliseconds(250))).WhenTypeIs<Instant>()
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
				new AccountCashDeposited(_dataFixture.DefaultAccountId, deposit1, SystemClock.Instance.GetCurrentInstant()),
				new AccountCashDeposited(_dataFixture.DefaultAccountId, deposit2, SystemClock.Instance.GetCurrentInstant())
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
						.Using<Instant>(ctx => ctx.Subject.Should().BeCloseTo(ctx.Expectation, Duration.FromMilliseconds(250))).WhenTypeIs<Instant>()
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

		// ** Check Deposit ********************************************************************************

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

		// ** Cash Withdrawal ********************************************************************************

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
				new AccountCashWithdrawalRejected(_dataFixture.DefaultAccountId, withdrawal, SystemClock.Instance.GetCurrentInstant(), "Account does not have sufficient funds.")
			};

			// ** Act

			account.WithdrawCash(withdrawal);

			// ** Assert

			account.GetUncommittedEvents().Should()
				.BeEquivalentTo(expectedEvents, options =>
					options
						.RespectingRuntimeTypes()
						.Excluding(x => x.Id)
						.Using<Instant>(ctx => ctx.Subject.Should().BeCloseTo(ctx.Expectation, Duration.FromMilliseconds(250))).WhenTypeIs<Instant>()
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
				new AccountCashWithdrawn(_dataFixture.DefaultAccountId, withdrawal, SystemClock.Instance.GetCurrentInstant())
			};

			// ** Act

			account.WithdrawCash(withdrawal);

			// ** Assert

			account.GetUncommittedEvents().Should()
				.BeEquivalentTo(expectedEvents, options =>
					options
						.RespectingRuntimeTypes()
						.Excluding(x => x.Id)
						.Using<Instant>(ctx => ctx.Subject.Should().BeCloseTo(ctx.Expectation, Duration.FromMilliseconds(250))).WhenTypeIs<Instant>()
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
				new AccountCashWithdrawn(_dataFixture.DefaultAccountId, withdrawal, SystemClock.Instance.GetCurrentInstant())
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
						.Using<Instant>(ctx => ctx.Subject.Should().BeCloseTo(ctx.Expectation, Duration.FromMilliseconds(250))).WhenTypeIs<Instant>()
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
				new AccountCashWithdrawalRejected(_dataFixture.DefaultAccountId, withdrawal, SystemClock.Instance.GetCurrentInstant(), "Account does not have sufficient funds and withdrawal exceeded overdraft limit.")
			};

			// ** Act

			account.WithdrawCash(withdrawal);

			// ** Assert

			account.GetUncommittedEvents().Should()
				.BeEquivalentTo(expectedEvents, options =>
					options
						.RespectingRuntimeTypes()
						.Excluding(x => x.Id)
						.Using<Instant>(ctx => ctx.Subject.Should().BeCloseTo(ctx.Expectation, Duration.FromMilliseconds(250))).WhenTypeIs<Instant>()
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
				new AccountCashWithdrawalRejected(_dataFixture.DefaultAccountId, withdrawal,SystemClock.Instance.GetCurrentInstant(), "Account does not have sufficient funds.")
			};

			// ** Act

			account.WithdrawCash(withdrawal);

			// ** Assert

			account.GetUncommittedEvents().Should()
				.BeEquivalentTo(expectedEvents, options =>
					options
						.RespectingRuntimeTypes()
						.Excluding(x => x.Id)
						.Using<Instant>(ctx => ctx.Subject.Should().BeCloseTo(ctx.Expectation, Duration.FromMilliseconds(250))).WhenTypeIs<Instant>()
				);
		}

		[Fact()]
		[Trait("Class", nameof(Account))]
		[Trait("Method", nameof(Account.WithdrawCash))]
		public void When_CashIsWithdrawnFromAccountWithEnoughFundsDuetoClearedCheck_Expect_RequestToFail() {
			// ** Arrange

			var checkDepositedOn = SystemClock.Instance.GetCurrentInstant().Minus(Duration.FromDays(3));

			var reviveEvents = new IEvent<Guid>[] {
				new AccountCreated(_dataFixture.DefaultAccountId, _dataFixture.DefaultCustomerName, _dataFixture.DefaultCurrency),
				new AccountCheckDeposited(_dataFixture.DefaultAccountId, new Money(75m, _dataFixture.DefaultCurrency), checkDepositedOn),
				new AccountCashDeposited(_dataFixture.DefaultAccountId, new Money(75m, _dataFixture.DefaultCurrency), SystemClock.Instance.GetCurrentInstant())
			};

			var account = new Account(reviveEvents);
			account.ClearUncommittedEvents();

			var withdrawal = new Money(100m, _dataFixture.DefaultCurrency);

			var expectedEvents = new IEvent<Guid>[] {
				new AccountCashWithdrawn(_dataFixture.DefaultAccountId, withdrawal, SystemClock.Instance.GetCurrentInstant())
			};

			// ** Act

			account.WithdrawCash(withdrawal);

			// ** Assert

			account.GetUncommittedEvents().Should()
				.BeEquivalentTo(expectedEvents, options =>
					options
						.RespectingRuntimeTypes()
						.Excluding(x => x.Id)
						.Using<Instant>(ctx => ctx.Subject.Should().BeCloseTo(ctx.Expectation, Duration.FromMilliseconds(250))).WhenTypeIs<Instant>()
				);
		}

		[Fact()]
		[Trait("Class", nameof(Account))]
		[Trait("Method", nameof(Account.WithdrawCash))]
		public void When_CashIsWithdrawnFromBlockedAccount_Expect_RequestToFail() {
			// ** Arrange

			var checkDepositedOn = SystemClock.Instance.GetCurrentInstant().Minus(Duration.FromDays(3));

			var depositAmount = 75m;

			var reviveEvents = new IEvent<Guid>[] {
				new AccountCreated(_dataFixture.DefaultAccountId, _dataFixture.DefaultCustomerName, _dataFixture.DefaultCurrency),
				new AccountCashDeposited(_dataFixture.DefaultAccountId, new Money(depositAmount, _dataFixture.DefaultCurrency), SystemClock.Instance.GetCurrentInstant()),
				// Trigger blocking
				new AccountCashWithdrawalRejected(_dataFixture.DefaultAccountId, new Money(depositAmount + 25m, _dataFixture.DefaultCurrency), SystemClock.Instance.GetCurrentInstant())
			};

			var account = new Account(reviveEvents);
			account.ClearUncommittedEvents();

			var withdrawal = new Money(depositAmount - 25m, _dataFixture.DefaultCurrency);

			var expectedEvents = new IEvent<Guid>[] {
				new AccountCashWithdrawalRejected(_dataFixture.DefaultAccountId, withdrawal, SystemClock.Instance.GetCurrentInstant(), "Account is currently blocked from any debits.")
			};

			// ** Act

			account.WithdrawCash(withdrawal);

			// ** Assert

			account.GetUncommittedEvents().Should()
				.BeEquivalentTo(expectedEvents, options =>
					options
						.RespectingRuntimeTypes()
						.Excluding(x => x.Id)
						.Using<Instant>(ctx => ctx.Subject.Should().BeCloseTo(ctx.Expectation, Duration.FromMilliseconds(250))).WhenTypeIs<Instant>()
				);
		}

		// ** Cash Transfer ********************************************************************************

		[Fact()]
		[Trait("Class", nameof(Account))]
		[Trait("Method", nameof(Account.TransferCash))]
		public void When_CashIsTransferredFromAccountWithEnoughFunds_Expect_RequestToSucceed() {
			// ** Arrange

			var reviveEvents = new IEvent<Guid>[] {
				new AccountCreated(_dataFixture.DefaultAccountId, _dataFixture.DefaultCustomerName, _dataFixture.DefaultCurrency),
				new AccountDailyWireTransferLimitChanged(_dataFixture.DefaultAccountId, new Money(100m, _dataFixture.DefaultCurrency)),
				new AccountCashDeposited(_dataFixture.DefaultAccountId, new Money(100m, _dataFixture.DefaultCurrency), SystemClock.Instance.GetCurrentInstant())
			};

			var account = new Account(reviveEvents);
			account.ClearUncommittedEvents();

			var transfer = new Money(75m, _dataFixture.DefaultCurrency);

			var expectedEvents = new IEvent<Guid>[] {
				new AccountCashTransferred(_dataFixture.DefaultAccountId, transfer, SystemClock.Instance.GetCurrentInstant())
			};

			// ** Act

			account.TransferCash(transfer);

			// ** Assert

			account.GetUncommittedEvents().Should()
				.BeEquivalentTo(expectedEvents, options =>
					options
						.RespectingRuntimeTypes()
						.Excluding(x => x.Id)
						.Using<Instant>(ctx => ctx.Subject.Should().BeCloseTo(ctx.Expectation, Duration.FromMilliseconds(250))).WhenTypeIs<Instant>()
				);
		}

		[Fact()]
		[Trait("Class", nameof(Account))]
		[Trait("Method", nameof(Account.TransferCash))]
		public void When_CashIsTransferredFromAccountThatDoesNotHaveEnoughFunds_Expect_RequestToFail() {
			// ** Arrange

			var reviveEvents = new IEvent<Guid>[] {
				new AccountCreated(_dataFixture.DefaultAccountId, _dataFixture.DefaultCustomerName, _dataFixture.DefaultCurrency),
				new AccountCashDeposited(_dataFixture.DefaultAccountId, new Money(50m, _dataFixture.DefaultCurrency), SystemClock.Instance.GetCurrentInstant())
			};

			var account = new Account(reviveEvents);
			account.ClearUncommittedEvents();

			var transfer = new Money(75m, _dataFixture.DefaultCurrency);

			var expectedEvents = new IEvent<Guid>[] {
				new AccountCashTransferRejected(_dataFixture.DefaultAccountId, transfer, SystemClock.Instance.GetCurrentInstant(), "Account does not have sufficient funds.")
			};

			// ** Act

			account.TransferCash(transfer);

			// ** Assert

			account.GetUncommittedEvents().Should()
				.BeEquivalentTo(expectedEvents, options =>
					options
						.RespectingRuntimeTypes()
						.Excluding(x => x.Id)
						.Using<Instant>(ctx => ctx.Subject.Should().BeCloseTo(ctx.Expectation, Duration.FromMilliseconds(250))).WhenTypeIs<Instant>()
				);
		}

		[Fact()]
		[Trait("Class", nameof(Account))]
		[Trait("Method", nameof(Account.TransferCash))]
		public void When_CashIsTransferredFromAccountThatHasEnoughFundsButAmountIsMoreThanDailyLimit_Expect_RequestToFail() {
			// ** Arrange

			var dailyLimit = new Money(100m, _dataFixture.DefaultCurrency);

			var reviveEvents = new IEvent<Guid>[] {
				new AccountCreated(_dataFixture.DefaultAccountId, _dataFixture.DefaultCustomerName, _dataFixture.DefaultCurrency),
				new AccountDailyWireTransferLimitChanged(_dataFixture.DefaultAccountId, dailyLimit),
				new AccountCashDeposited(_dataFixture.DefaultAccountId, new Money(200m, _dataFixture.DefaultCurrency), SystemClock.Instance.GetCurrentInstant())
			};

			var account = new Account(reviveEvents);
			account.ClearUncommittedEvents();

			// Make sure the request is more than limit
			var transfer = new Money(dailyLimit.Amount + 75m, dailyLimit.Currency);

			var expectedEvents = new IEvent<Guid>[] {
				new AccountCashTransferRejected(_dataFixture.DefaultAccountId, transfer, SystemClock.Instance.GetCurrentInstant(), "Cannot transfer funds in amounts that total greater than daily limit.")
			};

			// ** Act

			account.TransferCash(transfer);

			// ** Assert

			account.GetUncommittedEvents().Should()
				.BeEquivalentTo(expectedEvents, options =>
					options
						.RespectingRuntimeTypes()
						.Excluding(x => x.Id)
						.Using<Instant>(ctx => ctx.Subject.Should().BeCloseTo(ctx.Expectation, Duration.FromMilliseconds(250))).WhenTypeIs<Instant>()
				);
		}

		[Fact()]
		[Trait("Class", nameof(Account))]
		[Trait("Method", nameof(Account.TransferCash))]
		public void When_CashIsTransferredFromAccountThatHasEnoughFundsButMultipleTransfersOnSameDayExceedDailyLimit_Expect_RequestToFail() {
			// ** Arrange

			var dailyLimit = new Money(100m, _dataFixture.DefaultCurrency);

			var reviveEvents = new IEvent<Guid>[] {
				new AccountCreated(_dataFixture.DefaultAccountId, _dataFixture.DefaultCustomerName, _dataFixture.DefaultCurrency),
				new AccountDailyWireTransferLimitChanged(_dataFixture.DefaultAccountId, dailyLimit),
				new AccountCashDeposited(_dataFixture.DefaultAccountId, new Money(200m, _dataFixture.DefaultCurrency), SystemClock.Instance.GetCurrentInstant()),
				new AccountCashTransferred(_dataFixture.DefaultAccountId, new Money(50m, _dataFixture.DefaultCurrency), SystemClock.Instance.GetCurrentInstant())
			};

			var account = new Account(reviveEvents);
			account.ClearUncommittedEvents();

			// 50 (first request) + 75 (this request) > 100 (limit)
			var transfer = new Money(75m, dailyLimit.Currency);

			var expectedEvents = new IEvent<Guid>[] {
				new AccountCashTransferRejected(_dataFixture.DefaultAccountId, transfer, SystemClock.Instance.GetCurrentInstant(), "Cannot transfer funds in amounts that total greater than daily limit.")
			};

			// ** Act

			account.TransferCash(transfer);

			// ** Assert

			account.GetUncommittedEvents().Should()
				.BeEquivalentTo(expectedEvents, options =>
					options
						.RespectingRuntimeTypes()
						.Excluding(x => x.Id)
						.Using<Instant>(ctx => ctx.Subject.Should().BeCloseTo(ctx.Expectation, Duration.FromMilliseconds(250))).WhenTypeIs<Instant>()
				);
		}


		[Fact()]
		[Trait("Class", nameof(Account))]
		[Trait("Method", nameof(Account.TransferCash))]
		public void When_CashIsTransferredFromAccountThatHasEnoughFundsAndMultipleTransfersButTransfersWereOnAnotherDay_Expect_RequestToSucceed() {
			// ** Arrange

			var now = SystemClock.Instance.GetCurrentInstant();
			var twoDaysAgo = now.Minus(Duration.FromDays(2));

			var dailyLimit = new Money(100m, _dataFixture.DefaultCurrency);

			var reviveEvents = new IEvent<Guid>[] {
				new AccountCreated(_dataFixture.DefaultAccountId, _dataFixture.DefaultCustomerName, _dataFixture.DefaultCurrency),
				new AccountDailyWireTransferLimitChanged(_dataFixture.DefaultAccountId, dailyLimit),
				new AccountCashDeposited(_dataFixture.DefaultAccountId, new Money(200m, _dataFixture.DefaultCurrency), SystemClock.Instance.GetCurrentInstant()),
				new AccountCashTransferred(_dataFixture.DefaultAccountId, new Money(50m, _dataFixture.DefaultCurrency), twoDaysAgo)
			};

			var account = new Account(reviveEvents);
			account.ClearUncommittedEvents();

			// 50 (first request) + 75 (this request) > 100 (limit)
			var transfer = new Money(75m, dailyLimit.Currency);

			var expectedEvents = new IEvent<Guid>[] {
				new AccountCashTransferred(_dataFixture.DefaultAccountId, transfer, now)
			};

			// ** Act

			account.TransferCash(transfer);

			// ** Assert

			account.GetUncommittedEvents().Should()
				.BeEquivalentTo(expectedEvents, options =>
					options
						.RespectingRuntimeTypes()
						.Excluding(x => x.Id)
						.Using<Instant>(ctx => ctx.Subject.Should().BeCloseTo(ctx.Expectation, Duration.FromMilliseconds(250))).WhenTypeIs<Instant>()
				);
		}


		[Fact()]
		[Trait("Class", nameof(Account))]
		[Trait("Method", nameof(Account.TransferCash))]
		public void When_CashIsTransferredFromBlockedAccount_Expect_RequestToFail() {
			// ** Arrange

			var checkDepositedOn = SystemClock.Instance.GetCurrentInstant().Minus(Duration.FromDays(3));

			var depositAmount = 75m;

			var reviveEvents = new IEvent<Guid>[] {
				new AccountCreated(_dataFixture.DefaultAccountId, _dataFixture.DefaultCustomerName, _dataFixture.DefaultCurrency),
				new AccountCashDeposited(_dataFixture.DefaultAccountId, new Money(depositAmount, _dataFixture.DefaultCurrency), SystemClock.Instance.GetCurrentInstant()),
				// Trigger blocking
				new AccountCashWithdrawalRejected(_dataFixture.DefaultAccountId, new Money(depositAmount + 25m, _dataFixture.DefaultCurrency), SystemClock.Instance.GetCurrentInstant())
			};

			var account = new Account(reviveEvents);
			account.ClearUncommittedEvents();

			var transfer = new Money(depositAmount - 25m, _dataFixture.DefaultCurrency);

			var expectedEvents = new IEvent<Guid>[] {
				new AccountCashTransferRejected(_dataFixture.DefaultAccountId, transfer, SystemClock.Instance.GetCurrentInstant(), "Account is currently blocked from any debits.")
			};

			// ** Act

			account.TransferCash(transfer);

			// ** Assert

			account.GetUncommittedEvents().Should()
				.BeEquivalentTo(expectedEvents, options =>
					options
						.RespectingRuntimeTypes()
						.Excluding(x => x.Id)
						.Using<Instant>(ctx => ctx.Subject.Should().BeCloseTo(ctx.Expectation, Duration.FromMilliseconds(250))).WhenTypeIs<Instant>()
				);
		}

	}

}
