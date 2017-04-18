using System;
using LiteDB;
namespace eng_back.Model
{
    public class User
    {
        public ObjectId Id{get;set;}
        public string UserName{get;set;}
        public string PassWord{get;set;}
        public string NickName{get;set;}//昵称
        public DateTime BirthDay{get;set;}//生日
        public string Label{get;set;}//签名
        public int ResAssets{get;set;} //用户资源分
    }
}