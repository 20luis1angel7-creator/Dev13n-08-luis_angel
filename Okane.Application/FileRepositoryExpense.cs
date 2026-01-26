using System;
using System.Collections.Generic;
using System.IO;
using System.Net;

namespace Okane.Application
{
    public class FileRepositoryExpense : IRepository<Expense>
    {
        private List<Expense> _expenses;
        private int _lastId;
        private string _filePath = "expenses.txt";

        private void LoadFromFile()
        {
            //“Si el archivo no existe, sal del método”.
            if (!File.Exists(_filePath))
            {
                return;
            }
            //te dara todas las lineas como texto
            var lines = File.ReadAllLines((_filePath));
            foreach (var line in lines)
            {
                var parts = line.Split(',');

                if (parts.Length != 3)
                    continue;

                if (!int.TryParse(parts[0], out int id))
                    continue;

                if (!int.TryParse(parts[1], out int amount))
                    continue;

                string categoryName = parts[2];

                var expense = new Expense
                {
                    Id = id,
                    Amount = amount,
                    CategoryName = categoryName
                };

                _expenses.Add(expense);
            }


            if (_expenses.Any())
            {
                _lastId = _expenses.Max(e => e.Id) + 1;
            }

        }
        
        public FileRepositoryExpense()
        {
            _expenses = new List<Expense>();
            _lastId = 1;

            LoadFromFile();
        }

        public void Add(Expense entity)
        {
            //logica de negocio   
            entity.Id = _lastId++;
            _expenses.Add(entity);
            AppendExpenseToFILE(entity);
        }

        public void AppendExpenseToFILE(Expense entity)
        {
            //escritura (persistencia)
            var line = $"{entity.Id}, {entity.Amount},{entity.CategoryName}";
            File.AppendAllText(_filePath, line + Environment.NewLine);
        }
        

        public void Remove(int id)
        {
            var expenseTpRemove = _expenses.FirstOrDefault(e => e.Id == id);
            if (expenseTpRemove != null)
            {
                _expenses.Remove(expenseTpRemove);
                File.WriteAllLines(_filePath, _expenses.Select(e => $"{e.Id},{e.Amount},{e.CategoryName}"));
            }
        }
        //devuelve todos los expense en una coleccion
        public IEnumerable<Expense> All()
        {
            return _expenses;
        }

        public Expense? ById(int id)
        {
            var expenseById = _expenses.FirstOrDefault(e => e.Id == id);
            if (expenseById == null)
            {
                return null;
            }

            return expenseById;
        }

        public bool Exists(int id)
        {
            
            return _expenses.Any(e => e.Id == id);
        }

        public Expense Update(int id, UpdateExpenseRequest request)
        {
            var expenseToUpdate = _expenses.FirstOrDefault(e => e.Id == id);
            if (expenseToUpdate == null)
            {
                throw new KeyNotFoundException();
            }
            expenseToUpdate.Amount =  request.Amount;
            expenseToUpdate.CategoryName = request.CategoryName;
            
            //toma toda la lista _expenses
            File.AppendAllLines(_filePath, _expenses.Select(e => $"{e.Id},{e.Amount},{e.CategoryName}"));
            return expenseToUpdate;
        }
        
    }
}