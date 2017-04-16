using System;
using LiteDB;
namespace eng_back.Model
{
    public class User
    {
        public ObjectId Id{get;set;}
        public string UserName{get;set;}
        public string PassWord{get;set;}
    }
    public class UserInfo
    {
        public ObjectId Id{get;set;}
        public string NickName{get;set;}
        public DateTime BirthDay{get;set;}
        public string Label{get;set;}
        public ObjectId UserId{get;set;}
    }
    public class UserAssets
    {
        public ObjectId Id{get;set;}
        public ObjectId UserId{get;set;}
        public int ResAssets{get;set;} //用户资源分
    }
}