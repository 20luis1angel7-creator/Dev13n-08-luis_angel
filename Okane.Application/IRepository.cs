namespace Okane.Application;

public interface IRepository<T> where T : IEntity
{
    void Add(T entity);
    T? ById(int id);
    IEnumerable<T> All();
    void Remove(int id);
    bool Exists(int id);

    
}