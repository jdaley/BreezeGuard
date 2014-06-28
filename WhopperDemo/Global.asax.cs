using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;

namespace WhopperDemo
{
    public class Global : System.Web.HttpApplication
    {
        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            if (Request.RawUrl == "/")
            {
                Response.Redirect("demo.html");
            }
        }
    }
}