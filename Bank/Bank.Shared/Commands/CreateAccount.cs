namespace Bank.Shared.Commands {

	using Bank.Shared.Domain.ValueObjects;

	public class CreateAccount {

		// Does the idea of "no setters" also apply to Commands or just Domain model? It would make sense to
		// only allow ctor parameters since validation can be applied.


		public Guid Id { get; set; }

		public string Name { get; set; } = default!;

		// Should Overdraft and Dail Wire Limit values be set during creation? We definitely need
		// methods that allow them to be changed which could also be used for setting "initial" values.
	}

}
