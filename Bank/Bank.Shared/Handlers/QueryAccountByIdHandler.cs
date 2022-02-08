namespace Bank.Shared.Handlers {

	using System.Threading;
	using System.Threading.Tasks;
	using Ardalis.GuardClauses;
	using Bank.Shared.Domain.Entities;
	using Bank.Shared.Queries;
	using Bank.Shared.Repositories;

	public class QueryAccountByIdHandler :
		IQueryHandler<GetAccountById, Account> {

		private readonly IAccountRepository _accountRepo;

		public QueryAccountByIdHandler(IAccountRepository accountRepo) {
			_accountRepo = Guard.Against.Null(accountRepo);
		}

		public async Task<Account> Handle(GetAccountById request, CancellationToken cancellationToken) {
			Guard.Against.Null(request);

			return await _accountRepo.GetByIdAsync(request.Id);
		}

	}

}
