using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Associations.API.Services;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Associations.API.Controllers
{
    [Authorize]
    public class UsersController : Controller
    {
        private readonly IIdentityService _identityService;
        private readonly IMediator _mediator;

        public UsersController(
            IIdentityService identityService,
            IMediator mediator)
        {
            _identityService = identityService;
            _mediator = mediator;
        }
    }
}
