namespace Okane.Application;

public class ExpensesService(
    IExpensesRepository expenses,
    ICategoriesRepository categories,
    ExpenseResponseFactory expenseResponseFactory)
{
    public Result<ExpenseResponse> Create(CreateExpenseRequest request)
    {
        if (request.Amount < 1)
            return new ErrorResult<ExpenseResponse>(
                $"{nameof(request.Amount)} must be greater than 1.");

        var category = categories.ByName(request.CategoryName);
        
        var expense = Expense(request, category);
        expenses.Add(expense);

        var response = expenseResponseFactory.Create(expense);
        return new OkResult<ExpenseResponse>(response);
    }

    public Result<ExpenseResponse> Retrieve(int expenseId)
    {
        var expense = expenses.ById(expenseId);

        if (expense is null)
            return new NotFoundResult<ExpenseResponse>(
                $"Expense with id {expenseId} was not found.");

        var response = expenseResponseFactory.Create(expense);

        return new OkResult<ExpenseResponse>(response);
    }

    public Result<IEnumerable<ExpenseResponse>> All()
    {
        var response = expenses.All()
            .Select(expenseResponseFactory.Create);

        return new OkResult<IEnumerable<ExpenseResponse>>(response);
    }

    public Result<ExpenseResponse> Update(int id, UpdateExpenseRequest request)
    {
        if (request.Amount < 1)
            return new ErrorResult<ExpenseResponse>(
                $"{nameof(request.Amount)} must be greater than 1.");

        if (!expenses.Exists(id))
            return new NotFoundResult<ExpenseResponse>(
                $"Expense with id {id} was not found.");
        //asegura que la actualización de un gasto solo ocurra si la categoría existe
        Category category;//Declara una variable category
        try
        {
            category = categories.ByName(request.CategoryName);//Intenta obtener un objeto Category
        }
        catch (KeyNotFoundException)//captura esta excepción y devuelve un ErrorResult con un mensaje claro.
        {
            return new ErrorResult<ExpenseResponse>(
                $"Category name '{request.CategoryName}' not found.");
        }
        
        
        //var category =  categories.ByName(request.CategoryName);
        
        //if (category == null)
        //    return new ErrorResult<ExpenseResponse>(
        //        $"Category name '{request.CategoryName}' not found.");/////

        var updated = expenses.Update(id, request, category);

        var response = expenseResponseFactory.Create(updated);
        return new OkResult<ExpenseResponse>(response);
    }

    public Result Delete(int id)
    {
        if (!expenses.Exists(id))
            return new NotFoundResult(
                $"Expense with id {id} was not found.");

        expenses.Remove(id);

        return new OkResult();
    }
    
    private static Expense Expense(CreateExpenseRequest request, Category category) =>
        new()
        {
            Amount = request.Amount,
            Category = category,
            Description = request.Description
        };
}