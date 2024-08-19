using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace giftcard_api.Migrations
{
    /// <inheritdoc />
    public partial class AjoutdeEmailaBeneficiary : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "Beneficiaries",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Email",
                table: "Beneficiaries");
        }
    }
}
