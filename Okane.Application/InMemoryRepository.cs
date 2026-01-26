using System.Diagnostics.CodeAnalysis;
using System.IO;
namespace Okane.Application;

public class InMemoryRepository<T> : IRepository<Expense>
{
    private int _lastId;
    private readonly List<Expense> _entities = [];

    public void Add(Expense entity)
    {
        entity.Id = ++_lastId;
        _entities.Add(entity);
    }

    public Expense? ById(int id) => 
        _entities.FirstOrDefault(x => x.Id == id);

    public IEnumerable<Expense> All() => _entities;

    public void Remove(int id)
    {
        var existing = _entities.First(x => x.Id == id);
        _entities.Remove(existing);
    }

    public bool Exists(int id) => 
        _entities.Any(x => x.Id == id);

    public Expense Update(int id, UpdateExpenseRequest request)
    {
        var existing = _entities.First(e => e.Id == id);
        
        existing.Amount = request.Amount;
        existing.CategoryName = request.CategoryName;

        return existing;
    }
    
}