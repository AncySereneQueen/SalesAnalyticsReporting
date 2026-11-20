
using System.Text.Json;
using SalesAnalytics.Models;

namespace SalesAnalytics.Services;

public class SalesService
{
    private readonly string _dataFile;
    private readonly string _reportFile;

    private List<Sale> _sales = new List<Sale>();

    public SalesService()
    {
        string projectPath = "";

        DirectoryInfo? folder = new DirectoryInfo(AppContext.BaseDirectory);

        while (folder != null)
        {
            projectPath = Path.Combine(folder.FullName, "Data", "sales.json");

            if (File.Exists(projectPath))
            {
                _dataFile = projectPath;

                _reportFile = Path.Combine(folder.FullName, "Reports", "summary.json");

                return;
            }

            folder = folder.Parent;
        }

        throw new FileNotFoundException("Data/sales.json was not found.");
    }

    // 1. Import sales from JSON
    public void ImportSales()
    {
        try
        {
            string json = File.ReadAllText(_dataFile);

            List<Sale> sales = JsonSerializer.Deserialize<List<Sale>>(json)?? new List<Sale>();

            // Simple data validation
            if (sales.Count == 0 ||
                sales.Any(s =>
                    s.SaleID <= 0 ||
                    string.IsNullOrWhiteSpace(s.CustomerName) ||
                    string.IsNullOrWhiteSpace(s.ProductName) ||
                    s.Quantity <= 0 ||
                    s.UnitPrice <= 0) )
            {
                Console.WriteLine("Invalid or empty sales data.");
                return;
            }

            _sales = sales;

            Console.WriteLine($"\nSuccessfully imported {_sales.Count} sales.");
        }
        catch (IOException ex)
        {
            Console.WriteLine($"File error: {ex.Message}");
        }
        catch (UnauthorizedAccessException ex)
        {
            Console.WriteLine($"Access error: {ex.Message}");
        }
        catch (JsonException ex)
        {
            Console.WriteLine($"Invalid JSON: {ex.Message}");
        }
    }

    // 2. Display all sales
    public void DisplaySales()
    {
        if (_sales.Count == 0)
        {
            Console.WriteLine("Please import sales first.");
            return;
        }

        Console.WriteLine("\n--- ALL SALES ---");

        foreach (Sale sale in _sales)
        {
            Console.WriteLine(
                $"{sale.SaleID} | " +
                $"{sale.SaleDate:dd-MM-yyyy} | " +
                $"{sale.CustomerName} | " +
                $"{sale.ProductName} | " +
                $"Qty: {sale.Quantity} | " +
                $"Total: {sale.TotalAmount:N2}");
        }
    }

    // 3. Monthly report
    public void MonthlyReport()
    {
        if (_sales.Count == 0)
        {
            Console.WriteLine("Please import sales first.");
            return;
        }

        Console.Write("Enter year: ");
        if (!int.TryParse(Console.ReadLine(), out int year))
        {
            Console.WriteLine("Invalid year.");
            return;
        }

        var report = _sales
            .Where(s => s.SaleDate.Year == year)
            .GroupBy(s => s.SaleDate.Month)
            .Select(g => new
            {
                Month = g.Key,
                TotalSales = g.Sum(s => s.TotalAmount),
                Transactions = g.Count()
            })
            .OrderBy(r => r.Month);

        Console.WriteLine($"\n--- MONTHLY REPORT {year} ---");

        foreach (var item in report)
        {
            Console.WriteLine(
                $"{new DateTime(year, item.Month, 1):MMMM} | " +
                $"Transactions: {item.Transactions} | " +
                $"Sales: {item.TotalSales:N2}");
        }
    }

    // 4. Yearly report
    public void YearlyReport()
    {
        if (_sales.Count == 0)
        {
            Console.WriteLine("Please import sales first.");
            return;
        }

        var report = _sales
            .GroupBy(s => s.SaleDate.Year)
            .Select(g => new
            {
                Year = g.Key,
                TotalSales = g.Sum(s => s.TotalAmount),
                Transactions = g.Count()
            })
            .OrderBy(r => r.Year);

        Console.WriteLine("\n--- YEARLY REPORT ---");

        foreach (var item in report)
        {
            Console.WriteLine(
                $"{item.Year} | " +
                $"Transactions: {item.Transactions} | " +
                $"Sales: {item.TotalSales:N2}");
        }
    }

