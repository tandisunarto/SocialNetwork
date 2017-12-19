using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace SocialNetwork.Api.Controllers
{
    [Authorize]
    public class TestController : ControllerBase
    {
        [Route("test")]
        public IActionResult Get()
        {
            var claims = User.Claims.Select(x => $"{x.Type}:{x.Value}");
            return Ok(new
            {
                message = "Hello MVC Core API!",
                claims = claims.ToArray()
            });
        }
    }
}
