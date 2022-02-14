using Bank.Shared.Domain.Entities;

namespace Bank.Shared.Repositories {

	// An argument could be made for creating a specific interface for read-only queries but
	// for this exercise it may not be needed.
	public interface IRepository<TEntity, TId>
		where TEntity : IAggregateRoot {

		Task<TEntity> CreateAsync(TEntity entity);

		Task<TEntity> GetByIdAsync(TId id);

		Task<TEntity> UpdateAsync(TEntity entity);

		Task DeleteAsync(TId id);

	}

}
