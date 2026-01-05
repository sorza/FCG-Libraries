using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FCG_Libraries.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PaymentType",
                table: "Libraries");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PaymentType",
                table: "Libraries",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
