namespace BusinessLogic
{
    public interface IRepository<T> where T : IEntity
    {
        void Add(T entity);
        void Remove(int index);
        T GetById(Guid id);
        List<T> GetAll();
    }
}