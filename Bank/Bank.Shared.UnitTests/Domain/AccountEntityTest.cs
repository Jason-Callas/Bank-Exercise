namespace Bank.Shared.Domain.UnitTests {

	using System;
	using FluentAssertions;
	using Bank.Shared.Domain.Entities;
	using Bank.Shared.Domain.ValueObjects;
	using Xunit;
	using Bank.Shared.Events;
	using Bank.Shared.UnitTests.Fixtures;

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
				new AccountCreated(_dataFixture.DefaultAccountId, _dataFixture.DefaultCustomerName)
			};

			// ** Act

			var actual = new Account(_dataFixture.DefaultAccountId, _dataFixture.DefaultCustomerName);

			// ** Assert

			actual.UncommittedEvents.Should()
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

			Action act = () => new Account(_dataFixture.DefaultAccountId, "Joe Dirt with a name that is too long to fit in the property but I need more words...");

			// ** Assert

			// Pretty sure I need to review/adjust validation handling and the types of exceptions thrown
			act.Should()
				.Throw<ArgumentOutOfRangeException>();
		}

		[Fact()]
		[Trait("Class", nameof(Account))]
		[Trait("Method", nameof(Account.SetOverdraftLimit))]
		public void When_AccountOverdraftLimitIsChangedToValidValue_Expect_CommandToBeCompleted() {
			// ** Arrange

			var account = new Account(_dataFixture.DefaultAccountId, _dataFixture.DefaultCustomerName);
			account.ClearUncommittedEvents();

			var newLimit = new Money(300m, _dataFixture.DefaultCurrency);

			var expectedEvents = new IEvent<Guid>[] {
				new AccountOverdraftLimitChanged(_dataFixture.DefaultAccountId, newLimit)
			};

			// ** Act

			account.SetOverdraftLimit(newLimit);

			// ** Assert

			account.UncommittedEvents.Should()
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

			var account = new Account(_dataFixture.DefaultAccountId, _dataFixture.DefaultCustomerName);
			account.ClearUncommittedEvents();

			var newLimit = new Money(0m, _dataFixture.DefaultCurrency);

			var expectedEvents = new IEvent<Guid>[] {
				new AccountOverdraftLimitChanged(_dataFixture.DefaultAccountId, newLimit)
			};

			// ** Act

			account.SetOverdraftLimit(newLimit);

			// ** Assert

			account.UncommittedEvents.Should()
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

			var account = new Account(_dataFixture.DefaultAccountId, _dataFixture.DefaultCustomerName);
			account.ClearUncommittedEvents();

			var newLimit = new Money(-250m, _dataFixture.DefaultCurrency);        // negative value not allowed

			// ** Act

			Action act = () => account.SetOverdraftLimit(newLimit);

			// ** Assert

			act.Should()
				.Throw<ArgumentException>();
		}

	}

}
