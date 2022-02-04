namespace Bank.Shared.Commands {

	using Bank.Shared.Domain.ValueObjects;

	public class CreateAccount {

		// Does the idea of "no setters" also apply to Commands or just Domain model?
		public Guid Id { get; set; }

		public FullName Name { get; set; }

	}

}
