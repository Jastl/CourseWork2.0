namespace BusinessLogic
{
    public class Patient : IEntity
    {
        public Guid Id { get; } = Guid.NewGuid();
        public string Name { get; set; }
        public DateTime DateOfBirth { get; set; }
    }
}