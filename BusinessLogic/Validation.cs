namespace BusinessLogic
{
    public static class Validation
    {
        public static void ValidateName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Ім'я не може бути порожнім.");
            if (name.Any(char.IsDigit))
                throw new ArgumentException("Ім'я не може містити цифри.");
        }

        public static void ValidateDateOfBirth(DateTime dateOfBirth)
        {
            if (dateOfBirth > DateTime.Now)
                throw new ArgumentException("Дата народження не може бути в майбутньому.");
        }

        public static void ValidateAppointmentDate(DateTime appointmentDate)
        {
            if (appointmentDate < DateTime.Now)
                throw new ArgumentException("Час прийому не може бути в минулому.");
        }
    }
}