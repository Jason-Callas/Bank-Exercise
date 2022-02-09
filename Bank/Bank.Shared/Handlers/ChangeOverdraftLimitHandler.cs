namespace Bank.Shared.Handlers {

	using System.Threading;
	using System.Threading.Tasks;
	using Ardalis.GuardClauses;
	using Bank.Shared.Commands;
	using Bank.Shared.Domain.ValueObjects;
	using Bank.Shared.Exceptions;
	using Bank.Shared.Repositories;
	using MediatR;

	public class ChangeOverdraftLimitHandler :
		ICommandHandler<ChangeOverdraftLimit> {

		private readonly IAccountRepository _accountRepo;

		public ChangeOverdraftLimitHandler(IAccountRepository accountRepo) {
			_accountRepo = Guard.Against.Null(accountRepo);
		}

		public async Task<Unit> Handle(ChangeOverdraftLimit request, CancellationToken cancellationToken) {
			Guard.Against.Null(request);

			var existingAccount = await _accountRepo.GetByIdAsync(request.Id);
			if (existingAccount is null) {
				throw new AccountNotExistException(request.Id, "Unable to set overdraft limit on account that does not exist.");
			}

			await existingAccount.SetOverdraftLimit(new Money(request.Amount, request.Currency));

			await _accountRepo.UpdateAsync(existingAccount);

			return Unit.Value;
		}

	}

}
