namespace Bank.Shared.Repositories {

	// An argument could be made for creating a specific interface for read-only queries but
	// for this exercise it may not be needed.
	public interface IRepository<T> {

		Task<T> CreateAsync(T entity);

	}

}
