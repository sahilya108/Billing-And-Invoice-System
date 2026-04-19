using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BillingAndInvoiceSystem.Migrations
{
    /// <inheritdoc />
    public partial class AddIspdfcol : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsPdf",
                table: "Invoices",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsPdf",
                table: "Invoices");
        }
    }
}
