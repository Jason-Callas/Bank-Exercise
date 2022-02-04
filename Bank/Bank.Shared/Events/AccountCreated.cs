﻿using Bank.Shared.Domain.Entities;

namespace Bank.Shared.Events {

	public class AccountCreated :
		IEvent {

		public AccountCreated(Account account) {
			if (account is null) {
				throw new ArgumentNullException(nameof(account));
			}

			Account = account;
		}

		public Account Account { get; set; }

		public DateTime Timestamp { get; set; } = DateTime.UtcNow;

	}

}
