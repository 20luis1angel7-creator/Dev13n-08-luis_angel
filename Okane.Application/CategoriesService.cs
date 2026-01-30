namespace Okane.Application;

public class CategoriesService(ICategoriesRepository categories, IExpensesRepository expenses)
{
   
    public Result<CategoryResponse> Create(CreateCategoryRequest request)
    {
        //Este fragmento asegura que cada categoría tenga un nombre único antes de crearla. Es una regla de negocio esencial para mantener integridad y consistencia en la aplicación.
        // Verificar si ya existe una categoría con el mismo nombre
        var existingCategory = categories
            .All()
            .FirstOrDefault(c => c.Name.Equals(request.Name, StringComparison.OrdinalIgnoreCase));

        if (existingCategory != null)
        {
            // Si la categoría ya existe, devolver ErrorResult
            return new ErrorResult<CategoryResponse>("Category already exists");
        }

        // Si no existe, crear la categoría
        var category = new Category
        {
            Name = request.Name
        };

        categories.Add(category);
    
        return new OkResult<CategoryResponse>(new CategoryResponse(category.Id, category.Name));

        //var category = new Category
        //{
        //    Name = request.Name
        //};
        //categories.Add(category);
        //return new OkResult<CategoryResponse>(new CategoryResponse(category.Id, category.Name));
    }

    public Result<CategoryResponse> Retrieve(int id)
    {
        var category = categories.ById(id);

        if (category == null)
            return new NotFoundResult<CategoryResponse>($"{nameof(Category)} with id {id} was not found.");
        
        return new OkResult<CategoryResponse>(new CategoryResponse(category.Id, category.Name));
    }

    public Result<IEnumerable<CategoryResponse>> All()
    {
        var response = categories
            .All()
            .Select(c => new CategoryResponse(c.Id, c.Name));
        
        return new OkResult<IEnumerable<CategoryResponse>>(response);
    }

    public Result Remove(int id)
    {
        var category = categories.ById(id);

        if (category == null)
        {
            return new NotFoundResult($"Category with id {id} was not found.");
        }
        //Este fragmento asegura la integridad de los datos: no deja que elimines una categoría que aún está en uso por gastos existentes.
        //verifica si algún gasto está asociado a esa categoría.
        var hasExpenses = expenses.All()//obtiene todos los gastos
            .Any(e => e.Category.Id == id);//revisa si existe al menos un gasto que tenga esa categoria

        if (hasExpenses)//si encuentra
            return new ErrorResult("Category has expenses.");//no permite eliminar la categoria y devuelve error
        //hasta aqui
        
        categories.Remove(category.Id);
        return new OkResult();
    }

    public Result<CategoryResponse> Update(int id, UpdateCategoryRequest request)
    {
        var category = categories.ById(id);

        if (category == null)
        {
            return new NotFoundResult<CategoryResponse>($"category with id {id}  was not found.");
        }
        
        
        //previene duplicado en los nombre categorias al actualizar, asegurando que cada categoría tenga un nombre único.
        var duplicate = categories.All()//devuelve todas las categorias existente
            .FirstOrDefault(c => c.Name == request.Name && c.Id != id);//Buscamos si ya existe otra categoría con el mismo nombre y Que no sea la misma categoría que estamos actualizando 
        if (duplicate != null)//Si encontró un duplicado, devuelve un ErrorResult
            return new ErrorResult<CategoryResponse>("Ya existe una categoría con ese nombre.");

                
        category.Name = request.Name;

        //categories.Update(category);
        
        return new OkResult<CategoryResponse>(new CategoryResponse(category.Id, category.Name));
    }
}