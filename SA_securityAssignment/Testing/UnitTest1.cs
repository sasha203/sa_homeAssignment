using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

using Common;
using BusinessLogic;
using SA_securityAssignment;
using SA_securityAssignment.Controllers;
using System.Web.Mvc;

namespace Testing
{
    [TestClass]
    public class UnitTest1
    {
      
        
        [TestMethod]
       //[ExpectedException(typeof(CustomException))]
        public void TestingLoginMethod() {

            for (int i = 0; i < 3; i++)
            {
                var controller = new AccountsController();
                string password = GeneratePassword();
                var result = controller.Login("joe", password); //System.Web.Mvc.ActionResult
                Assert.IsNotNull(result);
                //Assert.AreNotEqual("123", password);
           }

        }
    
       

        public static string GeneratePassword()
        {
            Random r = new Random();
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            int length = r.Next(5, 41);
            return new string(Enumerable.Repeat(chars, length).Select(s => s[r.Next(s.Length)]).ToArray());
        }


       


        [TestMethod]
        [ExpectedException(typeof(NullReferenceException))]
        //[ExpectedException(typeof(CustomException))]
        public void Test_Register_Method_Validation()
        {
            var controller = new AccountsController();
            User u = new User
            {
                Username = GenerateUsername(),
                Password = GeneratePassword(),
                Email = GenerateEmail(),
                Country = "Malta",
                NoOfAttemps = 0,
                Blocked = false,
            };

            var result = controller.Register(u);
        }

        public static string GenerateEmail()
        {
            var chars = "abcdefghijklmnopqrstuvwxyz0123456789";
            var stringChars = new char[10];
            var r = new Random();

            for (int i = 0; i < stringChars.Length; i++)
            {
                stringChars[i] = chars[r.Next(chars.Length)];
            }

            var finalString = new string(stringChars);
            return finalString + "@gmail.com";
        }


        public static string GenerateUsername()
        {
            var chars = "abcdefghijklmnopqrstuvwxyz0123456789";
            var username = new char[10];

            var r = new Random();

            for (int i = 0; i < username.Length; i++)
            {
                username[i] = chars[r.Next(chars.Length)];
            }

            var output = new string(username);
            return output;
        }

    


    }
}
