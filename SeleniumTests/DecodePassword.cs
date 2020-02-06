using System;
using System.Text;

namespace SeleniumTests
{
    public static class DecodePassword
    {
        public static string Password => Encoding.UTF8.GetString(Convert.FromBase64String("a29zbW9z"));

        public static string User => Encoding.UTF8.GetString(Convert.FromBase64String("cGFibG8wNTEw"));
        //public DecodePassword()
        //{ 
        //    //var pass = Convert.ToBase64String(Encoding.UTF8.GetBytes("".ToCharArray()));
        //    //var user = Convert.ToBase64String(Encoding.UTF8.GetBytes("".ToCharArray()));
        //    this.Password = Encoding.UTF8.GetString(Convert.FromBase64String("a29zbW9z"));
        //    this.User = Encoding.UTF8.GetString(Convert.FromBase64String("cGFibG8wNTEw"));
        //}
    }
}
