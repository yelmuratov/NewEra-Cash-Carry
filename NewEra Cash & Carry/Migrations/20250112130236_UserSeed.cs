using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace NewEra_Cash___Carry.Migrations
{
    /// <inheritdoc />
    public partial class UserSeed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserDto",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Roles = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserDto", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "UserDto",
                columns: new[] { "Id", "PhoneNumber", "Roles" },
                values: new object[,]
                {
                    { 1, "+998913892033", "[\"Admin\"]" },
                    { 2, "+998913892034", "[\"User\"]" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserDto");
        }
    }
}
