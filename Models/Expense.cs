namespace ExpenseTracker.Models
{
    public class Expense
    {
        public int ID { get; set; }
        public decimal Amount { get; set; }
        public DateTime Date { get; set; } = DateTime.Now;
        public string Category { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }
}