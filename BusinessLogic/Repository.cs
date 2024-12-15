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

        public void Remove(int index)
        {
            try
            {
                _items.Remove(_items[index]);
            }
            catch (ArgumentOutOfRangeException)
            {
                Console.WriteLine("Індексу не існує.");
            }
        }

        public T GetById(Guid id)
        {
            return _items.FirstOrDefault(e => e.Id == id) ?? throw new KeyNotFoundException("Entity not found.");
        }

        public List<T> GetAll() => _items;

        public int IndexOf(T item) => _items.IndexOf(item);
    }
}
