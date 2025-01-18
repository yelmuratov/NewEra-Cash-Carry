using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NewEra_Cash___Carry.Migrations
{
    /// <inheritdoc />
    public partial class NotificationEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Notifications",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Message = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notifications", x => x.Id);
                });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "PasswordHash", "PhoneNumber" },
                values: new object[] { "$2a$11$2szAjncynkvznTQbIPFSieM9YnkjH/qpFsui9SloH.suRaAeCjVty", "1234567890" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "PasswordHash", "PhoneNumber" },
                values: new object[] { "$2a$11$hvnPcYeR5eSPVt2K7Od.rOXWo2PScCw./nfBUT8BGm03fPUVEDC26", "0987654321" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Notifications");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "PasswordHash", "PhoneNumber" },
                values: new object[] { "$2a$11$65sWhs2Kmh/1skJgiRFwuebT8QnYO8DnhaV4mgC0JR04FmV1QOO0O", "+998913892033" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "PasswordHash", "PhoneNumber" },
                values: new object[] { "$2a$11$tIS2kfkJGo2gZgQHQNaejOhMrEAszTpUKbgjLbbAUiVZZ3b7WpUAu", "+998913892034" });
        }
    }
}
