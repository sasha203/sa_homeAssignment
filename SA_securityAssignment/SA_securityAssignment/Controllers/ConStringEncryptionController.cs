using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using System.Web.Configuration;
using System.Web.Security;
using System.Configuration;

namespace SA_securityAssignment.Controllers
{
    public class ConStringEncryptionController : Controller
    {
        // GET: ConStringEncryption
        //http://www.beansoftware.com/ASP.NET-Tutorials/Encrypting-Connection-String.aspx

        public ActionResult Index()
        {   
            Configuration config = WebConfigurationManager.OpenWebConfiguration(Request.ApplicationPath);
            ConfigurationSection section = config.GetSection("connectionStrings");
            if (!section.SectionInformation.IsProtected)
            {
                section.SectionInformation.ProtectSection("RsaProtectedConfigurationProvider");
                config.Save();
            } 
            return View();
        }
    }
}