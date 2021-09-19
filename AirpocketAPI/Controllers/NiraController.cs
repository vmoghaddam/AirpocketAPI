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
using System.Web.Http.Cors;
using System.IO;
using System.Xml;
using System.Transactions;

namespace AirpocketAPI.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class NiraController : ApiController
    {
        [Route("api/nira/conflicts/{dfrom}/{dto}")]
        [AcceptVerbs("GET")]
        public IHttpActionResult GetNiraConflicts(DateTime dfrom, DateTime dto)
        {
            List<NiraConflictResult> conflictResult = new List<NiraConflictResult>();
            var _dfrom = dfrom.Date.ToString("yyyy-MM-dd");
            var _dto = dto.Date.ToString("yyyy-MM-dd");
            dfrom = dfrom.Date;
            dto = dto.Date.AddHours(23).AddMinutes(59).AddSeconds(59);
            List<NRSCRSFlightData> niraFlights = new List<NRSCRSFlightData>();
            using (var context = new AirpocketAPI.Models.FLYEntities())
            {
                var flights = context.ViewLegTimes.Where(q => q.STDLocal >= dfrom && q.STDLocal <= dto && (q.FlightStatusID == 1 || q.FlightStatusID == 4)).ToList();
                var flightNos = flights.Select(q => q.FlightNumber).Distinct().ToList();
                

                foreach (var no in flightNos)
                {
                    string apiUrl = "http://iv.nirasoft.ir:882/NRSCWS.jsp?ModuleType=SP&ModuleName=CRSFlightData&DepartureDateFrom="
                        + _dfrom
                        + "&DepartureDateTo=" + _dto
                        + "&FlightNo=" + no
                        + "&OfficeUser=Thr003.airpocket&OfficePass=nira123";

                    WebClient client = new WebClient();
                    client.Headers["Content-type"] = "application/json";
                    client.Encoding = Encoding.UTF8;
                    string json = client.DownloadString(apiUrl);
                    json = json.Replace("Child SA-Book", "Child_SA_Book").Replace("Adult SA-Book", "Adult_SA_Book");
                    var obj = JsonConvert.DeserializeObject<NRSCWSResult>(json);
                    niraFlights = niraFlights.Concat(obj.NRSCRSFlightData).ToList();
                }
                foreach (var x in niraFlights)
                    x.Proccessed = false;
                flights = flights.OrderBy(q => q.STD).ToList();

                foreach (var aflt in flights)
                {
                    var niraflt = niraFlights.FirstOrDefault(q => q.FlightNo.PadLeft(4, '0') == aflt.FlightNumber && q.STDDay == ((DateTime)aflt.STDLocal).Date);
                    var conflict = new NiraConflictResult()
                    {
                        Date = ((DateTime)aflt.STDLocal).Date,
                        AirPocket = new _FLT()
                        {
                            Destination = aflt.ToAirportIATA,
                            Origin = aflt.FromAirportIATA,
                            FlightNo = aflt.FlightNumber,
                            Register = aflt.Register,
                            STA = (DateTime)aflt.STALocal,
                            STD = (DateTime)aflt.STDLocal,
                            StatusId = aflt.FlightStatusID,
                            Status = aflt.FlightStatusID == 1 ? "SCHEDULED" : "CNL",
                        },
                    };
                    if (niraflt != null  )
                    {
                        conflict.Nira = new _FLT()
                        {
                            Destination = niraflt.Destination,
                            FlightNo = niraflt.FlightNo,
                            Origin = niraflt.Origin,
                            Register = niraflt.Register,
                            STA = niraflt.STA,
                            STD = niraflt.STD,
                            StatusId = niraflt.FlightStatusId,
                            Status = niraflt.FlightStatusId == 1 ? "SCHEDULED" : "CNL",
                        };
                    }
                    conflictResult.Add(conflict);
                }
                // var niraFlights = obj.NRSCRSFlightData;


            }

            //var response = obj.d_envelope.d_body.response.result;
            //var responseJson = JsonConvert.DeserializeObject<List<IdeaSessionX>>(response);
            conflictResult = conflictResult.OrderBy(q => q.Date).ThenByDescending(q => q.IsConflicted).ThenBy(q => q.AirPocket.StatusId).ThenBy(q => q.AirPocket.Register)
                .ThenBy(q => q.AirPocket.STD).ToList();
            return Ok(conflictResult);
        }
    }

    public class _FLT
    {
        public string FlightNo { get; set; }
        public string Origin { get; set; }
        public string Destination { get; set; }
        public DateTime? STD { get; set; }
        public DateTime? STA { get; set; }
        public int? StatusId { get; set; }
        public string Status { get; set; }
        public string Register { get; set; }
        public string Route
        {
            get
            {
                return this.Origin + '-' + this.Destination;
            }
        }
    }
    public class NiraConflictResult
    {
        public DateTime Date { get; set; }
        public _FLT AirPocket { get; set; }
        public _FLT Nira { get; set; }

        public bool IsNiraFound { get { return this.Nira != null; } }
        public bool? IsRegister
        {
            get
            {
                if (!this.IsNiraFound)
                    return null;
                if (this.AirPocket.StatusId == 4)
                    return true;
                if (this.Nira.Register.ToLower().EndsWith(this.AirPocket.Register.ToLower()))
                    return true;
                else
                    return false;
            }
        }
        public bool? IsStatus
        {
            get
            {
                if (!this.IsNiraFound)
                    return null;
                if (this.Nira.StatusId ==this.AirPocket.StatusId)
                    return true;
                else
                    return false;
            }
        }
        public bool? IsSTD
        {
            get
            {
                if (!this.IsNiraFound)
                    return null;
                if (this.Nira.STD == this.AirPocket.STD)
                    return true;
                else
                    return false;
            }
        }
        public bool? IsSTA
        {
            get
            {
                if (!this.IsNiraFound)
                    return null;
                if (this.Nira.STA == this.AirPocket.STA)
                    return true;
                else
                    return false;
            }
        }

        public bool? IsRoute
        {
            get
            {
                if (!this.IsNiraFound)
                    return null;
                if (this.Nira.Route == this.AirPocket.Route)
                    return true;
                else
                    return false;
            }
        }

        public bool IsConflicted
        {
            get
            {
                if (!this.IsNiraFound)
                    return true;
                var result = IsRegister == true && IsSTA == true && IsSTD == true && IsStatus == true && IsRoute == true;
                return !result;
            }
        }

    }

    //Child SA-Book
    //Adult SA-Book
    public class NRSCWSResult
    {
        public List<NRSCRSFlightData> NRSCRSFlightData { get; set; }
    }

    public class NRSCRSFlightData
    {
        public string Origin { get; set; }
        public string Destination { get; set; }
        public int? TotalBook { get; set; }
        public string FlightNo { get; set; }
        public int? Child_SA_Book { get; set; }
        public string FlightStatus { get; set; }
        public string DepartureDateTime { get; set; }
        public string Register { get; set; }
        public string ArrivalDateTime { get; set; }
        public int? ChildBook { get; set; }
        public int? Adult_SA_Book { get; set; }
        public string AircraftTypeCode { get; set; }
        public int? AdultBook { get; set; }

        public bool? Proccessed { get; set; }
        public int FlightStatusId
        {
            get
            {
                switch (this.FlightStatus.ToLower())
                {
                    case "o":
                        return 1;
                    case "x":
                        return 4;
                    default:
                        return -1;
                }
            }
        }

        private DateTime? std = null;
        private DateTime? sta = null;
        public DateTime? STD
        {
            get
            {
                if (std == null)
                {
                    //2021-07-16 23:00:00
                    var prts = this.DepartureDateTime.Split(' ');
                    var dtprts = prts[0].Split('-').Select(q => Convert.ToInt32(q)).ToList();
                    var tiprts = prts[1].Split(':').Select(q => Convert.ToInt32(q)).ToList();
                    std = new DateTime(dtprts[0], dtprts[1], dtprts[2], tiprts[0], tiprts[1], tiprts[2]);
                }
                return std;
            }
        }
        public DateTime? STDDay
        {
            get
            {
                if (this.STD == null)
                    return null;
                return ((DateTime)this.STD).Date;
            }
        }
        public DateTime? STA
        {
            get
            {
                if (sta == null)
                {
                    //2021-07-16 23:00:00
                    var prts = this.ArrivalDateTime.Split(' ');
                    var dtprts = prts[0].Split('-').Select(q => Convert.ToInt32(q)).ToList();
                    var tiprts = prts[1].Split(':').Select(q => Convert.ToInt32(q)).ToList();
                    sta = new DateTime(dtprts[0], dtprts[1], dtprts[2], tiprts[0], tiprts[1], tiprts[2]);
                }
                return sta;
            }
        }


    }

}
