using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Microsoft.Web.WebSockets;

namespace Reports.Workers.UI.Controllers
{
    public class WebSocketsController : ApiController
    {
        public HttpResponseMessage Get()
        {
            if (System.Web.HttpContext.Current.IsWebSocketRequest)
            {
                System.Web.HttpContext.Current.AcceptWebSocketRequest(new Handlers.ProcessWebSocketHandler());
                return Request.CreateResponse(HttpStatusCode.SwitchingProtocols);
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }
        }
    }
}
