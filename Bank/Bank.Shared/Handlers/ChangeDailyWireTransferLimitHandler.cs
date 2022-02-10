namespace Bank.Shared.Handlers {

	using System.Threading;
	using System.Threading.Tasks;
	using Ardalis.GuardClauses;
	using Bank.Shared.Commands;
	using Bank.Shared.Domain.ValueObjects;
	using Bank.Shared.Exceptions;
	using Bank.Shared.Repositories;
	using MediatR;

	public class ChangeDailyWireTransferLimitHandler :
		ICommandHandler<ChangeDailyTransferLimit> {

		private readonly IAccountRepository _accountRepo;

		public ChangeDailyWireTransferLimitHandler(IAccountRepository accountRepo) {
			_accountRepo = Guard.Against.Null(accountRepo);
		}

		public async Task<Unit> Handle(ChangeDailyTransferLimit request, CancellationToken cancellationToken) {
			Guard.Against.Null(request);

			var existingAccount = await _accountRepo.GetByIdAsync(request.AccountId);
			if (existingAccount is null) {
				throw new AccountNotExistException(request.AccountId, "Unable to set daily wire transfer limit on account that does not exist.");
			}

			existingAccount.SetDailyWireTransferLimit(new Money(request.Amount, request.Currency));

			await _accountRepo.UpdateAsync(existingAccount);

			return Unit.Value;
		}

	}

}
