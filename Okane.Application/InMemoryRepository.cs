using System.Diagnostics.CodeAnalysis;

namespace Okane.Application;

public abstract class InMemoryRepository<T> : IRepository<T> where T : IEntity
{
    private int _lastId;
    protected readonly List<T> Entities = [];

    public void Add(T entity)
    {
        entity.Id = ++_lastId;
        Entities.Add(entity);
    }

    public T? ById(int id) => 
        Entities.FirstOrDefault(x => x.Id == id);

    public IEnumerable<T> All() => Entities;

    public void Remove(int id)
    {
        var existing = Entities.First(x => x.Id == id);
        Entities.Remove(existing);
    }
    //Delete es la versión “segura” de Remove, que no falla si el elemento no existe.
    public void Delete(int id)
    {
        var item = Entities.FirstOrDefault(x => x is IEntity entity && entity.Id == id);
        //x is IEntity entity → comprueba que x implementa IEntity y lo asigna a la variable entity.
        //entity.Id == id → verifica que el Id del objeto coincida con el que queremos eliminar.
        if (item != null)//Si item no es null lo elimina de la lista. Si item es null no hace nada, evitando errores.
        {
            Entities.Remove(item);
        }
    }
    //Este método expone todos los elementos del repositorio de forma segura y controlada, permitiendo solo su lectura mediante IEnumerable<T>.
    public IEnumerable<T> GetAll()//IEnumerable<T>: Encapsulación, está pensado para recorrer datos, no para modificarlos.
    {
        return Entities;
    }
    
    public bool Exists(int id) => 
        Entities.Any(x => x.Id == id);
}