using ExpenseTracker.Models;
using ExpenseTracker.Services;

var service = new ExpenseService();
bool exit = false;

Console.Clear();
Console.WriteLine("╔══════════════════════════════════════╗");
Console.WriteLine("║    Welcome to Expense Tracker CLI    ║");
Console.WriteLine("╚══════════════════════════════════════╝");
Console.WriteLine();

while (!exit)
{
    Console.WriteLine("┌──────────────────────────────────────┐");
    Console.WriteLine("│          EXPENSE TRACKER MENU        │");
    Console.WriteLine("├──────────────────────────────────────┤");
    Console.WriteLine("│ 1. Add Expense                       │");
    Console.WriteLine("│ 2. View All Expenses                 │");
    Console.WriteLine("│ 3. Update Expense                    │");
    Console.WriteLine("│ 4. Delete Expense                    │");
    Console.WriteLine("│ 5. View Expense Summary              │");
    Console.WriteLine("│ 6. Filter by Category                │");
    Console.WriteLine("│ 7. Filter by Date Range              │");
    Console.WriteLine("│ 8. Filter by Month                   │");
    Console.WriteLine("│ 9. View Total Expenses               │");
    Console.WriteLine("│ 0. Clear All Expenses                │");
    Console.WriteLine("│ X. Exit                              │");
    Console.WriteLine("└──────────────────────────────────────┘");
    Console.Write("Select an option: ");

    var input = Console.ReadLine()?.Trim().ToUpper();
    Console.WriteLine();

    switch (input)
    {
        case "1":
            AddExpense(service);
            break;
        case "2":
            ViewExpenses(service);
            break;
        case "3":
            UpdateExpense(service);
            break;
        case "4":
            DeleteExpense(service);
            break;
        case "5":
            ViewExpenseSummary(service);
            break;
        case "6":
            FilterByCategory(service);
            break;
        case "7":
            FilterByDateRange(service);
            break;
        case "8":
            FilterByMonth(service);
            break;
        case "9":
            ViewTotalExpenses(service);
            break;
        case "0":
            ClearAllExpenses(service);
            break;
        case "X":
            exit = true;
            Console.WriteLine("Thank you for using Expense Tracker! Goodbye! 👋");
            break;
        default:
            Console.WriteLine("❌ Invalid option. Please try again.");
            break;
    }

    if (!exit)
    {
        Console.WriteLine("\nPress any key to continue...");
        Console.ReadKey();
        Console.Clear();
    }
}

// ==================== HELPER METHODS ====================

static void AddExpense(ExpenseService service)
{
    Console.WriteLine("═══ ADD NEW EXPENSE ═══");

    try
    {
        Console.Write("Amount: Rs.");
        if (!decimal.TryParse(Console.ReadLine(), out var amount) || amount <= 0)
        {
            Console.WriteLine("❌ Invalid amount. Must be a positive number.");
            return;
        }

        Console.Write("Category: ");
        var category = Console.ReadLine()?.Trim();
        if (string.IsNullOrWhiteSpace(category))
        {
            Console.WriteLine("❌ Category cannot be empty.");
            return;
        }

        Console.Write("Description: ");
        var description = Console.ReadLine()?.Trim() ?? string.Empty;

        Console.Write("Date (press Enter for today, or enter MM/DD/YYYY): ");
        var dateInput = Console.ReadLine()?.Trim();
        DateTime date = DateTime.Now;

        if (!string.IsNullOrWhiteSpace(dateInput))
        {
            if (!DateTime.TryParse(dateInput, out date))
            {
                Console.WriteLine("❌ Invalid date format. Using today's date.");
                date = DateTime.Now;
            }
        }

        var expense = new Expense
        {
            Amount = amount,
            Category = category,
            Description = description,
            Date = date
        };

        service.AddExpense(expense);
        Console.WriteLine($"✅ Expense added successfully! (ID: {expense.ID})");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"❌ Error adding expense: {ex.Message}");
    }
}

static void ViewExpenses(ExpenseService service)
{
    Console.WriteLine("═══ ALL EXPENSES ═══");
    var expenses = service.GetAllExpenses();

    if (expenses.Count == 0)
    {
        Console.WriteLine("📭 No expenses found.");
        return;
    }

    Console.WriteLine($"{"ID",-5} {"Date",-12} {"Category",-15} {"Amount",-12} {"Description"}");
    Console.WriteLine(new string('─', 80));

    foreach (var expense in expenses.OrderByDescending(e => e.Date))
    {
        Console.WriteLine($"{expense.ID,-5} {expense.Date,-12:MM/dd/yyyy} {expense.Category,-15} {FormatCurrency(expense.Amount),-12} {expense.Description}");
    }

    Console.WriteLine(new string('─', 80));
    Console.WriteLine($"Total Expenses: {expenses.Count}");
}

