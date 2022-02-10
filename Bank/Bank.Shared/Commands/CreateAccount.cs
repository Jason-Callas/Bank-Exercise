namespace Bank.Shared.Commands {

	public class CreateAccount :
		ICommand {

		// Does the idea of "no setters" also apply to Commands or just Domain model? It would make sense to
		// only allow ctor parameters since validation can be applied.

		public Guid Id { get; set; } = Guid.NewGuid();

		public string Name { get; set; } = default!;

		// Should Overdraft and Dail Wire Limit values be set during creation? We definitely need
		// methods that allow them to be changed which could also be used for setting "initial" values.

		public string Currency { get; set; }

	}

}
