﻿// <auto-generated />
using System;
using Beacon.API.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace Beacon.API.Persistence.Migrations
{
    [DbContext(typeof(BeaconDbContext))]
    [Migration("20230723212925_AddLabInstruments")]
    partial class AddLabInstruments
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.9")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("Beacon.API.Persistence.Entities.Invitation", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid?>("AcceptedById")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("CreatedById")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTimeOffset>("CreatedOn")
                        .HasColumnType("datetimeoffset");

                    b.Property<double>("ExpireAfterDays")
                        .HasColumnType("float");

                    b.Property<Guid>("LaboratoryId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("MembershipType")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("nvarchar(20)");

                    b.Property<string>("NewMemberEmailAddress")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.HasKey("Id");

                    b.HasIndex("AcceptedById");

                    b.HasIndex("CreatedById");

                    b.HasIndex("LaboratoryId");

                    b.ToTable("Invitations");
                });

            modelBuilder.Entity("Beacon.API.Persistence.Entities.InvitationEmail", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTimeOffset>("ExpiresOn")
                        .HasColumnType("datetimeoffset");

                    b.Property<Guid>("LaboratoryId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("LaboratoryInvitationId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("OperationId")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTimeOffset>("SentOn")
                        .HasColumnType("datetimeoffset");

                    b.HasKey("Id");

                    b.HasIndex("LaboratoryId");

                    b.HasIndex("LaboratoryInvitationId");

                    b.ToTable("InvitationEmails");
                });

            modelBuilder.Entity("Beacon.API.Persistence.Entities.Laboratory", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.HasKey("Id");

                    b.ToTable("Laboratories");
                });

            modelBuilder.Entity("Beacon.API.Persistence.Entities.LaboratoryInstrument", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("InstrumentType")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<Guid>("LaboratoryId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("SerialNumber")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.HasKey("Id");

                    b.HasIndex("LaboratoryId");

                    b.ToTable("LaboratoryInstruments");
                });

            modelBuilder.Entity("Beacon.API.Persistence.Entities.Membership", b =>
                {
                    b.Property<Guid>("LaboratoryId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("MemberId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("MembershipType")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("nvarchar(20)");

                    b.HasKey("LaboratoryId", "MemberId");

                    b.HasIndex("MemberId");

                    b.ToTable("Memberships");
                });

            modelBuilder.Entity("Beacon.API.Persistence.Entities.Project", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("CreatedById")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("CustomerName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("LaboratoryId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid?>("LeadAnalystId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("ProjectStatus")
                        .IsRequired()
                        .HasMaxLength(25)
                        .HasColumnType("nvarchar(25)");

                    b.HasKey("Id");

                    b.HasIndex("CreatedById");

                    b.HasIndex("LaboratoryId");

                    b.HasIndex("LeadAnalystId");

                    b.HasIndex("ProjectStatus");

                    b.ToTable("Projects");
                });

            modelBuilder.Entity("Beacon.API.Persistence.Entities.ProjectContact", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("EmailAddress")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<Guid>("LaboratoryId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("PhoneNumber")
                        .HasMaxLength(20)
                        .HasColumnType("nvarchar(20)");

                    b.Property<Guid>("ProjectId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("LaboratoryId");

                    b.HasIndex("ProjectId");

                    b.ToTable("ProjectContacts");
                });

            modelBuilder.Entity("Beacon.API.Persistence.Entities.ProjectEvent", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("LaboratoryId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("ProjectId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTimeOffset>("ScheduledEnd")
                        .HasColumnType("datetimeoffset");

                    b.Property<DateTimeOffset>("ScheduledStart")
                        .HasColumnType("datetimeoffset");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.HasKey("Id");

                    b.HasIndex("LaboratoryId");

                    b.HasIndex("ProjectId");

                    b.ToTable("ProjectEvents");
                });

            modelBuilder.Entity("Beacon.API.Persistence.Entities.SampleGroup", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("ContainerType")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool?>("IsHazardous")
                        .HasColumnType("bit");

                    b.Property<bool?>("IsLightSensitive")
                        .HasColumnType("bit");

                    b.Property<Guid>("LaboratoryId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Notes")
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("ProjectId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<int?>("Quantity")
                        .HasColumnType("int");

                    b.Property<string>("SampleName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<double?>("TargetStorageHumidity")
                        .HasColumnType("float");

                    b.Property<double?>("TargetStorageTemperature")
                        .HasColumnType("float");

                    b.Property<double?>("Volume")
                        .HasColumnType("float");

                    b.HasKey("Id");

                    b.HasIndex("LaboratoryId");

                    b.HasIndex("ProjectId");

                    b.ToTable("SampleGroups");
                });

            modelBuilder.Entity("Beacon.API.Persistence.Entities.User", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("DisplayName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("EmailAddress")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("HashedPassword")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<byte[]>("HashedPasswordSalt")
                        .IsRequired()
                        .HasColumnType("varbinary(max)");

                    b.HasKey("Id");

                    b.HasIndex("EmailAddress")
                        .IsUnique();

                    b.ToTable("Users");
                });

            modelBuilder.Entity("Beacon.API.Persistence.Entities.Invitation", b =>
                {
                    b.HasOne("Beacon.API.Persistence.Entities.User", "AcceptedBy")
                        .WithMany()
                        .HasForeignKey("AcceptedById");

                    b.HasOne("Beacon.API.Persistence.Entities.User", "CreatedBy")
                        .WithMany()
                        .HasForeignKey("CreatedById")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("Beacon.API.Persistence.Entities.Laboratory", "Laboratory")
                        .WithMany()
                        .HasForeignKey("LaboratoryId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("AcceptedBy");

                    b.Navigation("CreatedBy");

                    b.Navigation("Laboratory");
                });

            modelBuilder.Entity("Beacon.API.Persistence.Entities.InvitationEmail", b =>
                {
                    b.HasOne("Beacon.API.Persistence.Entities.Laboratory", "Laboratory")
                        .WithMany()
                        .HasForeignKey("LaboratoryId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("Beacon.API.Persistence.Entities.Invitation", "LaboratoryInvitation")
                        .WithMany("EmailInvitations")
                        .HasForeignKey("LaboratoryInvitationId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Laboratory");

                    b.Navigation("LaboratoryInvitation");
                });

            modelBuilder.Entity("Beacon.API.Persistence.Entities.LaboratoryInstrument", b =>
                {
                    b.HasOne("Beacon.API.Persistence.Entities.Laboratory", "Laboratory")
                        .WithMany()
                        .HasForeignKey("LaboratoryId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Laboratory");
                });

            modelBuilder.Entity("Beacon.API.Persistence.Entities.Membership", b =>
                {
                    b.HasOne("Beacon.API.Persistence.Entities.Laboratory", "Laboratory")
                        .WithMany("Memberships")
                        .HasForeignKey("LaboratoryId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("Beacon.API.Persistence.Entities.User", "Member")
                        .WithMany("Memberships")
                        .HasForeignKey("MemberId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Laboratory");

                    b.Navigation("Member");
                });

            modelBuilder.Entity("Beacon.API.Persistence.Entities.Project", b =>
                {
                    b.HasOne("Beacon.API.Persistence.Entities.User", "CreatedBy")
                        .WithMany()
                        .HasForeignKey("CreatedById")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("Beacon.API.Persistence.Entities.Laboratory", "Laboratory")
                        .WithMany()
                        .HasForeignKey("LaboratoryId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("Beacon.API.Persistence.Entities.User", "LeadAnalyst")
                        .WithMany("Projects")
                        .HasForeignKey("LeadAnalystId");

                    b.OwnsOne("Beacon.Common.Models.ProjectCode", "ProjectCode", b1 =>
                        {
                            b1.Property<Guid>("ProjectId")
                                .HasColumnType("uniqueidentifier");

                            b1.Property<string>("CustomerCode")
                                .IsRequired()
                                .HasMaxLength(3)
                                .HasColumnType("nvarchar(3)");

                            b1.Property<int>("Suffix")
                                .HasColumnType("int");

                            b1.HasKey("ProjectId");

                            b1.HasIndex("CustomerCode", "Suffix");

                            b1.ToTable("Projects");

                            b1.WithOwner()
                                .HasForeignKey("ProjectId");
                        });

                    b.Navigation("CreatedBy");

                    b.Navigation("Laboratory");

                    b.Navigation("LeadAnalyst");

                    b.Navigation("ProjectCode")
                        .IsRequired();
                });

            modelBuilder.Entity("Beacon.API.Persistence.Entities.ProjectContact", b =>
                {
                    b.HasOne("Beacon.API.Persistence.Entities.Laboratory", "Laboratory")
                        .WithMany()
                        .HasForeignKey("LaboratoryId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Beacon.API.Persistence.Entities.Project", "Project")
                        .WithMany("Contacts")
                        .HasForeignKey("ProjectId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Laboratory");

                    b.Navigation("Project");
                });

            modelBuilder.Entity("Beacon.API.Persistence.Entities.ProjectEvent", b =>
                {
                    b.HasOne("Beacon.API.Persistence.Entities.Laboratory", "Laboratory")
                        .WithMany()
                        .HasForeignKey("LaboratoryId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("Beacon.API.Persistence.Entities.Project", "Project")
                        .WithMany()
                        .HasForeignKey("ProjectId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Laboratory");

                    b.Navigation("Project");
                });

            modelBuilder.Entity("Beacon.API.Persistence.Entities.SampleGroup", b =>
                {
                    b.HasOne("Beacon.API.Persistence.Entities.Laboratory", "Laboratory")
                        .WithMany()
                        .HasForeignKey("LaboratoryId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Beacon.API.Persistence.Entities.Project", "Project")
                        .WithMany("SampleGroups")
                        .HasForeignKey("ProjectId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Laboratory");

                    b.Navigation("Project");
                });

            modelBuilder.Entity("Beacon.API.Persistence.Entities.Invitation", b =>
                {
                    b.Navigation("EmailInvitations");
                });

            modelBuilder.Entity("Beacon.API.Persistence.Entities.Laboratory", b =>
                {
                    b.Navigation("Memberships");
                });

            modelBuilder.Entity("Beacon.API.Persistence.Entities.Project", b =>
                {
                    b.Navigation("Contacts");

                    b.Navigation("SampleGroups");
                });

            modelBuilder.Entity("Beacon.API.Persistence.Entities.User", b =>
                {
                    b.Navigation("Memberships");

                    b.Navigation("Projects");
                });
#pragma warning restore 612, 618
        }
    }
}
