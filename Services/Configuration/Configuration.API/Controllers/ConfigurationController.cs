using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Configuration.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ConfigurationController : ControllerBase
    {
        private readonly SpaConfig _spaConfig;

        public ConfigurationController(
            IOptions<SpaConfig> spaConfig)
        {
            _spaConfig = spaConfig.Value;
        }

        // GET api/values
        [HttpGet("{name}", Name = nameof(GetSpaConfig))]
        [AllowAnonymous]
        public ActionResult<IEnumerable<string>> GetSpaConfig()
        {
            return Ok(_spaConfig);
        }
    }
}
