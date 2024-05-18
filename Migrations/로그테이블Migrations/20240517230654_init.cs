using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DSEV.Migrations.로그테이블Migrations
{
    public partial class init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "public");

            migrationBuilder.CreateTable(
                name: "logs",
                schema: "public",
                columns: table => new
                {
                    ltime = table.Column<DateTime>(nullable: false),
                    ltype = table.Column<int>(nullable: false),
                    larea = table.Column<string>(nullable: true),
                    lsubj = table.Column<string>(nullable: true),
                    lcont = table.Column<string>(nullable: true),
                    luser = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_logs", x => x.ltime);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "logs",
                schema: "public");
        }
    }
}
