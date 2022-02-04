namespace Shared.Bank.UnitTests {

	using System;
	using System.Threading.Tasks;
	using FluentAssertions;
	using global::Bank.Shared.Domain.Entities;
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

	}

}
