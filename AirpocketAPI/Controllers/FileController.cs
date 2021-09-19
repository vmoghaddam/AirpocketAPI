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
    public class FileController : ApiController
    {

        [Route("api/file/import/flights")]
        public async Task<IHttpActionResult> ImportFlights()
        {
            return Ok(true);
        }


    }
}
