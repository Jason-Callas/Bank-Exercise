namespace Bank.Shared.Queries {

	using Bank.Shared.Domain.Entities;

	public class GetAccountById :
		IQuery<Account> {

		public GetAccountById(Guid id) {
			Id = id;
		}

		public Guid Id { get; }

	}

}
