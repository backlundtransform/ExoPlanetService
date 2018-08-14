namespace ExoPlanetHunter.Service.Dto
{
    public class ExoSystemPlanetsDto
    {
        public string Name { get; set; }

        public ImgDto Img { get; set; }

        public decimal Radius { get; set; }

        public decimal? StarDistance { get; set; }
    }
}