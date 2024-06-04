using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CatalogAPI.Migrations;

/// <inheritdoc />
public partial class populateProducts : Migration
{
  /// <inheritdoc />
  protected override void Up(MigrationBuilder migrationBuilder)
  {
    migrationBuilder.Sql("Insert into Products(Name, Description, Price, ImageUrl, Stock, RegisterData, CategoryID) Values('Coca Cola Diet', 'Refrigerante de cola 350ml', '5.45', 'cocacola.jpg', 50, getdate(), 1)");
  }

  /// <inheritdoc />
  protected override void Down(MigrationBuilder migrationBuilder)
  {
    migrationBuilder.Sql("Delete from Products");
  }
}
