using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace SA_securityAssignment
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }


        //by Default is hidden.
        //how to authenticate a user on mvc

        
        protected void Application_AuthenticateRequest(object sender, EventArgs args)
        {
            if (Context.User != null)
            {

                if (Context.User.Identity.IsAuthenticated)
                {

                    //get th roles of the current loggd in user.
                    var rolesBelongingToUser = new BusinessLogic.UsersBL().GetRolesOfUser(Context.User.Identity.Name);

                    string[] roles = rolesBelongingToUser.Select(x => x.Role1).ToArray();

                    GenericPrincipal gp = new GenericPrincipal(Context.User.Identity, roles);

                    Context.User = gp;

                }
            }

        }
        


    }
}
