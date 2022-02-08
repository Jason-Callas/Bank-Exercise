namespace Bank.Shared.Domain.Entities {

	// Since an aggregate root **has** to be an entiy, I wonder if it makes sense to have IAggregateRoot
	// inherit from IEntity<T>. The "problem" is that this interface would have to implement T also and that
	// _seems_ unclean. (Why should the agrregate root marker have to care about about type Id type...)
	//
	// We _could_ also create a non-generate IAggregateRoot for the marker but then are we creating too many abstractions??
	public interface IAggregateRoot { }

}
