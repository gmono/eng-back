using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using LiteDB;
using eng_back.Model;
namespace eng_back.Controllers
{
    //这是登录接口
    [Route("api/[controller]/[action]")]
    public class UserController : Controller
    {
        // POST api/login
        [HttpPost]
        public string Login([FromBody]string username,[FromBody]string password)
        {
            using(var db=new LiteDatabase(@"Data/Users.db"))
            {
                var col=db.GetCollection<User>("users");
                col.EnsureIndex(x=>x.UserName,true);
                var result=col.Find(x=>x.UserName==username);
                if(result.Count()==0) return "false";
                var res=result.First();
                if(res.PassWord!=password) return "false";
                
            }
        }
    }
}
