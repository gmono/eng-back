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
        //从guid->username的查找表 用于验证登录
        protected static Dictionary<string,string> logincache=new Dictionary<string,string>();
        //从username到guid的查找表 用于验证是否重复登录
        protected static Dictionary<string,string> rlogincache=new Dictionary<string,string>();
        // POST api/login
        [HttpPost]
        public string Login([FromBody]string username,[FromBody]string password)
        {
            if(rlogincache.ContainsKey(username)) return "1"; //1表示已经登录 
            using(var db=new LiteDatabase(@"Data/Users.db"))
            {
                var col=db.GetCollection<User>("users");
                col.EnsureIndex(x=>x.UserName,true);
                var result=col.FindOne(x=>x.UserName==username);
                if(result==null) return "2"; //2表示登录失败 用户名不存在
                if(result.PassWord!=password) return "3";//3表示密码错误
                //登录成功 加入缓存
                string gid=Guid.NewGuid().ToString();
                logincache.Add(gid,username);
                rlogincache.Add(username,gid);
                return "{"+gid+"}";//返回一个对象 表示登录验证guid
            }
        }
        [HttpPost]
        public bool Regist([FromBody]string username,[FromBody]string password,[FromBody]string nickname)
        {
            using(var db=new LiteDatabase(@"Data/Users.db"))
            {
                var col=db.GetCollection<User>("users");
                col.EnsureIndex(x=>x.UserName,true);
                var result=col.FindOne(x=>x.UserName==username);
                if(result!=null) return false;//已经存在用户
                if(password.Length<8) return false;//至少8位密码
                User info=new User();
                info.NickName=nickname;
                info.UserName=username;
                info.PassWord=password;
                col.Insert(info);
                return true;
            }
        }

        public static bool IsLogined(string guid)
        {
            return logincache.ContainsKey(guid);
        }
    }
}
