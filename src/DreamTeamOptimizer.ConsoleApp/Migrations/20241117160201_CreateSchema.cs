using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace DreamTeamOptimizer.ConsoleApp.Migrations
{
    /// <inheritdoc />
    public partial class CreateSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "employees",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "text", nullable: false),
                    grade = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_employees", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "hackathons",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    status = table.Column<string>(type: "text", nullable: false),
                    result = table.Column<double>(type: "double precision", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_hackathons", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "hackathons_employees",
                columns: table => new
                {
                    employee_id = table.Column<int>(type: "integer", nullable: false),
                    hackathon_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_hackathons_employees", x => new { x.employee_id, x.hackathon_id });
                    table.ForeignKey(
                        name: "FK_hackathons_employees_employees_employee_id",
                        column: x => x.employee_id,
                        principalTable: "employees",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_hackathons_employees_hackathons_hackathon_id",
                        column: x => x.hackathon_id,
                        principalTable: "hackathons",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "preferences",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    employee_id = table.Column<int>(type: "integer", nullable: false),
                    desired_employee_id = table.Column<int>(type: "integer", nullable: false),
                    hackathon_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_preferences", x => x.id);
                    table.ForeignKey(
                        name: "FK_preferences_employees_desired_employee_id",
                        column: x => x.desired_employee_id,
                        principalTable: "employees",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_preferences_employees_employee_id",
                        column: x => x.employee_id,
                        principalTable: "employees",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_preferences_hackathons_hackathon_id",
                        column: x => x.hackathon_id,
                        principalTable: "hackathons",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "satisfactions",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    employee_id = table.Column<int>(type: "integer", nullable: false),
                    hackathon_id = table.Column<int>(type: "integer", nullable: false),
                    rank = table.Column<double>(type: "double precision", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_satisfactions", x => x.id);
                    table.ForeignKey(
                        name: "FK_satisfactions_employees_employee_id",
                        column: x => x.employee_id,
                        principalTable: "employees",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_satisfactions_hackathons_hackathon_id",
                        column: x => x.hackathon_id,
                        principalTable: "hackathons",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "teams",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    junior_id = table.Column<int>(type: "integer", nullable: false),
                    team_lead_id = table.Column<int>(type: "integer", nullable: false),
                    hackathon_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_teams", x => x.id);
                    table.ForeignKey(
                        name: "FK_teams_employees_junior_id",
                        column: x => x.junior_id,
                        principalTable: "employees",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_teams_employees_team_lead_id",
                        column: x => x.team_lead_id,
                        principalTable: "employees",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_teams_hackathons_hackathon_id",
                        column: x => x.hackathon_id,
                        principalTable: "hackathons",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_hackathons_employees_hackathon_id",
                table: "hackathons_employees",
                column: "hackathon_id");

            migrationBuilder.CreateIndex(
                name: "IX_preferences_desired_employee_id",
                table: "preferences",
                column: "desired_employee_id");

            migrationBuilder.CreateIndex(
                name: "IX_preferences_employee_id_desired_employee_id_hackathon_id",
                table: "preferences",
                columns: new[] { "employee_id", "desired_employee_id", "hackathon_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_preferences_hackathon_id",
                table: "preferences",
                column: "hackathon_id");

            migrationBuilder.CreateIndex(
                name: "IX_satisfactions_employee_id_hackathon_id",
                table: "satisfactions",
                columns: new[] { "employee_id", "hackathon_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_satisfactions_hackathon_id",
                table: "satisfactions",
                column: "hackathon_id");

            migrationBuilder.CreateIndex(
                name: "IX_teams_hackathon_id",
                table: "teams",
                column: "hackathon_id");

            migrationBuilder.CreateIndex(
                name: "IX_teams_junior_id_team_lead_id_hackathon_id",
                table: "teams",
                columns: new[] { "junior_id", "team_lead_id", "hackathon_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_teams_team_lead_id",
                table: "teams",
                column: "team_lead_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "hackathons_employees");

            migrationBuilder.DropTable(
                name: "preferences");

            migrationBuilder.DropTable(
                name: "satisfactions");

            migrationBuilder.DropTable(
                name: "teams");

            migrationBuilder.DropTable(
                name: "employees");

            migrationBuilder.DropTable(
                name: "hackathons");
        }
    }
}
