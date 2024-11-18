﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using NEFORmal.ua.Dating.Infrastructure;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Dating.Infrastructure.Migrations
{
    [DbContext(typeof(DatingDbContext))]
    [Migration("20241121013239_InitialDatingDbContext")]
    partial class InitialDatingDbContext
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("NEFORmal.ua.Dating.ApplicationCore.Models.Date", b =>
                {
                    b.Property<int>("SenderId")
                        .HasColumnType("integer");

                    b.Property<int>("ReceiverId")
                        .HasColumnType("integer");

                    b.Property<bool>("IsApproved")
                        .HasColumnType("boolean");

                    b.Property<string>("Message")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.HasKey("SenderId", "ReceiverId");

                    b.HasIndex("ReceiverId")
                        .HasDatabaseName("IX_Date_ReceiverId");

                    b.HasIndex("SenderId")
                        .HasDatabaseName("IX_Date_SenderId");

                    b.ToTable("Dates");
                });

            modelBuilder.Entity("NEFORmal.ua.Dating.ApplicationCore.Models.Profile", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int>("Age")
                        .HasColumnType("integer");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(32)
                        .HasColumnType("character varying(32)");

                    b.PrimitiveCollection<string[]>("ProfilePhotos")
                        .IsRequired()
                        .HasColumnType("text[]");

                    b.Property<string>("Sex")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Sid")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("Sex", "Age")
                        .HasDatabaseName("IX_Profile_Sex_Age");

                    b.ToTable("Profiles");
                });
#pragma warning restore 612, 618
        }
    }
}
