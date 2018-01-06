using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Exoplanetservice.Migrations
{
    public partial class first : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Constellations",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Constellations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Stars",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Age = table.Column<decimal>(nullable: true),
                    ApparMag = table.Column<decimal>(nullable: true),
                    ConstellationId = table.Column<int>(nullable: true),
                    Dec = table.Column<decimal>(nullable: true),
                    Distance = table.Column<decimal>(nullable: true),
                    FeH = table.Column<decimal>(nullable: true),
                    HabCat = table.Column<bool>(nullable: true),
                    HabZoneMax = table.Column<decimal>(nullable: true),
                    HabZoneMin = table.Column<decimal>(nullable: true),
                    Luminosity = table.Column<decimal>(nullable: true),
                    MagfromPlanet = table.Column<decimal>(nullable: true),
                    Mass = table.Column<decimal>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    NameHD = table.Column<string>(nullable: true),
                    NameHIP = table.Column<string>(nullable: true),
                    NoPlanets = table.Column<int>(nullable: true),
                    NoPlanetsHZ = table.Column<int>(nullable: true),
                    Ra = table.Column<decimal>(nullable: true),
                    Radius = table.Column<decimal>(nullable: true),
                    SizefromPlanet = table.Column<decimal>(nullable: true),
                    Teff = table.Column<decimal>(nullable: true),
                    Type = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Stars", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Stars_Constellations_ConstellationId",
                        column: x => x.ConstellationId,
                        principalTable: "Constellations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Planets",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ApparSize = table.Column<decimal>(nullable: true),
                    AtmosphereClass = table.Column<string>(nullable: true),
                    CompositionClass = table.Column<string>(nullable: true),
                    Confirmed = table.Column<bool>(nullable: true),
                    Density = table.Column<decimal>(nullable: true),
                    Disc_Method = table.Column<string>(nullable: true),
                    Disc_Year = table.Column<int>(nullable: true),
                    Eccentricity = table.Column<decimal>(nullable: true),
                    EscVel = table.Column<decimal>(nullable: true),
                    Esi = table.Column<decimal>(nullable: true),
                    Gravity = table.Column<decimal>(nullable: true),
                    HabMoon = table.Column<bool>(nullable: true),
                    Habitable = table.Column<bool>(nullable: true),
                    HabitableClass = table.Column<string>(nullable: true),
                    Hza = table.Column<decimal>(nullable: true),
                    Hzc = table.Column<decimal>(nullable: true),
                    Hzd = table.Column<decimal>(nullable: true),
                    Hzi = table.Column<decimal>(nullable: true),
                    Inclination = table.Column<decimal>(nullable: true),
                    IntEsi = table.Column<decimal>(nullable: true),
                    Mag = table.Column<decimal>(nullable: true),
                    Mass = table.Column<decimal>(nullable: true),
                    MassClass = table.Column<string>(nullable: true),
                    MaxMass = table.Column<decimal>(nullable: true),
                    MeanDistance = table.Column<decimal>(nullable: true),
                    MinMass = table.Column<decimal>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    NameKOI = table.Column<string>(nullable: true),
                    NameKepler = table.Column<string>(nullable: true),
                    Omega = table.Column<decimal>(nullable: true),
                    Period = table.Column<decimal>(nullable: true),
                    Radius = table.Column<decimal>(nullable: true),
                    SFluxMax = table.Column<decimal>(nullable: true),
                    SFluxMean = table.Column<decimal>(nullable: true),
                    SFluxMin = table.Column<decimal>(nullable: true),
                    SemMajorAxis = table.Column<decimal>(nullable: true),
                    Sph = table.Column<decimal>(nullable: true),
                    StarId = table.Column<int>(nullable: true),
                    SurfEsi = table.Column<decimal>(nullable: true),
                    SurfPress = table.Column<decimal>(nullable: true),
                    TeqMax = table.Column<decimal>(nullable: true),
                    TeqMean = table.Column<decimal>(nullable: true),
                    TeqMin = table.Column<decimal>(nullable: true),
                    TsMax = table.Column<decimal>(nullable: true),
                    TsMean = table.Column<decimal>(nullable: true),
                    TsMin = table.Column<decimal>(nullable: true),
                    ZoneClass = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Planets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Planets_Stars_StarId",
                        column: x => x.StarId,
                        principalTable: "Stars",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Planets_StarId",
                table: "Planets",
                column: "StarId");

            migrationBuilder.CreateIndex(
                name: "IX_Stars_ConstellationId",
                table: "Stars",
                column: "ConstellationId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Planets");

            migrationBuilder.DropTable(
                name: "Stars");

            migrationBuilder.DropTable(
                name: "Constellations");
        }
    }
}
