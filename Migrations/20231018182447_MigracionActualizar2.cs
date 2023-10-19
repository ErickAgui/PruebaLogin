using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PruebaLogin.Migrations
{
    /// <inheritdoc />
    public partial class MigracionActualizar2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "auxPassword",
                table: "logins",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "auxPassword",
                table: "logins");
        }
    }
}
