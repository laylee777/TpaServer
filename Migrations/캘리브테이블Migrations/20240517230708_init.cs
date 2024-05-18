using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DSEV.Migrations.캘리브테이블Migrations
{
    public partial class init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "public");

            migrationBuilder.CreateTable(
                name: "calibs",
                schema: "public",
                columns: table => new
                {
                    ctime = table.Column<DateTime>(nullable: false),
                    ccame = table.Column<int>(nullable: false),
                    cindx = table.Column<int>(nullable: false),
                    cwidt = table.Column<decimal>(nullable: false),
                    cheig = table.Column<decimal>(nullable: false),
                    clent = table.Column<decimal>(nullable: false),
                    clenb = table.Column<decimal>(nullable: false),
                    clenl = table.Column<decimal>(nullable: false),
                    clenr = table.Column<decimal>(nullable: false),
                    cangt = table.Column<decimal>(nullable: false),
                    cangb = table.Column<decimal>(nullable: false),
                    cangl = table.Column<decimal>(nullable: false),
                    cangr = table.Column<decimal>(nullable: false),
                    ccalx = table.Column<decimal>(nullable: false),
                    ccaly = table.Column<decimal>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_calibs", x => x.ctime);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "calibs",
                schema: "public");
        }
    }
}
