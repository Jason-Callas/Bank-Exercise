namespace Bank.Shared.Domain.ValueObjects {

	using Ardalis.GuardClauses;

	public class Money :
		ValueObject {

		public Money(decimal amount, string currency) {
			// Unsure if blank currency should be allowed..._Probably_ not...
			Amount = amount;
			Currency = Guard.Against.NullOrWhiteSpace(currency, nameof(currency));
		}

		protected override IEnumerable<object> GetEqualityComponents() {
			yield return Amount;
			yield return Currency.ToUpper();
		}

		public decimal Amount { get; protected set; }

		public string Currency { get; protected set; }

	}

}
