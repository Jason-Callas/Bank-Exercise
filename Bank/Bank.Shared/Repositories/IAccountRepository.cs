namespace Bank.Shared.Repositories {

	using Bank.Shared.Domain.Entities;

	public interface IAccountRepository :
		IRepository<Account, Guid> {

	}

}
