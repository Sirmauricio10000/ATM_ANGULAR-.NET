﻿// <auto-generated />
using System;
using ATM.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace ATM.Data.Migrations
{
    [DbContext(typeof(ATMContext))]
    [Migration("20220921042425_Initial")]
    partial class Initial
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.9")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder, 1L, 1);

            modelBuilder.Entity("ATM.Core.Model.AmountTransaction", b =>
                {
                    b.Property<string>("TransactionId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<int>("Denomination")
                        .HasColumnType("int");

                    b.Property<int>("Quantity")
                        .HasColumnType("int");

                    b.HasKey("TransactionId", "Denomination");

                    b.HasIndex("Denomination");

                    b.ToTable("AmountTransactions");
                });

            modelBuilder.Entity("ATM.Core.Model.Bill", b =>
                {
                    b.Property<int>("Denomination")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Denomination"), 1L, 1);

                    b.Property<int>("Quantity")
                        .HasColumnType("int");

                    b.HasKey("Denomination");

                    b.ToTable("Bills");

                    b.HasData(
                        new
                        {
                            Denomination = 10,
                            Quantity = 30
                        },
                        new
                        {
                            Denomination = 20,
                            Quantity = 25
                        },
                        new
                        {
                            Denomination = 50,
                            Quantity = 20
                        },
                        new
                        {
                            Denomination = 100,
                            Quantity = 15
                        });
                });

            modelBuilder.Entity("ATM.Core.Model.Transaction", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<DateTime>("DateTime")
                        .HasColumnType("datetime2");

                    b.Property<int>("Type")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("Transactions");
                });

            modelBuilder.Entity("ATM.Core.Model.AmountTransaction", b =>
                {
                    b.HasOne("ATM.Core.Model.Bill", "Bill")
                        .WithMany()
                        .HasForeignKey("Denomination")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("ATM.Core.Model.Transaction", "Transaction")
                        .WithMany("Amount")
                        .HasForeignKey("TransactionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Bill");

                    b.Navigation("Transaction");
                });

            modelBuilder.Entity("ATM.Core.Model.Transaction", b =>
                {
                    b.Navigation("Amount");
                });
#pragma warning restore 612, 618
        }
    }
}
