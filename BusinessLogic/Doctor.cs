namespace BusinessLogic
{
    public class Doctor : IEntity
    {
        public Guid Id { get; } = Guid.NewGuid();
        public string Name { get; set; }
        public Specialization Specialization { get; set; }
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
}
