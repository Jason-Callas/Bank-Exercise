namespace Bank.Shared.UnitTests.Fixtures {

	using System;
	using Bank.Shared.Domain.Entities;

	public class AccountDataFixture {

		private Account CreateNewAccountWithNoTransactions(Guid? id) {
			return new Account(id ?? Guid.NewGuid(), "Joe Dirt");
		}

		public Account GetNewAccount(Guid? id = null) {
			return CreateNewAccountWithNoTransactions(id);
		}

	}

}
