using Dapper;
using System.Data;

namespace Produkty24_API.Db
{
    public static class DatabaseInitializer
    {
        public static void Initialize(IDbConnection connection)
        {
            connection.Open();

            connection.Execute(@"
CREATE TABLE IF NOT EXISTS Countries (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    Name TEXT NOT NULL
);

CREATE TABLE IF NOT EXISTS Currencies (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    Code TEXT NOT NULL
);

CREATE TABLE IF NOT EXISTS Manufacturers (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    Name TEXT NOT NULL
);

CREATE TABLE IF NOT EXISTS OrderStatuses (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    Name TEXT NOT NULL
);

CREATE TABLE IF NOT EXISTS ShippingMethods (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    Name TEXT NOT NULL
);

CREATE TABLE IF NOT EXISTS ExchangeRates (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    Date TEXT NOT NULL,
    CurrencyId INTEGER NOT NULL,
    Value REAL NOT NULL,
    FOREIGN KEY (CurrencyId) REFERENCES Currencies(Id) ON DELETE CASCADE
);

CREATE TABLE IF NOT EXISTS StockItems (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    Name TEXT,
    ManufacturerId INTEGER,
    Description TEXT,
    CurrencyId INTEGER,
    PurchasePrice REAL NOT NULL,
    RetailPrice REAL NOT NULL,
    FOREIGN KEY (ManufacturerId) REFERENCES Manufacturers(Id),
    FOREIGN KEY (CurrencyId) REFERENCES Currencies(Id)
);

CREATE TABLE IF NOT EXISTS Clients (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    Date TEXT,
    Name TEXT NOT NULL,
    Nickname TEXT,
    Phone TEXT,
    Email TEXT,
    City TEXT,
    Address TEXT,
    PostalCode TEXT,
    Notes TEXT,
    ShippingMethodId INTEGER,
    CountryId INTEGER,
    FOREIGN KEY (ShippingMethodId) REFERENCES ShippingMethods(Id),
    FOREIGN KEY (CountryId) REFERENCES Countries(Id)
);

CREATE TABLE IF NOT EXISTS StockArrivals (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    Date TEXT NOT NULL,
    StockItemId INTEGER NOT NULL,
    Quantity REAL NOT NULL,
    FOREIGN KEY (StockItemId) REFERENCES StockItems(Id) ON DELETE CASCADE
);

CREATE TABLE IF NOT EXISTS Orders (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    Date TEXT NOT NULL,
    ClientId INTEGER NOT NULL,
    StatusId INTEGER NOT NULL,
    Notes TEXT,
    FOREIGN KEY (ClientId) REFERENCES Clients(Id) ON DELETE CASCADE,
    FOREIGN KEY (StatusId) REFERENCES OrderStatuses(Id) ON DELETE CASCADE
);

CREATE TABLE IF NOT EXISTS OrdersItems (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    OrderId INTEGER NOT NULL,
    StockItemId INTEGER NOT NULL,
    Quantity REAL NOT NULL,
    Price REAL,
    Discount REAL,
    Total REAL,
    Profit REAL,
    Expenses REAL,
    ExchangeRate REAL,
    FOREIGN KEY (OrderId) REFERENCES Orders(Id) ON DELETE CASCADE,
    FOREIGN KEY (StockItemId) REFERENCES StockItems(Id) ON DELETE CASCADE
);

CREATE TABLE IF NOT EXISTS Payments (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    Date TEXT NOT NULL,
    ClientId INTEGER NOT NULL,
    OrderId INTEGER,
    Amount REAL NOT NULL,
    Notes TEXT,
    FOREIGN KEY (ClientId) REFERENCES Clients(Id) ON DELETE CASCADE,
    FOREIGN KEY (OrderId) REFERENCES Orders(Id)
);
");

            // Seed data only if tables are empty
            var countryCount = connection.ExecuteScalar<int>("SELECT COUNT(*) FROM Countries");
            if (countryCount == 0)
            {
                connection.Execute(@"
INSERT INTO Countries (Id, Name) VALUES (1, 'Украина'), (2, 'Молдова'), (3, 'Польша');
INSERT INTO Currencies (Id, Code) VALUES (1, 'EUR'), (2, 'USD'), (3, 'UAH');
INSERT INTO ShippingMethods (Id, Name) VALUES (1, 'Новая почта'), (2, 'Укрпочта'), (3, 'Самовывоз');
INSERT INTO OrderStatuses (Id, Name) VALUES (1, 'Готов'), (2, 'К отправке'), (3, 'Оплачен полностью'), (4, 'НОВЫЙ'), (5, 'Выставлен счёт'), (6, 'Оплачен частично'), (7, 'Отправлен');
INSERT INTO ExchangeRates (Id, Date, CurrencyId, Value) VALUES (1, '2000-01-01', 3, 1);
");
            }
        }
    }
}
