using System;
using BusinessManagement.Api.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace BusinessManagement.Api.Migrations
{
    [DbContext(typeof(AppDbContext))]
    partial class AppDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "10.0.1")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            modelBuilder.Entity("BusinessManagement.Api.Entities.InventoryBatch", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime>("ExpirationDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<Guid>("ProductId")
                        .HasColumnType("uuid");

                    b.Property<int>("Quantity")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("ProductId");

                    b.ToTable("InventoryBatches");

                    b.HasData(
                        new
                        {
                            Id = new Guid("55555555-5555-5555-5555-555555555555"),
                            CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc),
                            ExpirationDate = new DateTime(2024, 7, 1, 0, 0, 0, 0, DateTimeKind.Utc),
                            ProductId = new Guid("22222222-2222-2222-2222-222222222222"),
                            Quantity = 100
                        },
                        new
                        {
                            Id = new Guid("66666666-6666-6666-6666-666666666666"),
                            CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc),
                            ExpirationDate = new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc),
                            ProductId = new Guid("33333333-3333-3333-3333-333333333333"),
                            Quantity = 150
                        },
                        new
                        {
                            Id = new Guid("77777777-7777-7777-7777-777777777777"),
                            CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc),
                            ExpirationDate = new DateTime(2024, 4, 1, 0, 0, 0, 0, DateTimeKind.Utc),
                            ProductId = new Guid("44444444-4444-4444-4444-444444444444"),
                            Quantity = 80
                        });
                });

            modelBuilder.Entity("BusinessManagement.Api.Entities.Product", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<bool>("IsActive")
                        .HasColumnType("boolean");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<decimal>("Price")
                        .HasColumnType("numeric(18,2)");

                    b.HasKey("Id");

                    b.ToTable("Products");

                    b.HasData(
                        new
                        {
                            Id = new Guid("22222222-2222-2222-2222-222222222222"),
                            IsActive = true,
                            Name = "Popcorn",
                            Price = 5.99m
                        },
                        new
                        {
                            Id = new Guid("33333333-3333-3333-3333-333333333333"),
                            IsActive = true,
                            Name = "Soda",
                            Price = 3.49m
                        },
                        new
                        {
                            Id = new Guid("44444444-4444-4444-4444-444444444444"),
                            IsActive = true,
                            Name = "Nachos",
                            Price = 6.99m
                        });
                });

            modelBuilder.Entity("BusinessManagement.Api.Entities.Sale", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<Guid>("ShiftId")
                        .HasColumnType("uuid");

                    b.Property<decimal>("TotalAmount")
                        .HasColumnType("numeric(18,2)");

                    b.HasKey("Id");

                    b.HasIndex("ShiftId");

                    b.ToTable("Sales");
                });

            modelBuilder.Entity("BusinessManagement.Api.Entities.SaleItem", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<Guid>("ProductId")
                        .HasColumnType("uuid");

                    b.Property<int>("Quantity")
                        .HasColumnType("integer");

                    b.Property<Guid>("SaleId")
                        .HasColumnType("uuid");

                    b.Property<decimal>("UnitPrice")
                        .HasColumnType("numeric(18,2)");

                    b.HasKey("Id");

                    b.HasIndex("ProductId");

                    b.HasIndex("SaleId");

                    b.ToTable("SaleItems");
                });

            modelBuilder.Entity("BusinessManagement.Api.Entities.Shift", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<Guid>("CashierId")
                        .HasColumnType("uuid");

                    b.Property<decimal?>("ClosingCash")
                        .HasColumnType("numeric(18,2)");

                    b.Property<DateTime?>("EndTime")
                        .HasColumnType("timestamp with time zone");

                    b.Property<decimal>("OpeningCash")
                        .HasColumnType("numeric(18,2)");

                    b.Property<DateTime>("StartTime")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("Id");

                    b.HasIndex("CashierId");

                    b.ToTable("Shifts");
                });

            modelBuilder.Entity("BusinessManagement.Api.Entities.User", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<bool>("IsActive")
                        .HasColumnType("boolean");

                    b.Property<string>("PasswordHash")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Role")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("Email")
                        .IsUnique();

                    b.ToTable("Users");
                });

            modelBuilder.Entity("BusinessManagement.Api.Entities.InventoryBatch", b =>
                {
                    b.HasOne("BusinessManagement.Api.Entities.Product", "Product")
                        .WithMany("InventoryBatches")
                        .HasForeignKey("ProductId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Product");
                });

            modelBuilder.Entity("BusinessManagement.Api.Entities.Sale", b =>
                {
                    b.HasOne("BusinessManagement.Api.Entities.Shift", "Shift")
                        .WithMany("Sales")
                        .HasForeignKey("ShiftId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Shift");
                });

            modelBuilder.Entity("BusinessManagement.Api.Entities.SaleItem", b =>
                {
                    b.HasOne("BusinessManagement.Api.Entities.Product", "Product")
                        .WithMany("SaleItems")
                        .HasForeignKey("ProductId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("BusinessManagement.Api.Entities.Sale", "Sale")
                        .WithMany("SaleItems")
                        .HasForeignKey("SaleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Product");

                    b.Navigation("Sale");
                });

            modelBuilder.Entity("BusinessManagement.Api.Entities.Shift", b =>
                {
                    b.HasOne("BusinessManagement.Api.Entities.User", "Cashier")
                        .WithMany("Shifts")
                        .HasForeignKey("CashierId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Cashier");
                });

            modelBuilder.Entity("BusinessManagement.Api.Entities.Product", b =>
                {
                    b.Navigation("InventoryBatches");

                    b.Navigation("SaleItems");
                });

            modelBuilder.Entity("BusinessManagement.Api.Entities.Sale", b =>
                {
                    b.Navigation("SaleItems");
                });

            modelBuilder.Entity("BusinessManagement.Api.Entities.Shift", b =>
                {
                    b.Navigation("Sales");
                });

            modelBuilder.Entity("BusinessManagement.Api.Entities.User", b =>
                {
                    b.Navigation("Shifts");
                });
        }
    }
}