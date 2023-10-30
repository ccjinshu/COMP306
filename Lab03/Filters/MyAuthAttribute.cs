
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc; 


namespace Lab03.Filters
{
    

    public class MyAuthAttribute : AuthorizeAttribute
    {
        public override void OnAuthorization( AuthorizationContext filterContext)
        {
            
        }
    }

}
