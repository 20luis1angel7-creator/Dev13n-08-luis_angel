using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Okane.Storage.EntityFramework.Migrations
{
    /// <inheritdoc />
    public partial class AddMyMigrationHomework : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //Renombrar columna existente
            migrationBuilder.RenameColumn(
                name: "Category",
                table: "Expenses",
                newName: "CategoryName");

            //Crear tabla Categories
            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation(
                            "Npgsql:ValueGenerationStrategy",
                            NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.Id);
                });

            //Insertar categorías únicas desde Expenses
            migrationBuilder.Sql(@"
                INSERT INTO ""Categories"" (""Name"")
                SELECT DISTINCT ""CategoryName""
                FROM ""Expenses""
                WHERE ""CategoryName"" IS NOT NULL;
            ");

            //Agregar CategoryId como NULLABLE
            migrationBuilder.AddColumn<int>(
                name: "CategoryId",
                table: "Expenses",
                type: "integer",
                nullable: true);

            //Mapear CategoryId con Categories
            migrationBuilder.Sql(@"
                UPDATE ""Expenses"" e
                SET ""CategoryId"" = c.""Id""
                FROM ""Categories"" c
                WHERE e.""CategoryName"" = c.""Name"";
            ");

            //Hacer CategoryId NOT NULL
            migrationBuilder.AlterColumn<int>(
                name: "CategoryId",
                table: "Expenses",
                type: "integer",
                nullable: false);

            //Eliminar columna vieja
            migrationBuilder.DropColumn(
                name: "CategoryName",
                table: "Expenses");

            //Crear índice
            migrationBuilder.CreateIndex(
                name: "IX_Expenses_CategoryId",
                table: "Expenses",
                column: "CategoryId");

            //Crear Foreign Key
            migrationBuilder.AddForeignKey(
                name: "FK_Expenses_Categories_CategoryId",
                table: "Expenses",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            //Quitar Foreign Key
            migrationBuilder.DropForeignKey(
                name: "FK_Expenses_Categories_CategoryId",
                table: "Expenses");

            //Quitar índice
            migrationBuilder.DropIndex(
                name: "IX_Expenses_CategoryId",
                table: "Expenses");

            //Agregar CategoryName nuevamente
            migrationBuilder.AddColumn<string>(
                name: "CategoryName",
                table: "Expenses",
                type: "text",
                nullable: true);

            //Restaurar valores de Category
            migrationBuilder.Sql(@"
                UPDATE ""Expenses"" e
                SET ""CategoryName"" = c.""Name""
                FROM ""Categories"" c
                WHERE e.""CategoryId"" = c.""Id"";
            ");

            //Eliminar CategoryId
            migrationBuilder.DropColumn(
                name: "CategoryId",
                table: "Expenses");

            //Eliminar tabla Categories
            migrationBuilder.DropTable(
                name: "Categories");

            //Renombrar columna a su nombre original
            migrationBuilder.RenameColumn(
                name: "CategoryName",
                table: "Expenses",
                newName: "Category");
        }
    }
}
