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

using System.Text;
using System.Configuration;
using Newtonsoft.Json;

namespace AirpocketAPI.Controllers
{
    public class FlightStatusController : ApiController
    {
        
        [Route("api/flight/status")]
        [AcceptVerbs("POST")]
        ///<summary>
        ///Get Status Of Flight
        ///</summary>
        ///<remarks>


        ///</remarks>
        public async Task<IHttpActionResult> PostFlightStatus(AuthInfo authInfo, string date, string no)
        {
            try
            {
                if (!(authInfo.userName == "pnl.airpocket" && authInfo.password == "Pnl1234@z"))
                    return BadRequest("Authentication Failed");

                no = no.PadLeft(4, '0');
                List<int> prts = new List<int>();
                try
                {
                    prts = date.Split('-').Select(q => Convert.ToInt32(q)).ToList();
                }
                catch (Exception ex)
                {
                    return BadRequest("Incorrect Date");
                }

                if (prts.Count != 3)
                    return BadRequest("Incorrect Date");
                if (prts[0] < 1300)
                    return BadRequest("Incorrect Date (Year)");
                //if (prts[1] < 1 || prts[1]>12)
                //    return BadRequest("Incorrect Date (Month)");
                //if (prts[2] < 1 || prts[1] > 31)
                //    return BadRequest("Incorrect Date (Day)");

                System.Globalization.PersianCalendar pc = new System.Globalization.PersianCalendar();
                var gd = (pc.ToDateTime(prts[0], prts[1], prts[2], 0, 0, 0, 0)).Date;
                var context = new AirpocketAPI.Models.FLYEntities();
                var flight = await context.ExpFlights.Where(q => q.DepartureDay == gd && q.FlightNo == no).FirstOrDefaultAsync();
                if (flight == null)
                    return NotFound();
                var delay = (((DateTime)flight.Departure) - ((DateTime)flight.STD)).TotalMinutes;
                if (delay < 0)
                    delay = 0;
                var result = new
                {
                    flightId = flight.Id,
                    flightNo = flight.FlightNo,
                    date = flight.DepartureDay,
                    departure = flight.DepartureLocal,
                    arrival = flight.ArrivalLocal,
                    departureUTC = flight.Departure,
                    arrivalUTC = flight.Arrival,
                    status = flight.FlightStatus,
                    statusId = flight.FlightStatusId,
                    origin = flight.Origin,
                    destination = flight.Destination,
                    aircraftType = flight.AircraftType,
                    register = flight.Register,
                    isDelayed = delay > 0,
                    delay

                };
                return Ok(result);
            }
            catch (Exception ex)
            {
                var msg = ex.Message;
                if (ex.InnerException != null)
                    msg += "    Inner Exception:" + ex.InnerException.Message;
                return BadRequest(msg);
            }







        }


    }



    
}
