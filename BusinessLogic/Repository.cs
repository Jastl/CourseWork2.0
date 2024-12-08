namespace BusinessLogic
{
    public class Repository<T> : IRepository<T> where T : IEntity
    {
        private readonly List<T> _items = new();

        public void Add(T entity)
        {
            if (_items.Any(e => e.Id == entity.Id))
                throw new InvalidOperationException("Entity with the same ID already exists.");
            _items.Add(entity);
        }

        public void Remove(Guid id)
        {
            var entity = GetById(id);
            _items.Remove(entity);
        }

        public T GetById(Guid id)
        {
            return _items.FirstOrDefault(e => e.Id == id) ?? throw new KeyNotFoundException("Entity not found.");
        }

        public IEnumerable<T> GetAll() => _items;
    }
}
