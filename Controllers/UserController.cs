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
        public static readonly string dbpath=@"Data/Users.db";
        public static readonly string ucolname="users";
        public static readonly string signname="Login-Sign";
        //从guid->username的查找表 用于验证登录
        protected static Dictionary<string,string> logincache=new Dictionary<string,string>();
        //从username到guid的查找表 用于验证是否重复登录
        protected static Dictionary<string,string> rlogincache=new Dictionary<string,string>();
        // POST api/login
        [HttpPost]
        public int Login([FromBody]string username,[FromBody]string password)
        {
            if(rlogincache.ContainsKey(username)) return 1; //1表示已经登录 
            using(var db=new LiteDatabase(dbpath))
            {
                var col=db.GetCollection<User>(ucolname);
                col.EnsureIndex(x=>x.UserName,true);
                var result=col.FindOne(x=>x.UserName==username);
                if(result==null) return 2; //2表示登录失败 用户名不存在
                if(result.PassWord!=password) return 3;//3表示密码错误
                //登录成功 加入缓存
                string gid=Guid.NewGuid().ToString();
                logincache.Add(gid,username);
                rlogincache.Add(username,gid);
                //添加到cookie
                HttpContext.Response.Cookies.Append(signname,gid);
                return 0;
            }
        }
        [HttpPost]
        public bool Regist([FromBody]string username,[FromBody]string password,[FromBody]string nickname)
        {
            using(var db=new LiteDatabase(dbpath))
            {
                var col=db.GetCollection<User>(ucolname);
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
        [HttpGet]
        public object GetUserInfo()
        {
            string guid=HttpContext.Request.Cookies[signname];
            if(IsLogined(guid))
            {
                string username=GuidToUserName(guid);
                using(var db=new LiteDatabase(dbpath))
                {
                    var col=db.GetCollection<User>(ucolname);
                    col.EnsureIndex(x=>x.UserName);
                    var res=col.FindOne(x=>x.UserName==username);
                    if(res==null) throw new Exception("错误，登录信息与数据库不同步！");
                    return GetInfo(res);
                }
            }
            return null;
        }
        public static object GetInfo(User uinfo)
        {
                    var ret=new {
                        UserName=uinfo.UserName,
                        NickName=uinfo.NickName,
                        Label=uinfo.Label,
                        BirthDay=uinfo.BirthDay,
                        ResAssets=uinfo.ResAssets
                    };
                    return ret;
        }
        public static bool IsLogined(string guid)
        {
            return logincache.ContainsKey(guid);
        }
        public static string GuidToUserName(string guid)
        {
            return logincache[guid];
        }
        public static string UserNameToGuid(string un)
        {
            return rlogincache[un];
        }


    }
}
