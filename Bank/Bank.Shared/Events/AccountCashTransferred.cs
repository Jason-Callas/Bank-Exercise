﻿namespace Bank.Shared.Events {

	using Ardalis.GuardClauses;
	using Bank.Shared.Domain.ValueObjects;
	using NodaTime;

	public class AccountCashTransferred :
		AccountTransaction {

		public AccountCashTransferred(Guid accountId, Money amount, Instant when) : base(accountId, when) {
			Amount = Guard.Against.Null(amount);
		}

		public Money Amount { get; }

	}

}