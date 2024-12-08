namespace BusinessLogic
{
    public interface IRepository<T> where T : IEntity
    {
        void Add(T entity);
        void Remove(Guid id);
        T GetById(Guid id);
        IEnumerable<T> GetAll();
    }
}