using Microsoft.AspNetCore.Mvc;
using System;

namespace ecs.Controllers
{
    
    public class EventsController : Controller
    {
        // GET: Events API
        [HttpGet]
        [Route("events")]
        public ActionResult Index()
        {
            var systeminfo = new sysinfo
            {
                OSDescription = System.Runtime.InteropServices.RuntimeInformation.OSDescription,
                OSArchitecture = System.Runtime.InteropServices.RuntimeInformation.OSArchitecture.ToString(),
                FrameworkDescription = System.Runtime.InteropServices.RuntimeInformation.FrameworkDescription,
                ProcessArchitecture = System.Runtime.InteropServices.RuntimeInformation.ProcessArchitecture.ToString(),
                RuntimeIdentifier = System.Runtime.InteropServices.RuntimeInformation.RuntimeIdentifier,
                Feature = "X"
            };
            Console.WriteLine($"Hello from feature {systeminfo.Feature}");
            return Ok(systeminfo);

        }

        // GET: Health API
        [HttpGet]
        [Route("events/health")]
        public ActionResult Health()
        {

            return Ok("Healthy");

        }
    }
}
