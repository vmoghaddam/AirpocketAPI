using AirpocketTRN.Models;
using AirpocketTRN.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace AirpocketTRN.Services
{
    public interface ICourseService
    {

    }
    public class CourseService : ICourseService
    {
        FLYEntities context = null;
        public CourseService()
        {
            context = new FLYEntities();
            context.Configuration.LazyLoadingEnabled = false;
        }
        public async Task<DataResponse> GetCourseTypes()
        {
            var result = await context.ViewCourseTypes.OrderBy(q => q.Title).ToListAsync();

            return new DataResponse()
            {
                Data = result,
                IsSuccess = true,
            };
        }

        public async Task<DataResponse> GetCourseTypeJobGroups(int cid)
        {
            var result = await context.ViewCourseTypeJobGroups.Where(q => q.CourseTypeId == cid).OrderBy(q => q.Title).ToListAsync();

            return new DataResponse()
            {
                Data = result,
                IsSuccess = true,
            };
        }

        public async Task<DataResponse> GetCertificateTypes()
        {
            var result = await context.CertificateTypes.OrderBy(q => q.Title).ToListAsync();

            return new DataResponse()
            {
                Data = result,
                IsSuccess = true,
            };
        }


        public async Task<DataResponse> GetCoursesByType(int tid, int sid)
        {
            var query = context.ViewCourseNews.Where(q => q.CourseTypeId == tid);
            if (sid != -1)
                query = query.Where(q => q.StatusId == sid);
            var result = await query.OrderBy(q => q.CourseType).ThenByDescending(q => q.DateStart).ToListAsync();

            return new DataResponse()
            {
                Data = result,
                IsSuccess = true,
            };
        }

        public async Task<DataResponse> GetPersonCourses(int pid)
        {
            var result = await context.ViewCoursePeopleRankeds.Where(q => q.PersonId == pid).OrderByDescending(q => q.DateStart).ToListAsync();

            return new DataResponse()
            {
                Data = result,
                IsSuccess = true,
            };
        }

        public async Task<DataResponse> GetCoursePeople(int cid)
        {
            var result = await context.ViewCoursePeoples.OrderBy(q => q.CourseId == cid).ToListAsync();

            return new DataResponse()
            {
                Data = result,
                IsSuccess = true,
            };
        }

        public async Task<DataResponse> GetCourseView(int cid)
        {
            var result = await context.ViewCourseNews.Where(q => q.Id == cid).FirstOrDefaultAsync();


            return new DataResponse()
            {
                Data = result,
                IsSuccess = true,
            };
        }

        public async Task<DataResponse> GetCourseViewObject(int cid)
        {
            var course = await context.ViewCourseNews.Where(q => q.Id == cid).FirstOrDefaultAsync();
            var sessions = await context.CourseSessions.Where(q => q.CourseId == cid).OrderBy(q => q.DateStart).ToListAsync();

            return new DataResponse()
            {
                Data = new
                {
                    course,
                    sessions

                },
                IsSuccess = true,
            };
        }

        public async Task<DataResponse> GetCourse(int cid)
        {
            var result = await context.Courses.Where(q => q.Id == cid).FirstOrDefaultAsync();

            return new DataResponse()
            {
                Data = result,
                IsSuccess = true,
            };
        }
        public async Task<DataResponse> GetCertificatesHistory(int pid)
        {
            var result = await context.ViewCoursePeoplePassedRankeds.Where(q => q.PersonId == pid && q.RankLast == 1).OrderBy(q => q.DateExpire).ToListAsync();

            return new DataResponse()
            {
                Data = result,
                IsSuccess = true,
            };
        }

        public async Task<DataResponse> GetCoursesPassedHistory(int pid)
        {
            var result = await context.ViewCoursePeopleRankeds.Where(q => q.PersonId == pid && q.CoursePeopleStatusId == 1).OrderBy(q => q.CertificateType).ThenBy(q => q.DateStart).ToListAsync();

            return new DataResponse()
            {
                Data = result,
                IsSuccess = true,
            };
        }
        public async Task<DataResponse> DeleteCourseType(int id)
        {
            var view = await context.ViewCourseTypes.Where(q => q.Id == id).FirstOrDefaultAsync();
            if (view.CoursesCount > 0)
            {
                return new DataResponse()
                {
                    Data = null,
                    IsSuccess = false,
                    Errors = new List<string>() { "Please remove related course(s)." }
                };
            }
            var obj = await context.CourseTypes.Where(q => q.Id == id).FirstOrDefaultAsync();
            context.CourseTypes.Remove(obj);
            var saveResult = await context.SaveAsync();
            if (!saveResult.IsSuccess)
                return saveResult;

            return new DataResponse()
            {
                IsSuccess = true,
                Data = obj,
            };
        }

        public async Task<DataResponse> DeleteCourse(int id)
        {
            var view = await context.CoursePeoples.Where(q => q.CourseId == id).FirstOrDefaultAsync();
            if (view != null)
            {
                return new DataResponse()
                {
                    Data = null,
                    IsSuccess = false,
                    Errors = new List<string>() { "Please remove related People." }
                };
            }
            var obj = await context.Courses.Where(q => q.Id == id).FirstOrDefaultAsync();
            context.Courses.Remove(obj);
            var saveResult = await context.SaveAsync();
            if (!saveResult.IsSuccess)
                return saveResult;

            return new DataResponse()
            {
                IsSuccess = true,
                Data = obj,
            };
        }

        public async Task<DataResponse> DeleteCoursePeople(int pid, int cid)
        {

            var obj = await context.CoursePeoples.Where(q => q.CourseId == cid && q.PersonId == pid).FirstOrDefaultAsync();
            var employee = await context.ViewEmployeeAbs.Where(q => q.PersonId == pid).Select(q => q.Id).FirstOrDefaultAsync();
            context.CoursePeoples.Remove(obj);
            var sessionFdps = await context.CourseSessionFDPs.Where(q => q.CourseId == cid && q.EmployeeId == employee).Select(q => q.FDPId).ToListAsync();
            var fdps = await context.FDPs.Where(q => sessionFdps.Contains(q.Id)).ToListAsync();
            context.FDPs.RemoveRange(fdps);
                
            var saveResult = await context.SaveAsync();
            if (!saveResult.IsSuccess)
                return saveResult;

            return new DataResponse()
            {
                IsSuccess = true,
                Data = obj,
            };
        }

        public async Task<DataResponse> SaveCourseType(ViewModels.CourseTypeViewModel dto)
        {
            CourseType entity = null;

            if (dto.Id == -1)
            {
                entity = new CourseType();
                context.CourseTypes.Add(entity);
            }

            else
            {
                entity = await context.CourseTypes.FirstOrDefaultAsync(q => q.Id == dto.Id);

            }

            if (entity == null)
                return new DataResponse()
                {
                    Data = dto,
                    IsSuccess = false,
                    Errors = new List<string>() { "entity not found" }
                };

            //ViewModels.Location.Fill(entity, dto);
            ViewModels.CourseTypeViewModel.Fill(entity, dto);

            if (dto.Id != -1)
            {
                var djgs = await context.CourseTypeJobGroups.Where(q => q.CourseTypeId == entity.Id).ToListAsync();
                context.CourseTypeJobGroups.RemoveRange(djgs);
            }

            var jgsIds = dto.JobGroups.Select(q => q.Id);
            var jgs = await context.JobGroups.Where(q => jgsIds.Contains(q.Id)).ToListAsync();
            foreach (var x in jgs)
            {
                entity.CourseTypeJobGroups.Add(new CourseTypeJobGroup()
                {
                    JobGroupId = x.Id,
                });
            }

            await context.SaveChangesAsync();
            dto.Id = entity.Id;
            return new DataResponse()
            {
                IsSuccess = true,
                Data = dto,
            };
        }


        public async Task<DataResponse> SaveCourse(ViewModels.CourseViewModel dto)
        {
            Course entity = null;

            if (dto.Id == -1)
            {
                entity = new Course();
                context.Courses.Add(entity);
            }

            else
            {
                entity = await context.Courses.FirstOrDefaultAsync(q => q.Id == dto.Id);

            }

            if (entity == null)
                return new DataResponse()
                {
                    Data = dto,
                    IsSuccess = false,
                    Errors = new List<string>() { "entity not found" }
                };

            //ViewModels.Location.Fill(entity, dto);
            entity.CourseTypeId = dto.CourseTypeId;
            entity.DateStart = (DateTime)DateObject.ConvertToDate(dto.DateStart).Date;
            entity.DateEnd = (DateTime)DateObject.ConvertToDate(dto.DateEnd).Date;
            entity.Instructor = dto.Instructor;
            entity.Location = dto.Location;
            entity.OrganizationId = dto.OrganizationId;
            entity.Duration = dto.Duration;
            entity.DurationUnitId = dto.DurationUnitId;
            entity.Remark = dto.Remark;
            entity.TrainingDirector = dto.TrainingDirector;
            entity.Title = dto.Title;
            entity.Recurrent = dto.Recurrent;
            entity.Interval = dto.Interval;
            entity.CalanderTypeId = dto.CalanderTypeId;
            entity.IsGeneral = dto.IsGeneral;
            entity.CustomerId = dto.CustomerId;
            entity.No = dto.No;
            entity.IsNotificationEnabled = dto.IsNotificationEnabled;
            entity.StatusId = dto.StatusId;
            if (dto.Id == -1)
            {
                if (dto.Sessions.Count > 0)
                {
                    foreach (var s in dto.Sessions)
                    {
                        var dtobj = DateObject.ConvertToDateTimeSession(s);
                        entity.CourseSessions.Add(new CourseSession()
                        {
                            Done = false,
                            Key = s,
                            DateStart = dtobj[0].Date,
                            DateStartUtc = dtobj[0].DateUtc,
                            DateEnd = dtobj[1].Date,
                            DateEndUtc = dtobj[1].DateUtc,
                        });
                    }
                }
            }
            else
            {
                var _sessions = await context.CourseSessions.Where(q => q.CourseId == dto.Id).ToListAsync();
                var _sessionKeys = _sessions.Select(q => q.Key).ToList();


                var _deleted = _sessions.Where(q => !dto.Sessions.Contains(q.Key)).ToList();
                var _deletedKeys = _deleted.Select(q => q.Key).ToList();
                
                var sessionFdps = await context.CourseSessionFDPs.Where(q => q.CourseId == dto.Id && _deletedKeys.Contains(q.SessionKey)).ToListAsync();
                var fdpIds = sessionFdps.Select(q => q.FDPId).ToList();
                var fdps = await context.FDPs.Where(q => fdpIds.Contains(q.Id)).ToListAsync();
                context.FDPs.RemoveRange(fdps);


                context.CourseSessions.RemoveRange(_deleted);

                var _newSessions = dto.Sessions.Where(q => !_sessionKeys.Contains(q)).ToList();
                foreach (var s in _newSessions)
                {
                    var dtobj = DateObject.ConvertToDateTimeSession(s);
                    entity.CourseSessions.Add(new CourseSession()
                    {
                        Done = false,
                        Key = s,
                        DateStart = dtobj[0].Date,
                        DateStartUtc = dtobj[0].DateUtc,
                        DateEnd = dtobj[1].Date,
                        DateEndUtc = dtobj[1].DateUtc,
                    });
                }
            }

            await context.SaveChangesAsync();
            dto.Id = entity.Id;
            return new DataResponse()
            {
                IsSuccess = true,
                Data = dto,
            };
        }

        public async Task<DataResponse> SaveCertificate(ViewModels.CertificateViewModel dto)
        {
            var _dateStart = (DateTime)DateObject.ConvertToDate(dto.DateStart).Date;
            var _dateEnd = (DateTime)DateObject.ConvertToDate(dto.DateEnd).Date;

            Course entity = await context.Courses.Where(q => q.DateStart == _dateStart && q.DateEnd == _dateEnd && q.CourseTypeId == dto.CourseTypeId
            && q.OrganizationId == dto.OrganizationId).FirstOrDefaultAsync();

            if (entity == null)
            {
                entity = new Course();
                context.Courses.Add(entity);
                entity.CourseTypeId = dto.CourseTypeId;
                entity.DateStart = (DateTime)DateObject.ConvertToDate(dto.DateStart).Date;
                entity.DateEnd = (DateTime)DateObject.ConvertToDate(dto.DateEnd).Date;
                entity.Instructor = dto.Instructor;
                entity.Location = dto.Location;
                entity.OrganizationId = dto.OrganizationId;
                entity.Duration = dto.Duration;
                entity.DurationUnitId = dto.DurationUnitId;
                entity.Remark = dto.Remark;
                entity.TrainingDirector = dto.TrainingDirector;
                entity.Title = dto.Title;
                entity.Recurrent = dto.Recurrent;
                entity.Interval = dto.Interval;
                entity.CalanderTypeId = dto.CalanderTypeId;
                entity.IsGeneral = dto.IsGeneral;
                entity.CustomerId = dto.CustomerId;
                entity.No = dto.No;
                entity.IsNotificationEnabled = dto.IsNotificationEnabled;
                entity.StatusId = 3;
            }
            if (dto.PersonId != null)
            {
                var cp = await context.CoursePeoples.Where(q => q.PersonId == dto.PersonId && q.CourseId == entity.Id).FirstOrDefaultAsync();
                if (cp == null)
                {
                    cp = new CoursePeople()
                    {
                        PersonId = dto.PersonId,
                        StatusId = 1,
                        DateStatus = DateTime.Now,
                        DateExpire = (DateTime)DateObject.ConvertToDate(dto.DateExpire).Date,
                        DateIssue = (DateTime)DateObject.ConvertToDate(dto.DateIssue).Date,
                        CertificateNo = dto.CertificateNo,
                    };
                    entity.CoursePeoples.Add(cp);
                }
                else
                {
                    cp.DateExpire = (DateTime)DateObject.ConvertToDate(dto.DateExpire).Date;
                    cp.DateIssue = (DateTime)DateObject.ConvertToDate(dto.DateIssue).Date;
                    cp.CertificateNo = dto.CertificateNo;
                    cp.StatusId = 1;
                }
                //////////////////////////
                var person = await context.People.Where(q => q.Id == cp.PersonId).FirstOrDefaultAsync();
                var ct = await context.CourseTypes.Where(q => q.Id == dto.CourseTypeId).FirstOrDefaultAsync();
                if (ct != null)
                {
                    switch (ct.CertificateTypeId)
                    {
                        //dg
                        case 3:
                            if ((DateTime)cp.DateExpire > person.DangerousGoodsExpireDate)
                            {
                                person.DangerousGoodsExpireDate = cp.DateExpire;
                                person.DangerousGoodsIssueDate = cp.DateIssue;
                            }
                            break;
                        //1	SEPT-P
                        case 1:
                            if ((DateTime)cp.DateExpire > person.SEPTPExpireDate)
                            {
                                person.SEPTPExpireDate = cp.DateExpire;
                                person.SEPTPIssueDate = cp.DateIssue;
                            }
                            break;
                        //2   SEPT - T
                        case 2:
                            if ((DateTime)cp.DateExpire > person.SEPTExpireDate)
                            {
                                person.SEPTExpireDate = cp.DateExpire;
                                person.SEPTIssueDate = cp.DateIssue;
                            }
                            break;
                        //4	CRM
                        case 4:
                            if ((DateTime)cp.DateExpire > person.UpsetRecoveryTrainingExpireDate)
                            {
                                person.UpsetRecoveryTrainingExpireDate = cp.DateExpire;
                                person.UpsetRecoveryTrainingIssueDate = cp.DateIssue;
                            }
                            break;
                        //5	CCRM
                        case 5:
                            if ((DateTime)cp.DateExpire > person.CCRMExpireDate)
                            {
                                person.CCRMExpireDate = cp.DateExpire;
                                person.CCRMIssueDate = cp.DateIssue;
                            }
                            break;
                        //6	SMS
                        case 6:
                            if ((DateTime)cp.DateExpire > person.SMSExpireDate)
                            {
                                person.SMSExpireDate = cp.DateExpire;
                                person.SMSIssueDate = cp.DateIssue;
                            }
                            break;
                        //7	AV-SEC
                        case 7:
                            if ((DateTime)cp.DateExpire > person.AviationSecurityExpireDate)
                            {
                                person.AviationSecurityExpireDate = cp.DateExpire;
                                person.AviationSecurityIssueDate = cp.DateIssue;
                            }
                            break;
                        //8	COLD-WX
                        case 8:
                            if ((DateTime)cp.DateExpire > person.ColdWeatherOperationExpireDate)
                            {
                                person.ColdWeatherOperationExpireDate = cp.DateExpire;
                                person.ColdWeatherOperationIssueDate = cp.DateIssue;
                            }
                            break;
                        //9	HOT-WX
                        case 9:
                            if ((DateTime)cp.DateExpire > person.HotWeatherOperationExpireDate)
                            {
                                person.HotWeatherOperationExpireDate = cp.DateExpire;
                                person.HotWeatherOperationIssueDate = cp.DateIssue;
                            }
                            break;
                        //10	FIRSTAID
                        case 10:
                            if ((DateTime)cp.DateExpire > person.FirstAidExpireDate)
                            {
                                person.FirstAidExpireDate = cp.DateExpire;
                                person.FirstAidIssueDate = cp.DateIssue;
                            }
                            break;
                        default:
                            break;
                    }
                }


                /////////////////////////


            }




            await context.SaveChangesAsync();
            dto.Id = entity.Id;
            var result = await context.ViewCoursePeopleRankeds.Where(q => q.PersonId == dto.PersonId && q.CourseId == entity.Id).FirstOrDefaultAsync();
            return new DataResponse()
            {
                IsSuccess = true,
                Data = result,
            };
        }

        public async Task<DataResponse> SaveCoursePeople(dynamic dto)
        {
            int courseId = Convert.ToInt32(dto.Id);
            string pid = Convert.ToString(dto.pid);
            // string eid = Convert.ToString(dto.eid);

            var personIds = pid.Split('-').Select(q => Convert.ToInt32(q)).ToList();
            // var employeeIds = eid.Split('-').Select(q => Convert.ToInt32(q)).ToList();

            var exists = await context.CoursePeoples.Where(q => q.CourseId == courseId).Select(q => q.PersonId).ToListAsync();
            var newids = personIds.Where(q => !exists.Contains(q)).ToList();

            foreach (var id in newids)
            {
                context.CoursePeoples.Add(new CoursePeople()
                {
                    CourseId = courseId,
                    PersonId = id,
                    StatusId = -1,
                });
            }

            await context.SaveChangesAsync();

            return new DataResponse()
            {
                IsSuccess = true,
                Data = dto,
            };
        }

        public async Task<DataResponse> SaveCourseSessionPresence(dynamic dto)
        {
            int courseId = Convert.ToInt32(dto.cid);
            int pid = Convert.ToInt32(dto.pid);
            string sid = Convert.ToString(dto.sid);
            sid = sid.Replace("Session", "");



            var exists = await context.CourseSessionPresences.Where(q => q.CourseId == courseId && q.PersonId == pid && q.SessionKey == sid).FirstOrDefaultAsync();

            if (exists != null)
            {
                context.CourseSessionPresences.Remove(exists);
            }
            else
            {
                context.CourseSessionPresences.Add(new CourseSessionPresence()
                {
                    PersonId = pid,
                    SessionKey = sid,
                    CourseId = courseId,
                    Date = DateTime.Now
                });
            }

            await context.SaveChangesAsync();

            return new DataResponse()
            {
                IsSuccess = true,
                Data = dto,
            };
        }


        public async Task<DataResponse> UpdateCoursePeopleStatus(CoursePeopleStatusViewModel dto)
        {
            CoursePeople cp = null;
            if (dto.Id != -1)
                cp = await context.CoursePeoples.Where(q => q.Id == dto.Id).FirstOrDefaultAsync();
            else
                cp = await context.CoursePeoples.Where(q => q.PersonId == dto.PersonId && q.CourseId == dto.CourseId).FirstOrDefaultAsync();


            //-1: UnKnown 0:Failed 1:Passed
            if (dto.StatusId != 1)
            {
                cp.DateIssue = null;
                cp.DateExpire = null;
                cp.CertificateNo = null;


            }
            else
            {
                cp.DateExpire = string.IsNullOrEmpty(dto.Expire) ? null : DateObject.ConvertToDate(dto.Expire).Date;
                cp.DateIssue = string.IsNullOrEmpty(dto.Issue) ? null : DateObject.ConvertToDate(dto.Issue).Date;
                cp.CertificateNo = dto.No;

            }

            cp.StatusId = dto.StatusId;
            cp.StatusRemark = dto.Remark;

            if (dto.StatusId == 1 && cp.DateIssue != null && cp.DateExpire != null && !string.IsNullOrEmpty(cp.CertificateNo))
            {

                var person = await context.People.Where(q => q.Id == cp.PersonId).FirstOrDefaultAsync();
                var course = await context.ViewCourseNews.Where(q => q.Id == cp.CourseId).FirstOrDefaultAsync();
                switch (course.CertificateTypeId)
                {
                    //dg
                    case 3:
                        if ((DateTime)cp.DateExpire > person.DangerousGoodsExpireDate)
                        {
                            person.DangerousGoodsExpireDate = cp.DateExpire;
                            person.DangerousGoodsIssueDate = cp.DateIssue;
                        }
                        break;
                    //1	SEPT-P
                    case 1:
                        if ((DateTime)cp.DateExpire > person.SEPTPExpireDate)
                        {
                            person.SEPTPExpireDate = cp.DateExpire;
                            person.SEPTPIssueDate = cp.DateIssue;
                        }
                        break;
                    //2   SEPT - T
                    case 2:
                        if ((DateTime)cp.DateExpire > person.SEPTExpireDate)
                        {
                            person.SEPTExpireDate = cp.DateExpire;
                            person.SEPTIssueDate = cp.DateIssue;
                        }
                        break;
                    //4	CRM
                    case 4:
                        if ((DateTime)cp.DateExpire > person.UpsetRecoveryTrainingExpireDate)
                        {
                            person.UpsetRecoveryTrainingExpireDate = cp.DateExpire;
                            person.UpsetRecoveryTrainingIssueDate = cp.DateIssue;
                        }
                        break;
                    //5	CCRM
                    case 5:
                        if ((DateTime)cp.DateExpire > person.CCRMExpireDate)
                        {
                            person.CCRMExpireDate = cp.DateExpire;
                            person.CCRMIssueDate = cp.DateIssue;
                        }
                        break;
                    //6	SMS
                    case 6:
                        if ((DateTime)cp.DateExpire > person.SMSExpireDate)
                        {
                            person.SMSExpireDate = cp.DateExpire;
                            person.SMSIssueDate = cp.DateIssue;
                        }
                        break;
                    //7	AV-SEC
                    case 7:
                        if ((DateTime)cp.DateExpire > person.AviationSecurityExpireDate)
                        {
                            person.AviationSecurityExpireDate = cp.DateExpire;
                            person.AviationSecurityIssueDate = cp.DateIssue;
                        }
                        break;
                    //8	COLD-WX
                    case 8:
                        if ((DateTime)cp.DateExpire > person.ColdWeatherOperationExpireDate)
                        {
                            person.ColdWeatherOperationExpireDate = cp.DateExpire;
                            person.ColdWeatherOperationIssueDate = cp.DateIssue;
                        }
                        break;
                    //9	HOT-WX
                    case 9:
                        if ((DateTime)cp.DateExpire > person.HotWeatherOperationExpireDate)
                        {
                            person.HotWeatherOperationExpireDate = cp.DateExpire;
                            person.HotWeatherOperationIssueDate = cp.DateIssue;
                        }
                        break;
                    //10	FIRSTAID
                    case 10:
                        if ((DateTime)cp.DateExpire > person.FirstAidExpireDate)
                        {
                            person.FirstAidExpireDate = cp.DateExpire;
                            person.FirstAidIssueDate = cp.DateIssue;
                        }
                        break;
                    default:
                        break;
                }
            }


            await context.SaveChangesAsync();

            return new DataResponse()
            {
                IsSuccess = true,
                Data = dto,
            };
        }


        public IQueryable<CertificateType> GetCertificateTypesQuery()
        {
            IQueryable<CertificateType> query = context.Set<CertificateType>().AsNoTracking();
            return query;
        }

        public IQueryable<ViewCourseType> GetCourseTypesQuery()
        {
            IQueryable<ViewCourseType> query = context.Set<ViewCourseType>().AsNoTracking();
            return query;
        }

        public IQueryable<ViewCourseNew> GetCourseQuery()
        {
            IQueryable<ViewCourseNew> query = context.Set<ViewCourseNew>().AsNoTracking();
            return query;
        }

        public async Task<DataResponse> GetCourseSessions(int cid)
        {
            var result = await context.CourseSessions.Where(q => q.CourseId == cid).OrderBy(q => q.DateStart).ToListAsync();

            return new DataResponse()
            {
                Data = result,
                IsSuccess = true,
            };
        }

        public async Task<DataResponse> GetCoursePeopleAndSessions(int cid)
        {
            var sessions = await context.CourseSessions.Where(q => q.CourseId == cid).OrderBy(q => q.DateStart).ToListAsync();
            var people = await context.ViewCoursePeoples.Where(q => q.CourseId == cid).OrderBy(q => q.DateStart).ToListAsync();
            var press = await context.CourseSessionPresences.Where(q => q.CourseId == cid).ToListAsync();

            return new DataResponse()
            {
                Data = new
                {
                    sessions,
                    people,
                    press
                },
                IsSuccess = true,
            };
        }

        public async Task<DataResponse> SyncSessionsToRoster(int cid)
        {
            var sessions = await context.ViewCourseSessions.Where(q => q.CourseId == cid).OrderBy(q => q.DateStart).ToListAsync();
            var cps = await context.CoursePeoples.Where(q => q.CourseId == cid).ToListAsync();
            var personIds = cps.Select(q => q.PersonId).ToList();
            var employees = await context.ViewEmployeeAbs.Where(q => personIds.Contains(q.PersonId)).ToListAsync();
            var currents = await context.CourseSessionFDPs.Where(q => q.CourseId == cid).ToListAsync();
            var fdps = new List<FDP>();
            var errors = new List<object>();
            foreach (var session in sessions)
            {
                foreach (var emp in employees)
                {
                    var exist = currents.Where(q => q.EmployeeId == emp.Id && q.SessionKey == session.Key).FirstOrDefault();
                    if (exist == null)
                    {

                        var ofdp = (from x in context.ViewFDPIdeas.AsNoTracking()
                                    where x.CrewId == emp.Id && x.DutyType == 1165
                                    && (
                                      (session.DateStartUtc >= x.DutyStart && session.DateStartUtc <= x.RestUntil) || (session.DateEndUtc >= x.DutyStart && session.DateEndUtc <= x.RestUntil)
                                      || (x.DutyStart >= session.DateStartUtc && x.DutyStart <= session.DateEndUtc) || (x.RestUntil >= session.DateStartUtc && x.RestUntil <= session.DateEndUtc)
                                      )
                                    select x).FirstOrDefault();
                        if (ofdp == null)
                        {
                            var duty = new FDP();
                            duty.DateStart = session.DateStartUtc;
                            duty.DateEnd = session.DateEndUtc;

                            duty.CrewId = emp.Id;
                            duty.DutyType = 5000;
                            duty.GUID = Guid.NewGuid();
                            duty.IsTemplate = false;
                            duty.Remark = session.CT_Title + "\r\n" + session.Title;
                            duty.UPD = 1;

                            duty.InitStart = duty.DateStart;
                            duty.InitEnd = duty.DateEnd;
                            var rest = new List<int>() { 1167, 1168, 1170, 5000, 5001, 100001, 100003 };
                            duty.InitRestTo = rest.Contains(duty.DutyType) ? ((DateTime)duty.InitEnd).AddHours(12) : duty.DateEnd;
                            //rec.FDP = duty;
                            var csf = new CourseSessionFDP()
                            {
                                FDP = duty,
                                CourseId = session.CourseId,
                                SessionKey = session.Key,
                                EmployeeId = emp.Id,

                            };
                            context.FDPs.Add(duty);
                            context.CourseSessionFDPs.Add(csf);


                            fdps.Add(duty);
                        }
                        else
                        {
                            errors.Add(new {
                                FDPId = ofdp.Id,
                                EmployeeId = emp.Id,
                                SessionItemId = session.Id,
                                Name = emp.Name,
                                Flights = ofdp.InitFlts,
                                Route = ofdp.InitRoute,
                                // DutyEnd = ofdp.DutyEndLocal,
                                DutyStart = ofdp.DutyStart,
                                RestUntil = ofdp.RestUntil,
                                CourseCode = session.CT_Title,
                                CourseTitle = session.Title,
                                SessionDateFrom = session.DateStart,
                                SessionDateTo = session.DateEnd,
                                DateCreate = DateTime.Now
                            });
                        }
                    }
                   
                }
            }
            var saveResult = await context.SaveAsync();
            return new DataResponse()
            {
                Data = new
                {
                    fdps=fdps.Select(q=>new { 
                      q.Id, q.CrewId
                    }).ToList(),
                    errors
                },
                IsSuccess = true,
            };
        }



    }
}