namespace Bank.Shared.Domain.Entities {

	// Unsure if I should bother with generic...this really exists to enforce consistent PK property naming
	public interface IEntity<T> {

		T Id { get; }

	}

}
