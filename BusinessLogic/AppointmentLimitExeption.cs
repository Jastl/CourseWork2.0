namespace BusinessLogic
{
    public class AppointmentLimitException : Exception
    {
        public AppointmentLimitException(string message) : base(message) { }
    }
}