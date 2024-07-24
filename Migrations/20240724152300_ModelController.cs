using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace giftcard_api.Migrations
{
    /// <inheritdoc />
    public partial class ModelController : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "BeneficiaryWallets",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Solde = table.Column<double>(type: "double", nullable: false),
                    Devise = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BeneficiaryWallets", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "MerchantWallets",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Solde = table.Column<double>(type: "double", nullable: false),
                    Devise = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MerchantWallets", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Packages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Budget = table.Column<double>(type: "double", nullable: false),
                    Prix = table.Column<double>(type: "double", nullable: false),
                    MaxCarte = table.Column<int>(type: "int", nullable: false),
                    MontantBase = table.Column<int>(type: "int", nullable: false),
                    DateExpirations = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Packages", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    RoleNom = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "SubscriberWallets",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Solde = table.Column<double>(type: "double", nullable: false),
                    Devise = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubscriberWallets", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    IdRole = table.Column<int>(type: "int", nullable: false),
                    Email = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Password = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    RefreshToken = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    RefreshTokenExpiryTime = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    DateInscription = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    Adresse = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IsActive = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    Discriminator = table.Column<string>(type: "varchar(13)", maxLength: 13, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Has_gochap = table.Column<bool>(type: "tinyint(1)", nullable: true),
                    Beneficiary_IdUser = table.Column<int>(type: "int", nullable: true),
                    IdBeneficiaryWallet = table.Column<int>(type: "int", nullable: true),
                    Beneficiary_Nom = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Beneficiary_Prenom = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Beneficiary_ProfilePhoto = table.Column<byte[]>(type: "longblob", nullable: true),
                    Merchant_IdUser = table.Column<int>(type: "int", nullable: true),
                    IdMerchantWallet = table.Column<int>(type: "int", nullable: true),
                    Nom = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Prenom = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ProfilePhoto = table.Column<byte[]>(type: "longblob", nullable: true),
                    IdUser = table.Column<int>(type: "int", nullable: true),
                    IdSubscriberWallet = table.Column<int>(type: "int", nullable: true),
                    SubscriberName = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Users_BeneficiaryWallets_IdBeneficiaryWallet",
                        column: x => x.IdBeneficiaryWallet,
                        principalTable: "BeneficiaryWallets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Users_MerchantWallets_IdMerchantWallet",
                        column: x => x.IdMerchantWallet,
                        principalTable: "MerchantWallets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Users_Roles_IdRole",
                        column: x => x.IdRole,
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Users_SubscriberWallets_IdSubscriberWallet",
                        column: x => x.IdSubscriberWallet,
                        principalTable: "SubscriberWallets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Users_Users_Beneficiary_IdUser",
                        column: x => x.Beneficiary_IdUser,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Users_Users_IdUser",
                        column: x => x.IdUser,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Users_Users_Merchant_IdUser",
                        column: x => x.Merchant_IdUser,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "BeneficiaryHistories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    IdBeneficiary = table.Column<int>(type: "int", nullable: false),
                    Action = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Montant = table.Column<double>(type: "double", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BeneficiaryHistories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BeneficiaryHistories_Users_IdBeneficiary",
                        column: x => x.IdBeneficiary,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "MerchantHistories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    IdMerchant = table.Column<int>(type: "int", nullable: false),
                    Action = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Montant = table.Column<double>(type: "double", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MerchantHistories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MerchantHistories_Users_IdMerchant",
                        column: x => x.IdMerchant,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "SubscriberHistories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    IdSubscriber = table.Column<int>(type: "int", nullable: false),
                    Action = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Montant = table.Column<double>(type: "double", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubscriberHistories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SubscriberHistories_Users_IdSubscriber",
                        column: x => x.IdSubscriber,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Subscriptions",
                columns: table => new
                {
                    IdPackage = table.Column<int>(type: "int", nullable: false),
                    IdSubscriber = table.Column<int>(type: "int", nullable: false),
                    DateSouscription = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Subscriptions", x => new { x.IdSubscriber, x.IdPackage });
                    table.ForeignKey(
                        name: "FK_Subscriptions_Packages_IdPackage",
                        column: x => x.IdPackage,
                        principalTable: "Packages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Subscriptions_Users_IdSubscriber",
                        column: x => x.IdSubscriber,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_BeneficiaryHistories_IdBeneficiary",
                table: "BeneficiaryHistories",
                column: "IdBeneficiary");

            migrationBuilder.CreateIndex(
                name: "IX_MerchantHistories_IdMerchant",
                table: "MerchantHistories",
                column: "IdMerchant");

            migrationBuilder.CreateIndex(
                name: "IX_SubscriberHistories_IdSubscriber",
                table: "SubscriberHistories",
                column: "IdSubscriber");

            migrationBuilder.CreateIndex(
                name: "IX_Subscriptions_IdPackage",
                table: "Subscriptions",
                column: "IdPackage");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Beneficiary_IdUser",
                table: "Users",
                column: "Beneficiary_IdUser");

            migrationBuilder.CreateIndex(
                name: "IX_Users_IdBeneficiaryWallet",
                table: "Users",
                column: "IdBeneficiaryWallet");

            migrationBuilder.CreateIndex(
                name: "IX_Users_IdMerchantWallet",
                table: "Users",
                column: "IdMerchantWallet");

            migrationBuilder.CreateIndex(
                name: "IX_Users_IdRole",
                table: "Users",
                column: "IdRole");

            migrationBuilder.CreateIndex(
                name: "IX_Users_IdSubscriberWallet",
                table: "Users",
                column: "IdSubscriberWallet");

            migrationBuilder.CreateIndex(
                name: "IX_Users_IdUser",
                table: "Users",
                column: "IdUser");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Merchant_IdUser",
                table: "Users",
                column: "Merchant_IdUser");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BeneficiaryHistories");

            migrationBuilder.DropTable(
                name: "MerchantHistories");

            migrationBuilder.DropTable(
                name: "SubscriberHistories");

            migrationBuilder.DropTable(
                name: "Subscriptions");

            migrationBuilder.DropTable(
                name: "Packages");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "BeneficiaryWallets");

            migrationBuilder.DropTable(
                name: "MerchantWallets");

            migrationBuilder.DropTable(
                name: "Roles");

            migrationBuilder.DropTable(
                name: "SubscriberWallets");
        }
    }
}
