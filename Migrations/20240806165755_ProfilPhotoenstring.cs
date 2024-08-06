using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace giftcard_api.Migrations
{
    /// <inheritdoc />
    public partial class ProfilPhotoenstring : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProfilePhoto",
                table: "Merchants");

            migrationBuilder.DropColumn(
                name: "ProfilePhoto",
                table: "Beneficiaries");

            migrationBuilder.AddColumn<string>(
                name: "ProfilPhoto",
                table: "Merchants",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "ProfilPhoto",
                table: "Beneficiaries",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProfilPhoto",
                table: "Merchants");

            migrationBuilder.DropColumn(
                name: "ProfilPhoto",
                table: "Beneficiaries");

            migrationBuilder.AddColumn<byte[]>(
                name: "ProfilePhoto",
                table: "Merchants",
                type: "longblob",
                nullable: true);

            migrationBuilder.AddColumn<byte[]>(
                name: "ProfilePhoto",
                table: "Beneficiaries",
                type: "longblob",
                nullable: true);
        }
    }
}
