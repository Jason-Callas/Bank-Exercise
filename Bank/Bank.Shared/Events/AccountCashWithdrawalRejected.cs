﻿namespace Bank.Shared.Events {

	using Ardalis.GuardClauses;
	using Bank.Shared.Domain.ValueObjects;
	using NodaTime;

	public class AccountCashWithdrawalRejected :
		AccountTransaction {

		public AccountCashWithdrawalRejected(Guid accountId, Money amount, Instant when, string? reason = null) : base(accountId, when) {
			Amount = Guard.Against.Null(amount);
			Reason = reason;
		}

		public Money Amount { get; }

		public string? Reason { get; }

	}

}
