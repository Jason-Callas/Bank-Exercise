namespace Bank.Shared.Handlers {

	using Ardalis.GuardClauses;
	using Bank.Shared.Commands;
	using Bank.Shared.Domain.Entities;
	using Bank.Shared.Repositories;
	using MediatR;

	public class CreateAccountHandler :
		ICommandHandler<CreateAccount> {

		private readonly IAccountRepository _accountRepo;

		public CreateAccountHandler(IAccountRepository accountRepo) {
			_accountRepo = Guard.Against.Null(accountRepo);
		}

		public async Task<Unit> Handle(CreateAccount request, CancellationToken cancellationToken) {
			Guard.Against.Null(request);

			var newAccount = new Account(request.Id, request.Name);

			// The use of Repository pattern may change after Event Source work is done. However, it is possible that
			// Repo will still stay in play with the implementation hiding the use of events.
			await _accountRepo.CreateAsync(newAccount);

			return Unit.Value;
		}

	}

}
