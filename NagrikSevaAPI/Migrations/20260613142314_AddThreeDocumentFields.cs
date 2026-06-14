using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NagrikSevaAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddThreeDocumentFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "DocumentType",
                table: "ApplicationDetails",
                newName: "PhotoDocType");

            migrationBuilder.RenameColumn(
                name: "DocumentPath",
                table: "ApplicationDetails",
                newName: "PhotoDocPath");

            migrationBuilder.RenameColumn(
                name: "DocumentOriginalName",
                table: "ApplicationDetails",
                newName: "PhotoDocName");

            migrationBuilder.AddColumn<string>(
                name: "AadhaarDocName",
                table: "ApplicationDetails",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AadhaarDocPath",
                table: "ApplicationDetails",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AadhaarDocType",
                table: "ApplicationDetails",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AddressDocName",
                table: "ApplicationDetails",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AddressDocPath",
                table: "ApplicationDetails",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AddressDocType",
                table: "ApplicationDetails",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AadhaarDocName",
                table: "ApplicationDetails");

            migrationBuilder.DropColumn(
                name: "AadhaarDocPath",
                table: "ApplicationDetails");

            migrationBuilder.DropColumn(
                name: "AadhaarDocType",
                table: "ApplicationDetails");

            migrationBuilder.DropColumn(
                name: "AddressDocName",
                table: "ApplicationDetails");

            migrationBuilder.DropColumn(
                name: "AddressDocPath",
                table: "ApplicationDetails");

            migrationBuilder.DropColumn(
                name: "AddressDocType",
                table: "ApplicationDetails");

            migrationBuilder.RenameColumn(
                name: "PhotoDocType",
                table: "ApplicationDetails",
                newName: "DocumentType");

            migrationBuilder.RenameColumn(
                name: "PhotoDocPath",
                table: "ApplicationDetails",
                newName: "DocumentPath");

            migrationBuilder.RenameColumn(
                name: "PhotoDocName",
                table: "ApplicationDetails",
                newName: "DocumentOriginalName");
        }
    }
}
