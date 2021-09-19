using AirpocketTRN.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace AirpocketTRN.Services
{
    public interface IPeopleService
    {

    }
    public class PeopleService:IPeopleService
    {
        FLYEntities context = null;
        public PeopleService()
        {
            context = new FLYEntities();
            context.Configuration.LazyLoadingEnabled = false;
        }

        public IQueryable<Person> GetPeople()
        {
            IQueryable<Person> query = context.Set<Person>().AsNoTracking();
            return query;
        }
        public IQueryable<ViewEmployeeAb> GetViewEmployeesAbs()
        {
            IQueryable<ViewEmployeeAb> query = context.Set<ViewEmployeeAb>().AsNoTracking();
            return query;
        }
        public IQueryable<ViewEmployeeAb> GetEmployeeByGroups(string grps)
        {
            if (string.IsNullOrEmpty(grps))
                return context.Set<ViewEmployeeAb>().AsNoTracking();
            var groups = grps.Split('-').ToList();
            IQueryable<ViewEmployeeAb> query = context.Set<ViewEmployeeAb>().Where(q=>groups.Contains(q.JobGroupCode)).AsNoTracking();
            return query;
        }

        public async Task<DataResponse> GetPeopleById(int id)
        {
            var person = await context.People.FirstOrDefaultAsync(q => q.Id == id);

            return new DataResponse() { 
             Data=person,
             IsSuccess=true,
            };
        }
    }
}