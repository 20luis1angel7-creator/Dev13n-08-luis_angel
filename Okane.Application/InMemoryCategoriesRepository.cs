namespace Okane.Application;

public class InMemoryCategoriesRepository : InMemoryRepository<Category>, ICategoriesRepository
{
    public Category ByName(string name)
    {
        var category = Entities.FirstOrDefault(c => c.Name == name);
        
        //Esta sección garantiza que ByName siempre devuelva una categoría válida o lance un error controlado si no existe.
        // Junto con el try/catch en Update, asegura que nunca se intente actualizar un gasto con una categoría inexistente.
        if (category == null)//Si category es null (no existe una categoría con ese nombre), lanza una excepción KeyNotFoundException.
            throw new KeyNotFoundException($"No se encontró una categoría con el nombre: {name}");

        return category;//devuelve la categoria encontrada
    }

    //public Category ByName(string name) => 
    //    Entities.First(category => category.Name == name);
}