using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Vertex.Services;
using Models;

namespace Vertex.Controllers
{
    [Route("api/[controller]")]
    public class GetController : Controller
    {
        private IGetOperations _operations { get; set; }
        public GetController(IGetOperations getOper)
        {
            _operations = getOper;   
        }
        // GET api/values
        [HttpGet]
        [Authorize]
        public IActionResult Get()
        {
            var conString = HttpContext.User.Claims.Where(c => c.Type == "sub").Single().Value;
            return Ok(_operations.Get<Operation>(conString,"OperationsList"));
        }

       
    }
}
