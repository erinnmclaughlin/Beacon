using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Beacon.StorageProviders.Postgres.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "laboratories",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_laboratories", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    display_name = table.Column<string>(type: "text", nullable: false),
                    email_address = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    hashed_password = table.Column<string>(type: "text", nullable: false),
                    hashed_password_salt = table.Column<byte[]>(type: "bytea", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_users", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "laboratory_instruments",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    serial_number = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    instrument_type = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    laboratory_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_laboratory_instruments", x => x.id);
                    table.ForeignKey(
                        name: "fk_laboratory_instruments_laboratories_laboratory_id",
                        column: x => x.laboratory_id,
                        principalTable: "laboratories",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "project_applications",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    laboratory_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_project_applications", x => x.id);
                    table.ForeignKey(
                        name: "fk_project_applications_laboratories_laboratory_id",
                        column: x => x.laboratory_id,
                        principalTable: "laboratories",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "invitations",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_on = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    expire_after_days = table.Column<double>(type: "double precision", nullable: false),
                    new_member_email_address = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    membership_type = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    accepted_by_id = table.Column<Guid>(type: "uuid", nullable: true),
                    created_by_id = table.Column<Guid>(type: "uuid", nullable: false),
                    laboratory_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_invitations", x => x.id);
                    table.ForeignKey(
                        name: "fk_invitations_laboratories_laboratory_id",
                        column: x => x.laboratory_id,
                        principalTable: "laboratories",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_invitations_users_accepted_by_id",
                        column: x => x.accepted_by_id,
                        principalTable: "users",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_invitations_users_created_by_id",
                        column: x => x.created_by_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "memberships",
                columns: table => new
                {
                    laboratory_id = table.Column<Guid>(type: "uuid", nullable: false),
                    member_id = table.Column<Guid>(type: "uuid", nullable: false),
                    membership_type = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_memberships", x => new { x.laboratory_id, x.member_id });
                    table.ForeignKey(
                        name: "fk_memberships_laboratories_laboratory_id",
                        column: x => x.laboratory_id,
                        principalTable: "laboratories",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_memberships_users_member_id",
                        column: x => x.member_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "projects",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    customer_name = table.Column<string>(type: "text", nullable: false),
                    project_code_customer_code = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false),
                    project_code_date = table.Column<string>(type: "text", nullable: false),
                    project_code_suffix = table.Column<int>(type: "integer", nullable: false),
                    created_on = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    project_status = table.Column<string>(type: "character varying(25)", maxLength: 25, nullable: false),
                    created_by_id = table.Column<Guid>(type: "uuid", nullable: false),
                    lead_analyst_id = table.Column<Guid>(type: "uuid", nullable: true),
                    laboratory_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_projects", x => x.id);
                    table.ForeignKey(
                        name: "fk_projects_laboratories_laboratory_id",
                        column: x => x.laboratory_id,
                        principalTable: "laboratories",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_projects_users_created_by_id",
                        column: x => x.created_by_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_projects_users_lead_analyst_id",
                        column: x => x.lead_analyst_id,
                        principalTable: "users",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "invitation_emails",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    sent_on = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    expires_on = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    operation_id = table.Column<string>(type: "text", nullable: true),
                    laboratory_invitation_id = table.Column<Guid>(type: "uuid", nullable: false),
                    laboratory_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_invitation_emails", x => x.id);
                    table.ForeignKey(
                        name: "fk_invitation_emails_invitations_laboratory_invitation_id",
                        column: x => x.laboratory_invitation_id,
                        principalTable: "invitations",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_invitation_emails_laboratories_laboratory_id",
                        column: x => x.laboratory_id,
                        principalTable: "laboratories",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "project_application_tags",
                columns: table => new
                {
                    application_id = table.Column<Guid>(type: "uuid", nullable: false),
                    project_id = table.Column<Guid>(type: "uuid", nullable: false),
                    laboratory_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_project_application_tags", x => new { x.application_id, x.project_id });
                    table.ForeignKey(
                        name: "fk_project_application_tags_laboratories_laboratory_id",
                        column: x => x.laboratory_id,
                        principalTable: "laboratories",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_project_application_tags_project_applications_application_id",
                        column: x => x.application_id,
                        principalTable: "project_applications",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_project_application_tags_projects_project_id",
                        column: x => x.project_id,
                        principalTable: "projects",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "project_contacts",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    email_address = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    phone_number = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    project_id = table.Column<Guid>(type: "uuid", nullable: false),
                    laboratory_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_project_contacts", x => x.id);
                    table.ForeignKey(
                        name: "fk_project_contacts_laboratories_laboratory_id",
                        column: x => x.laboratory_id,
                        principalTable: "laboratories",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_project_contacts_projects_project_id",
                        column: x => x.project_id,
                        principalTable: "projects",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "project_events",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    title = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    description = table.Column<string>(type: "text", nullable: true),
                    scheduled_start = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    scheduled_end = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    project_id = table.Column<Guid>(type: "uuid", nullable: false),
                    laboratory_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_project_events", x => x.id);
                    table.ForeignKey(
                        name: "fk_project_events_laboratories_laboratory_id",
                        column: x => x.laboratory_id,
                        principalTable: "laboratories",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_project_events_projects_project_id",
                        column: x => x.project_id,
                        principalTable: "projects",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "sample_groups",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    sample_name = table.Column<string>(type: "text", nullable: false),
                    quantity = table.Column<int>(type: "integer", nullable: true),
                    container_type = table.Column<string>(type: "text", nullable: true),
                    volume = table.Column<double>(type: "double precision", nullable: true),
                    is_hazardous = table.Column<bool>(type: "boolean", nullable: true),
                    is_light_sensitive = table.Column<bool>(type: "boolean", nullable: true),
                    target_storage_temperature = table.Column<double>(type: "double precision", nullable: true),
                    target_storage_humidity = table.Column<double>(type: "double precision", nullable: true),
                    notes = table.Column<string>(type: "text", nullable: true),
                    project_id = table.Column<Guid>(type: "uuid", nullable: false),
                    laboratory_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_sample_groups", x => x.id);
                    table.ForeignKey(
                        name: "fk_sample_groups_laboratories_laboratory_id",
                        column: x => x.laboratory_id,
                        principalTable: "laboratories",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_sample_groups_projects_project_id",
                        column: x => x.project_id,
                        principalTable: "projects",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "laboratory_instrument_usage",
                columns: table => new
                {
                    instrument_id = table.Column<Guid>(type: "uuid", nullable: false),
                    project_event_id = table.Column<Guid>(type: "uuid", nullable: false),
                    laboratory_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_laboratory_instrument_usage", x => new { x.instrument_id, x.project_event_id });
                    table.ForeignKey(
                        name: "fk_laboratory_instrument_usage_laboratories_laboratory_id",
                        column: x => x.laboratory_id,
                        principalTable: "laboratories",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_laboratory_instrument_usage_laboratory_instruments_instrume",
                        column: x => x.instrument_id,
                        principalTable: "laboratory_instruments",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_laboratory_instrument_usage_project_events_project_event_id",
                        column: x => x.project_event_id,
                        principalTable: "project_events",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_invitation_emails_laboratory_id",
                table: "invitation_emails",
                column: "laboratory_id");

            migrationBuilder.CreateIndex(
                name: "ix_invitation_emails_laboratory_invitation_id",
                table: "invitation_emails",
                column: "laboratory_invitation_id");

            migrationBuilder.CreateIndex(
                name: "ix_invitations_accepted_by_id",
                table: "invitations",
                column: "accepted_by_id");

            migrationBuilder.CreateIndex(
                name: "ix_invitations_created_by_id",
                table: "invitations",
                column: "created_by_id");

            migrationBuilder.CreateIndex(
                name: "ix_invitations_laboratory_id",
                table: "invitations",
                column: "laboratory_id");

            migrationBuilder.CreateIndex(
                name: "ix_laboratory_instrument_usage_laboratory_id",
                table: "laboratory_instrument_usage",
                column: "laboratory_id");

            migrationBuilder.CreateIndex(
                name: "ix_laboratory_instrument_usage_project_event_id",
                table: "laboratory_instrument_usage",
                column: "project_event_id");

            migrationBuilder.CreateIndex(
                name: "ix_laboratory_instruments_laboratory_id",
                table: "laboratory_instruments",
                column: "laboratory_id");

            migrationBuilder.CreateIndex(
                name: "ix_memberships_member_id",
                table: "memberships",
                column: "member_id");

            migrationBuilder.CreateIndex(
                name: "ix_project_application_tags_laboratory_id",
                table: "project_application_tags",
                column: "laboratory_id");

            migrationBuilder.CreateIndex(
                name: "ix_project_application_tags_project_id",
                table: "project_application_tags",
                column: "project_id");

            migrationBuilder.CreateIndex(
                name: "ix_project_applications_laboratory_id",
                table: "project_applications",
                column: "laboratory_id");

            migrationBuilder.CreateIndex(
                name: "ix_project_applications_name",
                table: "project_applications",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_project_contacts_laboratory_id",
                table: "project_contacts",
                column: "laboratory_id");

            migrationBuilder.CreateIndex(
                name: "ix_project_contacts_project_id",
                table: "project_contacts",
                column: "project_id");

            migrationBuilder.CreateIndex(
                name: "ix_project_events_laboratory_id",
                table: "project_events",
                column: "laboratory_id");

            migrationBuilder.CreateIndex(
                name: "ix_project_events_project_id",
                table: "project_events",
                column: "project_id");

            migrationBuilder.CreateIndex(
                name: "ix_projects_created_by_id",
                table: "projects",
                column: "created_by_id");

            migrationBuilder.CreateIndex(
                name: "ix_projects_laboratory_id",
                table: "projects",
                column: "laboratory_id");

            migrationBuilder.CreateIndex(
                name: "ix_projects_lead_analyst_id",
                table: "projects",
                column: "lead_analyst_id");

            migrationBuilder.CreateIndex(
                name: "ix_projects_project_code_customer_code_project_code_suffix",
                table: "projects",
                columns: new[] { "project_code_customer_code", "project_code_suffix" });

            migrationBuilder.CreateIndex(
                name: "ix_projects_project_status",
                table: "projects",
                column: "project_status");

            migrationBuilder.CreateIndex(
                name: "ix_sample_groups_laboratory_id",
                table: "sample_groups",
                column: "laboratory_id");

            migrationBuilder.CreateIndex(
                name: "ix_sample_groups_project_id",
                table: "sample_groups",
                column: "project_id");

            migrationBuilder.CreateIndex(
                name: "ix_users_email_address",
                table: "users",
                column: "email_address",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "invitation_emails");

            migrationBuilder.DropTable(
                name: "laboratory_instrument_usage");

            migrationBuilder.DropTable(
                name: "memberships");

            migrationBuilder.DropTable(
                name: "project_application_tags");

            migrationBuilder.DropTable(
                name: "project_contacts");

            migrationBuilder.DropTable(
                name: "sample_groups");

            migrationBuilder.DropTable(
                name: "invitations");

            migrationBuilder.DropTable(
                name: "laboratory_instruments");

            migrationBuilder.DropTable(
                name: "project_events");

            migrationBuilder.DropTable(
                name: "project_applications");

            migrationBuilder.DropTable(
                name: "projects");

            migrationBuilder.DropTable(
                name: "laboratories");

            migrationBuilder.DropTable(
                name: "users");
        }
    }
}
