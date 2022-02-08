
namespace Bank.Shared.Queries {

	using MediatR;

	/// <summary>
	/// Marker interface to group MediatR (Query) Requests
	/// </summary>
	public class IQuery<T> :
		IRequest<T> {

	}

}
