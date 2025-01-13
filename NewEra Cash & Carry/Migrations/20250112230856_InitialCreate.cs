using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NewEra_Cash___Carry.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "PasswordHash", "PhoneNumber" },
                values: new object[] { "$2a$11$0UvNmXfNEoj9yxUW3BBB.OvNzUaE6LBw09smAC04QVnxWNizBhdeK", "+998913892033" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "PasswordHash", "PhoneNumber" },
                values: new object[] { "$2a$11$vmZEl7IOkphbSgUz0tusKOKvks6dfE1jJ6RnE1GlX8.AT2fR3s4rW", "+998913892034" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "PasswordHash", "PhoneNumber" },
                values: new object[] { "hashedpassword1", "1234567890" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "PasswordHash", "PhoneNumber" },
                values: new object[] { "hashedpassword2", "0987654321" });
        }
    }
}
