using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using PFAuth;
using System.Collections.Specialized;
using System.Net;
using System.Reflection;

public partial class test : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
       Response.Write(HPUID.PF_Authentication("PF_Auth URL","subject"));
       Response.Write("</br>");
       Response.Write(HPUID.PF_Authentication("PF_Auth URL", "uid"));
    }
}