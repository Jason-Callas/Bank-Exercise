namespace Bank.Shared.Extensions {

	using NodaTime;

	public static class NodaTimeInstantExtensions {

		public static Instant NextBusinessDay(this Instant value) {
			// Rules for next business day are as follows
			//    - Mon-Fri 0900-1700
			//        - Mon-Thurs before 1700 is next day 0900
			//        - Fri before 1700 is Monday 0900
			//        - After 1700 is two business days away

			// ** Very, very inefficient way of doing this
			// ** Would definitely want to think through the logic to reduce the number of checks and calculations. And
			// ** we are of course not considering holidays...

			var startTime = new LocalTime(9, 0);
			var endTime = new LocalTime(17, 0);

			var utcValue = value.InUtc();

			// Start by assuming next business day is NEXT DAY
			var nextBusinessDay = utcValue + Duration.FromDays(1);

			// If the "next" day is now Saturday or Sunday then bump it up to Monday
			if (nextBusinessDay.DayOfWeek >= IsoDayOfWeek.Saturday) {
				// This works since Sat and Sun have values of 6 and 7, respectively
				nextBusinessDay += Duration.FromDays(8 - (int)nextBusinessDay.DayOfWeek);
			}

			// If time was after business closing then bump it an extra day
			//    Does not consider that period between midnight and "morning" start... Does 0200 mean we should bump another day???
			if (nextBusinessDay.TimeOfDay > endTime) {
				nextBusinessDay += Duration.FromDays(1);
			}

			// Are we again on Saturday or Sunday then bump it up to Monday
			if (nextBusinessDay.DayOfWeek >= IsoDayOfWeek.Saturday) {
				// This works since Sat and Sun have values of 6 and 7, respectively
				nextBusinessDay += Duration.FromDays(8 - (int)nextBusinessDay.DayOfWeek);
			}

			// "Truncate" time to business start
			nextBusinessDay = (nextBusinessDay.Date + startTime).InUtc();

			// Give it back
			return nextBusinessDay.ToInstant();
		}

	}

}
