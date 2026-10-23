using APIWeb.Models;
using Microsoft.EntityFrameworkCore;

namespace APIWeb.Data;

public static class DbSeeder
{
    public static async Task InitializeAsync(
        NorthwindContext db,
        AppIdentityDbContext identityDb,
        ILogger logger,
        CancellationToken cancellationToken = default)
    {
        // Identity: si las tablas AspNet* ya existen (por ejemplo, creadas por otra
        // versión del proyecto sobre la misma base nwind) se reutilizan y solo se
        // aseguran las columnas propias (Name, Picture).
        await EnsureIdentitySchemaAsync(identityDb, logger, cancellationToken);

        if (await NorthwindSchemaExistsAsync(db, cancellationToken))
        {
            logger.LogInformation("Esquema Northwind detectado: no se recrean sus tablas.");
        }
        else
        {
            logger.LogInformation("Esquema Northwind no encontrado: creando tablas...");
            await db.Database.MigrateAsync(cancellationToken);
        }

        if (await db.Categories.AnyAsync(cancellationToken))
        {
            logger.LogInformation("La base de datos ya contiene datos de Northwind; no se siembra de nuevo.");
            return;
        }

        logger.LogInformation("Sembrando datos de ejemplo de Northwind...");

        var categories = new List<Category>
        {
            new() { CategoryName = "Bebidas", Description = "Bebidas refrescantes, cafés, tés y cervezas." },
            new() { CategoryName = "Condimentos", Description = "Salsas dulces y saladas, especias y aderezos." },
            new() { CategoryName = "Repostería", Description = "Panes, galletas, pasteles y postres." },
            new() { CategoryName = "Lácteos", Description = "Quesos y productos lácteos." },
            new() { CategoryName = "Granos", Description = "Cereales, arroz, pastas y legumbres." },
            new() { CategoryName = "Carnes", Description = "Carnes preparadas y embutidos." },
            new() { CategoryName = "Frutas", Description = "Frutas frescas y deshidratadas." },
            new() { CategoryName = "Mariscos", Description = "Pescados y mariscos." }
        };

        var suppliers = new List<Supplier>
        {
            new() { CompanyName = "Exotic Liquids", ContactName = "Charlotte Cooper", ContactTitle = "Gerente de compras", City = "Londres", Country = "Reino Unido", Phone = "(171) 555-2222" },
            new() { CompanyName = "New Orleans Cajun Delights", ContactName = "Shelley Burke", ContactTitle = "Gerente de pedidos", City = "Nueva Orleans", Country = "EE. UU.", Phone = "(100) 555-4822" },
            new() { CompanyName = "Grandma's Boysenberry Spread", ContactName = "Regina Murphy", ContactTitle = "Ventas", City = "Nueva York", Country = "EE. UU.", Phone = "(212) 555-1212" },
            new() { CompanyName = "Tokyo Traders", ContactName = "Yoshi Nagase", ContactTitle = "Gerente de marketing", City = "Tokio", Country = "Japón", Phone = "(03) 3555-5011" },
            new() { CompanyName = "Pasta Buttini s.r.l.", ContactName = "Giovanni Giudici", ContactTitle = "Gerente", City = "Salerno", Country = "Italia", Phone = "(054) 305-2920" }
        };

        var shippers = new List<Shipper>
        {
            new() { CompanyName = "Speedy Express", Phone = "(503) 555-9831" },
            new() { CompanyName = "United Package", Phone = "(503) 555-3199" },
            new() { CompanyName = "Federal Shipping", Phone = "(503) 555-9931" }
        };

        db.Categories.AddRange(categories);
        db.Suppliers.AddRange(suppliers);
        db.Shippers.AddRange(shippers);
        await db.SaveChangesAsync(cancellationToken);

        var products = new List<Product>
        {
            new() { ProductName = "Chai", CategoryId = categories[0].CategoryId, SupplierId = suppliers[0].SupplierId, QuantityPerUnit = "10 cajas x 20 bolsas", UnitPrice = 18.00m, UnitsInStock = 39, UnitsOnOrder = 0, ReorderLevel = 10, Discontinued = false },
            new() { ProductName = "Chang", CategoryId = categories[0].CategoryId, SupplierId = suppliers[0].SupplierId, QuantityPerUnit = "24 botellas x 355 ml", UnitPrice = 19.00m, UnitsInStock = 17, UnitsOnOrder = 40, ReorderLevel = 25, Discontinued = false },
            new() { ProductName = "Aniseed Syrup", CategoryId = categories[1].CategoryId, SupplierId = suppliers[0].SupplierId, QuantityPerUnit = "12 frascos x 550 ml", UnitPrice = 10.00m, UnitsInStock = 13, UnitsOnOrder = 70, ReorderLevel = 25, Discontinued = false },
            new() { ProductName = "Chef Anton's Cajun Seasoning", CategoryId = categories[1].CategoryId, SupplierId = suppliers[1].SupplierId, QuantityPerUnit = "48 frascos x 6 oz", UnitPrice = 22.00m, UnitsInStock = 53, UnitsOnOrder = 0, ReorderLevel = 0, Discontinued = false },
            new() { ProductName = "Grandma's Boysenberry Spread", CategoryId = categories[1].CategoryId, SupplierId = suppliers[2].SupplierId, QuantityPerUnit = "12 frascos x 8 oz", UnitPrice = 25.00m, UnitsInStock = 120, UnitsOnOrder = 0, ReorderLevel = 25, Discontinued = false },
            new() { ProductName = "Alice Mutton", CategoryId = categories[5].CategoryId, SupplierId = suppliers[1].SupplierId, QuantityPerUnit = "20 latas x 1 kg", UnitPrice = 39.00m, UnitsInStock = 0, UnitsOnOrder = 0, ReorderLevel = 0, Discontinued = true },
            new() { ProductName = "Ikura", CategoryId = categories[7].CategoryId, SupplierId = suppliers[3].SupplierId, QuantityPerUnit = "12 frascos x 200 g", UnitPrice = 31.00m, UnitsInStock = 31, UnitsOnOrder = 0, ReorderLevel = 0, Discontinued = false },
            new() { ProductName = "Queso Cabrales", CategoryId = categories[3].CategoryId, SupplierId = suppliers[4].SupplierId, QuantityPerUnit = "1 kg", UnitPrice = 21.00m, UnitsInStock = 22, UnitsOnOrder = 30, ReorderLevel = 30, Discontinued = false },
            new() { ProductName = "Gorgonzola Telino", CategoryId = categories[3].CategoryId, SupplierId = suppliers[4].SupplierId, QuantityPerUnit = "12 x 100 g", UnitPrice = 12.50m, UnitsInStock = 0, UnitsOnOrder = 70, ReorderLevel = 20, Discontinued = false },
            new() { ProductName = "Mascarpone Fabioli", CategoryId = categories[3].CategoryId, SupplierId = suppliers[4].SupplierId, QuantityPerUnit = "24 x 200 g", UnitPrice = 32.00m, UnitsInStock = 9, UnitsOnOrder = 40, ReorderLevel = 25, Discontinued = false },
            new() { ProductName = "Gustaf's Knäckebröd", CategoryId = categories[2].CategoryId, SupplierId = suppliers[3].SupplierId, QuantityPerUnit = "24 paquetes x 500 g", UnitPrice = 21.00m, UnitsInStock = 104, UnitsOnOrder = 0, ReorderLevel = 25, Discontinued = false },
            new() { ProductName = "Ravioli Angelo", CategoryId = categories[4].CategoryId, SupplierId = suppliers[4].SupplierId, QuantityPerUnit = "24 paquetes x 250 g", UnitPrice = 19.50m, UnitsInStock = 36, UnitsOnOrder = 0, ReorderLevel = 20, Discontinued = false }
        };

        db.Products.AddRange(products);
        await db.SaveChangesAsync(cancellationToken);

        var customers = new List<Customer>
        {
            new() { CustomerId = "ALFKI", CompanyName = "Alfreds Futterkiste", ContactName = "Maria Anders", ContactTitle = "Ventas", Address = "Obere Str. 57", City = "Berlín", Country = "Alemania", Phone = "030-0074321" },
            new() { CustomerId = "ANATR", CompanyName = "Ana Trujillo Emparedados y helados", ContactName = "Ana Trujillo", ContactTitle = "Propietaria", Address = "Avda. de la Constitución 2222", City = "México D.F.", Country = "México", Phone = "(5) 555-4729" },
            new() { CustomerId = "ANTON", CompanyName = "Antonio Moreno Taquería", ContactName = "Antonio Moreno", ContactTitle = "Propietario", Address = "Mataderos 2312", City = "México D.F.", Country = "México", Phone = "(5) 555-3932" },
            new() { CustomerId = "BONAP", CompanyName = "Bon app'", ContactName = "Laurence Lebihan", ContactTitle = "Propietaria", Address = "12, rue des Bouchers", City = "Marsella", Country = "Francia", Phone = "91.24.45.40" },
            new() { CustomerId = "BOTTM", CompanyName = "Bottom-Dollar Markets", ContactName = "Elizabeth Lincoln", ContactTitle = "Gerente contable", Address = "23 Tsawassen Blvd.", City = "Tsawassen", Country = "Canadá", Phone = "(604) 555-4729" }
        };

        db.Customers.AddRange(customers);
        await db.SaveChangesAsync(cancellationToken);

        var employees = new List<Employee>
        {
            new() { FirstName = "Nancy", LastName = "Davolio", Title = "Representante de ventas", TitleOfCourtesy = "Sra.", BirthDate = new DateTime(1968, 12, 8), HireDate = new DateTime(1992, 5, 1), Address = "507 - 20th Ave. E.", City = "Seattle", Country = "EE. UU.", HomePhone = "(206) 555-9857", Extension = "5467", Notes = "Incluye un título universitario en psicología." },
            new() { FirstName = "Andrew", LastName = "Fuller", Title = "Vicepresidente de ventas", TitleOfCourtesy = "Dr.", BirthDate = new DateTime(1952, 2, 19), HireDate = new DateTime(1992, 8, 14), Address = "908 W. Capital Way", City = "Tacoma", Country = "EE. UU.", HomePhone = "(206) 555-9482", Extension = "3457", Notes = "Incluye un título universitario en química." },
            new() { FirstName = "Janet", LastName = "Leverling", Title = "Representante de ventas", TitleOfCourtesy = "Sra.", BirthDate = new DateTime(1963, 8, 30), HireDate = new DateTime(1992, 4, 1), Address = "722 Moss Bay Blvd.", City = "Kirkland", Country = "EE. UU.", HomePhone = "(206) 555-3412", Extension = "3355", Notes = "Tiene un título universitario en enfermería." },
            new() { FirstName = "Margaret", LastName = "Peacock", Title = "Representante de ventas", TitleOfCourtesy = "Sra.", BirthDate = new DateTime(1958, 9, 19), HireDate = new DateTime(1993, 5, 3), Address = "4110 Old Redmond Rd.", City = "Redmond", Country = "EE. UU.", HomePhone = "(206) 555-8122", Extension = "5176", Notes = "Incluye un título universitario en inglés." }
        };

        db.Employees.AddRange(employees);
        await db.SaveChangesAsync(cancellationToken);

        foreach (var employee in employees.Skip(2))
        {
            employee.ReportsTo = employees[1].EmployeeId;
        }

        await db.SaveChangesAsync(cancellationToken);

        var orders = new List<Order>
        {
            new() { CustomerId = customers[0].CustomerId, EmployeeId = employees[0].EmployeeId, OrderDate = new DateTime(2024, 7, 4), RequiredDate = new DateTime(2024, 8, 1), ShippedDate = new DateTime(2024, 7, 16), ShipVia = shippers[2].ShipperId, Freight = 32.38m, ShipName = "Alfreds Futterkiste", ShipAddress = "Obere Str. 57", ShipCity = "Berlín", ShipCountry = "Alemania" },
            new() { CustomerId = customers[1].CustomerId, EmployeeId = employees[1].EmployeeId, OrderDate = new DateTime(2024, 7, 5), RequiredDate = new DateTime(2024, 8, 16), ShippedDate = new DateTime(2024, 7, 10), ShipVia = shippers[1].ShipperId, Freight = 11.61m, ShipName = "Ana Trujillo Emparedados y helados", ShipAddress = "Avda. de la Constitución 2222", ShipCity = "México D.F.", ShipCountry = "México" },
            new() { CustomerId = customers[2].CustomerId, EmployeeId = employees[2].EmployeeId, OrderDate = new DateTime(2024, 7, 8), RequiredDate = new DateTime(2024, 8, 5), ShippedDate = new DateTime(2024, 7, 12), ShipVia = shippers[0].ShipperId, Freight = 65.83m, ShipName = "Antonio Moreno Taquería", ShipAddress = "Mataderos 2312", ShipCity = "México D.F.", ShipCountry = "México" },
            new() { CustomerId = customers[3].CustomerId, EmployeeId = employees[3].EmployeeId, OrderDate = new DateTime(2024, 7, 9), RequiredDate = new DateTime(2024, 8, 6), ShippedDate = new DateTime(2024, 7, 11), ShipVia = shippers[1].ShipperId, Freight = 41.34m, ShipName = "Bon app'", ShipAddress = "12, rue des Bouchers", ShipCity = "Marsella", ShipCountry = "Francia" },
            new() { CustomerId = customers[4].CustomerId, EmployeeId = employees[0].EmployeeId, OrderDate = new DateTime(2024, 7, 10), RequiredDate = new DateTime(2024, 8, 7), ShippedDate = null, ShipVia = shippers[0].ShipperId, Freight = 51.30m, ShipName = "Bottom-Dollar Markets", ShipAddress = "23 Tsawassen Blvd.", ShipCity = "Tsawassen", ShipCountry = "Canadá" },
            new() { CustomerId = customers[0].CustomerId, EmployeeId = employees[2].EmployeeId, OrderDate = new DateTime(2024, 7, 11), RequiredDate = new DateTime(2024, 8, 8), ShippedDate = new DateTime(2024, 7, 15), ShipVia = shippers[2].ShipperId, Freight = 3.35m, ShipName = "Alfreds Futterkiste", ShipAddress = "Obere Str. 57", ShipCity = "Berlín", ShipCountry = "Alemania" }
        };

        db.Orders.AddRange(orders);
        await db.SaveChangesAsync(cancellationToken);

        var orderDetails = new List<OrderDetail>
        {
            new() { OrderId = orders[0].OrderId, ProductId = products[0].ProductId, UnitPrice = 18.00m, Quantity = 12, Discount = 0 },
            new() { OrderId = orders[0].OrderId, ProductId = products[7].ProductId, UnitPrice = 21.00m, Quantity = 10, Discount = 0.05 },
            new() { OrderId = orders[1].OrderId, ProductId = products[1].ProductId, UnitPrice = 19.00m, Quantity = 5, Discount = 0 },
            new() { OrderId = orders[1].OrderId, ProductId = products[4].ProductId, UnitPrice = 25.00m, Quantity = 9, Discount = 0.10 },
            new() { OrderId = orders[2].OrderId, ProductId = products[6].ProductId, UnitPrice = 31.00m, Quantity = 3, Discount = 0 },
            new() { OrderId = orders[2].OrderId, ProductId = products[9].ProductId, UnitPrice = 32.00m, Quantity = 6, Discount = 0.15 },
            new() { OrderId = orders[3].OrderId, ProductId = products[10].ProductId, UnitPrice = 21.00m, Quantity = 20, Discount = 0 },
            new() { OrderId = orders[4].OrderId, ProductId = products[11].ProductId, UnitPrice = 19.50m, Quantity = 15, Discount = 0.05 },
            new() { OrderId = orders[5].OrderId, ProductId = products[2].ProductId, UnitPrice = 10.00m, Quantity = 24, Discount = 0 },
            new() { OrderId = orders[5].OrderId, ProductId = products[8].ProductId, UnitPrice = 12.50m, Quantity = 8, Discount = 0 }
        };

        db.OrderDetails.AddRange(orderDetails);
        await db.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Datos de ejemplo insertados correctamente.");
    }

