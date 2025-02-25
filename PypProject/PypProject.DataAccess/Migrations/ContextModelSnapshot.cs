﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using PypProject.DataAccess.Concrete.DataContext;

namespace PypProject.DataAccess.Migrations
{
    [DbContext(typeof(Context))]
    partial class ContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Relational:MaxIdentifierLength", 63)
                .HasAnnotation("ProductVersion", "5.0.17")
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            modelBuilder.Entity("PypProject.Entities.Concrete.ProductData", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<decimal>("COGS")
                        .HasColumnType("numeric");

                    b.Property<string>("Country")
                        .HasColumnType("text");

                    b.Property<DateTime>("Date")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("DiscountBand")
                        .HasColumnType("text");

                    b.Property<decimal?>("Discounts")
                        .HasColumnType("numeric");

                    b.Property<decimal>("GrossSales")
                        .HasColumnType("numeric");

                    b.Property<decimal>("ManufacturingPrice")
                        .HasColumnType("numeric");

                    b.Property<string>("Product")
                        .HasColumnType("text");

                    b.Property<decimal>("Profit")
                        .HasColumnType("numeric");

                    b.Property<decimal>("SalePrice")
                        .HasColumnType("numeric");

                    b.Property<decimal>("Sales")
                        .HasColumnType("numeric");

                    b.Property<string>("Segment")
                        .HasColumnType("text");

                    b.Property<decimal>("UnitsSold")
                        .HasColumnType("numeric");

                    b.HasKey("Id");

                    b.ToTable("Datas");
                });
#pragma warning restore 612, 618
        }
    }
}
