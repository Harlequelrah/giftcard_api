using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace giftcard_api.Migrations
{
    /// <inheritdoc />
    public partial class AddTelephoneNumeroToBeneficiary : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TelephoneNumero",
                table: "Beneficiaries",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TelephoneNumero",
                table: "Beneficiaries");
        }
    }
}
