using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NewEra_Cash___Carry.Migrations
{
    /// <inheritdoc />
    public partial class CapturePendingChanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "PasswordHash",
                value: "$2a$11$65sWhs2Kmh/1skJgiRFwuebT8QnYO8DnhaV4mgC0JR04FmV1QOO0O");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2,
                column: "PasswordHash",
                value: "$2a$11$tIS2kfkJGo2gZgQHQNaejOhMrEAszTpUKbgjLbbAUiVZZ3b7WpUAu");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "PasswordHash",
                value: "$2a$11$0UvNmXfNEoj9yxUW3BBB.OvNzUaE6LBw09smAC04QVnxWNizBhdeK");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2,
                column: "PasswordHash",
                value: "$2a$11$vmZEl7IOkphbSgUz0tusKOKvks6dfE1jJ6RnE1GlX8.AT2fR3s4rW");
        }
    }
}
