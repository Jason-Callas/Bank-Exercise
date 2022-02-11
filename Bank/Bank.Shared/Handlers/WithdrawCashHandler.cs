namespace Bank.Shared.Handlers {

	using System.Threading;
	using System.Threading.Tasks;
	using Ardalis.GuardClauses;
	using Bank.Shared.Commands;
	using Bank.Shared.Domain.ValueObjects;
	using Bank.Shared.Exceptions;
	using Bank.Shared.Repositories;
	using MediatR;

	public class WithdrawCashHandler :
		ICommandHandler<WithdrawCash> {

		private readonly IAccountRepository _accountRepo;

		public WithdrawCashHandler(IAccountRepository accountRepo) {
			_accountRepo = Guard.Against.Null(accountRepo);
		}

		public async Task<Unit> Handle(WithdrawCash request, CancellationToken cancellationToken) {
			Guard.Against.Null(request);

			var existingAccount = await _accountRepo.GetByIdAsync(request.AccountId);
			if (existingAccount is null) {
				throw new AccountNotExistException(request.AccountId, "Unable to process withdrawal from account that does not exist.");
			}

			existingAccount.WithdrawCash(new Money(request.Amount, request.Currency));

			await _accountRepo.UpdateAsync(existingAccount);

			return Unit.Value;
		}

	}

}
