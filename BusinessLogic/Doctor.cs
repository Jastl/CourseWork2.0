namespace BusinessLogic
{
    public class Doctor : IEntity
    {
        public Guid Id { get; } = Guid.NewGuid();
        public string Name { get; set; }
        public Specialization Specialization { get; set; }

        public DoctorSchedule WorkSchedule { get; set; } = new();
    }

    public enum Specialization
    {
        Therapist,
        Surgeon,
        Cardiologist,
        Pediatrician,
        Neurologist,
        Dentist
    }
    public class DoctorSchedule
    {
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
    }
} 

