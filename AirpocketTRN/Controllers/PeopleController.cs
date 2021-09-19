using AirpocketTRN.Models;
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
using System.Web.Http.OData;
using AirpocketTRN.Services;

namespace AirpocketTRN.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class PeopleController : ApiController
    {
        PeopleService peopleService = null;

        public PeopleController()
        {
            peopleService = new PeopleService();
        }

        [Route("api/people/query")]
        [EnableQuery]
        // [Authorize]
        public IQueryable<Person> GetPeople()
        {

            return peopleService.GetPeople();
        }

        [Route("api/employees/abs/query")]
        [EnableQuery]
        // [Authorize]
        public IQueryable<ViewEmployeeAb> GetViewEmployeesAbs()
        {

            return peopleService.GetViewEmployeesAbs();
        }

        [Route("api/employee/groups/query/{groups}")]
        [EnableQuery]
        // [Authorize]
        public IQueryable<ViewEmployeeAb> GetEmployees(string groups)
        {

            return peopleService.GetEmployeeByGroups(groups);
        }
        [Route("api/employee/groups/query")]
        [EnableQuery]
        // [Authorize]
        public IQueryable<ViewEmployeeAb> GetEmployees2()
        {

            return peopleService.GetEmployeeByGroups("");
        }


        [Route("api/people/{id}")]
        [EnableQuery]
        // [Authorize]
        public async Task<IHttpActionResult> GetPeopleById(int id)
        {
            var person = await peopleService.GetPeopleById(id);
            return Ok(person);
        }



    }
}
