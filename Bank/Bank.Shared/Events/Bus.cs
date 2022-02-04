namespace Bank.Shared.Events {

	public class Bus {

		public static async Task RaiseEventAsync<T>(T evt)
			where T : IEvent {

			if (evt is null) {
				throw new ArgumentNullException(nameof(evt));
			}

			// Do whatever... I am unsure if I should code out an (fake) event bus or use a third party like ZeroMQ

			await Task.CompletedTask;
		}

	}

}
