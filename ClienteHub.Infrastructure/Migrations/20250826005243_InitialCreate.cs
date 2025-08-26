using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ClienteHub.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TB_ADDRESSES",
                columns: table => new
                {
                    Id = table.Column<string>(type: "NVARCHAR2(36)", maxLength: 36, nullable: false),
                    Cep = table.Column<string>(type: "NVARCHAR2(8)", maxLength: 8, nullable: false),
                    Street = table.Column<string>(type: "NVARCHAR2(200)", maxLength: 200, nullable: false),
                    Neighborhood = table.Column<string>(type: "NVARCHAR2(120)", maxLength: 120, nullable: false),
                    City = table.Column<string>(type: "NVARCHAR2(120)", maxLength: 120, nullable: false),
                    State = table.Column<string>(type: "NVARCHAR2(2)", maxLength: 2, nullable: false),
                    Number = table.Column<string>(type: "NVARCHAR2(20)", maxLength: 20, nullable: false),
                    Complement = table.Column<string>(type: "NVARCHAR2(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TB_ADDRESSES", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TB_CUSTOMERS",
                columns: table => new
                {
                    Id = table.Column<string>(type: "NVARCHAR2(36)", maxLength: 36, nullable: false),
                    Name = table.Column<string>(type: "NVARCHAR2(200)", maxLength: 200, nullable: false),
                    Email = table.Column<string>(type: "NVARCHAR2(200)", maxLength: 200, nullable: false),
                    Cnpj = table.Column<string>(type: "NVARCHAR2(14)", maxLength: 14, nullable: false),
                    AddressId = table.Column<string>(type: "NVARCHAR2(36)", maxLength: 36, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TB_CUSTOMERS", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TB_CUSTOMERS_TB_ADDRESSES_AddressId",
                        column: x => x.AddressId,
                        principalTable: "TB_ADDRESSES",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TB_CUSTOMERS_AddressId",
                table: "TB_CUSTOMERS",
                column: "AddressId",
                unique: true,
                filter: "\"AddressId\" IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_TB_CUSTOMERS_Cnpj",
                table: "TB_CUSTOMERS",
                column: "Cnpj",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TB_CUSTOMERS");

            migrationBuilder.DropTable(
                name: "TB_ADDRESSES");
        }
    }
}
