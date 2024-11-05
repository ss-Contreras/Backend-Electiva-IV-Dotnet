﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SonrisasBackendv01.Migrations
{
    /// <inheritdoc />
    public partial class motivoCita : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Motivo",
                table: "Citas",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Motivo",
                table: "Citas");
        }
    }
}
