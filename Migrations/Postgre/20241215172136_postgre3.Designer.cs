﻿// <auto-generated />
using System;
using BDwAS_projekt.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace BDwAS_projekt.Migrations.Postgre
{
    [DbContext(typeof(PostgreContext))]
    [Migration("20241215172136_postgre3")]
    partial class postgre3
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("BDwAS_projekt.Models.Attachment", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.Property<string>("Path")
                        .HasColumnType("text");

                    b.Property<string>("PostId")
                        .HasColumnType("text");

                    b.Property<int>("Size")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("PostId");

                    b.ToTable("Attachments");
                });

            modelBuilder.Entity("BDwAS_projekt.Models.Category", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text");

                    b.Property<int>("MinimumAge")
                        .HasColumnType("integer");

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Categories");
                });

            modelBuilder.Entity("BDwAS_projekt.Models.Channel", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text");

                    b.Property<DateTime>("CreationDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Description")
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.Property<string>("OwnerId")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("OwnerId")
                        .IsUnique();

                    b.ToTable("Channels");
                });

            modelBuilder.Entity("BDwAS_projekt.Models.Comment", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text");

                    b.Property<string>("AuthorId")
                        .HasColumnType("text");

                    b.Property<string>("Content")
                        .HasColumnType("text");

                    b.Property<DateTime>("CreationDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("PostId")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("AuthorId");

                    b.HasIndex("PostId");

                    b.ToTable("Comments");
                });

            modelBuilder.Entity("BDwAS_projekt.Models.Image", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text");

                    b.Property<string>("Path")
                        .HasColumnType("text");

                    b.Property<string>("PostId")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("PostId");

                    b.ToTable("Images");
                });

            modelBuilder.Entity("BDwAS_projekt.Models.LiveStream", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text");

                    b.Property<string>("ChannelId")
                        .HasColumnType("text");

                    b.Property<DateTime>("EndDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.Property<string>("SavedPath")
                        .HasColumnType("text");

                    b.Property<DateTime>("StartDate")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("Id");

                    b.HasIndex("ChannelId");

                    b.ToTable("LiveStreams");
                });

            modelBuilder.Entity("BDwAS_projekt.Models.Message", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text");

                    b.Property<string>("Content")
                        .HasColumnType("text");

                    b.Property<DateTime>("Date")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("RecipientId")
                        .HasColumnType("text");

                    b.Property<string>("SenderId")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("RecipientId");

                    b.HasIndex("SenderId");

                    b.ToTable("Messages");
                });

            modelBuilder.Entity("BDwAS_projekt.Models.Payment", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text");

                    b.Property<string>("SubscriptionId")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("SubscriptionId");

                    b.ToTable("Payments");
                });

            modelBuilder.Entity("BDwAS_projekt.Models.Plan", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text");

                    b.Property<string>("ChannelId")
                        .HasColumnType("text");

                    b.Property<int>("Days")
                        .HasColumnType("integer");

                    b.Property<double>("Discount")
                        .HasColumnType("double precision");

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.Property<double>("Price")
                        .HasColumnType("double precision");

                    b.HasKey("Id");

                    b.HasIndex("ChannelId");

                    b.ToTable("Plans");
                });

            modelBuilder.Entity("BDwAS_projekt.Models.Post", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text");

                    b.Property<string>("ChannelId")
                        .HasColumnType("text");

                    b.Property<string>("Content")
                        .HasColumnType("text");

                    b.Property<DateTime>("CreationDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<bool>("IsSponsored")
                        .HasColumnType("boolean");

                    b.Property<string>("Title")
                        .HasColumnType("text");

                    b.Property<string>("UserId")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("ChannelId");

                    b.HasIndex("UserId");

                    b.ToTable("Posts");
                });

            modelBuilder.Entity("BDwAS_projekt.Models.Rating", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text");

                    b.Property<string>("AuthorId")
                        .HasColumnType("text");

                    b.Property<string>("PostId")
                        .HasColumnType("text");

                    b.Property<int>("Value")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("AuthorId");

                    b.HasIndex("PostId");

                    b.ToTable("Ratings");
                });

            modelBuilder.Entity("BDwAS_projekt.Models.Subscription", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text");

                    b.Property<bool>("AutoRenew")
                        .HasColumnType("boolean");

                    b.Property<string>("ChannelId")
                        .HasColumnType("text");

                    b.Property<DateTime>("EndDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime>("StartDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("UserId")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("ChannelId");

                    b.HasIndex("UserId");

                    b.ToTable("Subscriptions");
                });

            modelBuilder.Entity("BDwAS_projekt.Models.User", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text");

                    b.Property<string>("Email")
                        .HasColumnType("text");

                    b.Property<string>("FirstName")
                        .HasColumnType("text");

                    b.Property<bool>("IsVerified")
                        .HasColumnType("boolean");

                    b.Property<string>("LastName")
                        .HasColumnType("text");

                    b.Property<DateTime>("RegistrationDate")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("Id");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("CategoryChannel", b =>
                {
                    b.Property<string>("CategoriesId")
                        .HasColumnType("text");

                    b.Property<string>("ChannelsId")
                        .HasColumnType("text");

                    b.HasKey("CategoriesId", "ChannelsId");

                    b.HasIndex("ChannelsId");

                    b.ToTable("CategoryChannel");
                });

            modelBuilder.Entity("BDwAS_projekt.Models.Attachment", b =>
                {
                    b.HasOne("BDwAS_projekt.Models.Post", "Post")
                        .WithMany("Attachments")
                        .HasForeignKey("PostId");

                    b.Navigation("Post");
                });

            modelBuilder.Entity("BDwAS_projekt.Models.Channel", b =>
                {
                    b.HasOne("BDwAS_projekt.Models.User", "Owner")
                        .WithOne("Channel")
                        .HasForeignKey("BDwAS_projekt.Models.Channel", "OwnerId");

                    b.Navigation("Owner");
                });

            modelBuilder.Entity("BDwAS_projekt.Models.Comment", b =>
                {
                    b.HasOne("BDwAS_projekt.Models.User", "Author")
                        .WithMany("Comments")
                        .HasForeignKey("AuthorId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("BDwAS_projekt.Models.Post", "Post")
                        .WithMany("Comments")
                        .HasForeignKey("PostId");

                    b.Navigation("Author");

                    b.Navigation("Post");
                });

            modelBuilder.Entity("BDwAS_projekt.Models.Image", b =>
                {
                    b.HasOne("BDwAS_projekt.Models.Post", "Post")
                        .WithMany("Images")
                        .HasForeignKey("PostId");

                    b.Navigation("Post");
                });

            modelBuilder.Entity("BDwAS_projekt.Models.LiveStream", b =>
                {
                    b.HasOne("BDwAS_projekt.Models.Channel", "Channel")
                        .WithMany("Streams")
                        .HasForeignKey("ChannelId");

                    b.Navigation("Channel");
                });

            modelBuilder.Entity("BDwAS_projekt.Models.Message", b =>
                {
                    b.HasOne("BDwAS_projekt.Models.User", "Recipient")
                        .WithMany("ReceivedMessages")
                        .HasForeignKey("RecipientId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("BDwAS_projekt.Models.User", "Sender")
                        .WithMany("SendMessages")
                        .HasForeignKey("SenderId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.Navigation("Recipient");

                    b.Navigation("Sender");
                });

            modelBuilder.Entity("BDwAS_projekt.Models.Payment", b =>
                {
                    b.HasOne("BDwAS_projekt.Models.Subscription", "Subscription")
                        .WithMany("Payments")
                        .HasForeignKey("SubscriptionId");

                    b.OwnsOne("BDwAS_projekt.Models.PaymentDetails", "Details", b1 =>
                        {
                            b1.Property<string>("PaymentId")
                                .HasColumnType("text");

                            b1.Property<int>("AddedDays")
                                .HasColumnType("integer")
                                .HasColumnName("AddedDays");

                            b1.Property<double>("Discount")
                                .HasColumnType("double precision")
                                .HasColumnName("Discount");

                            b1.Property<double>("FullPrice")
                                .HasColumnType("double precision")
                                .HasColumnName("FullPrice");

                            b1.Property<double>("PaidPrice")
                                .HasColumnType("double precision")
                                .HasColumnName("PaidPrice");

                            b1.Property<DateTime>("PaymentDate")
                                .HasColumnType("timestamp with time zone")
                                .HasColumnName("PaymentDate");

                            b1.HasKey("PaymentId");

                            b1.ToTable("Payments");

                            b1.WithOwner()
                                .HasForeignKey("PaymentId");
                        });

                    b.Navigation("Details");

                    b.Navigation("Subscription");
                });

            modelBuilder.Entity("BDwAS_projekt.Models.Plan", b =>
                {
                    b.HasOne("BDwAS_projekt.Models.Channel", "Channel")
                        .WithMany("Plans")
                        .HasForeignKey("ChannelId");

                    b.Navigation("Channel");
                });

            modelBuilder.Entity("BDwAS_projekt.Models.Post", b =>
                {
                    b.HasOne("BDwAS_projekt.Models.Channel", "Channel")
                        .WithMany("Posts")
                        .HasForeignKey("ChannelId");

                    b.HasOne("BDwAS_projekt.Models.User", null)
                        .WithMany("ViewedPosts")
                        .HasForeignKey("UserId");

                    b.Navigation("Channel");
                });

            modelBuilder.Entity("BDwAS_projekt.Models.Rating", b =>
                {
                    b.HasOne("BDwAS_projekt.Models.User", "Author")
                        .WithMany("Ratings")
                        .HasForeignKey("AuthorId");

                    b.HasOne("BDwAS_projekt.Models.Post", "Post")
                        .WithMany("Ratings")
                        .HasForeignKey("PostId");

                    b.Navigation("Author");

                    b.Navigation("Post");
                });

            modelBuilder.Entity("BDwAS_projekt.Models.Subscription", b =>
                {
                    b.HasOne("BDwAS_projekt.Models.Channel", "Channel")
                        .WithMany("Subscriptions")
                        .HasForeignKey("ChannelId");

                    b.HasOne("BDwAS_projekt.Models.User", "User")
                        .WithMany("Subscriptions")
                        .HasForeignKey("UserId");

                    b.Navigation("Channel");

                    b.Navigation("User");
                });

            modelBuilder.Entity("CategoryChannel", b =>
                {
                    b.HasOne("BDwAS_projekt.Models.Category", null)
                        .WithMany()
                        .HasForeignKey("CategoriesId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("BDwAS_projekt.Models.Channel", null)
                        .WithMany()
                        .HasForeignKey("ChannelsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("BDwAS_projekt.Models.Channel", b =>
                {
                    b.Navigation("Plans");

                    b.Navigation("Posts");

                    b.Navigation("Streams");

                    b.Navigation("Subscriptions");
                });

            modelBuilder.Entity("BDwAS_projekt.Models.Post", b =>
                {
                    b.Navigation("Attachments");

                    b.Navigation("Comments");

                    b.Navigation("Images");

                    b.Navigation("Ratings");
                });

            modelBuilder.Entity("BDwAS_projekt.Models.Subscription", b =>
                {
                    b.Navigation("Payments");
                });

            modelBuilder.Entity("BDwAS_projekt.Models.User", b =>
                {
                    b.Navigation("Channel");

                    b.Navigation("Comments");

                    b.Navigation("Ratings");

                    b.Navigation("ReceivedMessages");

                    b.Navigation("SendMessages");

                    b.Navigation("Subscriptions");

                    b.Navigation("ViewedPosts");
                });
#pragma warning restore 612, 618
        }
    }
}
