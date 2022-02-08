namespace Bank.Shared.Handlers {

	using Bank.Shared.Queries;
	using MediatR;

	/// <summary>
	/// Marker interface to group MediatR (Query) Request Handlers
	/// </summary>
	public interface IQueryHandler<in TRequest, TResponse> :
		IRequestHandler<TRequest, TResponse>
		where TRequest : IQuery<TResponse> {

	}

}
