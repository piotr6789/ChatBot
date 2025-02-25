using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ChatBot.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ChangedCookieIdColumnToClientSessionIdInSession : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CookieId",
                table: "Sessions");

            migrationBuilder.AddColumn<Guid>(
                name: "ClientSessionId",
                table: "Sessions",
                type: "uniqueidentifier",
                maxLength: 100,
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ClientSessionId",
                table: "Sessions");

            migrationBuilder.AddColumn<string>(
                name: "CookieId",
                table: "Sessions",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");
        }
    }
}
