using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace TaskManager.Infraestructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "tdm");

            migrationBuilder.CreateTable(
                name: "TaskStatus",
                schema: "tdm",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Active = table.Column<bool>(type: "bit", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaskStatus", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TaskItem",
                schema: "tdm",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    DueDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    StatusId = table.Column<int>(type: "int", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Active = table.Column<bool>(type: "bit", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaskItem", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TaskItem_TaskStatus_StatusId",
                        column: x => x.StatusId,
                        principalSchema: "tdm",
                        principalTable: "TaskStatus",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                schema: "tdm",
                table: "TaskStatus",
                columns: new[] { "Id", "Active", "CreatedAt", "CreatedBy", "DeletedAt", "DeletedBy", "IsDeleted", "Name", "UpdatedAt", "UpdatedBy" },
                values: new object[,]
                {
                    { 1, true, new DateTime(2025, 8, 31, 19, 0, 0, 0, DateTimeKind.Unspecified), new Guid("123e4567-e89b-12d3-a456-426655440000"), null, new Guid("00000000-0000-0000-0000-000000000000"), false, "Pendiente", null, new Guid("00000000-0000-0000-0000-000000000000") },
                    { 2, true, new DateTime(2025, 8, 31, 19, 0, 0, 0, DateTimeKind.Unspecified), new Guid("123e4567-e89b-12d3-a456-426655440000"), null, new Guid("00000000-0000-0000-0000-000000000000"), false, "En Progreso", null, new Guid("00000000-0000-0000-0000-000000000000") },
                    { 3, true, new DateTime(2025, 8, 31, 19, 0, 0, 0, DateTimeKind.Unspecified), new Guid("123e4567-e89b-12d3-a456-426655440000"), null, new Guid("00000000-0000-0000-0000-000000000000"), false, "Completada", null, new Guid("00000000-0000-0000-0000-000000000000") }
                });

            migrationBuilder.InsertData(
                schema: "tdm",
                table: "TaskItem",
                columns: new[] { "Id", "Active", "CreatedAt", "CreatedBy", "DeletedAt", "DeletedBy", "Description", "DueDate", "IsDeleted", "StatusId", "Title", "UpdatedAt", "UpdatedBy" },
                values: new object[,]
                {
                    { 1, true, new DateTime(2025, 8, 31, 19, 0, 0, 0, DateTimeKind.Unspecified), new Guid("123e4567-e89b-12d3-a456-426655440000"), null, new Guid("00000000-0000-0000-0000-000000000000"), "Levantar proyecto con Clean Architecture", new DateTime(2025, 9, 7, 19, 0, 0, 0, DateTimeKind.Unspecified), false, 1, "Configurar arquitectura base", null, new Guid("00000000-0000-0000-0000-000000000000") },
                    { 2, true, new DateTime(2025, 8, 31, 19, 0, 0, 0, DateTimeKind.Unspecified), new Guid("123e4567-e89b-12d3-a456-426655440000"), null, new Guid("00000000-0000-0000-0000-000000000000"), "Configurar SaveChanges con ContextDefaultProvider", new DateTime(2025, 9, 10, 19, 0, 0, 0, DateTimeKind.Unspecified), false, 2, "Implementar auditoría", null, new Guid("00000000-0000-0000-0000-000000000000") },
                    { 3, true, new DateTime(2025, 8, 31, 19, 0, 0, 0, DateTimeKind.Unspecified), new Guid("123e4567-e89b-12d3-a456-426655440000"), null, new Guid("00000000-0000-0000-0000-000000000000"), "Pantalla para listar y gestionar tareas", new DateTime(2025, 9, 15, 19, 0, 0, 0, DateTimeKind.Unspecified), false, 3, "Crear UI en Blazor", null, new Guid("00000000-0000-0000-0000-000000000000") }
                });

            migrationBuilder.CreateIndex(
                name: "IX_TaskItem_StatusId",
                schema: "tdm",
                table: "TaskItem",
                column: "StatusId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TaskItem",
                schema: "tdm");

            migrationBuilder.DropTable(
                name: "TaskStatus",
                schema: "tdm");
        }
    }
}
