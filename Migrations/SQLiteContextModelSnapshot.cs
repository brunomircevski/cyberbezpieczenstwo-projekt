﻿// <auto-generated />
using System;
using Cyberbezpieczenstwo.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace Cyberbezpieczenstwo.Migrations
{
    [DbContext(typeof(SQLiteContext))]
    partial class SQLiteContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "9.0.0");

            modelBuilder.Entity("Cyberbezpieczenstwo.Models.Message", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Content")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("Date")
                        .HasColumnType("TEXT");

                    b.Property<int>("SenderId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("SenderId");

                    b.ToTable("Messages");
                });

            modelBuilder.Entity("Cyberbezpieczenstwo.Models.PasswordFragment", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Hash")
                        .HasColumnType("TEXT");

                    b.Property<string>("Pattern")
                        .HasColumnType("TEXT");

                    b.Property<int?>("UserId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("PasswordFragment");
                });

            modelBuilder.Entity("Cyberbezpieczenstwo.Models.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("FailedLoginsCounter")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("IsLocked")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime?>("LastFailedLogin")
                        .HasColumnType("TEXT");

                    b.Property<DateTime?>("LastLogin")
                        .HasColumnType("TEXT");

                    b.Property<int>("MaxFailedLogins")
                        .HasColumnType("INTEGER");

                    b.Property<int>("NextFragmentIndex")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Username")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("MessageUser", b =>
                {
                    b.Property<int>("EditableMessagesId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("EditorsId")
                        .HasColumnType("INTEGER");

                    b.HasKey("EditableMessagesId", "EditorsId");

                    b.HasIndex("EditorsId");

                    b.ToTable("MessageEditors", (string)null);
                });

            modelBuilder.Entity("Cyberbezpieczenstwo.Models.Message", b =>
                {
                    b.HasOne("Cyberbezpieczenstwo.Models.User", "Sender")
                        .WithMany("OwnMessages")
                        .HasForeignKey("SenderId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Sender");
                });

            modelBuilder.Entity("Cyberbezpieczenstwo.Models.PasswordFragment", b =>
                {
                    b.HasOne("Cyberbezpieczenstwo.Models.User", null)
                        .WithMany("PasswordFragments")
                        .HasForeignKey("UserId");
                });

            modelBuilder.Entity("MessageUser", b =>
                {
                    b.HasOne("Cyberbezpieczenstwo.Models.Message", null)
                        .WithMany()
                        .HasForeignKey("EditableMessagesId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Cyberbezpieczenstwo.Models.User", null)
                        .WithMany()
                        .HasForeignKey("EditorsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Cyberbezpieczenstwo.Models.User", b =>
                {
                    b.Navigation("OwnMessages");

                    b.Navigation("PasswordFragments");
                });
#pragma warning restore 612, 618
        }
    }
}
