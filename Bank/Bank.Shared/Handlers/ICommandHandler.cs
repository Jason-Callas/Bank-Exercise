namespace Bank.Shared.Handlers {

	using Bank.Shared.Commands;
	using MediatR;

	/// <summary>
	/// Marker interface to group MediatR (Command) Request Handlers
	/// </summary>
	public interface ICommandHandler<in TRequest> :
		IRequestHandler<TRequest, Unit>
		where TRequest : ICommand {

	}

}
