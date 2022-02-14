using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Ardalis.GuardClauses;
using Bank.Shared.Domain.Entities;
using Bank.Shared.Events;
using EventStore.Client;

namespace Bank.Shared.Repositories {

	/// <summary>
	/// Implementation of IAccountRepository that reads and writes to Event Store DB.
	/// </summary>
	public class AccountRepository :
		IAccountRepository {

		private readonly EventStoreClient _client;

		private static string GenerateStreamName(Guid id) {
			return $"{nameof(Account)}-{id}".ToLower();
		}

		public AccountRepository(EventStoreClient client) {
			_client = client ?? throw new ArgumentNullException(nameof(client));
		}

		// This repo can probably can abstracted to support any type since the flow is the
		// same for anything.
		protected async Task Store(Account account) {
			var events = account.GetUncommittedEvents();

			var payload = events.Select(e => new EventData(Uuid.FromGuid(e.Id), e.EventName.ToLower(), Encoding.UTF8.GetBytes(JsonSerializer.Serialize(e)))).ToArray();

			await _client.AppendToStreamAsync(
				GenerateStreamName(account.Id),
				StreamState.Any,
				payload,
				null
				);

			account.ClearUncommittedEvents();
		}

		public async Task<Account> CreateAsync(Account entity) {
			Guard.Against.Null(entity);

			await Store(entity);

			return entity;
		}

		public async Task<Account> GetByIdAsync(Guid id) {
			var stream = _client.ReadStreamAsync(Direction.Forwards, GenerateStreamName(id), StreamPosition.Start);

			var events = new List<IEvent<Guid>>();
			await foreach (var @event in stream) {
				var jsonPayload = Encoding.UTF8.GetString(@event.Event.Data.ToArray());

				// Really do not like the use if switch statement...wonder if there is some sort of pattern matching
				// or more abstract way we can do this...
				//
				// Even worse is the use of "magic strings"...
				//
				// Does System.Text.Json have a way to deserialize to a type as a string as opposed to using Generics???
				switch (@event.Event.EventType) {
					case "accountcreated":
						events.Add(JsonSerializer.Deserialize<AccountCreated>(jsonPayload));
						break;
					case "accountcashdeposited":
						events.Add(JsonSerializer.Deserialize<AccountCashDeposited>(jsonPayload));
						break;
					case "accountcheckdeposited":
						events.Add(JsonSerializer.Deserialize<AccountCheckDeposited>(jsonPayload));
						break;
					case "accountcashwithdrawn":
						events.Add(JsonSerializer.Deserialize<AccountCashWithdrawn>(jsonPayload));
						break;
					case "accountcashWithdrawalrejected":
						events.Add(JsonSerializer.Deserialize<AccountCashWithdrawalRejected>(jsonPayload));
						break;
					case "accountdailywiretransferlimitchanged":
						events.Add(JsonSerializer.Deserialize<AccountDailyWireTransferLimitChanged>(jsonPayload));
						break;
					case "accountoverdraftlimitvhanged":
						events.Add(JsonSerializer.Deserialize<AccountOverdraftLimitChanged>(jsonPayload));
						break;
					default:
						throw new InvalidOperationException($"Event '{@event.Event.EventType}' is not supported during event store instantiation.");
				}

			}

			return new Account(events);
		}

		public async Task<Account> UpdateAsync(Account entity) {
			Guard.Against.Null(entity);

			await Store(entity);

			return entity;
		}

	}

}
