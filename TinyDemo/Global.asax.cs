﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Routing;

namespace TinyDemo
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            GlobalConfiguration.Configure(WebApiConfig.Register);
        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            if (Request.RawUrl == "/")
            {
                Response.Redirect("demo.html");
            }
        }
    }
}
