namespace Bank.Shared.Handlers {

	using System.Threading;
	using System.Threading.Tasks;
	using Ardalis.GuardClauses;
	using Bank.Shared.Domain.Entities;
	using Bank.Shared.Queries;
	using Linedata.Foundation.Domain.EventSourcing;

	public class QueryAccountByIdHandler :
		IQueryHandler<GetAccountById, Account> {

		private readonly IEventSourcedRepository<Account> _accountRepo;

		public QueryAccountByIdHandler(IEventSourcedRepository<Account> accountRepo) {
			_accountRepo = Guard.Against.Null(accountRepo);
		}

		public async Task<Account> Handle(GetAccountById request, CancellationToken cancellationToken) {
			Guard.Against.Null(request);

			return await _accountRepo.FindAsync(request.Id);
		}

	}

}
