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
using System.Web;
using System.Text.RegularExpressions;

namespace AirpocketAPI.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class FlightController : ApiController
    {

        [Route("api/flights")]
        [AcceptVerbs("POST")]
        ///<summary>
        ///Get List of Flights
        ///</summary>
        ///<remarks>
        ///Flight Statuses
        ///1: Scheduled
        ///4: Canceled

        ///</remarks>


        public IQueryable<ExpFlight> GetFlights(AuthInfo authInfo, DateTime? dfrom = null, DateTime? dto = null, int? status = null, string register = "", string actype = "", string origin = "", string destination = "", string flightNo = "")
        {
            if (!(authInfo.userName == "fs.airpocket" && authInfo.password == "Ap1234@z"))
                return null;

            var context = new AirpocketAPI.Models.FLYEntities();
            var query = from x in context.ExpFlights

                        select x;
            if (dfrom != null)
            {
                var df = ((DateTime)dfrom).Date;
                query = query.Where(q => q.STDLocal >= df);
            }
            if (dto != null)
            {
                var dt = ((DateTime)dto).Date;
                query = query.Where(q => q.STDLocal <= dt);
            }
            if (status != null)
                query = query.Where(q => q.FlightStatusId == status);
            if (!string.IsNullOrEmpty(register))
                query = query.Where(q => q.Register == register);
            if (!string.IsNullOrEmpty(actype))
                query = query.Where(q => q.AircraftType == actype);
            if (!string.IsNullOrEmpty(origin))
                query = query.Where(q => q.Origin == origin);
            if (!string.IsNullOrEmpty(destination))
                query = query.Where(q => q.Destination == destination);

            if (!string.IsNullOrEmpty(flightNo))
                query = query.Where(q => q.FlightNo == flightNo);

            return query.OrderBy(q => q.STDLocal);


        }
        public class AptRpt
        {
            public int Row { get; set; }
            public int? OutId { get; set; }
            public string OutFlightNo { get; set; }
            public string OutRegister { get; set; }
            public string OutType { get; set; }
            public string OutRoute { get; set; }
            public string OutFrom { get; set; }
            public string OutTo { get; set; }
            public string OutRouteICAO { get; set; }
            public string OutFromICAO { get; set; }
            public string OutToICAO { get; set; }

            public DateTime? OutSTDLocal { get; set; }
            public string OutTimeLocal { get; set; }

            public int? InId { get; set; }
            public string InFlightNo { get; set; }
            public string InRegister { get; set; }
            public string InType { get; set; }
            public string InRoute { get; set; }
            public string InFrom { get; set; }
            public string InTo { get; set; }
            public string InRouteICAO { get; set; }
            public string InFromICAO { get; set; }
            public string InToICAO { get; set; }
            public DateTime? InSTDLocal { get; set; }
            public string InTimeLocal { get; set; }


            public string Temp1 { get; set; }
            public string Temp2 { get; set; }
            public string Temp3 { get; set; }
            public string Temp4 { get; set; }
            public string Temp5 { get; set; }
            public string Airline { get; set; }
            public string BaseApt { get; set; }
            public string Date { get; set; }
            public string DatePersian { get; set; }
            public string Day { get; set; }



        }

        [Route("api/day/apts")]
        [AcceptVerbs("GET")]
        public IHttpActionResult GetDayApts(DateTime dt)
        {

            var _dt = dt.Date;
            var context = new AirpocketAPI.Models.FLYEntities();
            var query = from x in context.ExpFlights
                        where x.FlightStatusId != 4 && x.DepartureDay == _dt
                        select new { x.Origin, x.Destination };
            var _result = query.ToList();
            var _r1 = _result.Select(q => q.Origin).ToList();
            var _r2 = _result.Select(q => q.Destination).ToList();
            var result = _r1.Concat(_r2).Distinct().OrderBy(q => q).ToList();
            return Ok(result);
        }

        [Route("api/person/history/save")]
        [AcceptVerbs("POST")]
        public IHttpActionResult PostPersonHistorySave(PersonHistory his)
        {

            
            var context = new AirpocketAPI.Models.FLYEntities();
            his.DateCreate = DateTime.Now;
            context.PersonHistories.Add(his);
            var x=context.SaveChanges();
            return Ok(x);
        }

        [Route("api/person/telegram")]
        [AcceptVerbs("Post")]
        public IHttpActionResult PostTelegram(dynamic dto)
        {
            int eid = Convert.ToInt32(dto.eid);
            string tel = Convert.ToString(dto.tel);
            var context = new AirpocketAPI.Models.FLYEntities();
            var personId = context.ViewEmployeeLights.Where(q => q.Id == eid).Select(q => q.PersonId).FirstOrDefault();

            var person = context.People.Where(q => q.Id == personId).FirstOrDefault();

            if (person == null)
                return Ok(0);

            person.Telegram = tel;
            context.SaveChanges();
            return Ok(1);


        }

        [Route("api/flights/apt")]
        [AcceptVerbs("GET")]
        public IHttpActionResult GetFlightsByApt(DateTime dt, string apt, string airline, string user = "", string phone = "")
        {

            var _dt = dt.Date;
            var context = new AirpocketAPI.Models.FLYEntities();
            var query = from x in context.ExpFlights
                        where x.FlightStatusId != 4 && x.DepartureDay == _dt && (x.Origin == apt || x.Destination == apt)
                        select x;
            var _result = query.ToList();
            var outResult = _result.Where(q => q.Origin == apt).OrderBy(q => q.STDLocal).ToList();
            var inResult = _result.Where(q => q.Destination == apt).OrderBy(q => q.STDLocal).ToList();
            var cnt = outResult.Count;
            if (inResult.Count > cnt)
                cnt = inResult.Count;

            var result = new List<AptRpt>();
            for (var c = 0; c < cnt; c++)
            {
                var row = new AptRpt()
                {
                    Row = c + 1,
                    Airline = airline,
                    BaseApt = apt,
                    Date = _dt.ToString("yyyy/MM/dd"),
                    DatePersian = _result.First().PersianDate,
                    Day = _result.First().PersianDayName,
                    Temp1 = user,
                    Temp2 = phone
                };

                if (c <= outResult.Count - 1)
                {
                    row.OutId = outResult[c].Id;
                    row.OutFlightNo = outResult[c].FlightNo;
                    row.OutRegister = outResult[c].Register;
                    row.OutType = outResult[c].AircraftType.ToLower().StartsWith("b737") ? "B737" : outResult[c].AircraftType;
                    row.OutRoute = outResult[c].Origin + "-" + outResult[c].Destination;
                    row.OutFrom = outResult[c].Origin;
                    row.OutTo = outResult[c].Destination;
                    row.OutRouteICAO = outResult[c].OriginICAO + "-" + outResult[c].DestinationICAO;
                    row.OutFromICAO = outResult[c].OriginICAO;
                    row.OutToICAO = outResult[c].DestinationICAO;
                    row.OutSTDLocal = outResult[c].STDLocal;
                    row.OutTimeLocal = ((DateTime)outResult[c].STDLocal).ToString("HH:mm");


                }
                if (c <= inResult.Count - 1)
                {
                    row.InId = inResult[c].Id;
                    row.InFlightNo = inResult[c].FlightNo;
                    row.InRegister = inResult[c].Register;
                    row.InType = inResult[c].AircraftType.ToLower().StartsWith("b737") ? "B737" : inResult[c].AircraftType;
                    row.InRoute = inResult[c].Origin + "-" + inResult[c].Destination;
                    row.InFrom = inResult[c].Origin;
                    row.InTo = inResult[c].Destination;
                    row.InRouteICAO = inResult[c].OriginICAO + "-" + inResult[c].DestinationICAO;
                    row.InFromICAO = inResult[c].OriginICAO;
                    row.InToICAO = inResult[c].DestinationICAO;
                    row.InSTDLocal = inResult[c].STALocal;
                    row.InTimeLocal = ((DateTime)inResult[c].STALocal).ToString("HH:mm");
                }


                result.Add(row);

            }




            return Ok(result);


        }

        [Route("api/flights/fdpitem/count/{id}")]
        [AcceptVerbs("GET")]
        public IHttpActionResult GetFDPItemsCount(int id)
        {
            var context = new AirpocketAPI.Models.FLYEntities();
            var cnt = context.FDPItems.Where(q => q.FlightId == id).Count();
            return Ok(cnt);
        }
        public string getAcType(string type)
        {
            if (type.ToLower().Contains("md82"))
                return "MD-82";
            if (type.ToLower().Contains("md83"))
                return "MD-83";
            if (type.ToLower().Contains("734"))
                return "B737";
            if (type.ToLower().Contains("735"))
                return "B737";
            return type;
        }
        [Route("api/flights/apt/range/{grouped}")]
        [AcceptVerbs("GET")]
        public IHttpActionResult GetFlightsByAptRange(int grouped, DateTime dtfrom, DateTime dtto, string apt, string airline, string user = "", string phone = "")
        {

            var _dtfrom = dtfrom.Date;
            var _dtto = dtto.Date.AddHours(23).AddMinutes(59).AddSeconds(59);
            var context = new AirpocketAPI.Models.FLYEntities();
            var query = from x in context.ExpFlights
                        where x.FlightStatusId != 4 && (x.DepartureDayLocal >= _dtfrom && x.DepartureDayLocal <= _dtto) && (x.Origin == apt || x.Destination == apt)
                        select x;
            var _result = query.OrderBy(q => q.DepartureDayLocal).ToList();

            var dates = _result.Select(q => q.DepartureDayLocal).Distinct().OrderBy(q => q).ToList();
            var result = new List<AptRpt>();
            foreach (var _dt1 in dates)
            {
                var _dt = (DateTime)_dt1;
                var subResult = _result.Where(q => q.DepartureDayLocal == _dt).ToList();
                var outResult = subResult.Where(q => q.Origin == apt).OrderBy(q => q.STDLocal).ToList();
                var inResult = subResult.Where(q => q.Destination == apt).OrderBy(q => q.STALocal).ToList();
                var cnt = outResult.Count;
                if (inResult.Count > cnt)
                    cnt = inResult.Count;
                for (var c = 0; c < cnt; c++)
                {
                    var row = new AptRpt()
                    {
                        Row = c + 1,
                        Airline = airline,
                        BaseApt = apt,
                        Date = _dt.ToString("yyyy/MM/dd"),
                        DatePersian = subResult.First().PersianDate,
                        Day = subResult.First().PersianDayName,
                        Temp1 = user,
                        Temp2 = phone,
                        Temp3 = _result.First().PersianDate,
                        Temp4 = _result.Last().PersianDate
                    };

                    if (c <= outResult.Count - 1)
                    {
                        row.OutId = outResult[c].Id;
                        row.OutFlightNo = outResult[c].FlightNo;
                        row.OutRegister = outResult[c].Register;
                        row.OutType = getAcType(outResult[c].AircraftType);
                        row.OutRoute = outResult[c].Origin + "-" + outResult[c].Destination;
                        row.OutFrom = outResult[c].Origin;
                        row.OutTo = outResult[c].Destination;
                        row.OutRouteICAO = outResult[c].OriginICAO + "-" + outResult[c].DestinationICAO;
                        row.OutFromICAO = outResult[c].OriginICAO;
                        row.OutToICAO = outResult[c].DestinationICAO;
                        row.OutSTDLocal = outResult[c].STDLocal;
                        row.OutTimeLocal = ((DateTime)outResult[c].STDLocal).ToString("HH:mm");


                    }

                    ///////////  LINKED  TO OUT FLIGHT
                    try
                    {
                        var _fn = Convert.ToInt32(row.OutFlightNo);
                        var _inresult = inResult.Where(q => Convert.ToInt32(q.FlightNo) == (_fn + 1)).FirstOrDefault();
                        if (_inresult != null)
                        {
                            row.InId = _inresult.Id;
                            row.InFlightNo = _inresult.FlightNo;
                            row.InRegister = _inresult.Register;
                            row.InType = getAcType(_inresult.AircraftType);
                            row.InRoute = _inresult.Origin + "-" + _inresult.Destination;
                            row.InFrom = _inresult.Origin;
                            row.InTo = _inresult.Destination;
                            row.InRouteICAO = _inresult.OriginICAO + "-" + _inresult.DestinationICAO;
                            row.InFromICAO = _inresult.OriginICAO;
                            row.InToICAO = _inresult.DestinationICAO;
                            row.InSTDLocal = _inresult.STALocal;
                            row.InTimeLocal = ((DateTime)_inresult.STALocal).ToString("HH:mm");
                            if (_inresult.ArrivalDayLocal != _inresult.DepartureDayLocal)
                                row.InTimeLocal = row.InTimeLocal + "+1";
                        }
                    }
                    catch (Exception ex)
                    {

                    }

                    ////////////////////////


                    ///// SORTED BY ARRIVAL
                    //if (c <= inResult.Count - 1)
                    //{
                    //    row.InId = inResult[c].Id;
                    //    row.InFlightNo = inResult[c].FlightNo;
                    //    row.InRegister = inResult[c].Register;
                    //    row.InType = getAcType(inResult[c].AircraftType);
                    //    row.InRoute = inResult[c].Origin + "-" + inResult[c].Destination;
                    //    row.InFrom = inResult[c].Origin;
                    //    row.InTo = inResult[c].Destination;
                    //    row.InRouteICAO = inResult[c].OriginICAO + "-" + inResult[c].DestinationICAO;
                    //    row.InFromICAO = inResult[c].OriginICAO;
                    //    row.InToICAO = inResult[c].DestinationICAO;
                    //    row.InSTDLocal = inResult[c].STALocal;
                    //    row.InTimeLocal = ((DateTime)inResult[c].STALocal).ToString("HH:mm");
                    //    if (inResult[c].ArrivalDayLocal != inResult[c].DepartureDayLocal)
                    //        row.InTimeLocal = row.InTimeLocal + "+1";
                    //}
                    //////////////////////////////////////


                    result.Add(row);

                }

            }




            if (grouped == 0)
                return Ok(result);
            else
            {
                var gresult = (from x in result
                               group x by new { x.Date, x.DatePersian, x.Day, x.Airline, x.BaseApt } into grp
                               select new
                               {
                                   //grp.Key.Airline,
                                   //grp.Key.BaseApt,
                                   Airline = _result.First().PersianDate,
                                   BaseApt = _result.Last().PersianDate,
                                   grp.Key.Date,
                                   grp.Key.DatePersian,
                                   grp.Key.Day,
                                   Items = grp.ToList()
                               }).ToList();

                return Ok(gresult);
            }


        }


        [Route("api/flights/apt/range/type2/{grouped}")]
        [AcceptVerbs("GET")]
        public IHttpActionResult GetFlightsByAptRangeType2(
            int grouped, DateTime dtfrom, string apt, string airline, string user = "", string phone = ""
            )
        {
            // var dtfrom = new DateTime(2021,7,31);
            // var dtto = new DateTime(2021, 7, 31);
            //var airline = "CSPN";
            //var apt = "THR";
            //var user = "X";
            //var phone = "0912";
            var _dtfrom = dtfrom.Date;
            var _dtto = _dtfrom.AddHours(23).AddMinutes(59).AddSeconds(59);
            var context = new AirpocketAPI.Models.FLYEntities();

            var query = from x in context.ExpFlights
                        where x.FlightStatusId != 4
                        //&& (x.DepartureDayLocal >= _dtfrom && x.DepartureDayLocal <= _dtto) 
                        && (x.DepartureDayLocal == _dtfrom || x.ArrivalDayLocal == _dtfrom)
                        && (x.Origin == apt || x.Destination == apt)
                        select x;
            var _result = query.OrderBy(q => q.DepartureDayLocal).ToList();

            var dates = new List<DateTime>() { _dtfrom };// _result.Select(q => q.DepartureDayLocal).Distinct().OrderBy(q => q).ToList();
            var predates = new List<DateTime>() { _dtfrom.AddDays(-1) };
            var lastRec = _result.Last();
            var result = new List<AptRpt>();

            var _cc = 0;
            foreach (var _dt1 in predates)
            {
                var _dt = (DateTime)_dt1;
                var subResult = _result.Where(q => q.DepartureDayLocal == _dt).ToList();
                //var outResult = subResult.Where(q => q.Origin == apt).OrderBy(q => q.STDLocal).ToList();
                var inResult = subResult.Where(q => q.Destination == apt).OrderBy(q => q.STALocal).ToList();
                _cc = inResult.Count;

                for (var c = 0; c < _cc; c++)
                {
                    var row = new AptRpt()
                    {
                        Row = c + 1,
                        Airline = airline,
                        BaseApt = apt,
                        Date = ((DateTime)lastRec.DepartureDayLocal).ToString("yyyy/MM/dd"),
                        DatePersian = lastRec.PersianDate,
                        Day = lastRec.PersianDayName,
                        Temp1 = user,
                        Temp2 = phone,
                        Temp3 = lastRec.PersianDate,
                        Temp4 = lastRec.PersianDate
                    };

                    //if (c <= outResult.Count - 1)
                    //{
                    //    row.OutId = outResult[c].Id;
                    //    row.OutFlightNo = outResult[c].FlightNo;
                    //    row.OutRegister = outResult[c].Register;
                    //    row.OutType = getAcType(outResult[c].AircraftType);
                    //    row.OutRoute = outResult[c].Origin + "-" + outResult[c].Destination;
                    //    row.OutFrom = outResult[c].Origin;
                    //    row.OutTo = outResult[c].Destination;
                    //    row.OutRouteICAO = outResult[c].OriginICAO + "-" + outResult[c].DestinationICAO;
                    //    row.OutFromICAO = outResult[c].OriginICAO;
                    //    row.OutToICAO = outResult[c].DestinationICAO;
                    //    row.OutSTDLocal = outResult[c].STDLocal;
                    //    row.OutTimeLocal = ((DateTime)outResult[c].STDLocal).ToString("HH:mm");


                    //}

                    ///////////  LINKED  TO OUT FLIGHT
                    var _inresult = inResult[c];
                    try
                    {
                        // var _fn = Convert.ToInt32(row.OutFlightNo);
                        // var _inresult = inResult.Where(q => Convert.ToInt32(q.FlightNo) == (_fn + 1)).FirstOrDefault();
                        if (_inresult != null)
                        {
                            row.InId = _inresult.Id;
                            row.InFlightNo = _inresult.FlightNo;
                            row.InRegister = _inresult.Register;
                            row.InType = getAcType(_inresult.AircraftType);
                            row.InRoute = _inresult.Origin + "-" + _inresult.Destination;
                            row.InFrom = _inresult.Origin;
                            row.InTo = _inresult.Destination;
                            row.InRouteICAO = _inresult.OriginICAO + "-" + _inresult.DestinationICAO;
                            row.InFromICAO = _inresult.OriginICAO;
                            row.InToICAO = _inresult.DestinationICAO;
                            row.InSTDLocal = _inresult.STALocal;
                            row.InTimeLocal = ((DateTime)_inresult.STALocal).ToString("HHmm");
                            // if (_inresult.ArrivalDayLocal != _inresult.DepartureDayLocal)
                            //     row.InTimeLocal = row.InTimeLocal + "+1";
                        }
                    }
                    catch (Exception ex)
                    {

                    }



                    result.Add(row);

                }

            }

            foreach (var _dt1 in dates)
            {
                var _dt = (DateTime)_dt1;
                var subResult = _result.Where(q => q.DepartureDayLocal == _dt).ToList();
                var outResult = subResult.Where(q => q.Origin == apt).OrderBy(q => q.STDLocal).ToList();
                var inResult = subResult.Where(q => q.Destination == apt).OrderBy(q => q.STALocal).ToList();
                var cnt = outResult.Count;
                if (inResult.Count > cnt)
                    cnt = inResult.Count;
                for (var c = 0; c < cnt; c++)
                {
                    var row = new AptRpt()
                    {
                        Row = c + 1 + _cc,
                        Airline = airline,
                        BaseApt = apt,
                        Date = ((DateTime)lastRec.DepartureDayLocal).ToString("yyyy/MM/dd"),
                        DatePersian = lastRec.PersianDate,
                        Day = lastRec.PersianDayName,
                        Temp1 = user,
                        Temp2 = phone,
                        Temp3 = lastRec.PersianDate,
                        Temp4 = lastRec.PersianDate
                    };

                    if (c <= outResult.Count - 1)
                    {
                        row.OutId = outResult[c].Id;
                        row.OutFlightNo = outResult[c].FlightNo;
                        row.OutRegister = outResult[c].Register;
                        row.OutType = getAcType(outResult[c].AircraftType);
                        row.OutRoute = outResult[c].Origin + "-" + outResult[c].Destination;
                        row.OutFrom = outResult[c].Origin;
                        row.OutTo = outResult[c].Destination;
                        row.OutRouteICAO = outResult[c].OriginICAO + "-" + outResult[c].DestinationICAO;
                        row.OutFromICAO = outResult[c].OriginICAO;
                        row.OutToICAO = outResult[c].DestinationICAO;
                        row.OutSTDLocal = outResult[c].STDLocal;
                        row.OutTimeLocal = ((DateTime)outResult[c].STDLocal).ToString("HHmm");


                    }

                    ///////////  LINKED  TO OUT FLIGHT
                    try
                    {
                        var _fn = Convert.ToInt32(row.OutFlightNo);
                        var _inresult = inResult.Where(q => Convert.ToInt32(q.FlightNo) == (_fn + 1)).FirstOrDefault();
                        if (_inresult != null && _inresult.ArrivalDayLocal == _inresult.DepartureDayLocal)
                        {
                            row.InId = _inresult.Id;
                            row.InFlightNo = _inresult.FlightNo;
                            row.InRegister = _inresult.Register;
                            row.InType = getAcType(_inresult.AircraftType);
                            row.InRoute = _inresult.Origin + "-" + _inresult.Destination;
                            row.InFrom = _inresult.Origin;
                            row.InTo = _inresult.Destination;
                            row.InRouteICAO = _inresult.OriginICAO + "-" + _inresult.DestinationICAO;
                            row.InFromICAO = _inresult.OriginICAO;
                            row.InToICAO = _inresult.DestinationICAO;
                            row.InSTDLocal = _inresult.STALocal;
                            row.InTimeLocal = ((DateTime)_inresult.STALocal).ToString("HHmm");
                            //if (_inresult.ArrivalDayLocal != _inresult.DepartureDayLocal)
                            //   row.InTimeLocal = row.InTimeLocal + "+1";
                        }
                    }
                    catch (Exception ex)
                    {

                    }



                    result.Add(row);

                }

            }




            if (grouped == 0)
                return Ok(result);
            else
            {
                var gresult = (from x in result
                               group x by new { x.Date, x.DatePersian, x.Day, x.Airline, x.BaseApt } into grp
                               select new
                               {
                                   //grp.Key.Airline,
                                   //grp.Key.BaseApt,
                                   Airline = _result.First().PersianDate,
                                   BaseApt = _result.Last().PersianDate,
                                   grp.Key.Date,
                                   grp.Key.DatePersian,
                                   grp.Key.Day,
                                   Items = grp.ToList()
                               }).ToList();

                return Ok(gresult);
            }


        }



        [Route("api/book/file/rename")]
        [AcceptVerbs("Post")]
        public IHttpActionResult PostBookFileRename(dynamic dto)
        {
            int id = Convert.ToInt32(dto.id);
            string name = Convert.ToString(dto.name);
            var context = new AirpocketAPI.Models.FLYEntities();
            var bf = context.BookFiles.Where(q => q.Id == id).FirstOrDefault();
            var doc = context.Documents.Where(q => q.Id == bf.DocumentId).FirstOrDefault();
            doc.SysUrl = name;
            context.SaveChanges();

            return Ok(name);



        }

        [Route("api/asmx")]
        [AcceptVerbs("GET")]
        public IHttpActionResult GetASMX()
        {
            var prms = new Dictionary<string, string>();
            prms.Add("job", "P1");
            var result = CallWebMethod("http://localhost:58908/soap.asmx/GetCrews2", prms);
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(result);


            var jresult = JsonConvert.SerializeXmlNode(doc);
            var obj = JsonConvert.DeserializeObject(jresult);
            WebService ws = new WebService("http://localhost:58908/soap.asmx", "GetCrews2");
            ws.Params.Add("job", "P1");

            ws.Invoke();


            //return Ok(ws.ResultJSON);

            return Ok(doc);
        }

        [Route("api/_idea")]
        [AcceptVerbs("GET")]
        public IHttpActionResult GetIdea()
        {
            string apiUrl = "http://fleet.caspianairlines.com/airpocketexternal/api/idea/alt/sessions/obj";
            var input = new
            {

            };
            string inputJson = JsonConvert.SerializeObject(input);
            WebClient client = new WebClient();
            client.Headers["Content-type"] = "application/json";
            client.Encoding = Encoding.UTF8;
            string json = client.DownloadString(apiUrl);
            json = json.Replace("?xml", "d_xml").Replace("soap:Envelope", "d_envelope").Replace("soap:Body", "d_body")
                .Replace("@xmlns:soap", "d_xmlns_soap").Replace("@xmlns:xsi", "d_xmlns_xsi").Replace("@xmlns:xsd", "d_xmlns_xsd")
                .Replace("@xmlns", "d_xmlns")
                .Replace("GetTotalDataJsonResponse", "response")
                .Replace("GetTotalDataJsonResult", "result");
            var obj = JsonConvert.DeserializeObject<IdeaResultSession>(json);
            //List<Customer> customers = (new JavaScriptSerializer()).Deserialize<List<Customer>>(json);
            //if (customers.Count > 0)
            //{
            //    foreach (Customer customer in customers)
            //    {
            //        Console.WriteLine(customer.ContactName);
            //    }
            //}
            //else
            //{
            //    Console.WriteLine("No records found.");
            //}
            var response = obj.d_envelope.d_body.response.result;
            var responseJson = JsonConvert.DeserializeObject<List<IdeaSessionX>>(response);
            return Ok(responseJson);
        }

        [Route("api/_idea/{prm}/{type}")]
        [AcceptVerbs("GET")]
        public IHttpActionResult GetIdeaCourseUnique(string prm, string type)
        {
            var prms = new Dictionary<string, string>();
            if (prm == "unique")
                prms.Add("title", "PersonelUniquePassCourse");
            if (prm == "all")
                prms.Add("title", "PersonelAllPassCourse");
            if (prm == "sessions")
                prms.Add("title", "PersonelClassSessions");
            prms.Add("filters", "");

            var result = CallWebMethod("https://192.168.101.33/IdeaWeb/Apps/Services/TrainingWS.asmx/GetTotalDataJson", prms);
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(result);


            var jresult = JsonConvert.SerializeXmlNode(doc);
            var obj = JsonConvert.DeserializeObject(jresult);

            if (type == "str")
                return Ok(jresult);
            else if (type == "xml")
                return Ok(doc);
            else return Ok(obj);
        }


        [Route("api/idea/alt/{prm}/{type}/{year}")]
        [AcceptVerbs("GET")]
        public IHttpActionResult GetIdeaCourseUniqueAlt(string prm, string type, string year)
        {



            WebService ws = new WebService("https://192.168.101.33/IdeaWeb/Apps/Services/TrainingWS.asmx", "GetTotalDataJson");
            if (prm == "unique")
                ws.Params.Add("title", "PersonelUniquePassCourse");
            if (prm == "all")
                ws.Params.Add("title", "PersonelAllPassCourse");
            if (prm == "sessions")
                ws.Params.Add("title", "PersonelClassSessions");
            if (year == "-1")
                ws.Params.Add("filters", "");
            else
                ws.Params.Add("filters", year);
            ws.Invoke();


            //return Ok(ws.ResultJSON);

            //return Ok(doc);
            if (type == "str")
                return Ok(ws.ResultString);
            else if (type == "xml")
                return Ok(ws.ResultXML);
            else return Ok(ws.ResultJsonObject);
        }

        [Route("api/idea/alt2/{prm}/{type}")]
        [AcceptVerbs("GET")]
        public IHttpActionResult GetIdeaCourseUniqueAlt2(string prm, string type)
        {

            var level = "a";
            try
            {
                WebService ws = new WebService("https://192.168.101.33/IdeaWeb/Apps/Services/TrainingWS.asmx", "GetTotalDataJson");
                if (prm == "unique")
                    ws.Params.Add("title", "PersonelUniquePassCourse");
                if (prm == "all")
                    ws.Params.Add("title", "PersonelAllPassCourse");
                if (prm == "sessions")
                    ws.Params.Add("title", "PersonelClassSessions");
                ws.Params.Add("filters", "2021");
                ws.Invoke();
                level = "b";

                //return Ok(ws.R  esultJSON);

                //return Ok(doc);
                if (type == "str")
                    return Ok(ws.ResultString);
                else if (type == "xml")
                    return Ok(ws.ResultXML);
                else return Ok(ws.ResultJsonObject);
            }
            catch (Exception ex)
            {
                var msg = ex.Message;
                if (ex.InnerException != null)
                    msg += "  INNER:   " + ex.InnerException.Message;
                return Ok(msg);
            }

        }


        [Route("api/mail/mvt/{flightId}/{sender}/{user}/{password}")]
        [AcceptVerbs("GET")]
        public IHttpActionResult GetIdeaUniqueSync(int flightId, string sender, string user, string password)
        {
            if (user != "vahid")
                return BadRequest("Not Authenticated");
            if (password != "Chico1359")
                return BadRequest("Not Authenticated");

            var helper = new MailHelper();
            var result = helper.CreateMVTMessage(flightId, sender);

            return Ok(result);
        }


        [Route("api/mvt/send/{user}/{password}/{force}/{day}/{fn}")]
        [AcceptVerbs("GET")]
        public IHttpActionResult GetSendMVTByFN(string user, string password, string day, string fn, int force)
        {
            if (user != "fp")
                return BadRequest("Not Authenticated");
            if (password != "Z12345aA")
                return BadRequest("Not Authenticated");

            var helper = new MailHelper();
            var result = helper.CreateMVTMessageByFlightNo(day, fn, force);

            return Ok(result);
        }


        [Route("api/dr/flight/{fltid}")]
        [AcceptVerbs("GET")]
        public IHttpActionResult GetDRByFlight(int fltid)
        {


            var context = new AirpocketAPI.Models.FLYEntities();
            var result = context.EFBDSPReleases.FirstOrDefault(q => q.FlightId == fltid);
            return Ok(result);
        }
        [Route("api/dr/save")]
        [AcceptVerbs("Post")]
        public IHttpActionResult PostDR(DSPReleaseViewModel DSPRelease)
        {
            var context = new AirpocketAPI.Models.FLYEntities();
            var release = context.EFBDSPReleases.FirstOrDefault(q => q.FlightId == DSPRelease.FlightId);
            if (release == null)
            {
                release = new EFBDSPRelease();
                context.EFBDSPReleases.Add(release);

            }

            release.User = DSPRelease.User;
            release.DateUpdate = DateTime.UtcNow.ToString("yyyyMMddHHmm");


            release.FlightId = DSPRelease.FlightId;
            release.ActualWXDSP = DSPRelease.ActualWXDSP;
            release.ActualWXCPT = DSPRelease.ActualWXCPT;
            release.ActualWXDSPRemark = DSPRelease.ActualWXDSPRemark;
            release.ActualWXCPTRemark = DSPRelease.ActualWXCPTRemark;
            release.WXForcastDSP = DSPRelease.WXForcastDSP;
            release.WXForcastCPT = DSPRelease.WXForcastCPT;
            release.WXForcastDSPRemark = DSPRelease.WXForcastDSPRemark;
            release.WXForcastCPTRemark = DSPRelease.WXForcastCPTRemark;
            release.SigxWXDSP = DSPRelease.SigxWXDSP;
            release.SigxWXCPT = DSPRelease.SigxWXCPT;
            release.SigxWXDSPRemark = DSPRelease.SigxWXDSPRemark;
            release.SigxWXCPTRemark = DSPRelease.SigxWXCPTRemark;
            release.WindChartDSP = DSPRelease.WindChartDSP;
            release.WindChartCPT = DSPRelease.WindChartCPT;
            release.WindChartDSPRemark = DSPRelease.WindChartDSPRemark;
            release.WindChartCPTRemark = DSPRelease.WindChartCPTRemark;
            release.NotamDSP = DSPRelease.NotamDSP;
            release.NotamCPT = DSPRelease.NotamCPT;
            release.NotamDSPRemark = DSPRelease.NotamDSPRemark;
            release.NotamCPTRemark = DSPRelease.NotamCPTRemark;
            release.ComputedFligthPlanDSP = DSPRelease.ComputedFligthPlanDSP;
            release.ComputedFligthPlanCPT = DSPRelease.ComputedFligthPlanCPT;
            release.ComputedFligthPlanDSPRemark = DSPRelease.ComputedFligthPlanDSPRemark;
            release.ComputedFligthPlanCPTRemark = DSPRelease.ComputedFligthPlanCPTRemark;
            release.ATCFlightPlanDSP = DSPRelease.ATCFlightPlanDSP;
            release.ATCFlightPlanCPT = DSPRelease.ATCFlightPlanCPT;
            release.ATCFlightPlanDSPRemark = DSPRelease.ATCFlightPlanDSPRemark;
            release.ATCFlightPlanCPTRemark = DSPRelease.ATCFlightPlanCPTRemark;
            release.PermissionsDSP = DSPRelease.PermissionsDSP;
            release.PermissionsCPT = DSPRelease.PermissionsCPT;
            release.PermissionsDSPRemark = DSPRelease.PermissionsDSPRemark;
            release.PermissionsCPTRemark = DSPRelease.PermissionsCPTRemark;
            release.JeppesenAirwayManualDSP = DSPRelease.JeppesenAirwayManualDSP;
            release.JeppesenAirwayManualCPT = DSPRelease.JeppesenAirwayManualCPT;
            release.JeppesenAirwayManualDSPRemark = DSPRelease.JeppesenAirwayManualDSPRemark;
            release.JeppesenAirwayManualCPTRemark = DSPRelease.JeppesenAirwayManualCPTRemark;
            release.MinFuelRequiredDSP = DSPRelease.MinFuelRequiredDSP;
            release.MinFuelRequiredCPT = DSPRelease.MinFuelRequiredCPT;
            release.MinFuelRequiredCFP = DSPRelease.MinFuelRequiredCFP;
            release.MinFuelRequiredPilotReq = DSPRelease.MinFuelRequiredPilotReq;
            release.GeneralDeclarationDSP = DSPRelease.GeneralDeclarationDSP;
            release.GeneralDeclarationCPT = DSPRelease.GeneralDeclarationCPT;
            release.GeneralDeclarationDSPRemark = DSPRelease.GeneralDeclarationDSPRemark;
            release.GeneralDeclarationCPTRemark = DSPRelease.GeneralDeclarationCPTRemark;
            release.FlightReportDSP = DSPRelease.FlightReportDSP;
            release.FlightReportCPT = DSPRelease.FlightReportCPT;
            release.FlightReportDSPRemark = DSPRelease.FlightReportDSPRemark;
            release.FlightReportCPTRemark = DSPRelease.FlightReportCPTRemark;
            release.TOLndCardsDSP = DSPRelease.TOLndCardsDSP;
            release.TOLndCardsCPT = DSPRelease.TOLndCardsCPT;
            release.TOLndCardsDSPRemark = DSPRelease.TOLndCardsDSPRemark;
            release.TOLndCardsCPTRemark = DSPRelease.TOLndCardsCPTRemark;
            release.LoadSheetDSP = DSPRelease.LoadSheetDSP;
            release.LoadSheetCPT = DSPRelease.LoadSheetCPT;
            release.LoadSheetDSPRemark = DSPRelease.LoadSheetDSPRemark;
            release.LoadSheetCPTRemark = DSPRelease.LoadSheetCPTRemark;
            release.FlightSafetyReportDSP = DSPRelease.FlightSafetyReportDSP;
            release.FlightSafetyReportCPT = DSPRelease.FlightSafetyReportCPT;
            release.FlightSafetyReportDSPRemark = DSPRelease.FlightSafetyReportDSPRemark;
            release.FlightSafetyReportCPTRemark = DSPRelease.FlightSafetyReportCPTRemark;
            release.AVSECIncidentReportDSP = DSPRelease.AVSECIncidentReportDSP;
            release.AVSECIncidentReportCPT = DSPRelease.AVSECIncidentReportCPT;
            release.AVSECIncidentReportDSPRemark = DSPRelease.AVSECIncidentReportDSPRemark;
            release.AVSECIncidentReportCPTRemark = DSPRelease.AVSECIncidentReportCPTRemark;
            release.OperationEngineeringDSP = DSPRelease.OperationEngineeringDSP;
            release.OperationEngineeringCPT = DSPRelease.OperationEngineeringCPT;
            release.OperationEngineeringDSPRemark = DSPRelease.OperationEngineeringDSPRemark;
            release.OperationEngineeringCPTRemark = DSPRelease.OperationEngineeringCPTRemark;
            release.VoyageReportDSP = DSPRelease.VoyageReportDSP;
            release.VoyageReportCPT = DSPRelease.VoyageReportCPT;
            release.VoyageReportDSPRemark = DSPRelease.VoyageReportDSPRemark;
            release.VoyageReportCPTRemark = DSPRelease.VoyageReportCPTRemark;
            release.PIFDSP = DSPRelease.PIFDSP;
            release.PIFCPT = DSPRelease.PIFCPT;
            release.PIFDSPRemark = DSPRelease.PIFDSPRemark;
            release.PIFCPTRemark = DSPRelease.PIFCPTRemark;
            release.GoodDeclarationDSP = DSPRelease.GoodDeclarationDSP;
            release.GoodDeclarationCPT = DSPRelease.GoodDeclarationCPT;
            release.GoodDeclarationDSPRemark = DSPRelease.GoodDeclarationDSPRemark;
            release.GoodDeclarationCPTRemark = DSPRelease.GoodDeclarationCPTRemark;
            release.IPADDSP = DSPRelease.IPADDSP;
            release.IPADCPT = DSPRelease.IPADCPT;
            release.IPADDSPRemark = DSPRelease.IPADDSPRemark;
            release.IPADCPTRemark = DSPRelease.IPADCPTRemark;
            release.DateConfirmed = DSPRelease.DateConfirmed;
            release.DispatcherId = DSPRelease.DispatcherId;
            context.SaveChanges();
            return Ok(release);


        }


        [Route("api/asr/flight/{fltid}")]
        [AcceptVerbs("GET")]
        public IHttpActionResult GetASRByFlight(int fltid)
        {


            var context = new AirpocketAPI.Models.FLYEntities();
            var result = context.EFBASRs.FirstOrDefault(q => q.FlightId == fltid);
            return Ok(result);
        }

        [Route("api/vr/flight/{fltid}")]
        [AcceptVerbs("GET")]
        public IHttpActionResult GetVRByFlight(int fltid)
        {


            var context = new AirpocketAPI.Models.FLYEntities();

            var result = context.EFBVoyageReports.FirstOrDefault(q => q.FlightId == fltid);
            if (result == null)
                return Ok(result);
            result.EFBReasons= context.EFBReasons.Where(q => q.VoyageReportId == result.Id).ToList();
            result.EFBFlightIrregularities = context.EFBFlightIrregularities.Where(q => q.VoyageReportId == result.Id).ToList();
            return Ok(result);
        }

        [Route("api/log/{fltid}")]
        [AcceptVerbs("GET")]
        public IHttpActionResult GetFlightLog(int fltid)
        {


            var context = new AirpocketAPI.Models.FLYEntities();

            var result = context.AppLegs.FirstOrDefault(q => q.FlightId == fltid);
            if (result == null)
                return Ok(new
                {
                    IsSuccess = false,
                });
            var crews = context.ViewFlightCrewNewXes.Where(q => q.FlightId == fltid).ToList();
            return Ok(new { 
              IsSuccess=true,
              Flight=result,
              Crews=crews,
            });
 
            
        }

        [Route("api/ofp/details/{fltid}")]
        [AcceptVerbs("GET")]
        public IHttpActionResult GetOFPDetails(int fltid)
        {


            var context = new AirpocketAPI.Models.FLYEntities();

            var result = context.OFPImports.FirstOrDefault(q => q.FlightId == fltid);
            if (result == null)
                return Ok(new
                {
                    IsSuccess = false,
                });
            var props = context.OFPImportProps.Where(q => q.OFPId == result.Id).ToList();
            return Ok(new
            {
                IsSuccess = true,
                OFP = result,
                Props = props,
            });

             
        }



        [Route("api/login/history/{user}/{from}/{to}")]
        [AcceptVerbs("GET")]
        public IHttpActionResult GetLoginHistory(string user, string from, string to)
        {
            var context = new AirpocketAPI.Models.FLYEntities();
            var query = from x in context.LoginInfoes

                        select x;
            if (user != "-1")
                query = query.Where(q => q.User == user);
            if (from != "-1")
            {
                var prts1 = from.Split('-').Select(q => Convert.ToInt32(q)).ToList();
                var _from = new DateTime(prts1[0], prts1[1], prts1[2]);
                query = query.Where(q => q.DateCreate >= _from);
            }

            if (to != "-1")
            {
                var prts1 = to.Split('-').Select(q => Convert.ToInt32(q)).ToList();
                var _to = new DateTime(prts1[0], prts1[1], prts1[2]);
                _to = _to.AddHours(23).AddMinutes(59).AddSeconds(50);
                query = query.Where(q => q.DateCreate <= _to);
            }



            var result = query.OrderBy(q => q.DateCreate).ToList();
            var abst = result.Select(q => new { q.User, q.IP, q.LocationCity, q.DateCreate }).ToList();

            return Ok(abst);
        }
        [Route("api/login/history/detailed/{user}/{from}/{to}")]
        [AcceptVerbs("GET")]
        public IHttpActionResult GetLoginHistoryDetailed(string user, string from, string to)
        {
            var context = new AirpocketAPI.Models.FLYEntities();
            var query = from x in context.LoginInfoes

                        select x;
            if (user != "-1")
                query = query.Where(q => q.User == user);
            if (from != "-1")
            {
                var prts1 = from.Split('-').Select(q => Convert.ToInt32(q)).ToList();
                var _from = new DateTime(prts1[0], prts1[1], prts1[2]);
                query = query.Where(q => q.DateCreate >= _from);
            }

            if (to != "-1")
            {
                var prts1 = to.Split('-').Select(q => Convert.ToInt32(q)).ToList();
                var _to = new DateTime(prts1[0], prts1[1], prts1[2]);
                _to = _to.AddHours(23).AddMinutes(59).AddSeconds(50);
                query = query.Where(q => q.DateCreate <= _to);
            }



            var result = query.OrderBy(q => q.DateCreate).ToList();

            return Ok(result);
        }
        [Route("api/login/save")]
        [AcceptVerbs("Post")]
        public IHttpActionResult PostLoginInfo(dynamic dto)
        {
            var context = new AirpocketAPI.Models.FLYEntities();
            string user = Convert.ToString(dto.user);
            string ip = Convert.ToString(dto.ip);
            string city = Convert.ToString(dto.city);
            string info = Convert.ToString(dto.info);
            var result = new LoginInfo()
            {
                DateCreate = DateTime.Now,
                Info = info,
                IP = ip,
                LocationCity = city,
                User = user
            };
            context.LoginInfoes.Add(result);
            context.SaveChanges();
            return Ok(new
            {
                result.IP,
                result.LocationCity
            });


        }

        [Route("api/ofp/check")]
        [AcceptVerbs("GET")]
        public IHttpActionResult GetCheckOFP()
        {
            var user = "demo";
            var fn = "FlightPlan_2021.09.01.OIAW.OIII.013.txt";
            var context = new AirpocketAPI.Models.FLYEntities();
            var fnprts = (fn.Split('_')[1]).Split('.');
            var _date = new DateTime(Convert.ToInt32(fnprts[0]), Convert.ToInt32(fnprts[1]), Convert.ToInt32(fnprts[2]));
            var _fltno = (fnprts[5]).PadLeft(4, '0');
            var _origin = fnprts[3];
            var _destination = fnprts[4];
            var cplan = context.OFPImports.FirstOrDefault(q => q.DateFlight == _date && q.FlightNo == _fltno && q.Origin == _origin && q.Destination == _destination);
            if (cplan == null)
                return Ok(new
                {
                    ofpId = -1,
                });
            else
            {
                return Ok(new
                {
                    ofpId = cplan.Id,
                    user = cplan.User,
                    date = cplan.DateCreate,
                    fileName = cplan.FileName,
                });
            }

        }

        [Route("api/ofp/check/flight/{id}")]
        [AcceptVerbs("GET")]
        public IHttpActionResult GetCheckOFP(int id)
        {

            var context = new AirpocketAPI.Models.FLYEntities();

            var cplan = context.OFPImports.FirstOrDefault(q => q.FlightId == id);
            if (cplan == null)
                return Ok(new
                {
                    ofpId = -1,
                });
            else
            {
                return Ok(new
                {
                    ofpId = cplan.Id,
                    user = cplan.User,
                    date = cplan.DateCreate,
                    fileName = cplan.FileName,
                });
            }

        }

        [Route("api/ofp/flight/{id}")]
        [AcceptVerbs("GET")]
        public IHttpActionResult GetFlightOFP(int id)
        {

            var context = new AirpocketAPI.Models.FLYEntities();
            // context.Configuration.LazyLoadingEnabled = false;

            var cplan = context.OFPImports.FirstOrDefault(q => q.FlightId == id);
            //var result=JsonConvert.SerializeObject(cplan, Newtonsoft.Json.Formatting.None,
            //            new JsonSerializerSettings()
            //            {
            //                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            //            });
            if (cplan == null)
                return Ok(new { Id = -1 });
            var result = new
            {
                cplan.Id,
                cplan.FlightId,
                cplan.DateCreate,
                cplan.DateFlight,
                cplan.Destination,
                cplan.FileName,

                cplan.FlightNo,
                cplan.Origin,
                cplan.Text,
                cplan.TextOutput,
                cplan.User,


            };
            return Ok(result);

        }


        [Route("api/ofp/parse")]
        [AcceptVerbs("GET")]
        public IHttpActionResult GetImportOFP()
        {
            var user = "demo";
            var fn = "FlightPlan_2021.09.01.OIAW.OIII.013.txt";
            var context = new AirpocketAPI.Models.FLYEntities();
            var fnprts = (fn.Split('_')[1]).Split('.');
            var _date = new DateTime(Convert.ToInt32(fnprts[0]), Convert.ToInt32(fnprts[1]), Convert.ToInt32(fnprts[2]));
            var _fltno = (fnprts[5]).PadLeft(4, '0');
            var _origin = fnprts[3];
            var _destination = fnprts[4];


            string folder = HttpContext.Current.Server.MapPath("~/upload");
            string path = Path.Combine(folder, fn);

            var ftext = File.ReadAllText(path);
            var lines = File.ReadAllLines(path).ToList();
            var linesNoSpace = lines.Select(q => q.Replace(" ", "")).ToList();
            var linesTrimStart = lines.Select(q => q.TrimStart()).ToList();

            var flight = context.ViewLegTimes.FirstOrDefault(q => q.STDDay == _date && q.FlightNumber == _fltno);

            var cplan = context.OFPImports.FirstOrDefault(q => q.DateFlight == _date && q.FlightNo == _fltno && q.Origin == _origin && q.Destination == _destination);
            if (cplan != null)
                context.OFPImports.Remove(cplan);

            var plan = new OFPImport()
            {
                DateCreate = DateTime.Now,
                DateFlight = _date,
                FileName = fn,
                FlightNo = _fltno,
                Origin = _origin,
                Destination = _destination,
                User = user,
                Text = ftext,


            };
            if (flight != null)
                plan.FlightId = flight.ID;
            context.OFPImports.Add(plan);


            var fuelParts = OFPHelper.GetFuelParts(lines, linesNoSpace);



            var linesProccessed = new List<string>();
            foreach (var ln in lines)
            {
                var nospace = ln.Replace(" ", "");
                //Cont 05%  000223 00.05            ............  PAX  : .../.../...
                if (nospace.ToLower().StartsWith("cont05"))
                {
                    var prts = ln.Split(new string[] { "...." }, StringSplitOptions.None);
                    linesProccessed.Add(prts[0]);

                }
                // HLD       001299 00.30            A.TAKEOFF W.  CREW : .../.../... 
                else if (nospace.ToLower().StartsWith("hld"))
                {
                    var prts = ln.Split(new string[] { "A." }, StringSplitOptions.None);
                    linesProccessed.Add(prts[0]);
                    //var prts = ln.Split(' ').ToList();
                    //var temp = new List<string>();
                    //foreach (var x in prts)
                    //{
                    //    if (x== "A.TAKEOFF")
                    //    {
                    //        temp.Add(x);
                    //        temp.Add(" ");
                    //    }
                    //    else
                    //    if (x == "CREW:")
                    //    {
                    //        temp.Add("CREW :");
                    //        temp.Add(" ");
                    //    }
                    //    else
                    //    if (x.StartsWith("."))
                    //    {
                    //        var ps = x.Split('/');
                    //        var plst = new List<string>();
                    //        for (int i = 0; i < ps.Count(); i++)
                    //            plst.Add("@crew_" + (i + 1).ToString());
                    //        temp.Add(string.Join("/",plst));
                    //    }
                    //    else
                    //    {
                    //        if (x == "" || x == " ")
                    //            temp.Add(" ");
                    //        else
                    //            temp.Add(x);
                    //    }
                    //}

                    //linesProccessed.Add(string.Join("",temp));
                }
                // TXY       000200                  A.TAKEOFF F.  T.O.B: ...........  
                else if (nospace.ToLower().StartsWith("txy"))
                {
                    var prts = ln.Split(new string[] { "A." }, StringSplitOptions.None);
                    linesProccessed.Add(prts[0]);
                }
                //REQUIRED  006414 02.12            ............
                else if (nospace.ToLower().StartsWith("required"))
                {
                    var prts = ln.Split(new string[] { "...." }, StringSplitOptions.None);
                    linesProccessed.Add(prts[0]);
                }
                //OFP WORKED By CAPT:  .. .. .. .. .. ..  FO: .. .. .. .. .. .. .. ..
                else if (nospace.ToLower().StartsWith("ofpworked"))
                {
                    // var prts = ln.Split(new string[] { "FO" }, StringSplitOptions.None);
                    linesProccessed.Add(" OFP WORKED By CAPT:" +/*@ofbcpt*/"<span class='prop' id='prop_ofbcpt'>  .. .. .. .. .. ..  </span>" + "FO: " +/*@ofbfo"*/"<span class='prop' id='prop_ofbfo'>.. .. .. .. .. .. .. ..</span>");
                }
                // CLEARANCE: .. .. .. .. .. .. .. .. .. .. .. .. .. .. .. .. .. .. ..
                else if (nospace.ToLower().StartsWith("clearance"))
                {
                    // var prts = ln.Split(new string[] { "FO" }, StringSplitOptions.None);
                    linesProccessed.Add(" CLEARANCE: " + "<span class='prop' id='prop_clearance'>.. .. .. .. .. .. .. .. .. .. .. .. .. .. .. .. .. .. ..</span>");
                }
                else
                    linesProccessed.Add(ln);
            }

            var nospaceproccessed = linesProccessed.Select(q => q.Trim().Replace(" ", "").ToLower()).ToList();
            var _indx1 = nospaceproccessed.IndexOf(nospaceproccessed.Where(q => q.ToLower().StartsWith("total")).First());
            _indx1++;
            linesProccessed.Insert(_indx1, " ");

            _indx1++;
            linesProccessed.Insert(_indx1, " CREW : <span class='prop' id='prop_crew1'>...</span>/<span class='prop' id='prop_crew2'>...</span>/<span class='prop' id='prop_crew3'>...</span>");

            _indx1++;
            linesProccessed.Insert(_indx1, " PAX  : <span class='prop' id='prop_pax1'>...</span>/<span class='prop' id='prop_pax2'>...</span>/<span class='prop' id='prop_pax3'>...</span>");

            _indx1++;
            linesProccessed.Insert(_indx1, " T.O.B: <span class='prop' id='prop_tob'>............</span>");

            _indx1++;
            linesProccessed.Insert(_indx1, " A.TAKEOFF W. : <span class='prop' id='prop_atakeoffw'>............</span>");

            _indx1++;
            linesProccessed.Insert(_indx1, " A.TAKEOFF F. : <span class='prop' id='prop_atakeofff'>............</span>");

            _indx1++;
            linesProccessed.Insert(_indx1, " ");

            // CPT    FL  SOT    TAS WIND   COM AW      ZT   ZD   ETO   ATO  REM
            nospaceproccessed = linesProccessed.Select(q => q.Trim().Replace(" ", "").ToLower()).ToList();
            var _indexS = nospaceproccessed.IndexOf(nospaceproccessed.Where(q => q.ToLower().StartsWith("cptflsottas")).First());
            //-------------------------WAYPOINT COORDINATION-----------------------
            var _indexE = nospaceproccessed.IndexOf(nospaceproccessed.Where(q => q.ToLower().Contains("waypointcoordination")).First());
            // OIFM   DES                   T12 GADLU1N 0.14 063  ....  .... 000214 
            //                                  062     0.35 0186 ....  .... 001902
            var propIndex = 1;
            for (int i = _indexS; i < _indexE; i++)
            {
                var ln = linesProccessed[i];
                var prts = ln.Split(new string[] { "...." }, StringSplitOptions.None);
                if (prts.Length > 1)
                {
                    var chrs = ln.ToCharArray();
                    var str = "";
                    foreach (var x in prts)
                    {
                        str += x;
                        if (x != prts.Last())
                        {

                            //if (!string.IsNullOrEmpty(x.Trim().Replace(" ", "")))
                            //    str += "  ";
                            str += "<span class='prop' id='prop_" + propIndex + "'>" + "...." + "</span>"; // "@prop_" + propIndex;
                            propIndex++;
                        }

                    }
                    linesProccessed[i] = str;
                }
            }
            var finalResult = new List<string>();
            //finalResult.Add("<pre>");
            foreach (var x in linesProccessed)
            //finalResult.Add("<div>" + /*Regex.Replace(x, @"\s+", "&nbsp;")*/ReplaceWhitespace(x, "&nbsp;") + " </div>") ;
            {
                finalResult.Add(x);
                plan.OFPImportItems.Add(new OFPImportItem()
                {
                    Line = x
                });
            }
            var _text = "<pre>" + string.Join("<br/>", finalResult) + "</pre>";
            plan.TextOutput = _text;
            context.SaveChanges();
            //finalResult.Add("</pre>");
            return Ok(_text);
        }


        [Route("api/ofp/parse/text2")]
        [AcceptVerbs("POST")]
        public IHttpActionResult PostImportOFP2(dynamic dto)
        {
            try {
                string user = Convert.ToString(dto.user);
                int fltId = Convert.ToInt32(dto.fltId);
                //var fn = "FlightPlan_2021.09.01.OIAW.OIII.013.txt";
                var context = new AirpocketAPI.Models.FLYEntities();

                var flight = context.ViewLegTimes.FirstOrDefault(q => q.ID == fltId);
                //var fnprts = (fn.Split('_')[1]).Split('.');
                //var _date = new DateTime(Convert.ToInt32(fnprts[0]), Convert.ToInt32(fnprts[1]), Convert.ToInt32(fnprts[2]));
                //var _fltno = (fnprts[5]).PadLeft(4, '0');
                // var _origin = fnprts[3];
                //var _destination = fnprts[4];


                //string folder = HttpContext.Current.Server.MapPath("~/upload");
                // string path = Path.Combine(folder, fn);

                //var ftext = File.ReadAllText(path);
                string ftext = Convert.ToString(dto.text);
                ftext = ftext.Replace("........", "^");
                ftext =ftext.Replace(" .......", "........");
                ftext = ftext.Replace("^","........");


                var lines = ftext.Split(new[] { '\r', '\n' }).ToList();
                var linesNoSpace = lines.Select(q => q.Replace(" ", "")).ToList();
                var linesTrimStart = lines.Select(q => q.TrimStart()).ToList();



                var cplan = context.OFPImports.FirstOrDefault(q => q.FlightId == fltId);
                if (cplan != null)
                    context.OFPImports.Remove(cplan);
                List<string> props = new List<string>();
                var plan = new OFPImport()
                {
                    DateCreate = DateTime.Now,
                    DateFlight = flight.STDDay,
                    FileName = "",
                    FlightNo = flight.FlightNumber,
                    Origin = flight.FromAirportICAO,
                    Destination = flight.ToAirportICAO,
                    User = user,
                    Text = ftext,


                };
                if (flight != null)
                    plan.FlightId = flight.ID;
                context.OFPImports.Add(plan);


                //var fuelParts = OFPHelper.GetFuelParts(lines, linesNoSpace);



                var linesProccessed = new List<string>();
                foreach (var ln in lines)
                {
                    var nospace = ln.Replace(" ", "");
                    //Cont 05%  000223 00.05            ............  PAX  : .../.../...
                    if (nospace.ToLower().StartsWith("cont05"))
                    {
                        var prts = ln.Split(new string[] { "...." }, StringSplitOptions.None);
                        linesProccessed.Add(prts[0]);

                    }
                    // HLD       001299 00.30            A.TAKEOFF W.  CREW : .../.../... 
                    else if (nospace.ToLower().StartsWith("hld"))
                    {
                        var prts = ln.Split(new string[] { "A." }, StringSplitOptions.None);
                        linesProccessed.Add(prts[0]);
                         
                    }
                    // TXY       000200                  A.TAKEOFF F.  T.O.B: ...........  
                    else if (nospace.ToLower().StartsWith("txy"))
                    {
                        var prts = ln.Split(new string[] { "A." }, StringSplitOptions.None);
                        linesProccessed.Add(prts[0]);
                    }
                    //REQUIRED  006414 02.12            ............
                    else if (nospace.ToLower().StartsWith("required"))
                    {
                        var prts = ln.Split(new string[] { "...." }, StringSplitOptions.None);
                        linesProccessed.Add(prts[0]);
                    }
                    //OFP WORKED By CAPT:  .. .. .. .. .. ..  FO: .. .. .. .. .. .. .. ..
                    else if (nospace.ToLower().StartsWith("ofpworked"))
                    {
                        // var prts = ln.Split(new string[] { "FO" }, StringSplitOptions.None);
                        linesProccessed.Add(" OFP WORKED By CAPT:" +/*@ofbcpt*/"<span class='prop' id='prop_ofbcpt' ng-click='propClick($event)'>  .. .. .. .. .. ..  </span>" + "FO: " +/*@ofbfo"*/"<span ng-click='propClick($event)' class='prop' id='prop_ofbfo'>.. .. .. .. .. .. .. ..</span>");
                        props.Add("prop_ofbcpt");
                        props.Add("prop_ofbfo");
                    }
                    // CLEARANCE: .. .. .. .. .. .. .. .. .. .. .. .. .. .. .. .. .. .. ..
                    else if (nospace.ToLower().StartsWith("clearance"))
                    {
                        // var prts = ln.Split(new string[] { "FO" }, StringSplitOptions.None);
                        linesProccessed.Add(" CLEARANCE: " + "<span ng-click='propClick($event)' class='prop' id='prop_clearance'>.. .. .. .. .. .. .. .. .. .. .. .. .. .. .. .. .. .. ..</span>");
                        props.Add("prop_clearance");
                    }
                    else
                        linesProccessed.Add(ln);
                }

                var nospaceproccessed = linesProccessed.Select(q => q.Trim().Replace(" ", "").ToLower()).ToList();
                var _indx1 = nospaceproccessed.IndexOf(nospaceproccessed.Where(q => q.ToLower().StartsWith("total")).First());
                _indx1++;
                linesProccessed.Insert(_indx1, " ");

                _indx1++;
                linesProccessed.Insert(_indx1, " CREW : <span ng-click='propClick($event)' class='prop' id='prop_crew1'>...</span>/<span ng-click='propClick($event)' class='prop' id='prop_crew2'>...</span>/<span ng-click='propClick($event)' class='prop' id='prop_crew3'>...</span>");
                props.Add("prop_crew1");
                props.Add("prop_crew2");
                props.Add("prop_crew3");
                _indx1++;
                linesProccessed.Insert(_indx1, " PAX  : <span ng-click='propClick($event)' class='prop' id='prop_pax1'>...</span>/<span ng-click='propClick($event)' class='prop' id='prop_pax2'>...</span>/<span ng-click='propClick($event)' class='prop' id='prop_pax3'>...</span>");
                props.Add("prop_pax1");
                props.Add("prop_pax2");
                props.Add("prop_pax3");
                _indx1++;
                linesProccessed.Insert(_indx1, " T.O.B: <span ng-click='propClick($event)' class='prop' id='prop_tob'>............</span>");
                props.Add("prop_tob");
                _indx1++;
                linesProccessed.Insert(_indx1, " A.TAKEOFF W. : <span ng-click='propClick($event)' class='prop' id='prop_atakeoffw'>............</span>");
                props.Add("prop_atakeoffw");
                _indx1++;
                linesProccessed.Insert(_indx1, " A.TAKEOFF F. : <span ng-click='propClick($event)' class='prop' id='prop_atakeofff'>............</span>");
                props.Add("prop_atakeofff");
                _indx1++;
                linesProccessed.Insert(_indx1, " ");

                // CPT    FL  SOT    TAS WIND   COM AW      ZT   ZD   ETO   ATO  REM
                nospaceproccessed = linesProccessed.Select(q => q.Trim().Replace(" ", "").ToLower()).ToList();
                var _indexS = nospaceproccessed.IndexOf(nospaceproccessed.Where(q => q.ToLower().StartsWith("cptflsottas")).First());
                //-------------------------WAYPOINT COORDINATION-----------------------
                var _indexE = nospaceproccessed.IndexOf(nospaceproccessed.Where(q => q.ToLower().Contains("waypointcoordination")).First());
                // OIFM   DES                   T12 GADLU1N 0.14 063  ....  .... 000214 
                //                                  062     0.35 0186 ....  .... 001902
                var propIndex = 1;
                for (int i = _indexS; i < _indexE; i++)
                {
                    var ln = linesProccessed[i];
                    List<string> prts = new List<string>();
                    var dots = "";
                    bool dot8 = false;
                    var prts8 = ln.Split(new string[] { "........" }, StringSplitOptions.None);
                    var prts4 = ln.Split(new string[] { "...." }, StringSplitOptions.None);
                    if (prts8.Length>1)
                    {
                        prts = prts8.ToList();
                        dots = "........";
                        dot8 = true;
                    }
                    else
                    {
                        prts = prts4.ToList();
                        dots = "....";
                    }
                    if (prts.Count > 1)
                    {
                        var chrs = ln.ToCharArray();
                        var str = "";
                        foreach (var x in prts)
                        {
                            str += x;
                            if (x != prts.Last())
                            {

                                //if (!string.IsNullOrEmpty(x.Trim().Replace(" ", "")))
                                //    str += "  ";
                                str += "<span ng-click='propClick($event)' class='prop' id='prop_" + propIndex + "'>" +dots + "</span>"; // "@prop_" + propIndex;
                                props.Add("prop_" + propIndex);
                                propIndex++;
                            }

                        }
                        linesProccessed[i] = str;
                    }
                }
                var finalResult = new List<string>();
                //finalResult.Add("<pre>");
                foreach (var x in linesProccessed)
                //finalResult.Add("<div>" + /*Regex.Replace(x, @"\s+", "&nbsp;")*/ReplaceWhitespace(x, "&nbsp;") + " </div>") ;
                {
                    finalResult.Add(x);
                    plan.OFPImportItems.Add(new OFPImportItem()
                    {
                        Line = x
                    });
                }
                var _text = "<pre>" + string.Join("<br/>", finalResult) + "</pre>";
                plan.TextOutput = _text;
                var dtupd = DateTime.UtcNow.ToString("yyyyMMddHHmm");
                foreach (var p in props)
                {
                    plan.OFPImportProps.Add(new OFPImportProp()
                    {
                        DateUpdate = dtupd,
                        PropName = p,
                        PropValue = "",
                        User = user,

                    });
                }
                context.SaveChanges();
                //context.SaveChangesAsync();
                //finalResult.Add("</pre>");
                return Ok(_text);
            }
            catch(Exception ex)
            {
                return Ok("-1");

            }
           
        }

        [Route("api/ofp/parse/text")]
        [AcceptVerbs("POST")]
        public IHttpActionResult PostImportOFP(dynamic dto)
        {
            try
            {
                string user = Convert.ToString(dto.user);
                int fltId = Convert.ToInt32(dto.fltId);
                //var fn = "FlightPlan_2021.09.01.OIAW.OIII.013.txt";
                var context = new AirpocketAPI.Models.FLYEntities();

                var flight = context.ViewLegTimes.FirstOrDefault(q => q.ID == fltId);
                var flightObj = context.FlightInformations.FirstOrDefault(q => q.ID == fltId);
                flightObj.ALT1 = "";
                flightObj.ALT2 = "";

                //var fnprts = (fn.Split('_')[1]).Split('.');
                //var _date = new DateTime(Convert.ToInt32(fnprts[0]), Convert.ToInt32(fnprts[1]), Convert.ToInt32(fnprts[2]));
                //var _fltno = (fnprts[5]).PadLeft(4, '0');
                // var _origin = fnprts[3];
                //var _destination = fnprts[4];


                //string folder = HttpContext.Current.Server.MapPath("~/upload");
                // string path = Path.Combine(folder, fn);

                //var ftext = File.ReadAllText(path);
                string ftext = Convert.ToString(dto.text);
                ftext = ftext.Replace("..............", "....");
                ftext = ftext.Replace(".............", "....");
                ftext = ftext.Replace("............", "....");
                ftext = ftext.Replace("...........", "....");
                ftext = ftext.Replace("........", "....");
                ftext = ftext.Replace(".......", "....");

                //   ftext = ftext.Replace("........", "^");
                //ftext = ftext.Replace(" .......", "........");
                // ftext = ftext.Replace("^", "........");
                ftext = ftext.Replace(".../.../...", "..../..../....");


                var lines = ftext.Split(new[] { '\r', '\n' }).ToList();
                var linesNoSpace = lines.Select(q => q.Replace(" ", "")).ToList();
                var linesTrimStart = lines.Select(q => q.TrimStart()).ToList();



                var cplan = context.OFPImports.FirstOrDefault(q => q.FlightId == fltId);
                if (cplan != null)
                    context.OFPImports.Remove(cplan);
                List<string> props = new List<string>();
                var plan = new OFPImport()
                {
                    DateCreate = DateTime.Now,
                    DateFlight = flight.STDDay,
                    FileName = "",
                    FlightNo = flight.FlightNumber,
                    Origin = flight.FromAirportICAO,
                    Destination = flight.ToAirportICAO,
                    User = user,
                    Text = ftext,


                };
                if (flight != null)
                    plan.FlightId = flight.ID;
                context.OFPImports.Add(plan);


                //var fuelParts = OFPHelper.GetFuelParts(lines, linesNoSpace);


                var propIndex = 1;
                var linesProccessed = new List<string>();
                //foreach (var ln in lines)
                for(int cnt=0;cnt<lines.Count;cnt++)
                {
                    var ln = lines[cnt];
                    if (ln.Replace(" ", "").ToUpper().Contains("CREW"))
                    {
                        var crewLine = " CREW  : ";
                        crewLine += "<span ng-click='propClick($event)' data-info='_null_' class='prop' id='prop_" + propIndex + "'>" + "" + "</span>";
                        crewLine += "/";
                        props.Add("prop_" + propIndex);
                        propIndex++;
                        crewLine += "<span ng-click='propClick($event)' data-info='_null_' class='prop' id='prop_" + propIndex + "'>" + "" + "</span>";
                        crewLine += "/";
                        props.Add("prop_" + propIndex);
                        propIndex++;
                        crewLine += "<span ng-click='propClick($event)' data-info='_null_' class='prop' id='prop_" + propIndex + "'>" + "" + "</span>";
                        props.Add("prop_" + propIndex);
                        propIndex++;

                        linesProccessed.Add(crewLine);
                        linesProccessed.Add("<br/>");

                        var paxLine = " PAX   : ";
                        paxLine += "<span ng-click='propClick($event)' data-info='_null_' class='prop noborder' id='prop_pax_adult'>" + "" + "</span>";
                        paxLine += "/";
                        props.Add("prop_pax_adult");
                        paxLine += "<span ng-click='propClick($event)' data-info='_null_' class='prop noborder' id='prop_pax_child'>" + "" + "</span>";
                        paxLine += "/";
                        props.Add("prop_pax_child");
                        paxLine += "<span ng-click='propClick($event)' data-info='_null_' class='prop noborder' id='prop_pax_infant'>" + "" + "</span>";
                        props.Add("prop_pax_infant");
                        linesProccessed.Add(paxLine);
                        linesProccessed.Add("<br/>");

                        var sobLine = " S.O.B : ";
                        sobLine += "<span ng-click='propClick($event)' data-info='_null_' class='prop' id='prop_" + propIndex + "'>" + "" + "</span>";
                        props.Add("prop_" + propIndex);
                        propIndex++;
                        linesProccessed.Add(sobLine);

                        ln = " ";


                    }
                    else
                    {
                        var alt1 = ln.Split(new string[] { "1ST ALT" }, StringSplitOptions.None).ToList();
                        if (alt1.Count() > 1)
                        {
                            if (!string.IsNullOrEmpty( alt1.Last().Replace(" ", "").Trim()))
                            {
                                var _alt1 = alt1.Last().Trim().Split(' ').ToList().FirstOrDefault();
                                flightObj.ALT1 = _alt1;
                            }
                            

                        }
                        var alt2 = ln.Split(new string[] { "2ND ALT" }, StringSplitOptions.None).ToList();
                        if (alt2.Count() > 1)
                        {
                            if (!string.IsNullOrEmpty(alt2.Last().Replace(" ", "").Trim()))
                            {
                                var _alt2 = alt2.Last().Trim().Split(' ').ToList().FirstOrDefault();
                                flightObj.ALT2 = _alt2;
                            }


                        }
                        var clr = ln.Split(new string[] { "ATC CLRNC:" }, StringSplitOptions.None).ToList();
                        if (clr.Count() > 1)
                        {
                            ln = clr[0] + "ATC CLRNC: " + "<span ng-click='propClick($event)' data-info='_null_' class='prop' id='prop_clearance_" + propIndex + "'>" + "" + "</span>";
                            props.Add("prop_clearance_" + propIndex);
                            propIndex++;
                        }
                        var offblk = ln.Split(new string[] { "OFF BLK" }, StringSplitOptions.None).ToList();
                        if (offblk.Count() > 1)
                        {
                            ln = offblk[0] + "OFF BLK : " + "<span ng-click='propClick($event)' data-info='_null_' class='prop noborder alignleft' id='prop_offblock"  + "'>" + "" + "</span>";
                            props.Add("prop_offblock");
                            //propIndex++;
                        }
                        var takeoff = ln.Split(new string[] { "TAKE OFF" }, StringSplitOptions.None).ToList();
                        if (!ln.Replace(" ", "").ToUpper().Contains("ALTN") && takeoff.Count() > 1)
                        {
                            ln = takeoff[0] + "TAKE OFF: " + "<span ng-click='propClick($event)' data-info='_null_' class='prop noborder alignleft' id='prop_takeoff" + "'>" + "" + "</span>";
                            props.Add("prop_takeoff");
                            //propIndex++;
                        }
                        var lnd = ln.Split(new string[] { "ON RUNWAY" }, StringSplitOptions.None).ToList();
                        if (lnd.Count() > 1)
                        {
                            ln = lnd[0] + "ON RUNWAY  : " + "<span ng-click='propClick($event)' data-info='_null_' class='prop noborder alignleft' id='prop_landing" + "'>" + "" + "</span>";
                            props.Add("prop_landing");
                            //propIndex++;
                        }
                        var onblock = ln.Split(new string[] { "ON BLK" }, StringSplitOptions.None).ToList();
                        if (!ln.Replace(" ", "").ToUpper().Contains("FUEL") && onblock.Count() > 1)
                        {
                            ln = onblock[0] + "ON BLK     : " + "<span ng-click='propClick($event)' data-info='_null_' class='prop noborder alignleft' id='prop_onblock" + "'>" + "" + "</span>";
                            props.Add("prop_onblock");
                            //propIndex++;
                        }
                        if (ln.Replace(" ", "").ToUpper().Contains("DISP"))
                        {
                            var picIndex = ln.IndexOf("PIC");
                            var dispIndex = ln.IndexOf("DISP");
                            var dispStr = ln.Substring(0, ln.Length - (picIndex + 3));
                            var _disps = dispStr.Replace(":", " ").Split(new string[] { "DISP" }, StringSplitOptions.None).ToList();
                            var dispatcher = _disps[1].Replace(" ", "");

                            var picStr = ln.Substring(picIndex);
                            var _pics = picStr.Replace(":", " ").Split(new string[] { "PIC" }, StringSplitOptions.None).ToList();
                            var pic = _pics[1].Replace(" ", "").Replace(".", ". ");


                            ln = "<div class='z5000 h70'> " + "<span id='sig_disp' data-info='_null_' class='sig'><span class='sig_name'>DISP : " + dispatcher + "</span><img id='sig_disp_img' class='sig_img' /></span>" + "          " + "<span id='sig_pic' data-info='_null_' class='sig left80'><span data-info='_null_' class='sig_name'>" + "PIC : " + pic + "</span><img id='sig_pic_img' class='sig_img' /></span></div>";

                        }

                        var sign = ln.Split(new string[] { "SIGNATURE  :" }, StringSplitOptions.None).ToList();
                        if (sign.Count() > 1)
                        {
                            //ln="<div> "+sign[0]+ "SIGNATURE : "+sign[1]+
                            ln = "";
                        }

                        //OFP WORKED By CAPT:
                        //var cpt = ln.Split(new string[] { "OFP WORKED By CAPT:" }, StringSplitOptions.None).ToList();
                        //if (clr.Count() > 1)
                        //{
                        //    ln = clr[0] + "OFP WORKED By CAPT: " + "<span ng-click='propClick($event)' class='prop' id='prop_" + propIndex + "'>" + "" + "</span>";
                        //    props.Add("prop_" + propIndex);
                        //    propIndex++;
                        //    ln += "      FO: " + "<span ng-click='propClick($event)' class='prop' id='prop_" + propIndex + "'>" + "" + "</span>";
                        //    props.Add("prop_" + propIndex);
                        //    propIndex++;
                        //}
                        //OFP WORKED
                        if (ln.Replace(" ", "").ToUpper().Contains("OFPWORKED"))
                        {

                            ln = " OFP WORKED By CAPT: " + "<span data-info='_null_' ng-click='propClick($event)' class='prop' id='prop_ofbcpt_" + propIndex + "'>" + "" + "</span>";
                            props.Add("prop_ofbcpt_" + propIndex);
                            propIndex++;
                            ln += "      FO: " + "<span data-info='_null_' ng-click='propClick($event)' class='prop' id='prop_ofbfo_" + propIndex + "'>" + "" + "</span>";
                            props.Add("prop_ofbfo_" + propIndex);
                            propIndex++;
                        }
                        if (ln.Replace(" ", "").ToUpper().Contains("ENRATIS1"))
                        {

                            //lines[cnt + 1] = "<span ng-click='propClick($event)' class='prop' id='prop_" + propIndex + "'>" + "" + "</span>";
                            ln = " ENR ATIS 1 : " + "<span data-info='_null_' ng-click='propClick($event)' class='prop' id='prop_clearance_" + propIndex + "'>" + "" + "</span>";
                            props.Add("prop_clearance_" + propIndex);
                            propIndex++;
                        }
                        if (ln.Replace(" ", "").ToUpper().Contains("ENRATIS2"))
                        {

                            //lines[cnt + 1] = "<span ng-click='propClick($event)' class='prop' id='prop_" + propIndex + "'>" + "" + "</span>";
                            ln = " ENR ATIS 2 : " + "<span data-info='_null_' ng-click='propClick($event)' class='prop' id='prop_clearance_" + propIndex + "'>" + "" + "</span>";
                            props.Add("prop_clearance_" + propIndex);
                            propIndex++;
                        }
                        if (ln.Replace(" ", "").ToUpper().Contains("ENRATIS3"))
                        {

                            //lines[cnt + 1] = "<span ng-click='propClick($event)' class='prop' id='prop_" + propIndex + "'>" + "" + "</span>";
                            ln = " ENR ATIS 3 : " + "<span data-info='_null_' ng-click='propClick($event)' class='prop' id='prop_clearance_" + propIndex + "'>" + "" + "</span>";
                            props.Add("prop_clearance_" + propIndex);
                            propIndex++;
                        }
                        if (ln.Replace(" ", "").ToUpper().Contains("ENRATIS4"))
                        {

                            //lines[cnt + 1] = "<span ng-click='propClick($event)' class='prop' id='prop_" + propIndex + "'>" + "" + "</span>";
                            ln = " ENR ATIS 4 : " + "<span data-info='_null_' ng-click='propClick($event)' class='prop' id='prop_" + propIndex + "'>" + "" + "</span>";
                            props.Add("prop_" + propIndex);
                            propIndex++;
                        }

                        //var crew = ln.Split(new string[] { "..../..../...." }, StringSplitOptions.None).ToList();
                        //if (crew.Count > 1)
                        //{

                        //    //CREW : .../.../...        PAX  : .../.../...       S.O.B: ...........
                        //    var cstr = "";
                        //    foreach (var q in crew)
                        //    {
                        //        if (q.ToLower().Contains("crew"))
                        //        {
                        //            cstr += q;
                        //            cstr += "<span ng-click='propClick($event)' class='prop' id='prop_" + propIndex + "'>" + "" + "</span>"; // 
                        //            cstr += "/";
                        //            props.Add("prop_" + propIndex);
                        //            propIndex++;

                        //            cstr += "<span ng-click='propClick($event)' class='prop' id='prop_" + propIndex + "'>" + "" + "</span>"; // 
                        //            cstr += "/";
                        //            props.Add("prop_" + propIndex);
                        //            propIndex++;

                        //            cstr += "<span ng-click='propClick($event)' class='prop' id='prop_" + propIndex + "'>" + "" + "</span>"; // 
                        //            props.Add("prop_" + propIndex);
                        //            propIndex++;
                        //        }
                        //        else if (q.ToLower().Contains("pax"))
                        //        {
                        //            cstr += q;
                        //            cstr += "<span ng-click='propClick($event)' class='prop' id='prop_" + propIndex + "'>" + "" + "</span>"; // 
                        //            cstr += "/";
                        //            props.Add("prop_" + propIndex);
                        //            propIndex++;

                        //            cstr += "<span ng-click='propClick($event)' class='prop' id='prop_" + propIndex + "'>" + "" + "</span>"; // 
                        //            cstr += "/";
                        //            props.Add("prop_" + propIndex);
                        //            propIndex++;

                        //            cstr += "<span ng-click='propClick($event)' class='prop' id='prop_" + propIndex + "'>" + "" + "</span>"; // 
                        //            props.Add("prop_" + propIndex);
                        //            propIndex++;
                        //        }
                        //        else
                        //            cstr += q;



                        //    }
                        //    ln = cstr;
                        //}


                        List<string> prts = new List<string>();
                        var dots = "";
                        bool dot8 = false;

                        var prts4 = ln.Split(new string[] { "...." }, StringSplitOptions.None).ToList();
                        if (prts4.Count > 1)
                        {
                            var chrs = ln.ToCharArray();
                            var str = "";
                            foreach (var x in prts4)
                            {
                                str += x;
                                if (x != prts4.Last())
                                {

                                    //if (!string.IsNullOrEmpty(x.Trim().Replace(" ", "")))
                                    //    str += "  ";
                                    str += "<span data-info='_null_' ng-click='propClick($event)' class='prop' id='prop_" + propIndex + "'>" + "" + "</span>"; // "@prop_" + propIndex;
                                    props.Add("prop_" + propIndex);
                                    propIndex++;
                                }

                            }
                            //linesProccessed.Add(str);
                            ln = str;
                        }

                        var prts8 = ln.Split(new string[] { "........" }, StringSplitOptions.None).ToList();
                        if (prts8.Count > 1)
                        {
                            var chrs = ln.ToCharArray();
                            var str = "";
                            foreach (var x in prts8)
                            {
                                str += x;
                                if (x != prts8.Last())
                                {

                                    //if (!string.IsNullOrEmpty(x.Trim().Replace(" ", "")))
                                    //    str += "  ";
                                    str += "<span data-info='_null_' ng-click='propClick($event)' class='prop' id='prop_" + propIndex + "'>" + "" + "</span>"; // "@prop_" + propIndex;
                                    props.Add("prop_" + propIndex);
                                    propIndex++;
                                }

                            }
                            //linesProccessed.Add(str);
                            ln = str;
                        }
                        var prts7 = ln.Split(new string[] { "......." }, StringSplitOptions.None).ToList();
                        if (prts7.Count > 1)
                        {
                            var chrs = ln.ToCharArray();
                            var str = "";
                            foreach (var x in prts7)
                            {
                                str += x;
                                if (x != prts7.Last())
                                {

                                    //if (!string.IsNullOrEmpty(x.Trim().Replace(" ", "")))
                                    //    str += "  ";
                                    str += "<span data-info='_null_' ng-click='propClick($event)' class='prop' id='prop_" + propIndex + "'>" + "" + "</span>"; // "@prop_" + propIndex;
                                    props.Add("prop_" + propIndex);
                                    propIndex++;
                                }

                            }
                            //linesProccessed.Add(str);
                            ln = str;
                        }


                        linesProccessed.Add(ln);
                    }
                   
                }

                 //                              062     0.35 0186 ....  .... 001902
                //var propIndex = 1;
                //for (int i = _indexS; i < _indexE; i++)
                //{
                //    var ln = linesProccessed[i];
                   
                //}
                var finalResult = new List<string>();
                //finalResult.Add("<pre>");
                var proOn = false;
                var _f = 0;
                for (int i = 0; i < linesProccessed.Count(); i++)
                {
                    
                    var pln = linesProccessed[i];
                    if (pln.ToUpper().Contains("SCHD DEP"))
                        proOn = true;
                    if (pln.ToUpper().Contains("SCHD ARR"))
                        proOn = false;
                   
                    if (proOn)
                    {
                        if (pln.Contains("-- -- -- --"))
                        {
                            _f++;
                            if (_f > 1)
                            {
                                var next = linesProccessed[i + 1];
                                var spanIndex = next.IndexOf("<span");
                                var str = next.Substring(0, spanIndex).Trim();
                                var prts = str.Split(' ').ToList();
                                var timeStr = prts[prts.Count() - 2].Split('.');
                                var mins = Convert.ToInt32(timeStr[0]) * 60 + Convert.ToInt32(timeStr[1]);
                                var infoIndex = next.IndexOf("_null_");
                                var fin = next.Substring(0, infoIndex) + "time_"+i+"_"+mins + next.Substring(infoIndex + 6);
                                linesProccessed[i + 1] = fin;

                            }
                        }
                       
                    }
                }
                foreach (var x in linesProccessed)
                //finalResult.Add("<div>" + /*Regex.Replace(x, @"\s+", "&nbsp;")*/ReplaceWhitespace(x, "&nbsp;") + " </div>") ;
                {
                    finalResult.Add(x);
                    plan.OFPImportItems.Add(new OFPImportItem()
                    {
                        Line = x
                    });
                }
                var _text = "<pre>" + string.Join("<br/>", finalResult) + "</pre>";
                plan.TextOutput = _text;
                var dtupd = DateTime.UtcNow.ToString("yyyyMMddHHmm");
                foreach (var p in props)
                {
                    plan.OFPImportProps.Add(new OFPImportProp()
                    {
                        DateUpdate = dtupd,
                        PropName = p,
                        PropValue = "",
                        User = user,

                    });
                }
                context.SaveChanges();
                //context.SaveChangesAsync();
                //finalResult.Add("</pre>");
                return Ok(_text);
            }
            catch (Exception ex)
            {
                return Ok("-1");

            }

        }

        public string ReplaceWhitespace(string input, string replace)
        {
            var chrs = input.ToCharArray();
            var str = "";
            foreach (var x in chrs)
            {
                if (Char.IsWhiteSpace(x))
                    str += replace;
                else
                    str += x;
            }
            return str;
        }
        public string CallWebMethod(string url, Dictionary<string, string> dicParameters)
        {
            try
            {
                byte[] requestData = this.CreateHttpRequestData(dicParameters);
                HttpWebRequest httpRequest = (HttpWebRequest)HttpWebRequest.Create(url);
                httpRequest.Method = "POST";
                httpRequest.KeepAlive = false;
                httpRequest.ContentType = "application/x-www-form-urlencoded";
                httpRequest.ContentLength = requestData.Length;
                httpRequest.Timeout = 30000;
                HttpWebResponse httpResponse = null;
                String response = String.Empty;

                httpRequest.GetRequestStream().Write(requestData, 0, requestData.Length);
                httpResponse = (HttpWebResponse)httpRequest.GetResponse();
                Stream baseStream = httpResponse.GetResponseStream();
                StreamReader responseStreamReader = new StreamReader(baseStream);
                response = responseStreamReader.ReadToEnd();
                responseStreamReader.Close();

                return response;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private byte[] CreateHttpRequestData(Dictionary<string, string> dic)
        {
            StringBuilder sbParameters = new StringBuilder();
            foreach (string param in dic.Keys)
            {
                sbParameters.Append(param);//key => parameter name
                sbParameters.Append('=');
                sbParameters.Append(dic[param]);//key value
                sbParameters.Append('&');
            }
            sbParameters.Remove(sbParameters.Length - 1, 1);

            UTF8Encoding encoding = new UTF8Encoding();

            return encoding.GetBytes(sbParameters.ToString());

        }

        //[Route("api/flight/status")]
        //[AcceptVerbs("POST")]
        /////<summary>
        /////Get Status Of Flight
        /////</summary>
        /////<remarks>


        /////</remarks>
        //public async Task<IHttpActionResult> PostFlightStatus(AuthInfo authInfo, string date, string no)
        //{
        //    try
        //    {
        //        if (!(authInfo.userName == "pnl.airpocket" && authInfo.password == "Pnl1234@z"))
        //            return BadRequest("Authentication Failed");

        //        no =no.PadLeft(4, '0');
        //        List<int> prts = new List<int>();
        //        try
        //        {
        //            prts = date.Split('-').Select(q => Convert.ToInt32(q)).ToList();
        //        }
        //        catch(Exception ex)
        //        {
        //            return BadRequest("Incorrect Date");
        //        }

        //        if (prts.Count != 3)
        //            return BadRequest("Incorrect Date");
        //        if (prts[0]<1300)
        //            return BadRequest("Incorrect Date (Year)");
        //        //if (prts[1] < 1 || prts[1]>12)
        //        //    return BadRequest("Incorrect Date (Month)");
        //        //if (prts[2] < 1 || prts[1] > 31)
        //        //    return BadRequest("Incorrect Date (Day)");

        //        System.Globalization.PersianCalendar pc = new System.Globalization.PersianCalendar();
        //        var gd = (pc.ToDateTime(prts[0], prts[1], prts[2], 0, 0, 0, 0)).Date;
        //        var context = new AirpocketAPI.Models.FLYEntities();
        //        var flight = await context.ExpFlights.Where(q => q.DepartureDay == gd && q.FlightNo == no).FirstOrDefaultAsync();
        //        if (flight == null)
        //            return NotFound();
        //        var delay = (((DateTime)flight.Departure) - ((DateTime)flight.STD)).TotalMinutes;
        //        if (delay < 0)
        //            delay = 0;
        //        var result = new { 
        //          flightId=flight.Id,
        //          flightNo=flight.FlightNo,
        //          date=flight.DepartureDay,
        //          departure=flight.DepartureLocal,
        //          arrival=flight.ArrivalLocal,
        //          departureUTC=flight.Departure,
        //          arrivalUTC=flight.Arrival,
        //          status=flight.FlightStatus,
        //          statusId=flight.FlightStatusId,
        //          origin=flight.Origin,
        //          destination=flight.Destination,
        //          aircraftType=flight.AircraftType,
        //          register=flight.Register,
        //          isDelayed=delay>0,
        //          delay

        //        };
        //        return Ok(result);
        //    }
        //    catch (Exception ex)
        //    {
        //        var msg = ex.Message;
        //        if (ex.InnerException != null)
        //            msg += "    Inner Exception:" + ex.InnerException.Message;
        //        return BadRequest(msg);
        //    }







        //}


    }



    public class AuthInfo
    {
        public string userName { get; set; }
        public string password { get; set; }
    }

    public class DSPReleaseViewModel
    {
        public int? FlightId { get; set; }
        public bool? ActualWXDSP { get; set; }
        public bool? ActualWXCPT { get; set; }
        public string ActualWXDSPRemark { get; set; }
        public string ActualWXCPTRemark { get; set; }
        public bool? WXForcastDSP { get; set; }
        public bool? WXForcastCPT { get; set; }
        public string WXForcastDSPRemark { get; set; }
        public string WXForcastCPTRemark { get; set; }
        public bool? SigxWXDSP { get; set; }
        public bool? SigxWXCPT { get; set; }
        public string SigxWXDSPRemark { get; set; }
        public string SigxWXCPTRemark { get; set; }
        public bool? WindChartDSP { get; set; }
        public bool? WindChartCPT { get; set; }
        public string WindChartDSPRemark { get; set; }
        public string WindChartCPTRemark { get; set; }
        public bool? NotamDSP { get; set; }
        public bool? NotamCPT { get; set; }
        public string NotamDSPRemark { get; set; }
        public string NotamCPTRemark { get; set; }
        public bool? ComputedFligthPlanDSP { get; set; }
        public bool? ComputedFligthPlanCPT { get; set; }
        public string ComputedFligthPlanDSPRemark { get; set; }
        public string ComputedFligthPlanCPTRemark { get; set; }
        public bool? ATCFlightPlanDSP { get; set; }
        public bool? ATCFlightPlanCPT { get; set; }
        public string ATCFlightPlanDSPRemark { get; set; }
        public string ATCFlightPlanCPTRemark { get; set; }
        public bool? PermissionsDSP { get; set; }
        public bool? PermissionsCPT { get; set; }
        public string PermissionsDSPRemark { get; set; }
        public string PermissionsCPTRemark { get; set; }
        public bool? JeppesenAirwayManualDSP { get; set; }
        public bool? JeppesenAirwayManualCPT { get; set; }
        public string JeppesenAirwayManualDSPRemark { get; set; }
        public string JeppesenAirwayManualCPTRemark { get; set; }
        public bool? MinFuelRequiredDSP { get; set; }
        public bool? MinFuelRequiredCPT { get; set; }
        public decimal? MinFuelRequiredCFP { get; set; }
        public decimal? MinFuelRequiredSFP { get; set; }
        public decimal? MinFuelRequiredPilotReq { get; set; }
        public bool? GeneralDeclarationDSP { get; set; }
        public bool? GeneralDeclarationCPT { get; set; }
        public string GeneralDeclarationDSPRemark { get; set; }
        public string GeneralDeclarationCPTRemark { get; set; }
        public bool? FlightReportDSP { get; set; }
        public bool? FlightReportCPT { get; set; }
        public string FlightReportDSPRemark { get; set; }
        public string FlightReportCPTRemark { get; set; }
        public bool? TOLndCardsDSP { get; set; }
        public bool? TOLndCardsCPT { get; set; }
        public string TOLndCardsDSPRemark { get; set; }
        public string TOLndCardsCPTRemark { get; set; }
        public bool? LoadSheetDSP { get; set; }
        public bool? LoadSheetCPT { get; set; }
        public string LoadSheetDSPRemark { get; set; }
        public string LoadSheetCPTRemark { get; set; }
        public bool? FlightSafetyReportDSP { get; set; }
        public bool? FlightSafetyReportCPT { get; set; }
        public string FlightSafetyReportDSPRemark { get; set; }
        public string FlightSafetyReportCPTRemark { get; set; }
        public bool? AVSECIncidentReportDSP { get; set; }
        public bool? AVSECIncidentReportCPT { get; set; }
        public string AVSECIncidentReportDSPRemark { get; set; }
        public string AVSECIncidentReportCPTRemark { get; set; }
        public bool? OperationEngineeringDSP { get; set; }
        public bool? OperationEngineeringCPT { get; set; }
        public string OperationEngineeringDSPRemark { get; set; }
        public string OperationEngineeringCPTRemark { get; set; }
        public bool? VoyageReportDSP { get; set; }
        public bool? VoyageReportCPT { get; set; }
        public string VoyageReportDSPRemark { get; set; }
        public string VoyageReportCPTRemark { get; set; }
        public bool? PIFDSP { get; set; }
        public bool? PIFCPT { get; set; }
        public string PIFDSPRemark { get; set; }
        public string PIFCPTRemark { get; set; }
        public bool? GoodDeclarationDSP { get; set; }
        public bool? GoodDeclarationCPT { get; set; }
        public string GoodDeclarationDSPRemark { get; set; }
        public string GoodDeclarationCPTRemark { get; set; }
        public bool? IPADDSP { get; set; }
        public bool? IPADCPT { get; set; }
        public string IPADDSPRemark { get; set; }
        public string IPADCPTRemark { get; set; }
        public DateTime? DateConfirmed { get; set; }
        public int? DispatcherId { get; set; }
        public int Id { get; set; }
        public string User { get; set; }
    }
}
