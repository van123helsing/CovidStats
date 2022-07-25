using CovidStats.Entities;
using CovidStats.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace CovidStats.Controllers
{
    [ApiController]
    [Route("/api/[controller]")]
    public class RegionController : ControllerBase
    {

        private string? _url;
        private CovidSledilnikCsv _covidSledilnik;

        public RegionController(IOptions<AppSettings> appSettings)
        {
            _url = appSettings.Value.RegionCasesUrl;
            _covidSledilnik = new CovidSledilnikCsv(_url);
        }


        [Authorize]
        [HttpGet("cases")]
        public IActionResult GetCases(Region? region, DateTime fromDate, DateTime? toDate)
        {
            // if toDate is NULL then use Max value
            toDate ??= DateTime.MaxValue;

            var dailyCases = _covidSledilnik.DailyCases;

            var results = from regionCases in dailyCases
                          where regionCases.Date >= fromDate
                                && regionCases.Date <= toDate
                                && regionCases.RegionCases != null
                          from d in regionCases.RegionCases
                          where d.Region == region || region == null
                          select new 
                          { 
                              regionCases.Date, 
                              Region = Convert.ToString(d.Region), 
                              d.ActiveCases, 
                              d.Vaccinated1st, 
                              d.Vaccinated2nd, 
                              d.Deceased 
                          };

            return Ok(results);
        }

        [Authorize]
        [HttpGet("lastweek")]
        public IActionResult GetLastWeek()
        {
            var dailyCases = _covidSledilnik.DailyCases;

            var sevenDaysAgo = DateTime.Today - TimeSpan.FromDays(7);

            var regions = from regionCases in dailyCases
                          where regionCases.Date >= sevenDaysAgo
                                && regionCases.RegionCases != null
                          orderby regionCases.Date
                          from region in regionCases.RegionCases
                          group region by new
                          {
                              region.Region,
                          }
                          into entryGroup
                          select new
                          {
                              Region = entryGroup.Key.Region.ToString(),
                              DailyAverage = (entryGroup.Last().ConfirmedCases - entryGroup.First().ConfirmedCases) / entryGroup.Count()
                          };


            return Ok(regions.ToList().OrderByDescending(r => r.DailyAverage));
        }
    }
}