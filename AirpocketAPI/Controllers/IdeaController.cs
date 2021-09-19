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
using System.Web.Http.OData;

namespace AirpocketAPI.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class IdeaController : ApiController
    {
        private FLYEntities db = new FLYEntities();
        [EnableQuery]
        [Route("api/notifications/taining")]
        public IQueryable<ViewTrainingSMSHistory> GetViewTrainingSMSHistory()
        {
            return db.ViewTrainingSMSHistories;
        }

        //[Route("odata/notifications/sms/all/{un}")]
        //[EnableQuery]
        //// [Authorize]
        //public IQueryable<ViewCrewPickupSM> GetViewCrewSMS2(string un)
        //{
        //    try
        //    {
        //        un = un.Replace(".", "").ToLower();
        //        var types = new List<int?>() { 10010, 10011, 10012, 10013 };
        //        return unitOfWork.NotificationRepository.GetViewCrewPickupSMS().Where(q => !types.Contains(q.Type) && q.Sender.ToLower() == un);
        //        // return db.ViewAirports.AsNoTracking() ;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new HttpResponseException(HttpStatusCode.Unauthorized);
        //    }



        //}


        [Route("api/idea/sessions/{year}")]
        [AcceptVerbs("GET")]
        public IHttpActionResult GetIdeaSessions(string year)
        {
            string apiUrl = "http://fleet.caspianairlines.com/airpocketexternal/api/idea/alt/sessions/obj/"+year;
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

            var response = obj.d_envelope.d_body.response.result;
            var responseJson = JsonConvert.DeserializeObject<List<IdeaSessionX>>(response);
            return Ok(responseJson);
        }


        [Route("api/idea/sessions/_sync/{year}/{user}/{password}")]
        [AcceptVerbs("GET")]
        public IHttpActionResult _GetIdeaSessionsSync(string year,string user, string password)
        {
            if (user != "vahid")
                return BadRequest("Not Authenticated");
            if (password != "Chico1359")
                return BadRequest("Not Authenticated");

            string apiUrl = "http://fleet.caspianairlines.com/airpocketexternal/api/idea/alt/sessions/obj/"+year;
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

            var response = obj.d_envelope.d_body.response.result;
            var responseJson = JsonConvert.DeserializeObject<List<IdeaSessionX>>(response);

            var ideaIds = responseJson.Select(q => q.id).ToList();


            using (var context = new AirpocketAPI.Models.FLYEntities())
            {
                var deleted = context.IdeaSessions.Where(q => ideaIds.Contains(q.IdeaId)).ToList();
                var deletedIds = deleted.Select(q => q.Id).ToList();
                var deletedSessionItems = context.IdeaSessionItems.Where(q => deletedIds.Contains(q.SessionId)).ToList();
                var deletedFdpIds = deletedSessionItems.Select(q => q.FDPId).ToList();
                var deletedFdps = context.FDPs.Where(q => deletedFdpIds.Contains(q.Id)).ToList();

                context.IdeaSessions.RemoveRange(deleted);
                context.FDPs.RemoveRange(deletedFdps);

                foreach (var item in responseJson)
                {
                    var dbItem = new IdeaSession()
                    {
                        ClassID = item.ClassID,
                        CourseCode = item.courseCode,
                        CourseTitle = item.CourseTitle,
                        DateBegin = item.BeginDate2,
                        DateCreate = DateTime.Now,
                        DateEnd = item.EndDate2,
                        IdeaId = item.id,
                        NID = item.nid,
                        PID = "",
                        SessionsStr = item.sessions,

                    };
                    var dbItemTemp = new IdeaSessionTemp()
                    {
                        ClassID = item.ClassID,
                        CourseCode = item.courseCode,
                        CourseTitle = item.CourseTitle,
                        DateBegin = item.BeginDate2,
                        DateCreate = DateTime.Now,
                        DateEnd = item.EndDate2,
                        IdeaId = item.id,
                        NID = item.nid,
                        PID = "",
                        SessionsStr = item.sessions,

                    };
                    dbItem.IdeaSessionItems = new List<IdeaSessionItem>();
                    dbItemTemp.IdeaSessionItemTemps = new List<IdeaSessionItemTemp>();
                    foreach (var x in item.Sessions2)
                    {
                        if (x.DateFrom != null && x.DateTo != null)
                        {
                            dbItem.IdeaSessionItems.Add(new IdeaSessionItem()
                            {
                                DateFrom = x.DateFrom,
                                DateFromUtc = x.DateFromUtc,
                                DateTo = x.DateTo,
                                DateToUtc = x.DateToUtc,
                                Remark = x.Remark,
                            });
                            dbItemTemp.IdeaSessionItemTemps.Add(new IdeaSessionItemTemp()
                            {
                                DateFrom = x.DateFrom,
                                DateFromUtc = x.DateFromUtc,
                                DateTo = x.DateTo,
                                DateToUtc = x.DateToUtc,
                                Remark = x.Remark,
                            });
                        }
                    }
                    context.IdeaSessions.Add(dbItem);
                    context.IdeaSessionTemps.Add(dbItemTemp);

                }

                context.SaveChanges();
            }



            return Ok(responseJson);
        }


        [Route("api/idea/sessions/sync/{year}/{user}/{password}")]
        [AcceptVerbs("GET")]
        public IHttpActionResult GetIdeaSessionsSync(string year, string user, string password)
        {
            if (user != "vahid")
                return BadRequest("Not Authenticated");
            if (password != "Chico1359")
                return BadRequest("Not Authenticated");

            string apiUrl = "http://fleet.caspianairlines.com/airpocketexternal/api/idea/alt/sessions/obj/" + year;
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

            var response = obj.d_envelope.d_body.response.result;
            var responseJson = JsonConvert.DeserializeObject<List<IdeaSessionX>>(response);

            var ideaIds = responseJson.Select(q => q.id).ToList();
            var newItems = new List<IdeaSessionX>();



            using (var context = new AirpocketAPI.Models.FLYEntities())
            {
                var dbIdeaSessions = context.IdeaSessions.ToList();
                var dbIds = dbIdeaSessions.Select(q => q.IdeaId).ToList();
                var deletedItems = dbIdeaSessions.Where(q => !ideaIds.Contains(q.IdeaId)).ToList();
                
                 newItems = responseJson.Where(q => !dbIds.Contains(q.id)).ToList();
                var newItemsIds = newItems.Select(q => q.id).ToList();

                var collection = responseJson.Where(q => !newItemsIds.Contains(q.id)).ToList();
                foreach (var c in collection)
                {
                    var dbitem = dbIdeaSessions.FirstOrDefault(q => q.IdeaId == c.id);
                    if (dbitem!=null && dbitem.SessionsStr != c.sessions)
                    {
                        newItems.Add(c);
                        deletedItems.Add(dbitem);
                    }
                }



               // var deleted = context.IdeaSessions.Where(q => ideaIds.Contains(q.IdeaId)).ToList();
                var deletedIds = deletedItems.Select(q => q.Id).ToList();
                var deletedSessionItems = context.IdeaSessionItems.Where(q => deletedIds.Contains(q.SessionId)).ToList();

                var deletedFdpIds = deletedSessionItems.Select(q => q.FDPId).ToList();
                var deletedFdps = context.FDPs.Where(q => deletedFdpIds.Contains(q.Id)).ToList();

                context.IdeaSessions.RemoveRange(deletedItems);
                context.FDPs.RemoveRange(deletedFdps);

                foreach (var item in newItems)
                {
                    var dbItem = new IdeaSession()
                    {
                        ClassID = item.ClassID,
                        CourseCode = item.courseCode,
                        CourseTitle = item.CourseTitle,
                        DateBegin = item.BeginDate2,
                        DateCreate = DateTime.Now,
                        DateEnd = item.EndDate2,
                        IdeaId = item.id,
                        NID = item.nid,
                        PID = "",
                        SessionsStr = item.sessions,

                    };
                    //var dbItemTemp = new IdeaSessionTemp()
                    //{
                    //    ClassID = item.ClassID,
                    //    CourseCode = item.courseCode,
                    //    CourseTitle = item.CourseTitle,
                    //    DateBegin = item.BeginDate2,
                    //    DateCreate = DateTime.Now,
                    //    DateEnd = item.EndDate2,
                    //    IdeaId = item.id,
                    //    NID = item.nid,
                    //    PID = "",
                    //    SessionsStr = item.sessions,

                    //};
                    dbItem.IdeaSessionItems = new List<IdeaSessionItem>();
                    //dbItemTemp.IdeaSessionItemTemps = new List<IdeaSessionItemTemp>();
                    foreach (var x in item.Sessions2)
                    {
                        if (x.DateFrom != null && x.DateTo != null)
                        {
                            dbItem.IdeaSessionItems.Add(new IdeaSessionItem()
                            {
                                DateFrom = x.DateFrom,
                                DateFromUtc = x.DateFromUtc,
                                DateTo = x.DateTo,
                                DateToUtc = x.DateToUtc,
                                Remark = x.Remark,
                            });
                            //dbItemTemp.IdeaSessionItemTemps.Add(new IdeaSessionItemTemp()
                            //{
                            //    DateFrom = x.DateFrom,
                            //    DateFromUtc = x.DateFromUtc,
                            //    DateTo = x.DateTo,
                            //    DateToUtc = x.DateToUtc,
                            //    Remark = x.Remark,
                            //});
                        }
                    }
                    context.IdeaSessions.Add(dbItem);
                   // context.IdeaSessionTemps.Add(dbItemTemp);

                }

                context.SaveChanges();
            }


            
            return Ok(newItems);
        }



        public bool ValidateDuty(IdeaSessionItem sessionItem)
        {
            return true;
        }


        [Route("api/idea/sessions/duties/delete/{user}/{password}")]
        [AcceptVerbs("GET")]
        public IHttpActionResult GetDeleteSessionDuties(string user, string password)
        {
            if (user != "vahid")
                return BadRequest("Not Authenticated");
            if (password != "Chico1359")
                return BadRequest("Not Authenticated");

             


            using (var context = new AirpocketAPI.Models.FLYEntities())
            {
                
                var deletedFdpIds = context.IdeaSessionItems.Where(q=>q.FDPId!=null).Select(q=>q.FDPId).ToList();
                 
                var deletedFdps = context.FDPs.Where(q => deletedFdpIds.Contains(q.Id)).ToList();

                
                context.FDPs.RemoveRange(deletedFdps);

                 

                context.SaveChanges();
            }



            return Ok(true);
        }




        [Route("api/idea/airpocket/duties/update/{take}/{nid}/{user}/{password}")]
        [AcceptVerbs("GET")]
        public IHttpActionResult GetUpdateAirpocketTrainingDuties(int take,string nid,string user, string password)
        {
            if (user != "vahid")
                return BadRequest("Not Authenticated");
            if (password != "Chico1359")
                return BadRequest("Not Authenticated");

            using (var context = new AirpocketAPI.Models.FLYEntities())
            {
               // context.Configuration.AutoDetectChangesEnabled = false;
               // context.Configuration.ValidateOnSaveEnabled = false;
                context.Database.CommandTimeout = 180;
                var qry = from x in context.ViewIdeaSessionItemXes select x;
                if (nid != "-1")
                    qry = qry.Where(q => q.NID == nid);
                var notproccesseditems = qry.OrderBy(q => q.Id).Take(take).ToList();
                var npids = notproccesseditems.Select(q => q.Id).ToList();
                // var ideaSessionItems = context.IdeaSessionItems.Where(q => q.FDPId == null && q.Remark!="ERR").OrderBy(q=>q.Id).Skip(skip).Take(100).ToList();
                var ideaSessionItems = context.IdeaSessionItems.Where(q=>npids.Contains(q.Id)).OrderBy(q => q.Id).ToList();
                var _sessionIds = ideaSessionItems.Select(q => q.SessionId).ToList();
                var ideaSessions = context.IdeaSessions.Where(q => _sessionIds.Contains(q.Id)).ToList();
                var _peopleIds = ideaSessions.Select(q => q.NID).ToList();
                var people = context.ViewEmployeeLights.Where(q => _peopleIds.Contains(q.NID)).ToList();


                var fdps = new List<FDP>();
                foreach (var rec in ideaSessionItems)
                {
                    var session = ideaSessions.FirstOrDefault(q => q.Id == rec.SessionId);
                    if (session != null)
                    {
                        var employee = people.Where(q => q.NID == session.NID).FirstOrDefault();
                        if (employee != null)
                        {
                            var ofdp = (from x in context.ViewFDPIdeas.AsNoTracking()
                                        where x.CrewId == employee.Id && x.DutyType == 1165
                                        && (
                                          (rec.DateFromUtc >= x.DutyStart && rec.DateFrom <= x.RestUntil) || (rec.DateToUtc >= x.DutyStart && rec.DateToUtc <= x.RestUntil)
                                          || (x.DutyStart >= rec.DateFromUtc && x.DutyStart <= rec.DateToUtc) || (x.RestUntil >= rec.DateFromUtc && x.RestUntil <= rec.DateToUtc)
                                          )
                                        select x).FirstOrDefault();
                            if (ofdp == null)
                            {
                                var duty = new FDP();
                                duty.DateStart = rec.DateFromUtc;
                                duty.DateEnd = rec.DateToUtc;

                                duty.CrewId = employee.Id;
                                duty.DutyType = 5000;
                                duty.GUID = Guid.NewGuid();
                                duty.IsTemplate = false;
                                duty.Remark = session.CourseCode + "\r\n" + session.CourseTitle;
                                duty.UPD = 1;

                                duty.InitStart = duty.DateStart;
                                duty.InitEnd = duty.DateEnd;
                                var rest = new List<int>() { 1167, 1168, 1170, 5000, 5001, 100001, 100003 };
                                duty.InitRestTo = rest.Contains(duty.DutyType) ? ((DateTime)duty.InitEnd).AddHours(12) : duty.DateEnd;
                                rec.FDP = duty;
                                context.FDPs.Add(duty);
                                fdps.Add(duty);
                            }
                            else
                            {
                                rec.Remark = "ERR";
                                var log = new IdeaSessionUpdateError()
                                {
                                    FDPId = ofdp.Id,
                                    EmployeeId = employee.Id,
                                    SessionItemId = rec.Id,
                                    Name = employee.Name,
                                    Flights = ofdp.InitFlts,
                                    Route = ofdp.InitRoute,
                                   // DutyEnd = ofdp.DutyEndLocal,
                                    DutyStart = ofdp.DutyStart,
                                    RestUntil = ofdp.RestUntil,
                                    CourseCode = session.CourseCode,
                                    CourseTitle = session.CourseTitle,
                                    SessionDateFrom = rec.DateFrom,
                                    SessionDateTo = rec.DateTo,
                                    DateCreate=DateTime.Now

                                };
                                context.IdeaSessionUpdateErrors.Add(log);
                            }

                        }
                    }


                }


                 context.SaveChanges();
                return Ok(fdps.Select(q => q.Id).ToList());
            }



        }

        [Route("api/idea/airpocket/duties/update2/{user}/{password}")]
        [AcceptVerbs("GET")]
        public IHttpActionResult GetUpdateAirpocketTrainingDuties2(  string user, string password)
        {
            if (user != "vahid")
                return BadRequest("Not Authenticated");
            if (password != "Chico1359")
                return BadRequest("Not Authenticated");

            using (var context = new AirpocketAPI.Models.FLYEntities())
            {
                context.Database.CommandTimeout = 180;
                var ideaSessionItems = context.IdeaSessionItems.Where(q => q.FDPId == null).OrderBy(q => q.Id).ToList();
                var _sessionIds = ideaSessionItems.Select(q => q.SessionId).ToList();
                var ideaSessions = context.IdeaSessions.Where(q => _sessionIds.Contains(q.Id)).ToList();
                var _peopleIds = ideaSessions.Select(q => q.NID).ToList();
                var people = context.ViewEmployeeLights.Where(q => _peopleIds.Contains(q.NID)).ToList();


                var fdps = new List<FDP>();
                foreach (var rec in ideaSessionItems)
                {
                    var session = ideaSessions.FirstOrDefault(q => q.Id == rec.SessionId);
                    if (session != null)
                    {
                        var employee = people.Where(q => q.NID == session.NID).FirstOrDefault();
                        if (employee != null)
                        {
                            var ofdp = (from x in context.ViewFDPIdeas
                                        where x.CrewId == employee.Id && x.DutyType == 1165
                                        && (
                                          (rec.DateFromUtc >= x.DutyStart && rec.DateFrom <= x.RestUntil) || (rec.DateToUtc >= x.DutyStart && rec.DateToUtc <= x.RestUntil)
                                          || (x.DutyStart >= rec.DateFromUtc && x.DutyStart <= rec.DateToUtc) || (x.RestUntil >= rec.DateFromUtc && x.RestUntil <= rec.DateToUtc)
                                          )
                                        select x).FirstOrDefault();
                            if (ofdp == null)
                            {
                                var duty = new FDP();
                                duty.DateStart = rec.DateFromUtc;
                                duty.DateEnd = rec.DateToUtc;

                                duty.CrewId = employee.Id;
                                duty.DutyType = 5000;
                                duty.GUID = Guid.NewGuid();
                                duty.IsTemplate = false;
                                duty.Remark = session.CourseCode + "\r\n" + session.CourseTitle;
                                duty.UPD = 1;

                                duty.InitStart = duty.DateStart;
                                duty.InitEnd = duty.DateEnd;
                                var rest = new List<int>() { 1167, 1168, 1170, 5000, 5001, 100001, 100003 };
                                duty.InitRestTo = rest.Contains(duty.DutyType) ? ((DateTime)duty.InitEnd).AddHours(12) : duty.DateEnd;
                                rec.FDP = duty;
                                context.FDPs.Add(duty);
                                fdps.Add(duty);
                            }
                            else
                            {
                                var log = new IdeaSessionUpdateError()
                                {
                                    FDPId = ofdp.Id,
                                    EmployeeId = employee.Id,
                                    SessionItemId = rec.Id,
                                    Name = employee.Name,
                                    Flights = ofdp.InitFlts,
                                    Route = ofdp.InitRoute,
                                    // DutyEnd = ofdp.DutyEndLocal,
                                    DutyStart = ofdp.DutyStart,
                                    RestUntil = ofdp.RestUntil,
                                    CourseCode = session.CourseCode,
                                    CourseTitle = session.CourseTitle,
                                    SessionDateFrom = rec.DateFrom,
                                    SessionDateTo = rec.DateTo,
                                    DateCreate = DateTime.Now

                                };
                                context.IdeaSessionUpdateErrors.Add(log);
                            }

                        }
                    }


                }


                context.SaveChanges();
                return Ok(fdps.Select(q => q.Id).ToList());
            }



        }

        [Route("api/idea/session/update/errors/new")]
        [AcceptVerbs("GET")]
        public IHttpActionResult GetNewSessionUpdateErrors()
        {
            using (var context = new AirpocketAPI.Models.FLYEntities())
            {
                var recs = context.ViewIdeaSessionUpdateErrors.Where(q => q.IsVisited == 0).OrderByDescending(q => q.DateCreate).ToList();
                 
                    return Ok(recs);
                
            }

        }

        [Route("api/idea/session/update/errors")]
        [AcceptVerbs("GET")]
        public IHttpActionResult GetSessionUpdateErrors()
        {
            using (var context = new AirpocketAPI.Models.FLYEntities())
            {
                var recs = context.ViewIdeaSessionUpdateErrors.OrderBy(q=>q.IsVisited).ThenByDescending(q => q.DateCreate).ToList();

                return Ok(recs);

            }

        }

        [Route("api/training/duties/{crewid}/{grouped}")]
        [AcceptVerbs("GET")]
        public IHttpActionResult GetTrainingDuties(int crewid,int grouped)
        {
            using (var context = new AirpocketAPI.Models.FLYEntities())
            {
                var recs = context.ViewTrainingDuties.Where(q => q.CrewId == crewid).OrderByDescending(q => q.DateLocal).ToList();
                if (grouped == 0)
                    return Ok(recs);
                else
                {
                    //var grps = (from x in recs
                    //            group x by new { x.YearName, x.MonthName, x.Month, x.CrewId, x.Name, x.FirstName, x.LastName, x.JobGroup, x.JobGroupCode, x.IsCabin, x.IsCockpit  } into g
                    //            select new
                    //            {
                    //                Year = Convert.ToInt32(g.Key.YearName),
                    //                Month = Convert.ToInt32(g.Key.Month),
                    //                g.Key.MonthName,
                    //                g.Key.FirstName,
                    //                g.Key.LastName,
                    //                g.Key.Name,
                    //                g.Key.JobGroup,
                    //                g.Key.JobGroupCode,
                    //                g.Key.IsCockpit,
                    //                g.Key.IsCabin,
                    //                g.Key.CrewId,
                    //                Items = g.OrderByDescending(q => q.DateLocal).ThenByDescending(q => q.DutyStartLocal).ToList()
                    //            }).OrderByDescending(q => q.Year).ThenByDescending(q => q.Month).ToList();
                    var grps = (from x in recs
                                group x by new {  x.CrewId, x.Name, x.FirstName, x.LastName, x.JobGroup, x.JobGroupCode, x.IsCabin, x.IsCockpit } into g
                                select new
                                {
                                    //Year = Convert.ToInt32(g.Key.YearName),
                                   // Month = Convert.ToInt32(g.Key.Month),
                                   // g.Key.MonthName,
                                   // g.Key.FirstName,
                                    g.Key.LastName,
                                    g.Key.Name,
                                    g.Key.JobGroup,
                                    g.Key.JobGroupCode,
                                    g.Key.IsCockpit,
                                    g.Key.IsCabin,
                                    g.Key.CrewId,
                                    //Items = g.OrderByDescending(q => q.DateLocal).ThenByDescending(q => q.DutyStartLocal).ToList()
                                    rows=(from z in g 
                                           group z by new { z.YearName, z.MonthName, z.Month } into gi
                                           select new
                                           {
                                               Year = Convert.ToInt32(gi.Key.YearName),
                                               Month = Convert.ToInt32(gi.Key.Month),
                                               gi.Key.MonthName,
                                               duties=gi.OrderBy(q => q.DateLocal).ThenBy(q => q.DutyStartLocal).ToList()

                                           }
                                           ).OrderByDescending(q => q.Year).ThenByDescending(q => q.Month).ToList()
                                }).FirstOrDefault();
                    return Ok(grps);
                }

            }

        }

        [Route("api/crew/light")]
        [AcceptVerbs("GET")]
        public IHttpActionResult GetCrewsLight()
        {
            using (var context = new AirpocketAPI.Models.FLYEntities())
            {
                var recs = context.ViewEmployeeLights.Where(q => q.JobGroupCode.StartsWith("00101") || q.JobGroupCode.StartsWith("00102"))
                    .Select (q=> new { 
                     q.Id,
                     q.LastName,
                     q.FirstName,
                     q.Name,
                     q.JobGroup,
                     q.JobGroupCode,
                     q.PID,
                     q.NID,

                    })
                    .OrderBy(q=>q.LastName)
                    .ToList();

                return Ok(recs);
            }

        }

        [Route("api/idea/unique/{year}")]
        [AcceptVerbs("GET")]
        public IHttpActionResult GetIdeaUnique(string year)
        {
            string apiUrl = "http://fleet.caspianairlines.com/airpocketexternal/api/idea/alt/unique/obj/"+year;
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
            var obj = JsonConvert.DeserializeObject<IdeaResultUnique>(json);

            var response = obj.d_envelope.d_body.response.result;
            var responseJson = JsonConvert.DeserializeObject<List<IdeaUniqueX>>(response);
            return Ok(responseJson);
        }

        [Route("api/airpocket/unique/{grouped}")]
        [AcceptVerbs("GET")]
        public IHttpActionResult GetAirpocketUnique(int grouped)
        {
            using (var context = new AirpocketAPI.Models.FLYEntities())
            {
                var recs = context.ViewIdeaUniqueLasts.OrderBy(q => q.JobGroup).ThenBy(q => q.LastName).ToList();
                if (grouped == 0)
                    return Ok(recs);
                else
                {
                    var _grouped = (from x in recs
                                    group x by new { x.JobGroup, x.LastName, x.FirstName, x.NID } into grp
                                    select new
                                    {
                                        grp.Key.JobGroup,
                                        grp.Key.LastName,
                                        grp.Key.FirstName,
                                        grp.Key.NID,
                                        Items = grp.Select(q => new
                                        {
                                            q.IdeaId,
                                            q.CourseType,
                                            q.CourseCode,
                                            q.CourseTitle,
                                            q.DateBegin,
                                            q.DateEnd,
                                            q.DateExpire,
                                            q.DateCreate

                                        }).OrderBy(q => q.CourseType).ToList(),
                                    }).OrderBy(q => q.JobGroup).ThenBy(q => q.LastName).ToList();
                    return Ok(_grouped);
                }
            }

        }


        [Route("api/idea/unique/sync/{year}/{user}/{password}")]
        [AcceptVerbs("GET")]
        public IHttpActionResult GetIdeaUniqueSync(string year,string user, string password)
        {
            if (user != "vahid")
                return BadRequest("Not Authenticated");
            if (password != "Chico1359")
                return BadRequest("Not Authenticated");

            string apiUrl = "http://fleet.caspianairlines.com/airpocketexternal/api/idea/alt/unique/obj/"+year;
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
            var obj = JsonConvert.DeserializeObject<IdeaResultUnique>(json);

            var response = obj.d_envelope.d_body.response.result;
            var responseJson = JsonConvert.DeserializeObject<List<IdeaUniqueX>>(response);
            using (var context = new AirpocketAPI.Models.FLYEntities())
            {
                context.Configuration.AutoDetectChangesEnabled = false;
                context.Configuration.ValidateOnSaveEnabled = false;
                // context.IdeaUniques.RemoveRange(context.IdeaUniques);
                // context.SaveChanges();
                context.Database.ExecuteSqlCommand("TRUNCATE TABLE [IdeaUnique]");
            }

            using (var context = new AirpocketAPI.Models.FLYEntities())
            {
                context.Configuration.AutoDetectChangesEnabled = false;
                context.Configuration.ValidateOnSaveEnabled = false;
                //context.IdeaUniques.RemoveRange(context.IdeaUniques);
                foreach (var item in responseJson)
                {
                    var dbItem = new IdeaUnique()
                    {
                        ClassID = item.ClassID,
                        CourseCode = item.courseCode,
                        CourseTitle = item.CourseTitle,
                        DateBegin = item.BeginDate2,
                        DateCreate = DateTime.Now,
                        DateEnd = item.EndDate2,
                        IdeaId = item.ID,
                        NID = item.nid,
                        PID = "",
                        DateExpire = item.ExpireDate2

                    };

                    context.IdeaUniques.Add(dbItem);

                }

                context.SaveChanges();
            }



            return Ok(responseJson);
        }


        private AirpocketAPI.Models.FLYEntities AddToContext(AirpocketAPI.Models.FLYEntities context, IdeaUnique entity, int count, int commitCount, bool recreateContext)
        {
            context.Set<IdeaUnique>().Add(entity);

            if (count % commitCount == 0)
            {
                context.SaveChanges();
                if (recreateContext)
                {
                    context.Dispose();
                    context = new AirpocketAPI.Models.FLYEntities();
                    context.Configuration.AutoDetectChangesEnabled = false;
                }
            }

            return context;
        }


        //[Route("api/idea/unique/sync2/{user}/{password}")]
        //[AcceptVerbs("GET")]
        //public IHttpActionResult GetIdeaUniqueSync2(string user, string password)
        //{
        //    if (user != "vahid")
        //        return BadRequest("Not Authenticated");
        //    if (password != "Chico1359")
        //        return BadRequest("Not Authenticated");

        //    string apiUrl = "http://fleet.caspianairlines.com/airpocketexternal/api/idea/alt/unique/obj";
        //    var input = new
        //    {

        //    };
        //    string inputJson = JsonConvert.SerializeObject(input);
        //    WebClient client = new WebClient();
        //    client.Headers["Content-type"] = "application/json";
        //    client.Encoding = Encoding.UTF8;
        //    string json = client.DownloadString(apiUrl);
        //    json = json.Replace("?xml", "d_xml").Replace("soap:Envelope", "d_envelope").Replace("soap:Body", "d_body")
        //        .Replace("@xmlns:soap", "d_xmlns_soap").Replace("@xmlns:xsi", "d_xmlns_xsi").Replace("@xmlns:xsd", "d_xmlns_xsd")
        //        .Replace("@xmlns", "d_xmlns")
        //        .Replace("GetTotalDataJsonResponse", "response")
        //        .Replace("GetTotalDataJsonResult", "result");
        //    var obj = JsonConvert.DeserializeObject<IdeaResultUnique>(json);

        //    var response = obj.d_envelope.d_body.response.result;
        //    var responseJson = JsonConvert.DeserializeObject<List<IdeaUniqueX>>(response);

        //    using (var context = new AirpocketAPI.Models.FLYEntities())
        //    {
        //        context.IdeaUniques.RemoveRange(context.IdeaUniques);
        //        context.SaveChanges();
        //    }
        //    //using (var context = new AirpocketAPI.Models.FLYEntities())
        //    //{

        //    //    foreach (var item in responseJson)
        //    //    {
        //    //        var dbItem = new IdeaUnique()
        //    //        {
        //    //            ClassID = item.ClassID,
        //    //            CourseCode = item.courseCode,
        //    //            CourseTitle = item.CourseTitle,
        //    //            DateBegin = item.BeginDate2,
        //    //            DateCreate = DateTime.Now,
        //    //            DateEnd = item.EndDate2,
        //    //            IdeaId = item.ID,
        //    //            NID = item.nid,
        //    //            PID = "",
        //    //            DateExpire = item.ExpireDate2

        //    //        };

        //    //        context.IdeaUniques.Add(dbItem);

        //    //    }

        //    //    context.SaveChanges();
        //    //}
        //    using (TransactionScope scope = new TransactionScope())
        //    {
        //        AirpocketAPI.Models.FLYEntities context = null;
        //        try
        //        {
        //            context = new AirpocketAPI.Models.FLYEntities();
        //            context.Configuration.AutoDetectChangesEnabled = false;

        //            int count = 0;
        //            foreach (var item in responseJson)
        //            {
        //                ++count;
        //                var dbItem = new IdeaUnique()
        //                {
        //                    ClassID = item.ClassID,
        //                    CourseCode = item.courseCode,
        //                    CourseTitle = item.CourseTitle,
        //                    DateBegin = item.BeginDate2,
        //                    DateCreate = DateTime.Now,
        //                    DateEnd = item.EndDate2,
        //                    IdeaId = item.ID,
        //                    NID = item.nid,
        //                    PID = "",
        //                    DateExpire = item.ExpireDate2

        //                };

        //                AddToContext(context, dbItem, count, 1000, true);

        //            }


        //            context.SaveChanges();
        //        }
        //        finally
        //        {
        //            if (context != null)
        //                context.Dispose();
        //        }

        //        scope.Complete();
        //    }


        //    return Ok(responseJson);
        //}

        [Route("api/idea/airpocket/certificates/update/{user}/{password}")]
        [AcceptVerbs("GET")]
        public IHttpActionResult GetUpdateAirpocketCertificates(string user, string password)
        {
            if (user != "vahid")
                return BadRequest("Not Authenticated");
            if (password != "Chico1359")
                return BadRequest("Not Authenticated");

            using (var context = new AirpocketAPI.Models.FLYEntities())
            {
                context.Database.CommandTimeout = 180;
                var people = context.People.ToList();
                var ideaRecords = context.ViewIdeaUniqueLasts.ToList();
                foreach (var rec in ideaRecords)
                {
                    if (rec.CrewId != null)
                    {
                        if (rec.CourseType == "CRM")
                        {
                            var person = people.FirstOrDefault(q => q.NID == rec.NID);
                            if (person != null)
                            {
                                person.UpsetRecoveryTrainingIssueDate = rec.DateEnd;
                                person.UpsetRecoveryTrainingExpireDate = rec.DateExpire;
                            }
                        }
                        if (rec.CourseType == "CCRM")
                        {
                            var person = people.FirstOrDefault(q => q.NID == rec.NID);
                            if (person != null)
                            {
                                person.CCRMIssueDate = rec.DateEnd;
                                person.CCRMExpireDate = rec.DateExpire;
                            }
                        }
                        if (rec.CourseType == "SMS")
                        {
                            var person = people.FirstOrDefault(q => q.NID == rec.NID);
                            if (person != null)
                            {
                                person.SMSIssueDate = rec.DateEnd;
                                person.SMSExpireDate = rec.DateExpire;
                            }
                        }
                        if (rec.CourseType == "FIRSTAID")
                        {
                            var person = people.FirstOrDefault(q => q.NID == rec.NID);
                            if (person != null)
                            {
                                person.FirstAidIssueDate = rec.DateEnd;
                                person.FirstAidExpireDate = rec.DateExpire;
                            }
                        }

                        if (rec.CourseType == "HOT-WX")
                        {
                            var person = people.FirstOrDefault(q => q.NID == rec.NID);
                            if (person != null)
                            {
                                person.HotWeatherOperationIssueDate = rec.DateEnd;
                                person.HotWeatherOperationExpireDate = rec.DateExpire;
                            }
                        }

                        if (rec.CourseType == "COLD-WX")
                        {
                            var person = people.FirstOrDefault(q => q.NID == rec.NID);
                            if (person != null)
                            {
                                person.ColdWeatherOperationIssueDate = rec.DateEnd;
                                person.ColdWeatherOperationExpireDate = rec.DateExpire;
                            }
                        }


                        if (rec.CourseType == "SEPT")
                        {
                            var person = people.FirstOrDefault(q => q.NID == rec.NID);
                            if (person != null)
                            {
                                person.SEPTPIssueDate = rec.DateEnd;
                                person.SEPTPExpireDate = rec.DateExpire;
                            }
                        }
                        if (rec.CourseType == "SEPT-TEORY")
                        {
                            var person = people.FirstOrDefault(q => q.NID == rec.NID);
                            if (person != null)
                            {
                                person.SEPTIssueDate = rec.DateEnd;
                                person.SEPTExpireDate = rec.DateExpire;
                            }
                        }
                        if (rec.CourseType == "AVSEC")
                        {
                            var person = people.FirstOrDefault(q => q.NID == rec.NID);
                            if (person != null)
                            {
                                person.AviationSecurityIssueDate = rec.DateEnd;
                                person.AviationSecurityExpireDate = rec.DateExpire;
                            }
                        }
                        if (rec.CourseType == "DG")
                        {
                            var person = people.FirstOrDefault(q => q.NID == rec.NID);
                            if (person != null)
                            {
                                person.DangerousGoodsIssueDate = rec.DateEnd;
                                person.DangerousGoodsExpireDate = rec.DateExpire;
                            }
                        }


                        if (rec.CourseType == "RE-TRAIN")
                        {
                            var person = people.FirstOrDefault(q => q.NID == rec.NID);
                            if (person != null)
                            {
                               
                                person.RecurrentIssueDate =rec.DateBegin!=null?rec.DateBegin: rec.DateEnd;
                                person.RecurrentExpireDate = rec.DateExpire;

                                person.FirstAidIssueDate = rec.DateBegin != null ? rec.DateBegin : rec.DateEnd;
                                person.FirstAidExpireDate = rec.DateExpire;

                                person.UpsetRecoveryTrainingIssueDate = rec.DateBegin != null ? rec.DateBegin : rec.DateEnd;
                                person.UpsetRecoveryTrainingExpireDate = rec.DateExpire;

                                person.SEPTIssueDate = rec.DateBegin != null ? rec.DateBegin : rec.DateEnd;
                                person.SEPTExpireDate = rec.DateExpire;



                            }
                        }


                    }

                }

                var history = new ThirdPartySyncHistory()
                {
                    App = "IDEA",
                    DateSync = DateTime.Now,
                    Remark = ideaRecords.Count + " Records Proccessed.",
                };
                context.ThirdPartySyncHistories.Add(history);
                context.SaveChanges();
                return Ok(history);
            }



        }



        [Route("api/idea/all/{year}")]
        [AcceptVerbs("GET")]
        public IHttpActionResult GetIdeaAll(string year)
        {
            string apiUrl = "http://fleet.caspianairlines.com/airpocketexternal/api/idea/alt/all/obj/"+year;
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
            var obj = JsonConvert.DeserializeObject<IdeaResultAll>(json);

            var response = obj.d_envelope.d_body.response.result;
            var responseJson = JsonConvert.DeserializeObject<List<IdeaAllX>>(response);
            return Ok(responseJson);
        }
        [Route("api/idea/all/sync/{year}/{user}/{password}")]
        [AcceptVerbs("GET")]
        public IHttpActionResult GetIdeaAllSync(string year,string user, string password)
        {
            if (user != "vahid")
                return BadRequest("Not Authenticated");
            if (password != "Chico1359")
                return BadRequest("Not Authenticated");

            string apiUrl = "http://fleet.caspianairlines.com/airpocketexternal/api/idea/alt/all/obj/"+year;
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
            var obj = JsonConvert.DeserializeObject<IdeaResultAll>(json);

            var response = obj.d_envelope.d_body.response.result;
            var responseJson = JsonConvert.DeserializeObject<List<IdeaAllX>>(response);


            using (var context = new AirpocketAPI.Models.FLYEntities())
            {
                foreach (var item in responseJson)
                {
                    var dbItem = new IdeaAll()
                    {
                        ClassID = item.ClassID,
                        CourseCode = item.courseCode,
                        CourseTitle = item.CourseTitle,
                        DateBegin = item.BeginDate2,
                        DateCreate = DateTime.Now,
                        DateEnd = item.EndDate2,
                        IdeaId = item.id,
                        NID = item.nid,
                        PID = "",
                        DateExpire = item.ExpireDate2

                    };

                    context.IdeaAlls.Add(dbItem);

                }

                context.SaveChanges();
            }



            return Ok(responseJson);
        }






    }
}