    private static async Task EnsureIdentitySchemaAsync(
        AppIdentityDbContext identityDb,
        ILogger logger,
        CancellationToken cancellationToken)
    {
        if (!await TableExistsAsync(identityDb, "aspnetusers", cancellationToken))
        {
            logger.LogInformation("Creando tablas de Identity...");
            await identityDb.Database.MigrateAsync(cancellationToken);
            return;
        }

        logger.LogInformation("Tablas de Identity detectadas: se reutilizan (sin migrar).");

        await EnsureColumnAsync(identityDb, "aspnetusers", "Name", "longtext NULL", cancellationToken);
        await EnsureColumnAsync(identityDb, "aspnetusers", "Picture", "longtext NULL", cancellationToken);
    }

    private static async Task EnsureColumnAsync(
        AppIdentityDbContext db,
        string table,
        string column,
        string definition,
        CancellationToken cancellationToken)
    {
        var exists = await db.Database
            .SqlQueryRaw<int>(
                """
                SELECT COUNT(*) AS Value
                FROM information_schema.columns
                WHERE table_schema = DATABASE()
                  AND LOWER(table_name) = {0}
                  AND LOWER(column_name) = {1}
                """,
                table,
                column.ToLowerInvariant())
            .SingleAsync(cancellationToken);

        if (exists > 0)
        {
            return;
        }

        // MySQL no permite parametrizar identificadores, así que el ALTER TABLE se
        // construye por concatenación. Los nombres llegan de constantes internas y
        // se validan aquí antes de usarlos.
        ValidateIdentifier(table);
        ValidateIdentifier(column);

        var sql = "ALTER TABLE `" + table + "` ADD COLUMN `" + column + "` " + definition;

        await db.Database.ExecuteSqlRawAsync(sql, cancellationToken);
    }

