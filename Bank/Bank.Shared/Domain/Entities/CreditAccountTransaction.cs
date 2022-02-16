namespace Bank.Shared.Domain.Entities {

	using NodaTime;

	/// <summary>
	/// Marker class to indicate that the (succesful) transaction adds money to the account.
	/// </summary>
	internal abstract class CreditAccountTransaction :
		AccountTransaction {

		public CreditAccountTransaction(decimal amount, Instant when, bool isSuccessful = true) : base(amount, when, isSuccessful) {
		}

	}

}
