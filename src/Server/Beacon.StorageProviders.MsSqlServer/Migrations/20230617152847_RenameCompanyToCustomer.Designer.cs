﻿// <auto-generated />
using Beacon.API.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Beacon.StorageProviders.MsSqlServer.Migrations;

[DbContext(typeof(BeaconDbContext))]
[Migration("20230617152847_RenameCompanyToCustomer")]
partial class RenameCompanyToCustomer
{
    /// <inheritdoc />
    protected override void BuildTargetModel(ModelBuilder modelBuilder)
    {
#pragma warning disable 612, 618
        modelBuilder
            .HasAnnotation("ProductVersion", "7.0.5")
            .HasAnnotation("Relational:MaxIdentifierLength", 128);

        SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

        modelBuilder.Entity("Beacon.App.Entities.Invitation", b =>
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

        modelBuilder.Entity("Beacon.App.Entities.InvitationEmail", b =>
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

        modelBuilder.Entity("Beacon.App.Entities.Laboratory", b =>
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

        modelBuilder.Entity("Beacon.App.Entities.Membership", b =>
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

        modelBuilder.Entity("Beacon.App.Entities.Project", b =>
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

                b.HasKey("Id");

                b.HasIndex("CreatedById");

                b.HasIndex("LaboratoryId");

                b.ToTable("Projects");
            });

        modelBuilder.Entity("Beacon.App.Entities.User", b =>
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

        modelBuilder.Entity("Beacon.App.Entities.Invitation", b =>
            {
                b.HasOne("Beacon.App.Entities.User", "AcceptedBy")
                    .WithMany()
                    .HasForeignKey("AcceptedById");

                b.HasOne("Beacon.App.Entities.User", "CreatedBy")
                    .WithMany()
                    .HasForeignKey("CreatedById")
                    .OnDelete(DeleteBehavior.Restrict)
                    .IsRequired();

                b.HasOne("Beacon.App.Entities.Laboratory", "Laboratory")
                    .WithMany()
                    .HasForeignKey("LaboratoryId")
                    .OnDelete(DeleteBehavior.Restrict)
                    .IsRequired();

                b.Navigation("AcceptedBy");

                b.Navigation("CreatedBy");

                b.Navigation("Laboratory");
            });

        modelBuilder.Entity("Beacon.App.Entities.InvitationEmail", b =>
            {
                b.HasOne("Beacon.App.Entities.Laboratory", "Laboratory")
                    .WithMany()
                    .HasForeignKey("LaboratoryId")
                    .OnDelete(DeleteBehavior.Restrict)
                    .IsRequired();

                b.HasOne("Beacon.App.Entities.Invitation", "LaboratoryInvitation")
                    .WithMany("EmailInvitations")
                    .HasForeignKey("LaboratoryInvitationId")
                    .OnDelete(DeleteBehavior.Cascade)
                    .IsRequired();

                b.Navigation("Laboratory");

                b.Navigation("LaboratoryInvitation");
            });

        modelBuilder.Entity("Beacon.App.Entities.Membership", b =>
            {
                b.HasOne("Beacon.App.Entities.Laboratory", "Laboratory")
                    .WithMany("Memberships")
                    .HasForeignKey("LaboratoryId")
                    .OnDelete(DeleteBehavior.Restrict)
                    .IsRequired();

                b.HasOne("Beacon.App.Entities.User", "Member")
                    .WithMany("Memberships")
                    .HasForeignKey("MemberId")
                    .OnDelete(DeleteBehavior.Cascade)
                    .IsRequired();

                b.Navigation("Laboratory");

                b.Navigation("Member");
            });

        modelBuilder.Entity("Beacon.App.Entities.Project", b =>
            {
                b.HasOne("Beacon.App.Entities.User", "CreatedBy")
                    .WithMany()
                    .HasForeignKey("CreatedById")
                    .OnDelete(DeleteBehavior.Restrict)
                    .IsRequired();

                b.HasOne("Beacon.App.Entities.Laboratory", "Laboratory")
                    .WithMany()
                    .HasForeignKey("LaboratoryId")
                    .OnDelete(DeleteBehavior.Restrict)
                    .IsRequired();

                b.OwnsOne("Beacon.App.ValueObjects.ProjectCode", "ProjectCode", b1 =>
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

                        b1.HasIndex("CustomerCode", "Suffix")
                            .IsUnique();

                        b1.ToTable("Projects");

                        b1.WithOwner()
                            .HasForeignKey("ProjectId");
                    });

                b.Navigation("CreatedBy");

                b.Navigation("Laboratory");

                b.Navigation("ProjectCode")
                    .IsRequired();
            });

        modelBuilder.Entity("Beacon.App.Entities.Invitation", b =>
            {
                b.Navigation("EmailInvitations");
            });

        modelBuilder.Entity("Beacon.App.Entities.Laboratory", b =>
            {
                b.Navigation("Memberships");
            });

        modelBuilder.Entity("Beacon.App.Entities.User", b =>
            {
                b.Navigation("Memberships");
            });
#pragma warning restore 612, 618
    }
}
