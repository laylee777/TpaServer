using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DSEV.Migrations
{
    public partial class init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "public");

            migrationBuilder.CreateTable(
                name: "inspd",
                schema: "public",
                columns: table => new
                {
                    idwdt = table.Column<DateTime>(nullable: false),
                    iditm = table.Column<int>(nullable: false),
                    idgrp = table.Column<int>(nullable: false),
                    iddev = table.Column<int>(nullable: false),
                    idcat = table.Column<int>(nullable: false),
                    iduni = table.Column<int>(nullable: false),
                    idstd = table.Column<decimal>(nullable: false),
                    idmin = table.Column<decimal>(nullable: false),
                    idmax = table.Column<decimal>(nullable: false),
                    idoff = table.Column<decimal>(nullable: false),
                    idcal = table.Column<decimal>(nullable: false),
                    idmes = table.Column<decimal>(nullable: false),
                    idval = table.Column<decimal>(nullable: false),
                    idres = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_inspd", x => new { x.idwdt, x.iditm });
                });

            migrationBuilder.CreateTable(
                name: "inspl",
                schema: "public",
                columns: table => new
                {
                    ilwdt = table.Column<DateTime>(nullable: false),
                    ilmcd = table.Column<int>(nullable: false),
                    ilnum = table.Column<int>(nullable: false),
                    ilres = table.Column<int>(nullable: false),
                    ilctq = table.Column<int>(nullable: false),
                    ilsuf = table.Column<int>(nullable: false),
                    ilqrs = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_inspl", x => x.ilwdt);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "inspd",
                schema: "public");

            migrationBuilder.DropTable(
                name: "inspl",
                schema: "public");
        }
    }
}
