﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using RestaurantManagement.Infrastructure.Restaurants;

namespace RestaurantManagement.Infrastructure.Migrations
{
    [DbContext(typeof(RestaurantDbContext))]
    [Migration("20210417131551_Init")]
    partial class Init
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Relational:MaxIdentifierLength", 64)
                .HasAnnotation("ProductVersion", "5.0.5");

            modelBuilder.Entity("RestaurantManagement.Domain.Restaurants.Restaurant", b =>
                {
                    b.Property<int>("RestaurantId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("City")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<int>("Cuisine")
                        .HasColumnType("int");

                    b.Property<string>("Description")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<float>("Rating")
                        .HasColumnType("float");

                    b.Property<string>("RestaurantName")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<int?>("Stars")
                        .HasColumnType("int");

                    b.Property<string>("Street")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<int>("maxNumberOfGuests")
                        .HasColumnType("int")
                        .HasColumnName("MaxNumberOfGuests");

                    b.HasKey("RestaurantId");

                    b.ToTable("Restaurants");
                });

            modelBuilder.Entity("RestaurantManagement.Domain.Restaurants.RestaurantGuest", b =>
                {
                    b.Property<int>("RestaurantGuestId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("CurrentGuestId");

                    b.Property<int>("GuestId")
                        .HasColumnType("int")
                        .HasColumnName("GuestId");

                    b.Property<int>("RestaurantId")
                        .HasColumnType("int");

                    b.HasKey("RestaurantGuestId");

                    b.HasIndex("RestaurantId");

                    b.ToTable("RestaurantGuests");
                });

            modelBuilder.Entity("RestaurantManagement.Domain.Restaurants.RestaurantGuest", b =>
                {
                    b.HasOne("RestaurantManagement.Domain.Restaurants.Restaurant", null)
                        .WithMany("currentGuests")
                        .HasForeignKey("RestaurantId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("RestaurantManagement.Domain.Restaurants.Restaurant", b =>
                {
                    b.Navigation("currentGuests");
                });
#pragma warning restore 612, 618
        }
    }
}