﻿using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace ABCStoreAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddExchangeRates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ExchangeRate",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TimeLastUpdateUnix = table.Column<long>(type: "bigint", nullable: false),
                    TimeNextUpdateUnix = table.Column<long>(type: "bigint", nullable: false),
                    Rate = table.Column<decimal>(type: "numeric", nullable: false),
                    SupportedCurrencyId = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: false),
                    UpdatedBy = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExchangeRate", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ExchangeRate_SupportedCurrency_SupportedCurrencyId",
                        column: x => x.SupportedCurrencyId,
                        principalTable: "SupportedCurrency",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ExchangeRate_SupportedCurrencyId",
                table: "ExchangeRate",
                column: "SupportedCurrencyId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ExchangeRate");
        }
    }
}
