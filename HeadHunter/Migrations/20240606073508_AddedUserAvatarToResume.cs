﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HeadHunter.Migrations
{
    /// <inheritdoc />
    public partial class AddedUserAvatarToResume : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserAvatar",
                table: "Resumes",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserAvatar",
                table: "Resumes");
        }
    }
}
