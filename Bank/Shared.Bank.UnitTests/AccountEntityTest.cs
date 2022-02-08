namespace Shared.Bank.UnitTests {

	using System;
	using System.Threading.Tasks;
	using FluentAssertions;
	using global::Bank.Shared.Domain.Entities;
	using global::Bank.Shared.Domain.ValueObjects;
	using Xunit;

	public class AccountEntityTest {

		[Fact()]
		[Trait("Class", nameof(Account))]
		[Trait("Method", nameof(Account.CreateAsync))]
		public async Task When_CreateWithValidParametersIsCalled_Expect_EntityToBeReturned() {
			// ** Arrange

			var accountId = System.Guid.Empty;
			var accountName = "Joe Dirt";

			var expected = new Account(accountId, accountName);

			// ** Act

			var actual = await Account.CreateAsync(accountId, accountName);

			// ** Assert

			actual.Should()
				.BeEquivalentTo(expected);
		}

		[Fact()]
		[Trait("Class", nameof(Account))]
		[Trait("Method", nameof(Account.CreateAsync))]
		public void When_CreateWithInvalidParametersIsCalled_Expect_ExceptionToBeThrown() {
			// ** Arrange

			var accountId = System.Guid.Empty;
			var accountName = "Joe Dirt with a name that is too long to fit in the property but I need more words...";

			// ** Act

			Func<Task> act = async () => await Account.CreateAsync(accountId, accountName);

			// ** Assert

			// Pretty sure I need to review/adjust validation handling and the types of exceptions thrown
			act.Should()
				.ThrowAsync<ArgumentOutOfRangeException>();
		}

		[Fact()]
		[Trait("Class", nameof(Account))]
		[Trait("Method", nameof(Account.CreateAsync))]
		public async Task When_AccountOverdraftLimitIsChangedToPositiveValue_Expect_ChangeToBeAccepted() {
			// ** Arrange

			var account = await Account.CreateAsync(System.Guid.Empty, "Joe Dirt");

			var newLimit = new Money(300m, "USD");

			// ** Act

			Func<Task> act = async () => await account.SetOverdraftLimit(newLimit);

			// ** Assert

			// Not really needed as a check but does show explicit "no exception thrown" result
			await act.Should()
				.NotThrowAsync();
		}

		[Fact()]
		[Trait("Class", nameof(Account))]
		[Trait("Method", nameof(Account.CreateAsync))]
		public async Task When_AccountOverdraftLimitIsChangedToZero_Expect_ChangeToBeAccepted() {
			// ** Arrange

			var account = await Account.CreateAsync(System.Guid.Empty, "Joe Dirt");

			var newLimit = new Money(0m, "USD");

			// ** Act

			Func<Task> act = async () => await account.SetOverdraftLimit(newLimit);

			// ** Assert

			// Not really needed as a check but does show explicit "no exception thrown" result
			await act.Should()
				.NotThrowAsync();
		}

		[Fact()]
		[Trait("Class", nameof(Account))]
		[Trait("Method", nameof(Account.CreateAsync))]
		public async Task When_AccountOverdraftLimitIsChangedToNegativeValue_Expect_ExceptionToBeThrown() {
			// ** Arrange

			var account = await Account.CreateAsync(System.Guid.Empty, "Joe Dirt");

			var newLimit = new Money(-250m, "USD");        // negative value not allowed

			// ** Act

			Func<Task> act = async () => await account.SetOverdraftLimit(newLimit);

			// ** Assert

			await act.Should()
				.ThrowAsync<ArgumentException>();
		}


	}

}
