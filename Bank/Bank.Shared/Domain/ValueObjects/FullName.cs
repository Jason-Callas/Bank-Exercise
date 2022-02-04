namespace Bank.Shared.Domain.ValueObjects {

	using System.Diagnostics.CodeAnalysis;

	public struct FullName :
		IEquatable<FullName> {

		public static bool operator ==(FullName left, FullName right) {
			return left.Equals(right);
		}

		public static bool operator !=(FullName left, FullName right) {
			return !(left == right);
		}

		public FullName(string name) {
			if (string.IsNullOrWhiteSpace(name)) {
				throw new ArgumentNullException(nameof(name));
			}

			// Unsure of this idea of appling validation here for several reasons.
			//   1. What is wrong with DataAnnotations which is built into .NET?
			//   2. Why do we need to go "read through code" to find out the allowed values? Using an attribute puts
			//        the rules in a single place for easy look up.
			//   3. How does this get documented for external use? Using attributes allows docs to be auto-generated.
			//   4. How can this be re-used but with a different length value? Here it is hard-coded for 30 characters but
			//        this means I cannot use it for another property that needs just 25 characters.
			//
			// Also begs the question if the validation characteristics (required, length, min or max values, etc.) should be
			// here on the type or back with the property where the type is used.
			if (name.Length > 30) {
				throw new ArgumentOutOfRangeException(nameof(name), "The length of the value must be 30 characters or less.");
			}

			Value = name;
		}

		public bool Equals(FullName other) {
			return Value == other.Value;
		}

		public override bool Equals([NotNullWhen(true)] object? obj) {
			if (obj is null) {
				return false;
			}

			return obj is FullName name && Equals(name);
		}

		public override int GetHashCode() {
			unchecked {
				return (Value != null ? Value.GetHashCode() : 0) * 397;
			}
		}

		public string Value { get; }


	}

}
