using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Produkty24_API.Migrations
{
    public partial class SeedDefaultData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Countries",
                columns: new[] { "Id", "Name" },
                values: new object[] { 1, "Украина" });

            migrationBuilder.InsertData(
                table: "Countries",
                columns: new[] { "Id", "Name" },
                values: new object[] { 2, "Молдова" });

            migrationBuilder.InsertData(
                table: "Countries",
                columns: new[] { "Id", "Name" },
                values: new object[] { 3, "Польша" });

            migrationBuilder.InsertData(
                table: "Currencies",
                columns: new[] { "Id", "Code" },
                values: new object[] { 1, "EUR" });

            migrationBuilder.InsertData(
                table: "Currencies",
                columns: new[] { "Id", "Code" },
                values: new object[] { 2, "USD" });

            migrationBuilder.InsertData(
                table: "Currencies",
                columns: new[] { "Id", "Code" },
                values: new object[] { 3, "UAH" });

            migrationBuilder.InsertData(
                table: "OrderStatuses",
                columns: new[] { "Id", "Name" },
                values: new object[] { 1, "Готов" });

            migrationBuilder.InsertData(
                table: "OrderStatuses",
                columns: new[] { "Id", "Name" },
                values: new object[] { 2, "К отправке" });

            migrationBuilder.InsertData(
                table: "OrderStatuses",
                columns: new[] { "Id", "Name" },
                values: new object[] { 3, "Оплачен полностью" });

            migrationBuilder.InsertData(
                table: "OrderStatuses",
                columns: new[] { "Id", "Name" },
                values: new object[] { 4, "НОВЫЙ" });

            migrationBuilder.InsertData(
                table: "OrderStatuses",
                columns: new[] { "Id", "Name" },
                values: new object[] { 5, "Выставлен счёт" });

            migrationBuilder.InsertData(
                table: "OrderStatuses",
                columns: new[] { "Id", "Name" },
                values: new object[] { 6, "Оплачен частично" });

            migrationBuilder.InsertData(
                table: "OrderStatuses",
                columns: new[] { "Id", "Name" },
                values: new object[] { 7, "Отправлен" });

            migrationBuilder.InsertData(
                table: "ShippingMethods",
                columns: new[] { "Id", "Name" },
                values: new object[] { 1, "Новая почта" });

            migrationBuilder.InsertData(
                table: "ShippingMethods",
                columns: new[] { "Id", "Name" },
                values: new object[] { 2, "Укрпочта" });

            migrationBuilder.InsertData(
                table: "ShippingMethods",
                columns: new[] { "Id", "Name" },
                values: new object[] { 3, "Самовывоз" });

            migrationBuilder.InsertData(
                table: "ExchangeRates",
                columns: new[] { "Id", "CurrencyId", "Date", "Value" },
                values: new object[] { 1, 3, new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1f });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Currencies",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Currencies",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "ExchangeRates",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "OrderStatuses",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "OrderStatuses",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "OrderStatuses",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "OrderStatuses",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "OrderStatuses",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "OrderStatuses",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "OrderStatuses",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "ShippingMethods",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "ShippingMethods",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "ShippingMethods",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Currencies",
                keyColumn: "Id",
                keyValue: 3);
        }
    }
}
