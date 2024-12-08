namespace BusinessLogic
{
    public class Appointment : IEntity
    {
        public Guid Id { get; } = Guid.NewGuid();
        public Guid DoctorId { get; set; }
        public Guid PatientId { get; set; }
        public DateTime Date { get; set; }
    }
}