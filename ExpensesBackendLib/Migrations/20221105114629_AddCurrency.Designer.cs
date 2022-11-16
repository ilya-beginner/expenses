﻿// <auto-generated />
using System;
using Expenses;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace Expenses.Migrations
{
    [DbContext(typeof(ExpensesDb))]
    [Migration("20221105114629_AddCurrency")]
    partial class AddCurrency
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.8")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            modelBuilder.Entity("Expenses.Expense", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<int>("Currency")
                        .HasColumnType("int");

                    b.Property<DateOnly>("Date")
                        .HasColumnType("date");

                    b.Property<string>("Notes")
                        .HasColumnType("longtext");

                    b.Property<decimal>("Sum")
                        .HasColumnType("decimal(65,30)");

                    b.Property<string>("Tag")
                        .HasColumnType("varchar(255)");

                    b.HasKey("Id");

                    b.HasIndex("Date", "Tag");

                    b.ToTable("Expenses");
                });
#pragma warning restore 612, 618
        }
    }
}
