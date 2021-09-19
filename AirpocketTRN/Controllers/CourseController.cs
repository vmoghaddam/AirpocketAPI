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
using AirpocketTRN.Services;
using System.Web.Http.OData;
using AirpocketTRN.ViewModels;

namespace AirpocketTRN.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class CourseController : ApiController
    {
        CourseService courseService = null;

        public CourseController()
        {
            courseService = new CourseService();
        }

        [Route("api/course/types")]
        [AcceptVerbs("GET")]
        public async Task<IHttpActionResult> GetCourseTypes()
        {
            var result =await courseService.GetCourseTypes();
            
            return Ok(result);
        }
        //GetCourseTypeJobGroups
        [Route("api/course/type/groups/{cid}")]
        [AcceptVerbs("GET")]
        public async Task<IHttpActionResult> GetCourseTypeJobGroups(int cid)
        {
            var result = await courseService.GetCourseTypeJobGroups(cid);

            return Ok(result);
        }


        [Route("api/certificate/types")]
        [AcceptVerbs("GET")]
        public async Task<IHttpActionResult> GetCertificateTypes()
        {
            var result = await courseService.GetCertificateTypes();

            return Ok(result);
        }

        [Route("api/certificate/types/query")]
        [EnableQuery]
        // [Authorize]
        public IQueryable<CertificateType> GetPeople()
        {

            return courseService.GetCertificateTypesQuery();
        }

        [Route("api/course/types/query")]
        [EnableQuery]
        // [Authorize]
        public IQueryable<ViewCourseType> GetcourseTypesQuery()
        {

            return courseService.GetCourseTypesQuery();
        }



        [Route("api/course/query")]
        [EnableQuery]
        // [Authorize]
        public IQueryable<ViewCourseNew> GetCourseQuery()
        {

            return courseService.GetCourseQuery();
        }
        [Route("api/course/bytype/{tid}/{sid}")]
        [AcceptVerbs("GET")]
        public async Task<IHttpActionResult> GetCoursesByType(int tid,int sid)
        {
            var result = await courseService.GetCoursesByType(tid,sid);

            return Ok(result);
        }
        [Route("api/certificates/history/{pid}")]
        [AcceptVerbs("GET")]
        public async Task<IHttpActionResult> GetCertificatesHistory(int pid)
        {
            var result = await courseService.GetCertificatesHistory(pid);

            return Ok(result.Data);
        }
        [Route("api/courses/passed/history/{pid}")]
        [AcceptVerbs("GET")]
        public async Task<IHttpActionResult> GetCoursesPassedHistory(int pid)
        {
            var result = await courseService.GetCoursesPassedHistory(pid);

            return Ok(result.Data);
        }
        //GetCertificatesHistory
        [Route("api/course/{cid}")]
        [AcceptVerbs("GET")]
        public async Task<IHttpActionResult> GetCourse (int cid)
        {
            var result = await courseService.GetCourse (cid);

            return Ok(result);
        }
        [Route("api/course/view/{cid}")]
        [AcceptVerbs("GET")]
        public async Task<IHttpActionResult> GetCourseView(int cid)
        {
            var result = await courseService.GetCourseView(cid);

            return Ok(result);
        }

        [Route("api/course/view/object/{cid}")]
        [AcceptVerbs("GET")]
        public async Task<IHttpActionResult> GetCourseViewObject(int cid)
        {
            var result = await courseService.GetCourseViewObject(cid);

            return Ok(result);
        }

        [Route("api/course/sessions/{cid}")]
        [AcceptVerbs("GET")]
        public async Task<IHttpActionResult> GetCourseSessions(int cid)
        {
            var result = await courseService.GetCourseSessions(cid);

            return Ok(result);
        }
        //SyncSessionsToRoster
        [Route("api/course/sessions/sync")]
        [AcceptVerbs("POST")]
        public async Task<IHttpActionResult> PostCourseSessionssync(dynamic dto)
        {
            int cid = Convert.ToInt32(dto.id);
            var result = await courseService.SyncSessionsToRoster(cid);

            return Ok(result);
        }

        [Route("api/course/sessions/sync/get/{id}")]
        [AcceptVerbs("GET")]
        public async Task<IHttpActionResult> GetCourseSessionssync(int id)
        {
             
            var result = await courseService.SyncSessionsToRoster(id);

            return Ok(result);
        }
        //GetCoursePeopleAndSessions
        [Route("api/course/peoplesessions/{cid}")]
        [AcceptVerbs("GET")]
        public async Task<IHttpActionResult> GetCoursePeopleSessions(int cid)
        {
            var result = await courseService.GetCoursePeopleAndSessions(cid);

            return Ok(result);
        }

        //GetPersonCourses
        [Route("api/person/courses/{pid}")]
        [AcceptVerbs("GET")]
        public async Task<IHttpActionResult> GetPersonCourses(int pid)
        {
            var result = await courseService.GetPersonCourses(pid);

            return Ok(result);
        }

        [Route("api/course/people/{cid}")]
        [AcceptVerbs("GET")]
        public async Task<IHttpActionResult> GetCoursePeople(int cid)
        {
            var result = await courseService.GetCoursePeople(cid);

            return Ok(result);
        }

        [Route("api/course/types/save")]
        [AcceptVerbs("POST")]
        public async Task<IHttpActionResult> PostCourseType(ViewModels.CourseTypeViewModel dto)
        {
            var result = await courseService.SaveCourseType(dto);

            return Ok(result);
        }

        [Route("api/course/types/delete")]
        [AcceptVerbs("POST")]
        public async Task<IHttpActionResult> PostCourseTypeDelete(dynamic dto)
        {
            int id = Convert.ToInt32(dto.Id);
            var result = await courseService.DeleteCourseType(id);

            return Ok(result);
        }

        [Route("api/course/save")]
        [AcceptVerbs("POST")]
        public async Task<IHttpActionResult> PostCourse(ViewModels.CourseViewModel dto)
        {
            var result = await courseService.SaveCourse(dto);

            return Ok(result);
        }
        [Route("api/course/delete")]
        [AcceptVerbs("POST")]
        public async Task<IHttpActionResult> PostCourseDelete(dynamic dto)
        {
            int id = Convert.ToInt32(dto.Id);
            var result = await courseService.DeleteCourse(id);

            return Ok(result);
        }
        //DeleteCoursePeople
        [Route("api/course/people/delete")]
        [AcceptVerbs("POST")]
        public async Task<IHttpActionResult> PostDeleteCoursePeople(dynamic dto)
        {
            int pid = Convert.ToInt32(dto.pid);
            int cid = Convert.ToInt32(dto.cid);
            var result = await courseService.DeleteCoursePeople(pid,cid);

            return Ok(result);
        }

        [Route("api/certificate/save")]
        [AcceptVerbs("POST")]
        public async Task<IHttpActionResult> PostCertificate(ViewModels.CertificateViewModel dto)
        {
            var result = await courseService.SaveCertificate(dto);

            return Ok(result);
        }

        [Route("api/course/people/save")]
        [AcceptVerbs("POST")]
        public async Task<IHttpActionResult> PostCoursePeople(dynamic dto)
        {
            var result = await courseService.SaveCoursePeople(dto);

            return Ok(result);
        }

        //SaveCourseSessionPresence
        [Route("api/course/session/pres/save")]
        [AcceptVerbs("POST")]
        public async Task<IHttpActionResult> PostCourseSessionPresence(dynamic dto)
        {
            var result = await courseService.SaveCourseSessionPresence(dto);

            return Ok(result);
        }

        //UpdateCoursePeopleStatus(CoursePeopleStatusViewModel dto)
        [Route("api/course/people/status/save")]
        [AcceptVerbs("POST")]
        public async Task<IHttpActionResult> PostUpdateCoursePeopleStatus(CoursePeopleStatusViewModel dto)
        {
            var result = await courseService.UpdateCoursePeopleStatus(dto);

            return Ok(result);
        }

    }
}
