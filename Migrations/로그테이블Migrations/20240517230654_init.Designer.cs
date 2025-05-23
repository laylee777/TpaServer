﻿// <auto-generated />
using System;
using IVM.Schemas;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace IVM.Migrations.로그테이블Migrations
{
    [DbContext(typeof(로그테이블))]
    [Migration("20240517230654_init")]
    partial class init
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasDefaultSchema("public")
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn)
                .HasAnnotation("ProductVersion", "3.1.32")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            modelBuilder.Entity("IVM.Schemas.로그정보", b =>
                {
                    b.Property<DateTime>("시간")
                        .HasColumnName("ltime")
                        .HasColumnType("timestamp without time zone");

                    b.Property<int>("구분")
                        .HasColumnName("ltype")
                        .HasColumnType("integer");

                    b.Property<string>("내용")
                        .HasColumnName("lcont")
                        .HasColumnType("text");

                    b.Property<string>("영역")
                        .HasColumnName("larea")
                        .HasColumnType("text");

                    b.Property<string>("작업자")
                        .HasColumnName("luser")
                        .HasColumnType("text");

                    b.Property<string>("제목")
                        .HasColumnName("lsubj")
                        .HasColumnType("text");

                    b.HasKey("시간");

                    b.ToTable("logs");
                });
#pragma warning restore 612, 618
        }
    }
}
