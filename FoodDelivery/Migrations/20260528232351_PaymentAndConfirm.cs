using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FoodDelivery.Migrations
{
    /// <inheritdoc />
    public partial class PaymentAndConfirm : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CancelamentoMotivo",
                table: "Pedidos",
                type: "TEXT",
                maxLength: 250,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DataPagamentoUtc",
                table: "Pedidos",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MetodoPagamento",
                table: "Pedidos",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "StatusPagamento",
                table: "Pedidos",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CancelamentoMotivo",
                table: "Pedidos");

            migrationBuilder.DropColumn(
                name: "DataPagamentoUtc",
                table: "Pedidos");

            migrationBuilder.DropColumn(
                name: "MetodoPagamento",
                table: "Pedidos");

            migrationBuilder.DropColumn(
                name: "StatusPagamento",
                table: "Pedidos");
        }
    }
}
