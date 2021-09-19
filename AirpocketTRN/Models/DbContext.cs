using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace AirpocketTRN.Models
{
    public partial class FLYEntities
    {
        public async Task<DataResponse> SaveAsync()
        {
            try
            {

                await this.SaveChangesAsync();
                return new DataResponse() { IsSuccess = true };
            }
            catch (DbEntityValidationException e)
            {
                foreach (var eve in e.EntityValidationErrors)
                {

                    foreach (var ve in eve.ValidationErrors)
                    {
                        var xxx =
                             ve.PropertyName + " " + ve.ErrorMessage;
                    }
                }
             
                return new DataResponse() { IsSuccess = false, Errors=new List<string>() { "DbEntityValidationException" } };
            }
            catch (DbUpdateException dbu)
            {
                var exception = Exceptions.HandleDbUpdateException(dbu);
                
                return new DataResponse() { IsSuccess = false, Errors = new List<string>() { exception.Message } };
            }
            catch (Exception ex)
            {
                return new DataResponse() { IsSuccess = false, Errors = new List<string>() { ex.Message } };
            }
        }

        //public DataResponse SaveActionResult()
        //{
        //    try
        //    {

        //        this.SaveChangesAsync();
        //        return new CustomActionResult(HttpStatusCode.OK, "");
        //    }
        //    catch (DbEntityValidationException e)
        //    {
        //        foreach (var eve in e.EntityValidationErrors)
        //        {

        //            foreach (var ve in eve.ValidationErrors)
        //            {
        //                var xxx =
        //                     ve.PropertyName + " " + ve.ErrorMessage;
        //            }
        //        }
        //        return new CustomActionResult(HttpStatusCode.InternalServerError, "DbEntityValidationException");
        //    }
        //    catch (DbUpdateException dbu)
        //    {
        //        var exception = Exceptions.HandleDbUpdateException(dbu);
        //        return new CustomActionResult(HttpStatusCode.InternalServerError, exception.Message);
        //    }
        //    catch (Exception ex)
        //    {
        //        return new CustomActionResult(HttpStatusCode.InternalServerError, ex.Message);
        //    }
        //}



    }
}