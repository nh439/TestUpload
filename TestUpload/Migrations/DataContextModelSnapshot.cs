﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using TestUpload;

namespace TestUpload.Migrations
{
    [DbContext(typeof(DataContext))]
    partial class DataContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Relational:MaxIdentifierLength", 64)
                .HasAnnotation("ProductVersion", "5.0.10");

            modelBuilder.Entity("TestUpload.Models.Entity.FileStorage", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("varchar(767)");

                    b.Property<DateTime>("AddDate")
                        .HasColumnType("datetime");

                    b.Property<string>("Comment")
                        .HasColumnType("text");

                    b.Property<string>("FileExtension")
                        .HasColumnType("text");

                    b.Property<decimal>("FileSize")
                        .HasColumnType("decimal(18, 2)");

                    b.Property<string>("Filename")
                        .HasColumnType("text");

                    b.Property<DateTime>("LastUpdate")
                        .HasColumnType("datetime");

                    b.Property<byte[]>("RawData")
                        .HasColumnType("longblob");

                    b.Property<long>("UserId")
                        .HasColumnType("bigint");

                    b.Property<string>("pass")
                        .HasMaxLength(255)
                        .HasColumnType("varchar(255)");

                    b.HasKey("Id");

                    b.ToTable("fileStorage");
                });

            modelBuilder.Entity("TestUpload.Models.Entity.FileUpload", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("varchar(767)");

                    b.Property<DateTime>("AddDate")
                        .HasColumnType("datetime");

                    b.Property<string>("Comment")
                        .HasColumnType("text");

                    b.Property<string>("FileExtension")
                        .HasColumnType("text");

                    b.Property<decimal>("FileSize")
                        .HasColumnType("decimal(18, 2)");

                    b.Property<string>("Filename")
                        .HasColumnType("text");

                    b.Property<DateTime>("LastUpdate")
                        .HasColumnType("datetime");

                    b.Property<long>("UserId")
                        .HasColumnType("bigint");

                    b.Property<string>("pass")
                        .HasMaxLength(255)
                        .HasColumnType("varchar(255)");

                    b.HasKey("Id");

                    b.ToTable("fileUploads");
                });

            modelBuilder.Entity("TestUpload.Models.Entity.Login", b =>
                {
                    b.Property<string>("Username")
                        .HasColumnType("varchar(767)");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasMaxLength(514)
                        .HasColumnType("varchar(514)");

                    b.HasKey("Username");

                    b.ToTable("login");
                });

            modelBuilder.Entity("TestUpload.Models.Entity.Session", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("varchar(767)");

                    b.Property<string>("IpAddress")
                        .HasColumnType("text");

                    b.Property<DateTime>("LoggedIn")
                        .HasColumnType("datetime");

                    b.Property<DateTime?>("Loggedout")
                        .HasColumnType("datetime");

                    b.Property<long>("UserId")
                        .HasColumnType("bigint");

                    b.HasKey("Id");

                    b.ToTable("sessions");
                });

            modelBuilder.Entity("TestUpload.Models.Entity.User", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    b.Property<DateTime>("BrithDay")
                        .HasColumnType("datetime");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("varchar(767)");

                    b.Property<string>("Firstname")
                        .HasColumnType("text");

                    b.Property<string>("Lastname")
                        .HasColumnType("text");

                    b.Property<string>("LoginUsername")
                        .HasColumnType("varchar(767)");

                    b.Property<bool>("Male")
                        .HasColumnType("tinyint(1)");

                    b.Property<DateTime>("Registerd")
                        .HasColumnType("datetime");

                    b.Property<string>("Rules")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("Email")
                        .IsUnique();

                    b.HasIndex("LoginUsername");

                    b.ToTable("User");
                });

            modelBuilder.Entity("TestUpload.Models.Entity.User", b =>
                {
                    b.HasOne("TestUpload.Models.Entity.Login", "Login")
                        .WithMany()
                        .HasForeignKey("LoginUsername");

                    b.Navigation("Login");
                });
#pragma warning restore 612, 618
        }
    }
}