    // 5. Top customers
    public void TopCustomers()
    {
        if (_sales.Count == 0)
        {
            Console.WriteLine("Please import sales first.");
            return;
        }

        var customers = _sales
            .GroupBy(s => s.CustomerName)
            .Select(g => new
            {
                Customer = g.Key,
                TotalSpent = g.Sum(s => s.TotalAmount)
            })
            .OrderByDescending(c => c.TotalSpent)
            .Take(5);

        Console.WriteLine("\n--- TOP 5 CUSTOMERS ---");

        foreach (var customer in customers)
        {
            Console.WriteLine(
                $"{customer.Customer} - {customer.TotalSpent:N2}");
        }
    }

    // 6. Top products
    public void TopProducts()
    {
        if (_sales.Count == 0)
        {
            Console.WriteLine("Please import sales first.");
            return;
        }

        var products = _sales
            .GroupBy(s => s.ProductName)
            .Select(g => new
            {
                Product = g.Key,
                QuantitySold = g.Sum(s => s.Quantity),
                Revenue = g.Sum(s => s.TotalAmount)
            })
            .OrderByDescending(p => p.Revenue)
            .Take(5);

        Console.WriteLine("\n--- TOP 5 PRODUCTS ---");

        foreach (var product in products)
        {
            Console.WriteLine(
                $"{product.Product} | " +
                $"Qty: {product.QuantitySold} | " +
                $"Revenue: {product.Revenue:N2}");
        }
    }

    // 7. Export summarized report
    public void ExportReport()
    {
        if (_sales.Count == 0)
        {
            Console.WriteLine("Please import sales first.");
            return;
        }

        try
        {
            var report = new
            {
                GeneratedOn = DateTime.Now,
                TotalTransactions = _sales.Count,
                TotalRevenue = _sales.Sum(s => s.TotalAmount),

                MonthlySales = _sales
                    .GroupBy(s => new
                    {
                        s.SaleDate.Year,
                        s.SaleDate.Month
                    })
                    .Select(g => new
                    {
                        g.Key.Year,
                        g.Key.Month,
                        Revenue = g.Sum(s => s.TotalAmount)
                    })
                    .OrderBy(r => r.Year)
                    .ThenBy(r => r.Month)
                    .ToList(),

                YearlySales = _sales
                    .GroupBy(s => s.SaleDate.Year)
                    .Select(g => new
                    {
                        Year = g.Key,
                        Revenue = g.Sum(s => s.TotalAmount)
                    })
                    .OrderBy(r => r.Year)
                    .ToList(),

                TopCustomers = _sales
                    .GroupBy(s => s.CustomerName)
                    .Select(g => new
                    {
                        Customer = g.Key,
                        TotalSpent = g.Sum(s => s.TotalAmount)
                    })
                    .OrderByDescending(c => c.TotalSpent)
                    .Take(5)
                    .ToList(),

                TopProducts = _sales
                    .GroupBy(s => s.ProductName)
                    .Select(g => new
                    {
                        Product = g.Key,
                        QuantitySold = g.Sum(s => s.Quantity),
                        Revenue = g.Sum(s => s.TotalAmount)
                    })
                    .OrderByDescending(p => p.Revenue)
                    .Take(5)
                    .ToList()
            };

            string? reportFolder = Path.GetDirectoryName(_reportFile);

            if (reportFolder != null)
                Directory.CreateDirectory(reportFolder);

            string json = JsonSerializer.Serialize(
                report,
                new JsonSerializerOptions
                {
                    WriteIndented = true
                });

            File.WriteAllText(_reportFile, json);

            Console.WriteLine(
                $"\nReport exported successfully!\n{_reportFile}");
        }
        catch (IOException ex)
        {
            Console.WriteLine($"File error: {ex.Message}");
        }
        catch (UnauthorizedAccessException ex)
        {
            Console.WriteLine($"Access error: {ex.Message}");
        }
    }
}