static void UpdateExpense(ExpenseService service)
{
    Console.WriteLine("═══ UPDATE EXPENSE ═══");

    Console.Write("Enter Expense ID to update: ");
    if (!int.TryParse(Console.ReadLine(), out var id))
    {
        Console.WriteLine("❌ Invalid ID.");
        return;
    }

    var expense = service.GetExpenseById(id);
    if (expense == null)
    {
        Console.WriteLine($"❌ Expense with ID {id} not found.");
        return;
    }

    Console.WriteLine($"\nCurrent Expense:");
    Console.WriteLine($"  Amount: {FormatCurrency(expense.Amount)}");
    Console.WriteLine($"  Category: {expense.Category}");
    Console.WriteLine($"  Description: {expense.Description}");
    Console.WriteLine($"  Date: {expense.Date:MM/dd/yyyy}");
    Console.WriteLine("\nEnter new values (press Enter to keep current value):");

    Console.Write($"New Amount (current: {FormatCurrency(expense.Amount)}): ₹ ");
    var amountInput = Console.ReadLine()?.Trim();
    if (!string.IsNullOrWhiteSpace(amountInput) && decimal.TryParse(amountInput, out var newAmount))
    {
        expense.Amount = newAmount;
    }

    Console.Write($"New Category (current: {expense.Category}): ");
    var categoryInput = Console.ReadLine()?.Trim();
    if (!string.IsNullOrWhiteSpace(categoryInput))
    {
        expense.Category = categoryInput;
    }

    Console.Write($"New Description (current: {expense.Description}): ");
    var descInput = Console.ReadLine()?.Trim();
    if (!string.IsNullOrWhiteSpace(descInput))
    {
        expense.Description = descInput;
    }

    Console.Write($"New Date (current: {expense.Date:MM/dd/yyyy}): ");
    var dateInput = Console.ReadLine()?.Trim();
    if (!string.IsNullOrWhiteSpace(dateInput) && DateTime.TryParse(dateInput, out var newDate))
    {
        expense.Date = newDate;
    }

    if (service.UpdateExpense(expense))
    {
        Console.WriteLine("✅ Expense updated successfully!");
    }
    else
    {
        Console.WriteLine("❌ Failed to update expense.");
    }
}

static void DeleteExpense(ExpenseService service)
{
    Console.WriteLine("═══ DELETE EXPENSE ═══");

    Console.Write("Enter Expense ID to delete: ");
    if (!int.TryParse(Console.ReadLine(), out var id))
    {
        Console.WriteLine("❌ Invalid ID.");
        return;
    }

    var expense = service.GetExpenseById(id);
    if (expense == null)
    {
        Console.WriteLine($"❌ Expense with ID {id} not found.");
        return;
    }

    Console.WriteLine($"\nExpense to delete:");
    Console.WriteLine($"  Amount: {expense.Amount:C}");
    Console.WriteLine($"  Category: {expense.Category}");
    Console.WriteLine($"  Description: {expense.Description}");
    Console.WriteLine($"  Date: {expense.Date:MM/dd/yyyy}");

    Console.Write("\nAre you sure you want to delete this expense? (Y/N): ");
    var confirm = Console.ReadLine()?.Trim().ToUpper();

    if (confirm == "Y" || confirm == "YES")
    {
        if (service.DeleteExpense(id))
        {
            Console.WriteLine("✅ Expense deleted successfully!");
        }
        else
        {
            Console.WriteLine("❌ Failed to delete expense.");
        }
    }
    else
    {
        Console.WriteLine("❌ Deletion cancelled.");
    }
}

static void ViewExpenseSummary(ExpenseService service)
{
    Console.WriteLine("═══ EXPENSE SUMMARY BY CATEGORY ═══");
    var summary = service.GetExpensesSummary();

    if (summary.Count == 0)
    {
        Console.WriteLine("📭 No expenses found.");
        return;
    }

    Console.WriteLine($"{"Category",-20} {"Total Amount",-15} {"Percentage"}");
    Console.WriteLine(new string('─', 60));

    var total = service.GetTotalExpenses();

    foreach (var item in summary.OrderByDescending(x => x.Value))
    {
        var percentage = total > 0 ? (item.Value / total * 100) : 0;
        Console.WriteLine($"{item.Key,-20} {FormatCurrency(item.Value),-15} {percentage,6:F1}%");
    }

    Console.WriteLine(new string('─', 60));
    Console.WriteLine($"{"TOTAL",-20} {FormatCurrency(total),-15} {"100.0%",6}");
}

static void FilterByCategory(ExpenseService service)
{
    Console.WriteLine("═══ FILTER BY CATEGORY ═══");

    Console.Write("Enter category name: ");
    var category = Console.ReadLine()?.Trim();

    if (string.IsNullOrWhiteSpace(category))
    {
        Console.WriteLine("❌ Category cannot be empty.");
        return;
    }

    var expenses = service.GetExpensesByCategory(category);

    if (expenses.Count == 0)
    {
        Console.WriteLine($"📭 No expenses found for category '{category}'.");
        return;
    }

    Console.WriteLine($"\nExpenses in '{category}':");
    Console.WriteLine($"{"ID",-5} {"Date",-12} {"Amount",-12} {"Description"}");
    Console.WriteLine(new string('─', 60));

    foreach (var expense in expenses.OrderByDescending(e => e.Date))
    {
        Console.WriteLine($"{expense.ID,-5} {expense.Date,-12:MM/dd/yyyy} {FormatCurrency(expense.Amount),-12} {expense.Description}");
    }

    Console.WriteLine(new string('─', 60));
    Console.WriteLine($"Total: {FormatCurrency(expenses.Sum(e => e.Amount))} ({expenses.Count} expenses)");
}