    private static void ValidateIdentifier(string identifier)
    {
        if (identifier.Length == 0 ||
            !identifier.All(character => char.IsLetterOrDigit(character) || character == '_'))
        {
            throw new ArgumentException($"Identificador SQL no válido: '{identifier}'.", nameof(identifier));
        }
    }

    private static async Task<bool> TableExistsAsync(
        AppIdentityDbContext db,
        string table,
        CancellationToken cancellationToken)
    {
        var count = await db.Database
            .SqlQueryRaw<int>(
                """
                SELECT COUNT(*) AS Value
                FROM information_schema.tables
                WHERE table_schema = DATABASE()
                  AND LOWER(table_name) = {0}
                """,
                table)
            .SingleAsync(cancellationToken);

        return count > 0;
    }

    private static async Task<bool> NorthwindSchemaExistsAsync(NorthwindContext db, CancellationToken cancellationToken)
    {
        try
        {
            var tables = await db.Database
                .SqlQueryRaw<int>(
                    """
                    SELECT COUNT(*) AS Value
                    FROM information_schema.tables
                    WHERE table_schema = DATABASE()
                      AND LOWER(table_name) IN ('categories', 'products', 'orders', 'order details')
                    """)
                .SingleAsync(cancellationToken);

            return tables >= 3;
        }
        catch
        {
            // Si la base de datos todavía no existe, la migración la creará.
            return false;
        }
    }
}
