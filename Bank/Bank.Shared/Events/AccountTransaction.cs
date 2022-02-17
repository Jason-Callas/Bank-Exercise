using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NodaTime;

namespace Bank.Shared.Events {

	/// <summary>
	/// Base event for anything that is considered a transaction (credit, debit) for an account.
	/// </summary>
	public class AccountTransaction :
		EventBase<Guid> {

		public AccountTransaction(Guid aggregateId, Instant when) : base(aggregateId) {
			When = when;
		}

		public Instant When { get; }

	}

}
