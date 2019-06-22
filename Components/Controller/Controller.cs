using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace MKLUODDD.Controllers {

    using Apps;

    [ApiController]
    [Route("[controller]")]
    public class BaseController : ControllerBase {

        App App { get; }

        public BaseController(App app) {
            this.App = app;
        }

        /**
            Authentication
         */

        [HttpPost("login")]
        public IActionResult Login([FromBody] JObject userData) {
            var loginResult = App.Authenticate(userData).ToJson();
            if (!App.IsLoggedIn()) return Unauthorized(loginResult);

            return Ok(loginResult);
        }

        /**
            Service Info
         */

        [HttpGet("services/")]
        public IActionResult GetInfo([FromQuery] string? service, [FromQuery] string? action) {
            if (!App.IsLoggedIn()) return Unauthorized();

            if (service == null) 
                return Ok(App.GetServiceInfos().ToJson());
            
            else {
                if (action == null)
                    return Ok(App.GetServiceInfo(service).ToJson());

                else 
                    return Ok(App.GetActionInfo(service, action).ToJson());
            }
        }

        /**
            Service
         */

        [HttpPost("services/")]
        public IActionResult UseService([FromQuery] string? service, [FromQuery] string? action, [FromBody] JObject payload) {
            if (!App.IsLoggedIn()) return Unauthorized();

            if ((service != null) && (action != null))
                return Ok(App.Run(service, action, payload));
            
            else 
                return Ok();
        }
    }
}