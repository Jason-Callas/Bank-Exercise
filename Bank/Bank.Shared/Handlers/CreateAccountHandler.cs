namespace Bank.Shared.Handlers {

	using Ardalis.GuardClauses;
	using Bank.Shared.Commands;
	using Bank.Shared.Domain.Entities;
	using Linedata.Foundation.Domain.EventSourcing;
	using MediatR;

	public class CreateAccountHandler :
		ICommandHandler<CreateAccount> {

		private readonly IEventSourcedRepository<Account> _accountRepo;

		public CreateAccountHandler(IEventSourcedRepository<Account> accountRepo) {
			_accountRepo = Guard.Against.Null(accountRepo);
		}

		public async Task<Unit> Handle(CreateAccount request, CancellationToken cancellationToken) {
			Guard.Against.Null(request);

			var newAccount = new Account(request.Id, request.Name, request.Currency);

			await _accountRepo.SaveAsync(newAccount);

			return Unit.Value;
		}

	}

}
