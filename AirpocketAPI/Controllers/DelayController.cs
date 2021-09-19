using AirpocketAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

using System.Data.Entity;
using System.Data.Entity.Infrastructure;


using System.Web.Http.Description;

using System.Data.Entity.Validation;

using System.Web.Http.ModelBinding;
using System.Web.Http.Cors;
using System.Text;
using System.Configuration;
using Newtonsoft.Json;

namespace AirpocketAPI.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class DelayController : ApiController
    {

        [Route("api/delays/report/airports")]
        
        // [Authorize]
        public async Task<IHttpActionResult> GetDelaysAirportReport( DateTime df, DateTime dt )
        {
            df = df.Date;
            dt = dt.Date;
            //var result = await unitOfWork.FlightRepository.GetDelaysAirportReport(/*dt, df*/);
            // return new CustomActionResult(HttpStatusCode.OK, result);
            var context = new AirpocketAPI.Models.FLYEntities();
            var query = from x in  context.DlyGrpFlights
                             where x.STDDay>=df && x.STDDay<=dt
                        group x by x.FromAirportIATA into grp
                        select new AirportDelayReport()
                        {
                            Airport = grp.Key,
                            Delay = grp.Sum(q => q.Delay),

                        };
            var airports = await query.ToListAsync();

            var query30 = from x in  context.DlyGrpFlights
                          where x.Delay > 30 && x.STDDay >= df && x.STDDay <= dt
                          group x by x.FromAirportIATA into grp
                          select new AirportDelayReport()
                          {
                              Airport = grp.Key,
                              Delay = grp.Sum(q => q.Delay),

                          };
            var airports30 = await query30.ToListAsync();

            var apts = airports.Select(q => q.Airport).ToList();
            var cycles = await (from x in  context.ViewLegTimes
                                where (x.FlightStatusID == 3 || x.FlightStatusID == 15 || x.FlightStatusID == 7) && apts.Contains(x.FromAirportIATA)
                                 && x.STDDay >= df && x.STDDay <= dt
                                group x by x.FromAirportIATA into grp
                                select new AirportDelayReport()
                                {
                                    Airport = grp.Key,
                                    Cycle = grp.Count()
                                }).ToListAsync();

            var total = airports.Sum(q => q.Delay);
            var total30 = airports30.Sum(q => q.Delay);

            foreach (var airport in airports)
            {
                var d30 = airports30.FirstOrDefault(q => q.Airport == airport.Airport);
                airport.Delay30 = d30 != null ? d30.Delay : 0;
                var cycle = cycles.FirstOrDefault(q => q.Airport == airport.Airport);
                airport.Cycle = cycle != null ? cycle.Cycle : 0;

                //5-7
                airport.DC = airport.Cycle == 0 ? 0 : Math.Round((double)((airport.Delay * 1.0) / airport.Cycle), 2, MidpointRounding.AwayFromZero);
                airport.DC30 = airport.Cycle == 0 ? 0 : Math.Round((double)((airport.Delay30 * 1.0) / airport.Cycle), 2, MidpointRounding.AwayFromZero);

                airport.Ratio = (airport.Delay * 1.0) / total;
                airport.Ratio30 = (airport.Delay30 * 1.0) / total30;
            }


            var result = airports.OrderByDescending(q => q.Delay30).ToList();
            return Ok(result);





        }


    }

    public class AirportDelayReport
    {
        public string Airport { get; set; }
        public int? Delay { get; set; }
        public int? Delay30 { get; set; }
        public int? Cycle { get; set; }
        public double? DC30 { get; set; }
        public double? DC { get; set; }
        public double? Ratio { get; set; }
        public double? Ratio30 { get; set; }

    }


}
