using CovidStats.Entities;
using System.Net;
using System.Reactive.Linq;
using System.Security.Cryptography;
using System.Text;

namespace CovidStats.Helpers
{
    public class CovidSledilnikCsv
    {
        private List<DailyCases> _dailyCases;
        private string _cachedHash;
        private string? _url;

        public CovidSledilnikCsv(string? url)
        {
            _cachedHash = "";
            _url = url;
            _dailyCases = new List<DailyCases>();

            ScheduledUpdate();
        }

        public List<DailyCases> DailyCases
        {
            get => _dailyCases;
        }


        private void UpdateStats()
        {
            var csvString = GetCSV();

            if (IsUpdated(csvString))
               _dailyCases = ReadCsv(csvString);
        }

        private void ScheduledUpdate()
        {
            Observable.Interval(TimeSpan.FromSeconds(20)).StartWith(0).Subscribe(p =>
            {
                UpdateStats();
            });
        }

        /// <summary>
        /// Reads csv column by column and generate list of DailyCases.
        /// </summary>
        /// <param name="csvString"></param>
        /// <returns></returns>
        private static List<DailyCases> ReadCsv(string csvString)
        {
            try
            {
                var result = new List<DailyCases>();

                StringReader strReader = new StringReader(csvString);

                // read headers
                var line = strReader.ReadLine();
                var headers = ReadLine(line);

                while (true)
                {
                    // read new line
                    line = strReader.ReadLine();
                    if (line == null)
                        break;

                    var row = ReadLine(line);

                    // create new daily record
                    var dailyCases = new DailyCases();

                    // read and parse the date
                    dailyCases.Date = DateTime.Parse(row[0]);
                    dailyCases.RegionCases = new List<RegionCases>();

                    for (int i = 1; i < row.Length; i++)
                    {
                        // create new RegionCases for each region
                        var regionCases = new RegionCases();

                        // get the region from headers
                        var regionString = headers[i].Split(".")[1];
                        var regionExists = Enum.TryParse(regionString.ToUpper(), out Region region);

                        // region not supported
                        if (!regionExists)
                            continue;

                        regionCases.Region = region;
                        regionCases.ActiveCases = ParseInt(row[i++]);
                        regionCases.ConfirmedCases = ParseInt(row[i++]);
                        regionCases.Deceased = ParseInt(row[i++]);
                        regionCases.Vaccinated1st = ParseInt(row[i++]);
                        regionCases.Vaccinated2nd = ParseInt(row[i++]);
                        regionCases.Vaccinated3nd = ParseInt(row[i]);

                        // add region to the daily cases
                        dailyCases.RegionCases.Add(regionCases);
                    }

                    // add daily cases to the list
                    result.Add(dailyCases);
                }

                return result;
            }
            catch
            {
                throw new Exception("Error reaading csv!");
            }
        }

        private static int ParseInt(string value)
        {
            var result = 0;

            if (!string.IsNullOrEmpty(value))
                result = int.Parse(value);

            return result;
        }

        private static string[]? ReadLine(string? line)
        {
            if (line != null)
                return line.Split(",");
            else
                return null;
        }

        /// <summary>
        /// Creates a HTTP request to retrieve a raw csv data.
        /// </summary>
        /// <returns></returns>
        private string GetCSV()
        {
            try
            {
                HttpWebRequest? req = HttpWebRequest.Create(_url) as HttpWebRequest;
                HttpWebResponse? resp = req.GetResponse() as HttpWebResponse;

                StreamReader sr = new StreamReader(resp.GetResponseStream());
                string results = sr.ReadToEnd();
                sr.Close();

                return results;
            }
            catch
            {
                throw new Exception("Error occured while trying to retrieve CSV!");
            }
        }

        /// <summary>
        /// Generates a MD5 hash from given string and compares it to the cashed hash.
        /// If it is different then replace the cashed hash with the new one.
        /// </summary>
        /// <param name="csv">Raw csv input data</param>
        /// <returns>True if the hash is diffrent, false otherwise.</returns>
        private bool IsUpdated(string csv)
        {
            using var md5 = MD5.Create();
            
            using var stream = new MemoryStream(Encoding.UTF8.GetBytes(csv ?? ""));
            
            var bytes = md5.ComputeHash(stream);
            
            var hash = BitConverter.ToString(bytes).Replace("-", "").ToLowerInvariant();

            if (!_cachedHash.Equals(hash))
            {
                _cachedHash = hash;
                return true;
            }

            return false;
        }
    }
}
