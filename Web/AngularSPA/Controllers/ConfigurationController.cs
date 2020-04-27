using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace AngularSPA.Controllers
{
    [Route("api/configuration")]
    [Controller]
    public class ConfigurationController : ControllerBase
    {
        private readonly AppSettings _appConfig;

        public ConfigurationController(IOptions<AppSettings> appConfig)
        {
            _appConfig = appConfig.Value;
        }

        [HttpGet(Name = nameof(GetConfiguration))]
        public IActionResult GetConfiguration() => Ok(_appConfig);
    }
}