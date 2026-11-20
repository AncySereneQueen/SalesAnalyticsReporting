
using SalesAnalytics.Services;

SalesService salesService;

try
{
    salesService = new SalesService();
}
catch (FileNotFoundException ex)
{
    Console.WriteLine(ex.Message);
    return;
}

while (true)
{
    Console.WriteLine("\n==================================");
    Console.WriteLine("    SALES ANALYTICS & REPORTING");
    Console.WriteLine("==================================");

    Console.WriteLine("1. Import Sales Data");
    Console.WriteLine("2. View All Sales");
    Console.WriteLine("3. Monthly Sales Report");
    Console.WriteLine("4. Yearly Sales Report");
    Console.WriteLine("5. Top 5 Customers");
    Console.WriteLine("6. Top 5 Products");
    Console.WriteLine("7. Export Summary Report");
    Console.WriteLine("8. Exit");

    Console.Write("\nEnter your choice: ");

    if (!int.TryParse(Console.ReadLine(), out int choice))
    {
        Console.WriteLine("Please enter a valid number.");
        continue;
    }

    switch (choice)
    {
        case 1:
            salesService.ImportSales();
            break;

        case 2:
            salesService.DisplaySales();
            break;

        case 3:
            salesService.MonthlyReport();
            break;

        case 4:
            salesService.YearlyReport();
            break;

        case 5:
            salesService.TopCustomers();
            break;

        case 6:
            salesService.TopProducts();
            break;

        case 7:
            salesService.ExportReport();
            break;

        case 8:
            Console.WriteLine("Thank you!");
            return;

        default:
            Console.WriteLine("Invalid choice. Try again.");
            break;
    }
}