static void FilterByDateRange(ExpenseService service)
{
    Console.WriteLine("═══ FILTER BY DATE RANGE ═══");

    Console.Write("Start Date (MM/DD/YYYY): ");
    if (!DateTime.TryParse(Console.ReadLine(), out var startDate))
    {
        Console.WriteLine("❌ Invalid start date.");
        return;
    }

    Console.Write("End Date (MM/DD/YYYY): ");
    if (!DateTime.TryParse(Console.ReadLine(), out var endDate))
    {
        Console.WriteLine("❌ Invalid end date.");
        return;
    }

    if (startDate > endDate)
    {
        Console.WriteLine("❌ Start date cannot be after end date.");
        return;
    }

    var expenses = service.GetExpensesByDateRange(startDate, endDate);

    if (expenses.Count == 0)
    {
        Console.WriteLine($"📭 No expenses found between {startDate:MM/dd/yyyy} and {endDate:MM/dd/yyyy}.");
        return;
    }

    Console.WriteLine($"\nExpenses from {startDate:MM/dd/yyyy} to {endDate:MM/dd/yyyy}:");
    Console.WriteLine($"{"ID",-5} {"Date",-12} {"Category",-15} {"Amount",-12} {"Description"}");
    Console.WriteLine(new string('─', 80));

    foreach (var expense in expenses.OrderByDescending(e => e.Date))
    {
        Console.WriteLine($"{expense.ID,-5} {expense.Date,-12:MM/dd/yyyy} {expense.Category,-15} {FormatCurrency(expense.Amount),-12} {expense.Description}");
    }

    Console.WriteLine(new string('─', 80));
    Console.WriteLine($"Total: {FormatCurrency(expenses.Sum(e => e.Amount))} ({expenses.Count} expenses)");
}

static void FilterByMonth(ExpenseService service)
{
    Console.WriteLine("═══ FILTER BY MONTH ═══");

    Console.Write("Month (1-12): ");
    if (!int.TryParse(Console.ReadLine(), out var month) || month < 1 || month > 12)
    {
        Console.WriteLine("❌ Invalid month. Must be between 1 and 12.");
        return;
    }

    Console.Write("Year (e.g., 2025): ");
    if (!int.TryParse(Console.ReadLine(), out var year) || year < 1900 || year > 2100)
    {
        Console.WriteLine("❌ Invalid year.");
        return;
    }

    var expenses = service.GetExpensesByMonth(month, year);

    if (expenses.Count == 0)
    {
        Console.WriteLine($"📭 No expenses found for {new DateTime(year, month, 1):MMMM yyyy}.");
        return;
    }

    Console.WriteLine($"\nExpenses for {new DateTime(year, month, 1):MMMM yyyy}:");
    Console.WriteLine($"{"ID",-5} {"Date",-12} {"Category",-15} {"Amount",-12} {"Description"}");
    Console.WriteLine(new string('─', 80));

    foreach (var expense in expenses.OrderByDescending(e => e.Date))
    {
        Console.WriteLine($"{expense.ID,-5} {expense.Date,-12:MM/dd/yyyy} {expense.Category,-15} {FormatCurrency(expense.Amount),-12} {expense.Description}");
    }

    Console.WriteLine(new string('─', 80));
    Console.WriteLine($"Total: {FormatCurrency(expenses.Sum(e => e.Amount))} ({expenses.Count} expenses)");
}

static void ViewTotalExpenses(ExpenseService service)
{
    Console.WriteLine("═══ TOTAL EXPENSES ═══");
    var total = service.GetTotalExpenses();
    var count = service.GetAllExpenses().Count;

    Console.WriteLine($"Total Amount: {FormatCurrency(total)}");
    Console.WriteLine($"Total Expenses: {count}");

    if (count > 0)
    {
        Console.WriteLine($"Average Expense: {FormatCurrency(total / count)}");
    }
}

static void ClearAllExpenses(ExpenseService service)
{
    Console.WriteLine("═══ CLEAR ALL EXPENSES ═══");

    var count = service.GetAllExpenses().Count;
    if (count == 0)
    {
        Console.WriteLine("📭 No expenses to clear.");
        return;
    }

    Console.WriteLine($"⚠️  WARNING: This will delete all {count} expenses!");
    Console.Write("Are you sure? Type 'DELETE ALL' to confirm: ");
    var confirm = Console.ReadLine()?.Trim();

    if (confirm == "DELETE ALL")
    {
        service.ClearAll();
        Console.WriteLine("✅ All expenses cleared successfully!");
    }
    else
    {
        Console.WriteLine("❌ Clear all cancelled.");
    }
}

static string FormatCurrency(decimal amount)
{
    return $"₹ {amount:N2}";
}