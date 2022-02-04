namespace Bank.Shared.Domain.ValueObjects {

	using System.Diagnostics.CodeAnalysis;

	public struct Money :
		IEquatable<Money> {

		public static bool operator ==(Money left, Money right) {
			return left.Equals(right);
		}

		public static bool operator !=(Money left, Money right) {
			return !(left == right);
		}

		public Money(decimal amount, string currency) {
			// Unsure if blank currency should be allowed..._Probably_ not...
			Amount = amount;
			Currency = currency;
		}

		public bool Equals(Money other) {
			return Amount == other.Amount && Currency == other.Currency;
		}

		public override bool Equals([NotNullWhen(true)] object? obj) {
			if (obj is null) {
				return false;
			}

			return obj is Money money && Equals(money);
		}

		public override int GetHashCode() {
			return HashCode.Combine(Amount, Currency);
		}

		public decimal Amount { get; }

		public string Currency { get; }

	}

}
