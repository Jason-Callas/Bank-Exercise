namespace Bank.Shared.Domain.UnitTests {

	using System;
	using FluentAssertions;
	using Bank.Shared.Domain.Entities;
	using Bank.Shared.Domain.ValueObjects;
	using Xunit;
	using Bank.Shared.Events;
	using Bank.Shared.UnitTests.Fixtures;
	using Bank.Shared.Exceptions;

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
						.Excluding(x => x.Id)
						.Excluding(x => x.TimestampUtc)
				);
		}

		[Fact()]
		[Trait("Class", nameof(Account))]
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
						.Excluding(x => x.Id)
						.Excluding(x => x.TimestampUtc)
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
						.Excluding(x => x.Id)
						.Excluding(x => x.TimestampUtc)
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
						.Excluding(x => x.Id)
						.Excluding(x => x.TimestampUtc)
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
						.Excluding(x => x.Id)
						.Excluding(x => x.TimestampUtc)
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

	}

}
