using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TMAProject.Migrations
{
    /// <inheritdoc />
    public partial class _ : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductVariants_ProductColors_ProductColorId",
                table: "ProductVariants");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductVariants_ProductColors_ProductColorId",
                table: "ProductVariants",
                column: "ProductColorId",
                principalTable: "ProductColors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductVariants_ProductColors_ProductColorId",
                table: "ProductVariants");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductVariants_ProductColors_ProductColorId",
                table: "ProductVariants",
                column: "ProductColorId",
                principalTable: "ProductColors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
