using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Session.API.Controllers
{
    [Route("api/session")]
    [Authorize]
    public class SessionController: ControllerBase
    {
    }
}
