using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace eng_back.Controllers
{
    [Route("api/[controller]/[action]")]
    public class LoginController : Controller
    {
        // POST api/login
        [HttpPost]
        public void Post([FromBody]string value)
        {

        }
        [HttpGet]
        public string GetA()
        {
            return "hello world";
        }
        [HttpGet()]
        public string GetB()
        {
            return "bbb";
        }
    }
}
