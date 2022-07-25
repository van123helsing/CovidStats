namespace CovidStats.Entities
{
    public class RegionCases
    {
        public Region Region { get; set; }
        public int ActiveCases { get; set; }
        public int ConfirmedCases { get; set; }
        public int Vaccinated1st { get; set; }
        public int Vaccinated2nd { get; set; }
        public int Vaccinated3nd { get; set; }
        public int Deceased { get; set; }
    }
}
