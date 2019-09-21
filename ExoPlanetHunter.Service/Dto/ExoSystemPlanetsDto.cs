namespace ExoPlanetHunter.Service.Dto
{
    public class ExoSystemPlanetsDto
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public ImgDto Img { get; set; }

        public decimal Radius { get; set; }

       public decimal? Eccentricity { get; set; }

        public decimal? StarDistance { get; set; }
    }
}