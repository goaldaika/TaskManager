﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using TaskManagement.Data;

#nullable disable

namespace TaskManagement.Migrations
{
    [DbContext(typeof(DataContext))]
    [Migration("20240324154356_InnitialCreate")]
    partial class InnitialCreate
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.3")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("TaskManagement.Models.Assignment", b =>
                {
                    b.Property<int>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("id"));

                    b.Property<int?>("ParentId")
                        .HasColumnType("int");

                    b.Property<int?>("ProgrammerId")
                        .HasColumnType("int");

                    b.Property<DateTime?>("closingDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("description")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("estimateHours")
                        .HasColumnType("int");

                    b.Property<string>("name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("startDate")
                        .HasColumnType("datetime2");

                    b.Property<int>("state")
                        .HasColumnType("int");

                    b.HasKey("id");

                    b.HasIndex("ParentId");

                    b.HasIndex("ProgrammerId");

                    b.ToTable("Assignments");
                });

            modelBuilder.Entity("TaskManagement.Models.Programmer", b =>
                {
                    b.Property<int>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("id"));

                    b.Property<string>("address")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("email")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("fname")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("lname")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<long?>("phonenumber")
                        .HasColumnType("bigint");

                    b.HasKey("id");

                    b.ToTable("Programmers");
                });

            modelBuilder.Entity("TaskManagement.Models.Assignment", b =>
                {
                    b.HasOne("TaskManagement.Models.Assignment", "Parent")
                        .WithMany("Child")
                        .HasForeignKey("ParentId");

                    b.HasOne("TaskManagement.Models.Programmer", "AssignedProgrammer")
                        .WithMany("assignments")
                        .HasForeignKey("ProgrammerId");

                    b.Navigation("AssignedProgrammer");

                    b.Navigation("Parent");
                });

            modelBuilder.Entity("TaskManagement.Models.Assignment", b =>
                {
                    b.Navigation("Child");
                });

            modelBuilder.Entity("TaskManagement.Models.Programmer", b =>
                {
                    b.Navigation("assignments");
                });
#pragma warning restore 612, 618
        }
    }
}
