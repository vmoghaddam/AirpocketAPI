using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AirpocketAPI
{
    public class IdeaResultSession
    {
        public object d_xml { get; set; }
        public IdeaEnvelopeSession d_envelope { get; set; }

    }
    public class IdeaEnvelopeSession
    {
        public string d_xmlns_soap { get; set; }
        public string d_xmlns_xsi { get; set; }
        public string d_xmlns_xsd { get; set; }
        public IdeaBodySession d_body { get; set; }


    }
    public class IdeaBodySession
    {
        public IdeaResponseSession response { get; set; }
    }
    public class IdeaResponseSession
    {
        public string d_xmlns { get; set; }
        //public List<IdeaSession> result { get; set; }
        public string result { get; set; }
    }

    // {\"BeginDate\":\"19/06/2021 12:00:00 ق.ظ\",\"EndDate\":\"26/06/2021 12:00:00 ق.ظ\",\"sessions\":\"2021.06.22,08:30,10:30$2021.06.22,10:30,12:30\"},
    //{\"id\":\"45175\",\"ClassID\":\"Re/Annual-Re/Cabin-00-TC-50\",\"nid\":\"0010511067\",\"courseCode\":\"Re/Annual-Re/Cabin\",\"CourseTitle\":\"Annual Recurrent Training\",\"BeginDate\":\"19/06/2021 12:00:00 ق.ظ\",\"EndDate\":\"26/06/2021 12:00:00 ق.ظ\",\"sessions\":\"2021.06.22,08:30,10:30$2021.06.22,10:30,12:30\"},
    public class IdeaSessionX
    {
        public string id { get; set; }
        public string ClassID { get; set; }
        public string nid { get; set; }
        public string courseCode { get; set; }
        public string CourseTitle { get; set; }
        public string BeginDate { get; set; }
        public string EndDate { get; set; }
        public string sessions { get; set; }
        public DateTime? BeginDate2
        {
            //19/06/2021 12:00:00 ق.ظ
            get
            {
                if (string.IsNullOrEmpty(this.BeginDate))
                    return null;
                var _date = this.BeginDate;
                try
                {
                    var dd =Convert.ToInt32( _date.Substring(0,2));
                    var mm = Convert.ToInt32(_date.Substring(3, 2));
                    var yy = Convert.ToInt32(_date.Substring(6, 4));
                    return (new DateTime(yy, mm, dd)).Date;

                }
                catch (Exception ex)
                {
                    return null;
                }

                 
            }
        }
        public DateTime? EndDate2
        {
            get
            {
                if (string.IsNullOrEmpty(this.EndDate))
                    return null;
                var _date = this.EndDate;
                try
                {
                    var dd = Convert.ToInt32(_date.Substring(0, 2));
                    var mm = Convert.ToInt32(_date.Substring(3, 2));
                    var yy = Convert.ToInt32(_date.Substring(6, 4));
                    return (new DateTime(yy, mm, dd)).Date;

                }
                catch (Exception ex)
                {
                    return null;
                }
            }
        }

        List<Session> sessions2 { get; set; }
        public List<Session> Sessions2
        {
            get
            {
                if (sessions2 == null)
                {
                    try
                    {
                        sessions2 = new List<Session>();
                        var days = this.sessions.Split('$');
                        foreach (var _day in days)
                        {
                            //var tzoffset3= TimeZoneInfo.Local.GetUtcOffset((DateTime)entity.STD).TotalMinutes;
                            var prts = _day.Split(',');
                            var _date = prts[0].Split('.').Select(q => Convert.ToInt32(q)).ToList();
                            var _t1=prts[1].Split(':').Select(q => Convert.ToInt32(q)).ToList();
                            var _t2= prts[2].Split(':').Select(q => Convert.ToInt32(q)).ToList();
                            var _from = new DateTime(_date[0],_date[1],_date[2],_t1[0],_t1[1],0);
                            var _to = new DateTime(_date[0], _date[1], _date[2], _t2[0], _t2[1], 0);
                            var item = new Session()
                            {
                                DateFrom = _from,
                                DateTo = _to,
                                Remark = this.sessions,
                                DateFromUtc = TimeZoneInfo.ConvertTimeToUtc(_from),
                                DateToUtc = TimeZoneInfo.ConvertTimeToUtc(_to),
                            };
                            sessions2.Add(item);
                        }


                    }
                    catch(Exception ex)
                    {
                        sessions2 = new List<Session>() { new Session() { Remark=ex.Message } };
                    }
                }
                return this.sessions2;
            }
        }
        //"sessions":"2021.06.22,08:30,10:30$2021.06.22,10:30,12:30"





    }

    public class Session
    {
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }

        public DateTime? DateFromUtc { get; set; }
        public DateTime? DateToUtc { get; set; }

        public string Remark { get; set; }

    }




    public class IdeaResultUnique
    {
        public object d_xml { get; set; }
        public IdeaEnvelopeUnique d_envelope { get; set; }

    }
    public class IdeaEnvelopeUnique
    {
        public string d_xmlns_soap { get; set; }
        public string d_xmlns_xsi { get; set; }
        public string d_xmlns_xsd { get; set; }
        public IdeaBodyUnique d_body { get; set; }


    }
    public class IdeaBodyUnique
    {
        public IdeaResponseUnique response { get; set; }
    }
    public class IdeaResponseUnique
    {
        public string d_xmlns { get; set; }
        //public List<IdeaSession> result { get; set; }
        public string result { get; set; }
    }

    //{\"BeginDate\":\"22/01/2020 12:00:00 ق.ظ\",\"EndDate\":\"22/01/2020 12:00:00 ق.ظ\",\"expire\":\"21/01/2021 12:00:00 ق.ظ\"}
    public class IdeaUniqueX
    {
        public string ID { get; set; }
        public string ClassID { get; set; }
        public string nid { get; set; }
        public string courseCode { get; set; }
        public string CourseTitle { get; set; }
        public string BeginDate { get; set; }
        public string EndDate { get; set; }
        public string expire { get; set; }
        
        public DateTime? BeginDate2
        {
            //19/06/2021 12:00:00 ق.ظ
            get
            {
                if (string.IsNullOrEmpty(this.BeginDate))
                    return null;
                var _date = this.BeginDate;
                try
                {
                    var dd = Convert.ToInt32(_date.Substring(0, 2));
                    var mm = Convert.ToInt32(_date.Substring(3, 2));
                    var yy = Convert.ToInt32(_date.Substring(6, 4));
                    return (new DateTime(yy, mm, dd)).Date;

                }
                catch (Exception ex)
                {
                    return null;
                }


            }
        }
        public DateTime? EndDate2
        {
            get
            {
                if (string.IsNullOrEmpty(this.EndDate))
                    return null;
                var _date = this.EndDate;
                try
                {
                    var dd = Convert.ToInt32(_date.Substring(0, 2));
                    var mm = Convert.ToInt32(_date.Substring(3, 2));
                    var yy = Convert.ToInt32(_date.Substring(6, 4));
                    return (new DateTime(yy, mm, dd)).Date;

                }
                catch (Exception ex)
                {
                    return null;
                }
            }
        }
        public DateTime? ExpireDate2
        {
            //19/06/2021 12:00:00 ق.ظ
            get
            {
                if (string.IsNullOrEmpty(this.expire))
                    return null;
                var _date = this.expire;
                try
                {
                    var dd = Convert.ToInt32(_date.Substring(0, 2));
                    var mm = Convert.ToInt32(_date.Substring(3, 2));
                    var yy = Convert.ToInt32(_date.Substring(6, 4));
                    return (new DateTime(yy, mm, dd)).Date;

                }
                catch (Exception ex)
                {
                    return null;
                }


            }
        }

    }


    public class IdeaResultAll
    {
        public object d_xml { get; set; }
        public IdeaEnvelopeAll d_envelope { get; set; }

    }
    public class IdeaEnvelopeAll
    {
        public string d_xmlns_soap { get; set; }
        public string d_xmlns_xsi { get; set; }
        public string d_xmlns_xsd { get; set; }
        public IdeaBodyUnique d_body { get; set; }


    }
    public class IdeaBodyAll
    {
        public IdeaResponseUnique response { get; set; }
    }
    public class IdeaResponseAll
    {
        public string d_xmlns { get; set; }
        //public List<IdeaSession> result { get; set; }
        public string result { get; set; }
    }

    //{\"BeginDate\":\"22/01/2020 12:00:00 ق.ظ\",\"EndDate\":\"22/01/2020 12:00:00 ق.ظ\",\"expire\":\"21/01/2021 12:00:00 ق.ظ\"}
    public class IdeaAllX
    {
        public string id { get; set; }
        public string ClassID { get; set; }
        public string nid { get; set; }
        public string courseCode { get; set; }
        public string CourseTitle { get; set; }
        public string BeginDate { get; set; }
        public string EndDate { get; set; }
        public string expire { get; set; }

        public DateTime? BeginDate2
        {
            //19/06/2021 12:00:00 ق.ظ
            get
            {
                if (string.IsNullOrEmpty(this.BeginDate))
                    return null;
                var _date = this.BeginDate;
                try
                {
                    var dd = Convert.ToInt32(_date.Substring(0, 2));
                    var mm = Convert.ToInt32(_date.Substring(3, 2));
                    var yy = Convert.ToInt32(_date.Substring(6, 4));
                    return (new DateTime(yy, mm, dd)).Date;

                }
                catch (Exception ex)
                {
                    return null;
                }


            }
        }
        public DateTime? EndDate2
        {
            get
            {
                if (string.IsNullOrEmpty(this.EndDate))
                    return null;
                var _date = this.EndDate;
                try
                {
                    var dd = Convert.ToInt32(_date.Substring(0, 2));
                    var mm = Convert.ToInt32(_date.Substring(3, 2));
                    var yy = Convert.ToInt32(_date.Substring(6, 4));
                    return (new DateTime(yy, mm, dd)).Date;

                }
                catch (Exception ex)
                {
                    return null;
                }
            }
        }

        public DateTime? ExpireDate2
        {
            //19/06/2021 12:00:00 ق.ظ
            get
            {
                if (string.IsNullOrEmpty(this.expire))
                    return null;
                var _date = this.expire;
                try
                {
                    var dd = Convert.ToInt32(_date.Substring(0, 2));
                    var mm = Convert.ToInt32(_date.Substring(3, 2));
                    var yy = Convert.ToInt32(_date.Substring(6, 4));
                    return (new DateTime(yy, mm, dd)).Date;

                }
                catch (Exception ex)
                {
                    return null;
                }


            }
        }

    }




}