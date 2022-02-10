namespace Bank.Shared.UnitTests.Fixtures {

	using System;
	using Bank.Shared.Domain.Entities;

	public class AccountDataFixture {

		public Guid DefaultAccountId { get => Guid.Empty; }

		public string DefaultCustomerName { get => "Joe Dirt"; }

		public string DefaultCurrency { get => "GBP"; }

		private Account CreateNewAccountWithNoTransactions(Guid? id) {
			return new Account(id ?? DefaultAccountId, DefaultCustomerName, DefaultCurrency);
		}

		public Account GetNewAccount(Guid? id = null) {
			return CreateNewAccountWithNoTransactions(id);
		}

	}

}
