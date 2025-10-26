using ExpenseTracker.Models;
using ExpenseTracker.Services;

var service = new ExpenseService();
bool exit = false;

while (!exit)
{
    Console.WriteLine("Expense Tracker");
    Console.WriteLine("1. Add Expense");
    Console.WriteLine("2. View Expenses");
    Console.WriteLine("3. Update Expense");
    Console.WriteLine("4. Delete Expense");
    Console.WriteLine("5. View Expense Summary");
    Console.WriteLine("6. Exit");
    Console.Write("Select an option: ");

    var input = Console.ReadLine();
    Console.WriteLine();

    switch (input)
    {
        case "1":
            break;
        case "2":
            break;
        case "3":
            break;
        case "4":
            break;
        case "5":
            break;
        case "6":
            exit = true;
            break;
        default:
            Console.WriteLine("Invalid option. Please try again.");
            break;
    }

    Console.WriteLine();
}