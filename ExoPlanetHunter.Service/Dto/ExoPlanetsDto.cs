
using ExoPlanetHunter.Service.Enum;
namespace ExoPlanetHunter.Service.Dto
{
    public class ExoPlanetsDto
    {
      public string  Name  { get; set; }
      public ImgDto Img { get; set; }
      public decimal? Period { get; set; }
      public decimal? Hzd { get; set; }
      public decimal?  Hzc { get; set; }
      public decimal?  Hza { get; set; }
      public decimal?  Hzi { get; set; }
      public string  Type { get; set; }
      public CompEnum  Comp { get; set; }
      public AtmosEnum  Atmosphere { get; set; }
      public decimal?  MeanDistance { get; set; }
      public decimal?   Distance{ get; set; }
      public decimal?  Esi { get; set; }
      public decimal?  Sph { get; set; }
      public decimal?  DiscYear { get; set; }
      public DiscEnum  DiscMethod { get; set; }
      public decimal   Radius { get; set; }
      public CoordinateDto Coordinate{ get; set; }
      public decimal?  StarDistance{ get; set; }
      public ExoStarDto Star { get; set; }
      public decimal?  Temp{ get; set; }
      public decimal?  TempMin{ get; set; }
      public decimal?  Mass{ get; set; }
      public decimal?  Density{ get; set; }
      public decimal?  Gravity{ get; set; }
      public decimal?  SurfacePressure { get; set; }
      public decimal?  EscapeVelocity { get; set; }
      public MassEnum  MassType { get; set; }
      public decimal?  TempMax { get; set; }
      public decimal?  TempZone{ get; set; }
      public bool?  Hab{ get; set; }
      public bool?  Moons{ get; set; }
      public string Message { get; set; }
    }
}
