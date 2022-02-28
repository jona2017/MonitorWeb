using Queue.Action;
using Queue.Models;
using Queue.ViewModels;
using System.Net.Http;
using System.Web.Http;

namespace Queue.Controllers
{
    [RoutePrefix("api/Logs")]
    public class LogsController : ApiController
    {
        aUtilities ut = new aUtilities();

        [HttpPost]
        [Route("Createlog")]
        public HttpResponseMessage Createlog(LogsViewModel t)
        {
            aAutomaticTakeTime s = new aAutomaticTakeTime();
            return ut.ReturnResponse(s.Createlog(t));
        }
    }
}