
using ExoPlanetHunter.Service.Enum;
namespace ExoPlanetHunter.Service.Dto
{
    public class ExoPlanetsDto
    {
      public int Id { get; set; }
      public string  Name  { get; set; }
      public ImgDto Img { get; set; }
      public decimal? Period { get; set; }
      public decimal? Hzd { get; set; }
      public decimal?  Hzc { get; set; }
      public decimal?  Hza { get; set; }
      public decimal?  Hzi { get; set; }
      public string  Type { get; set; }
      public int Comp { get; set; }
      public int Atmosphere { get; set; }
        public int HabType { get; set; }
        public decimal?  MeanDistance { get; set; }
      public decimal?   Distance{ get; set; }
      public decimal?  Esi { get; set; }
      public decimal?  Sph { get; set; }
      public decimal?  DiscYear { get; set; }
      public int DiscMethod { get; set; }
      public decimal   Radius { get; set; }  
      public decimal? Eccentricity { get; set; }
     public decimal RadiusEu { get; set; }
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
      public int  MassType { get; set; }
      public decimal?  TempMax { get; set; }
      public int  TempZone{ get; set; }
      public bool?  Hab{ get; set; }
      public bool?  Moons{ get; set; }
      public string Message { get; set; }
    }
}
