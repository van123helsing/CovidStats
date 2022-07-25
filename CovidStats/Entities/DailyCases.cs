namespace CovidStats.Entities
{
    public class DailyCases
    {
        public DateTime Date { get; set; }

        public List<RegionCases>? RegionCases { get; set; }
    }
}
