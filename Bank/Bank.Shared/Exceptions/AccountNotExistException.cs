namespace Bank.Shared.Exceptions {

	public class AccountNotExistException :
		Exception {

		public AccountNotExistException(Guid id) {
			Id = id;
		}

		public AccountNotExistException(Guid id, string? message) : base(message) {
			Id = id;
		}

		public AccountNotExistException(Guid id, string? message, Exception? innerException) : base(message, innerException) {
			Id = id;
		}

		public Guid Id { get; }

	}

}
