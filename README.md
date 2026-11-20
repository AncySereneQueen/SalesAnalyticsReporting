# Sales Analytics & Reporting System

A beginner-level C#/.NET 8 console application demonstrating LINQ, JSON import/export, file exceptions, and reporting.

## Run

From the project folder:

```bash
dotnet run
```

Choose **1** to import the bundled `Data/sales.json` before viewing reports. The app reads the JSON from its output directory, where the build copies it automatically.

Choose **7** to export `Reports/summary.json` (under the application output directory).

## Features
- Import JSON transactions with handling for missing files, denied access, I/O failures, and malformed JSON
- View transactions
- Monthly report for a selected year
- Yearly report
- Top 5 products and customers by revenue
- Export management summary as formatted JSON

## Learning topics
`File.ReadAllText`, `File.WriteAllText`, `JsonSerializer.Deserialize`, `JsonSerializer.Serialize`, `Where`, `GroupBy`, `Sum`, `OrderBy`, `OrderByDescending`, `Take`, `Select`.

The app intentionally uses only one model and two services. It uses in-memory data, not a database. Sample data is fictional.
