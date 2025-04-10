using System;
using System.Security.Cryptography;
using PFAuth;

namespace OktaSampleWebApp
{
    public partial class Test : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string uid = HPUID.PF_Authentication("http://localhost:60242/PF_Auth.aspx", "uid");

            lblMessage.Text = $"{uid}{" you are already authenticated"}";

            //lblMessage.Text = $"{" you are loggedout"}";
        }
    }
}