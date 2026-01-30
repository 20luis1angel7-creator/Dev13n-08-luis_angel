namespace Okane.Application;

public class InMemoryExpensesRepository : InMemoryRepository<Expense>, IExpensesRepository
{
    public Expense Update(int id, UpdateExpenseRequest request, Category category)
    {
        //FirstOrDefault: sirve para buscar el primer elemento que cumpla una condición y manejar el caso de que no exista de manera controlada.
        var existing = Entities.FirstOrDefault(e => e.Id == id);
        
        existing.Amount = request.Amount;
        existing.Category = category;

        return existing;
    }
}