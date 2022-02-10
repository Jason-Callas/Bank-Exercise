namespace Bank.Shared.Exceptions {

	public class InvalidCurrencyException :
		Exception {

		public InvalidCurrencyException() {
		}

		public InvalidCurrencyException(string? message) : base(message) {
		}

		public InvalidCurrencyException(string? message, Exception? innerException) : base(message, innerException) {
		}

	}

}
