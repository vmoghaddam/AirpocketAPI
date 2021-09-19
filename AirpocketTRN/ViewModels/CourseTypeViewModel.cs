using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AirpocketTRN.ViewModels
{
    public class RelatedJobGroupSimple
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string FullCode { get; set; }
    }
    public class CourseTypeViewModel
    {
        public int Id { get; set; }
        public int? CalenderTypeId { get; set; }
        public int? CourseCategoryId { get; set; }
        public int? LicenseResultBasicId { get; set; }
        public string Title { get; set; }
        public string Remark { get; set; }
        public int? Interval { get; set; }
        public bool? IsGeneral { get; set; }
        public bool? Status { get; set; }
        public int? Duration { get; set; }
        public int? CertificateTypeId { get; set; }

        public string Category { get; set; }
        public List<RelatedJobGroupSimple> JobGroups { get; set; }
        public static void Fill(Models.CourseType entity, ViewModels.CourseTypeViewModel coursetype)
        {
            entity.Id = coursetype.Id;
            entity.CalenderTypeId = coursetype.CalenderTypeId;
            entity.CourseCategoryId = coursetype.CourseCategoryId;
            entity.LicenseResultBasicId = coursetype.LicenseResultBasicId;
            entity.Title = coursetype.Title;
            entity.Remark = coursetype.Remark;
            entity.Interval = coursetype.Interval;
            entity.IsGeneral = coursetype.IsGeneral;
            entity.Status = coursetype.Status;
            entity.Duration = coursetype.Duration;
            entity.CertificateTypeId = coursetype.CertificateTypeId;

        }
        public static void FillDto(Models.CourseType entity, ViewModels.CourseTypeViewModel coursetype)
        {
            coursetype.Id = entity.Id;
            coursetype.CalenderTypeId = entity.CalenderTypeId;
            coursetype.CourseCategoryId = entity.CourseCategoryId;
            coursetype.LicenseResultBasicId = entity.LicenseResultBasicId;
            coursetype.Title = entity.Title;
            coursetype.Remark = entity.Remark;
            coursetype.Interval = entity.Interval;
            coursetype.IsGeneral = entity.IsGeneral;
            coursetype.Status = entity.Status;
            coursetype.Duration = entity.Duration;
            coursetype.CertificateTypeId = entity.CertificateTypeId;
        }



    }

    public class CourseViewModel
    {
        public int Id { get; set; }
        public int CourseTypeId { get; set; }
        public string  DateStart { get; set; }
        
        public string DateEnd { get; set; }
        
        public string Instructor { get; set; }
        public string Location { get; set; }
      
        public int? OrganizationId { get; set; }
        public int? Duration { get; set; }
        public int? DurationUnitId { get; set; }

       
        public string Remark { get; set; }
       
        public string TrainingDirector { get; set; }
        public string Title { get; set; }
        
        public bool? Recurrent { get; set; }
        public int? Interval { get; set; }
        public int? CalanderTypeId { get; set; }
       
       
        public bool? IsGeneral { get; set; }
        public int? CustomerId { get; set; }
        public string No { get; set; }
        public bool? IsNotificationEnabled { get; set; }

        public List<string> Sessions { get; set; }

        public int? StatusId { get; set; }
    }

    public class CertificateViewModel
    {
        public int Id { get; set; }
        public int CourseTypeId { get; set; }
        public string DateStart { get; set; }

        public string DateEnd { get; set; }

        public string Instructor { get; set; }
        public string Location { get; set; }

        public int? OrganizationId { get; set; }
        public int? Duration { get; set; }
        public int? DurationUnitId { get; set; }


        public string Remark { get; set; }

        public string TrainingDirector { get; set; }
        public string Title { get; set; }

        public bool? Recurrent { get; set; }
        public int? Interval { get; set; }
        public int? CalanderTypeId { get; set; }


        public bool? IsGeneral { get; set; }
        public int? CustomerId { get; set; }
        public string No { get; set; }
        public bool? IsNotificationEnabled { get; set; }

        public int? PersonId { get; set; }
        public string CertificateNo { get; set; }
        public string DateIssue { get; set; }
        public string DateExpire { get; set; }


        public List<string> Sessions { get; set; }

        public int? StatusId { get; set; }
    }

    public class CoursePeopleStatusViewModel
    {
        public int Id { get; set; }
        public int? CourseId { get; set; }
        public int? PersonId { get; set; }
        public int? StatusId { get; set; }
        public string Remark { get; set; }
        public string Issue { get; set; }

        public string Expire { get; set; }
        public string No { get; set; }
    }


}