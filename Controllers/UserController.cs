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
        public static readonly string dbpath=@"Data/User.db";
        public static readonly string ucolname="users";
        public static readonly string signname="Login-Sign";
        //从guid->username的查找表 用于验证登录
        protected static Dictionary<string,string> logincache=new Dictionary<string,string>();
        //从username到guid的查找表 用于验证是否重复登录
        protected static Dictionary<string,string> rlogincache=new Dictionary<string,string>();
        // POST api/login
        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="username">用户名</param>
        /// <param name="password">密码</param>
        /// <returns>0表示登录成功 1表示此用户已经登录 2表示用户名不存在 3表示密码错误</returns>
        [HttpPost]
        public int Login([FromForm]string username,[FromForm]string password)
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
        /// <summary>
        /// 注册
        /// </summary>
        /// <param name="username">用户名</param>
        /// <param name="password">密码</param>
        /// <param name="nickname">昵称</param>
        /// <returns>是否成功注册 注册失败的原因有 已经存在此用户名 密码长度不够等</returns>
        [HttpPost]
        public bool Regist([FromForm]string username,[FromForm]string password,[FromForm]string nickname)
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
        /// <summary>
        /// 获取用户信息
        /// </summary>
        /// <returns>用户信息</returns>
        [HttpGet]
        public object GetUserInfo()
        {
            User info=GetLoginedUser();
            return GetInfo(info);
        }
        /// <summary>
        /// 修改一般信息
        /// </summary>
        /// <param name="nickname">昵称</param>
        /// <param name="label">签名</param>
        /// <param name="birthday">生日</param>
        /// <returns>是否成功</returns>
        [HttpPut]
        public bool UpdateUserInfo([FromForm]string nickname,[FromForm]string label,[FromForm]DateTime birthday)
        {
            
            if (nickname.Length < 1) return false;//必须有昵称
            User info=GetLoginedUser();
            info.NickName = nickname;
            info.Label = label;
            info.BirthDay = birthday;
            using (var db = new LiteDatabase(dbpath))
            {
                var col = db.GetCollection<User>(ucolname);
                col.Update(info);
                return true;
            }
        }
        /// <summary>
        /// 修改密码
        /// </summary>
        /// <param name="nowpas">原密码</param>
        /// <param name="newpas">新密码</param>
        /// <returns>是否成功</returns>
        public bool UpdatePassword([FromForm]string nowpas,[FromForm]string newpas)
        {
            User info=GetLoginedUser();
            if (info.PassWord != nowpas) return false;
            info.PassWord = newpas;
            using (var db = new LiteDatabase(dbpath))
            {
                var col = db.GetCollection<User>(ucolname);
                col.Update(info);
                return true;
            }
        }
        //以下为辅助函数部分
        /// <summary>
        /// 取得当前登录的用户信息 
        /// </summary>
        /// <returns>当前登录的用户信息结构 没有登录则返回null</returns>
        public User GetLoginedUser()
        {
            string username=GetLoginedUserName();
            using(var db=new LiteDatabase(dbpath))
            {
                var col=db.GetCollection<User>(ucolname);
                User info=col.FindOne(x=>x.UserName==username);
                return info;
            }
        }
        /// <summary>
        /// 取得当前登录的用户的用户名
        /// </summary>
        /// <returns></returns>
        public string GetLoginedUserName()
        {
            string guid=GetLoginGuid();
            if(guid==null) return null;
            return GuidToUserName(guid);
        }
        public string GetLoginGuid()
        {
            return Request.Cookies[signname];
        }
        //以下为工具函数部分
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
