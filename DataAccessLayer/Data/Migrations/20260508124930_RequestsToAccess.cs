using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayer.Data.Migrations
{
    /// <inheritdoc />
    public partial class RequestsToAccess : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RoleChangeRequests",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    RequestedRoleId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ProcessedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ProcessedById = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    RejectionReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    AdminNotes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoleChangeRequests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RoleChangeRequests_AspNetRoles_RequestedRoleId",
                        column: x => x.RequestedRoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RoleChangeRequests_AspNetUsers_ProcessedById",
                        column: x => x.ProcessedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_RoleChangeRequests_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RoleChangeRequests_CreatedOn",
                table: "RoleChangeRequests",
                column: "CreatedOn");

            migrationBuilder.CreateIndex(
                name: "IX_RoleChangeRequests_ProcessedById",
                table: "RoleChangeRequests",
                column: "ProcessedById");

            migrationBuilder.CreateIndex(
                name: "IX_RoleChangeRequests_RequestedRoleId",
                table: "RoleChangeRequests",
                column: "RequestedRoleId");

            migrationBuilder.CreateIndex(
                name: "IX_RoleChangeRequests_Status",
                table: "RoleChangeRequests",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_RoleChangeRequests_UserId",
                table: "RoleChangeRequests",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RoleChangeRequests");
        }
    }
}
