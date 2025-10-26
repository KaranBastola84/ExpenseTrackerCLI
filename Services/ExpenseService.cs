using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using ExpenseTracker.Models;

namespace ExpenseTracker.Services
{
    /// <summary>
    /// Service for managing expense data with CRUD operations and persistence
    /// </summary>
    public class ExpenseService
    {
        private readonly string _filepath;
        private List<Expense> _expenses = new();

        /// <summary>
        /// Initializes a new instance of ExpenseService with default file path
        /// </summary>
        public ExpenseService() : this("Data/expenses.json")
        {
        }

        /// <summary>
        /// Initializes a new instance of ExpenseService with custom file path
        /// </summary>
        /// <param name="filepath">Custom path to the expenses data file</param>
        public ExpenseService(string filepath)
        {
            _filepath = filepath ?? throw new ArgumentNullException(nameof(filepath));
            LoadExpenses();
        }

        /// <summary>
        /// Adds a new expense to the collection
        /// </summary>
        /// <param name="expense">The expense to add</param>
        /// <exception cref="ArgumentNullException">Thrown when expense is null</exception>
        /// <exception cref="ArgumentNullException">Thrown when expense is null</exception>
        public void AddExpense(Expense expense)
        {
            if (expense == null)
                throw new ArgumentNullException(nameof(expense));

            // Generate ID using max existing ID to prevent duplicates after deletions
            expense.ID = _expenses.Any() ? _expenses.Max(e => e.ID) + 1 : 1;
            _expenses.Add(expense);
            SaveExpenses();
        }

        /// <summary>
        /// Updates an existing expense
        /// </summary>
        /// <param name="expense">The expense with updated information</param>
        /// <returns>True if the expense was found and updated; otherwise false</returns>
        /// <exception cref="ArgumentNullException">Thrown when expense is null</exception>
        public bool UpdateExpense(Expense expense)
        {
            if (expense == null)
                throw new ArgumentNullException(nameof(expense));

            var existingExpense = _expenses.FirstOrDefault(e => e.ID == expense.ID);
            if (existingExpense == null)
                return false;

            existingExpense.Amount = expense.Amount;
            existingExpense.Date = expense.Date;
            existingExpense.Category = expense.Category;
            existingExpense.Description = expense.Description;

            SaveExpenses();
            return true;
        }

        /// <summary>
        /// Deletes an expense by ID
        /// </summary>
        /// <param name="id">The ID of the expense to delete</param>
        /// <returns>True if the expense was found and deleted; otherwise false</returns>
        public bool DeleteExpense(int id)
        {
            var expense = _expenses.FirstOrDefault(e => e.ID == id);
            if (expense == null)
                return false;

            _expenses.Remove(expense);
            SaveExpenses();
            return true;
        }

        /// <summary>
        /// Gets an expense by ID
        /// </summary>
        /// <param name="id">The ID of the expense to retrieve</param>
        /// <returns>The expense if found; otherwise null</returns>
        public Expense? GetExpenseById(int id)
        {
            return _expenses.FirstOrDefault(e => e.ID == id);
        }

        /// <summary>
        /// Gets all expenses
        /// </summary>
        /// <returns>A list of all expenses</returns>
        /// <returns>A list of all expenses</returns>
        public List<Expense> GetAllExpenses() => _expenses.ToList();

        /// <summary>
        /// Gets expenses filtered by category
        /// </summary>
        /// <param name="category">The category to filter by</param>
        /// <returns>A list of expenses in the specified category</returns>
        public List<Expense> GetExpensesByCategory(string category)
        {
            if (string.IsNullOrWhiteSpace(category))
                return new List<Expense>();

            return _expenses.Where(e => e.Category.Equals(category, StringComparison.OrdinalIgnoreCase)).ToList();
        }

        /// <summary>
        /// Gets expenses within a date range
        /// </summary>
        /// <param name="startDate">The start date (inclusive)</param>
        /// <param name="endDate">The end date (inclusive)</param>
        /// <returns>A list of expenses within the date range</returns>
        public List<Expense> GetExpensesByDateRange(DateTime startDate, DateTime endDate)
        {
            return _expenses.Where(e => e.Date.Date >= startDate.Date && e.Date.Date <= endDate.Date).ToList();
        }

        /// <summary>
        /// Gets expenses for a specific month and year
        /// </summary>
        /// <param name="month">The month (1-12)</param>
        /// <param name="year">The year</param>
        /// <returns>A list of expenses for the specified month</returns>
        public List<Expense> GetExpensesByMonth(int month, int year)
        {
            if (month < 1 || month > 12)
                throw new ArgumentOutOfRangeException(nameof(month), "Month must be between 1 and 12");

            return _expenses.Where(e => e.Date.Month == month && e.Date.Year == year).ToList();
        }

        /// <summary>
        /// Gets summary of expenses grouped by category
        /// </summary>
        /// <returns>Dictionary with category as key and total amount as value</returns>
        public Dictionary<string, decimal> GetExpensesSummary()
        {
            return _expenses
                .GroupBy(e => e.Category)
                .ToDictionary(g => g.Key, g => g.Sum(e => e.Amount));
        }

        /// <summary>
        /// Gets the total of all expenses
        /// </summary>
        /// <returns>The total amount of all expenses</returns>
        public decimal GetTotalExpenses()
        {
            return _expenses.Sum(e => e.Amount);
        }

        /// <summary>
        /// Clears all expenses
        /// </summary>
        public void ClearAll()
        {
            _expenses.Clear();
            SaveExpenses();
        }

        /// <summary>
        /// Saves expenses to the file
        /// </summary>
        /// <summary>
        /// Saves expenses to the file
        /// </summary>
        private void SaveExpenses()
        {
            try
            {
                var directoryPath = Path.GetDirectoryName(_filepath);
                if (!string.IsNullOrEmpty(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                }

                var json = JsonSerializer.Serialize(_expenses, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(_filepath, json);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error saving expenses: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Loads expenses from the file
        /// </summary>
        private void LoadExpenses()
        {
            try
            {
                if (File.Exists(_filepath))
                {
                    var json = File.ReadAllText(_filepath);
                    _expenses = JsonSerializer.Deserialize<List<Expense>>(json) ?? new List<Expense>();
                }
            }
            catch (JsonException ex)
            {
                Console.Error.WriteLine($"Error parsing expenses file: {ex.Message}");
                _expenses = new List<Expense>();
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error loading expenses: {ex.Message}");
                _expenses = new List<Expense>();
            }
        }
    }
}