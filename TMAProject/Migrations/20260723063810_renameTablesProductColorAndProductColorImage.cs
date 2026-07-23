using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TMAProject.Migrations
{
    /// <inheritdoc />
    public partial class renameTablesProductColorAndProductColorImage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductColor_Colors_ColorId",
                table: "ProductColor");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductColor_Products_ProductId",
                table: "ProductColor");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductColorImage_ProductColor_ProductColorId",
                table: "ProductColorImage");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductVariants_ProductColor_ProductColorId",
                table: "ProductVariants");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ProductColorImage",
                table: "ProductColorImage");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ProductColor",
                table: "ProductColor");

            migrationBuilder.RenameTable(
                name: "ProductColorImage",
                newName: "ProductColorImages");

            migrationBuilder.RenameTable(
                name: "ProductColor",
                newName: "ProductColors");

            migrationBuilder.RenameIndex(
                name: "IX_ProductColorImage_ProductColorId",
                table: "ProductColorImages",
                newName: "IX_ProductColorImages_ProductColorId");

            migrationBuilder.RenameIndex(
                name: "IX_ProductColor_ProductId_ColorId",
                table: "ProductColors",
                newName: "IX_ProductColors_ProductId_ColorId");

            migrationBuilder.RenameIndex(
                name: "IX_ProductColor_ColorId",
                table: "ProductColors",
                newName: "IX_ProductColors_ColorId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProductColorImages",
                table: "ProductColorImages",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProductColors",
                table: "ProductColors",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductColorImages_ProductColors_ProductColorId",
                table: "ProductColorImages",
                column: "ProductColorId",
                principalTable: "ProductColors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductColors_Colors_ColorId",
                table: "ProductColors",
                column: "ColorId",
                principalTable: "Colors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductColors_Products_ProductId",
                table: "ProductColors",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductVariants_ProductColors_ProductColorId",
                table: "ProductVariants",
                column: "ProductColorId",
                principalTable: "ProductColors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductColorImages_ProductColors_ProductColorId",
                table: "ProductColorImages");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductColors_Colors_ColorId",
                table: "ProductColors");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductColors_Products_ProductId",
                table: "ProductColors");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductVariants_ProductColors_ProductColorId",
                table: "ProductVariants");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ProductColors",
                table: "ProductColors");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ProductColorImages",
                table: "ProductColorImages");

            migrationBuilder.RenameTable(
                name: "ProductColors",
                newName: "ProductColor");

            migrationBuilder.RenameTable(
                name: "ProductColorImages",
                newName: "ProductColorImage");

            migrationBuilder.RenameIndex(
                name: "IX_ProductColors_ProductId_ColorId",
                table: "ProductColor",
                newName: "IX_ProductColor_ProductId_ColorId");

            migrationBuilder.RenameIndex(
                name: "IX_ProductColors_ColorId",
                table: "ProductColor",
                newName: "IX_ProductColor_ColorId");

            migrationBuilder.RenameIndex(
                name: "IX_ProductColorImages_ProductColorId",
                table: "ProductColorImage",
                newName: "IX_ProductColorImage_ProductColorId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProductColor",
                table: "ProductColor",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProductColorImage",
                table: "ProductColorImage",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductColor_Colors_ColorId",
                table: "ProductColor",
                column: "ColorId",
                principalTable: "Colors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductColor_Products_ProductId",
                table: "ProductColor",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductColorImage_ProductColor_ProductColorId",
                table: "ProductColorImage",
                column: "ProductColorId",
                principalTable: "ProductColor",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductVariants_ProductColor_ProductColorId",
                table: "ProductVariants",
                column: "ProductColorId",
                principalTable: "ProductColor",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
