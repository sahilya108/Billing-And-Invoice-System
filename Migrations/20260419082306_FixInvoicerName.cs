using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BillingAndInvoiceSystem.Migrations
{
    /// <inheritdoc />
    public partial class FixInvoicerName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BillerName",
                table: "Invoices",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BillerName",
                table: "Invoices");
        }
    }
}
