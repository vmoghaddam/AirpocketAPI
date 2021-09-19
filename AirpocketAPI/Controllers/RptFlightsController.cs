using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.ModelBinding;
using System.Web.Http.OData;
using System.Web.Http.OData.Routing;
using AirpocketAPI.Models;

namespace AirpocketAPI.Controllers
{
    /*
    The WebApiConfig class may require additional changes to add a route for this controller. Merge these statements into the Register method of the WebApiConfig class as applicable. Note that OData URLs are case sensitive.

    using System.Web.Http.OData.Builder;
    using System.Web.Http.OData.Extensions;
    using AirpocketAPI.Models;
    ODataConventionModelBuilder builder = new ODataConventionModelBuilder();
    builder.EntitySet<RptFlight>("RptFlights");
    config.Routes.MapODataServiceRoute("odata", "odata", builder.GetEdmModel());
    */
    public class RptFlightsController : ODataController
    {
        private FLYEntities db = new FLYEntities();

        [Route("odata/flights2")]

        // [Authorize]
        [EnableQuery]
        public IQueryable<RptFlight> GetFlights2()
        {
            //var context = new AirpocketAPI.Models.FLYEntities();
            var query = from x in db.RptFlights
                        orderby x.STDDay
                        select x;
            return query.AsQueryable();


        }

        [Route("odata/app/crew/flights")]

        // [Authorize]
        [EnableQuery]
        public IQueryable<AppCrewFlight> GetAppCrewFlights()
        {
            //var context = new AirpocketAPI.Models.FLYEntities();
            var query = from x in db.AppCrewFlights
                        
                        select x;
            return query.AsQueryable();


        }

        [Route("odata/app/crew/flights/fdp/query/{fdp}")]
        [AcceptVerbs("GET")]
        [EnableQuery]
        public IQueryable<AppCrewFlight> GetAppCrewFlightsByFDPQuery(int fdp)
        {

            var query = from x in db.AppCrewFlights
                        where x.FDPId==fdp
                        orderby x.STD
                        select x;
            
            return query.AsQueryable();
        }

        [Route("odata/app/crew/flights/fdp/{fdp}")]
        [AcceptVerbs("GET")]
        public IHttpActionResult GetAppCrewFlightsByFDP(int fdp)
        {

            var query = from x in db.AppCrewFlights
                        where x.FDPId == fdp
                        orderby x.STD
                        select x;
            var result = query.ToList();
            return Ok(result);
        }

        // GET: odata/RptFlights

        [EnableQuery]
        public IQueryable<RptFlight> GetRptFlights()
        {
            return db.RptFlights;
        }

        // GET: odata/RptFlights(5)
        [EnableQuery]
        public SingleResult<RptFlight> GetRptFlight([FromODataUri] int key)
        {
            return SingleResult.Create(db.RptFlights.Where(rptFlight => rptFlight.ID == key));
        }

       

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool RptFlightExists(int key)
        {
            return db.RptFlights.Count(e => e.ID == key) > 0;
        }
    }
}
