using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace GisProxy.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Endpoints",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    Title = table.Column<string>(type: "TEXT", nullable: false),
                    Limit = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Endpoints", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserEndpoints",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "TEXT", nullable: false),
                    EndpointId = table.Column<string>(type: "TEXT", nullable: false),
                    RequestsUsed = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserEndpoints", x => new { x.UserId, x.EndpointId });
                    table.ForeignKey(
                        name: "FK_UserEndpoints_Endpoints_EndpointId",
                        column: x => x.EndpointId,
                        principalTable: "Endpoints",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserEndpoints_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Endpoints",
                columns: new[] { "Id", "Limit", "Title" },
                values: new object[,]
                {
                    { "https://portaltest.gismap.by/arcservertest/rest/services/A01_ZIS_WGS84/Land_Minsk_public/MapServer/0", 100, "Виды земель" },
                    { "https://portaltest.gismap.by/arcservertest/rest/services/A05_EGRNI_WGS84/Uchastki_Minsk_public/MapServer/0", 100, "земельные участки" },
                    { "https://portaltest.gismap.by/arcservertest/rest/services/A06_ATE_TE_WGS84/ATE_Minsk_public/MapServer/1", 100, "Населенные пункты" },
                    { "https://portaltest.gismap.by/arcservertest/rest/services/C01_Belarus_WGS84/Belarus_BaseMap_WGS84/MapServer", 100, "Oбзорная карта" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserEndpoints_EndpointId",
                table: "UserEndpoints",
                column: "EndpointId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserEndpoints");

            migrationBuilder.DropTable(
                name: "Endpoints");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
