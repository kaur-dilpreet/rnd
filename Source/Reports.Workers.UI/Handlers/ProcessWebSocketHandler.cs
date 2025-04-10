using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Web.WebSockets;
using System.Web.Script.Serialization;
using System.Threading;

namespace Reports.Workers.UI.Handlers
{
    public class ProcessWebSocketHandler : WebSocketHandler
    {
        private static WebSocketCollection _procClients = new WebSocketCollection();
        private String ClientId { get; set; }
        private Core.Domain.Models.WebSocketUserId WebSocketUserId { get; set; }
        private String Controller { get; set; }
        private String Action { get; set; }
        private Guid UniqueId { get; set; }
        private Guid ReportId { get; set; }
        private String LastAdminReportsMessageSent { get; set; }
        private String LastUserExportsMessageSent { get; set; }

        public delegate void Worker();
        private static Thread _worker;
        private static Boolean _initialized { get; set; }

        private static Core.Utilities.IUtilities _utilities;
        private static Core.Utilities.ISettings _settings;
        private static Core.Encryption.BlowFish _blowFish;
        private static Core.Encryption.IHash _hash;
        private static BLL.Providers.IWebSocketsProvider _webSocketsProvider;

        public ProcessWebSocketHandler()
        {
            _utilities = Microsoft.Practices.ServiceLocation.ServiceLocator.Current.GetInstance<Core.Utilities.IUtilities>();
            _settings = Microsoft.Practices.ServiceLocation.ServiceLocator.Current.GetInstance<Core.Utilities.ISettings>();
            _blowFish = new Core.Encryption.BlowFish(_settings.GetSettings(Core.Domain.Enumerations.AppSettings.HashKey));
            _hash = Microsoft.Practices.ServiceLocation.ServiceLocator.Current.GetInstance<Core.Encryption.IHash>();
            _webSocketsProvider = Microsoft.Practices.ServiceLocation.ServiceLocator.Current.GetInstance<BLL.Providers.IWebSocketsProvider>();
        }

        //public ProcessWebSocketHandler(Core.Domain.Enumerations.WebSocketType webSocketType)
        //{
        //    this.Controller = webSocketType;
        //}

        public override void OnOpen()
        {
            if (!_procClients.Contains(this))
            {
                if (!String.IsNullOrWhiteSpace(this.WebSocketContext.QueryString["userid"]) && this.WebSocketContext.QueryString["userid"].Trim() != "undefined" && !String.IsNullOrWhiteSpace(this.WebSocketContext.QueryString["controller"]) && !String.IsNullOrWhiteSpace(this.WebSocketContext.QueryString["action"]))
                {
                    this.ClientId = this.WebSocketContext.QueryString["userid"].Trim();

                    String serializedClientId = _blowFish.Decrypt_ECB(_utilities.ByteToString(Convert.FromBase64String(this.ClientId)));

                    this.WebSocketUserId = _utilities.Deserialize<Core.Domain.Models.WebSocketUserId>(serializedClientId);
                    this.Controller = this.WebSocketContext.QueryString["controller"].ToLower();
                    this.Action = this.WebSocketContext.QueryString["action"].ToLower();
                    this.UniqueId = Guid.NewGuid();

                    Core.Domain.Models.SocketMessage<Guid> message = new Core.Domain.Models.SocketMessage<Guid>()
                    {
                        MessageType = "SocketId",
                        Data = this.UniqueId
                    };

                    this.Send(_utilities.Serialize(message));

                    _procClients.Add(this);

                    if (!_initialized)
                    {
                        _initialized = true;
                        Init(Work);
                    }
                }
            }
        }

        public override void OnClose()
        {
            ProcessWebSocketHandler handler = this;

            _procClients.Remove(this);

            if (_procClients.Count == 0)
            {
                _initialized = false;

                if (_worker != null)
                    _worker.Abort();
            }
        }

        public static Boolean IsUserOnline(Int64 userId)
        {
            return _procClients.Any(c => ((ProcessWebSocketHandler)c).WebSocketUserId.UserId == userId);
        }

        public static Boolean IsUserOnline(Guid userUniqueId)
        {
            return _procClients.Any(c => ((ProcessWebSocketHandler)c).WebSocketUserId.UserUniqueId == userUniqueId);
        }


        public static void Init(Worker work)
        {
            _worker = new Thread(new ThreadStart(work));
            _worker.Start();
        }

        public static void Work()
        {
            while (_initialized)
            {
                List<KeyValuePair<Guid, String>> conversationBlocks = new List<KeyValuePair<Guid, String>>();

                String message = String.Empty;
                String hashedMessage = String.Empty;

                foreach (ProcessWebSocketHandler handler in _procClients.OrderBy(c => ((ProcessWebSocketHandler)c).WebSocketUserId.UserId))
                {
                    try
                    {
                        #region Reports

                        if (handler.Controller == "admin" && handler.Action == "reports")
                        {
                            Core.Domain.Models.WSReports reports = _webSocketsProvider.GetReports(handler.WebSocketUserId.UserId);

                            Core.Domain.Models.SocketMessage<Core.Domain.Models.WSReports> taskMessages = new Core.Domain.Models.SocketMessage<Core.Domain.Models.WSReports>()
                            {
                                MessageType = "ReportsUpdated",
                                Data = reports
                            };

                            message = _utilities.Serialize(taskMessages);

                            hashedMessage = _hash.GetHash(message, _settings.GetSettings(Core.Domain.Enumerations.AppSettings.HashKey));

                            if (String.IsNullOrEmpty(handler.LastAdminReportsMessageSent))
                                handler.LastAdminReportsMessageSent = hashedMessage;

                            if (handler.LastAdminReportsMessageSent != hashedMessage)
                            {
                                handler.Send(message);
                                handler.LastAdminReportsMessageSent = hashedMessage;
                            }
                        }
                        else if (handler.Controller == "reports" && handler.Action == "exports")
                        {
                            Core.Domain.Models.WSExports exports = _webSocketsProvider.GetExports(handler.WebSocketUserId.UserId);

                            Core.Domain.Models.SocketMessage<Core.Domain.Models.WSExports> taskMessages = new Core.Domain.Models.SocketMessage<Core.Domain.Models.WSExports>()
                            {
                                MessageType = "ExportsUpdated",
                                Data = exports
                            };

                            message = _utilities.Serialize(taskMessages);

                            hashedMessage = _hash.GetHash(message, _settings.GetSettings(Core.Domain.Enumerations.AppSettings.HashKey));

                            if (String.IsNullOrEmpty(handler.LastUserExportsMessageSent))
                                handler.LastUserExportsMessageSent = hashedMessage;

                            if (handler.LastUserExportsMessageSent != hashedMessage)
                            {
                                handler.Send(message);
                                handler.LastUserExportsMessageSent = hashedMessage;
                            }
                        }

                        #endregion
                    }
                    catch (Exception ex)
                    {

                    }
                }

                Thread.Sleep(1000);
            }
        }

        public override void OnMessage(string jsonData)
        {
            String test = jsonData;

            Core.Domain.Models.SocketMessage<String> message = _utilities.Deserialize<Core.Domain.Models.SocketMessage<String>>(jsonData);

            Thread thread;

            switch (message.MessageType)
            {
                case "ReportId":
                    thread = new Thread(new ThreadStart(() => SetReportId(message)));
                    thread.Start();

                    break;
            }
        }

        private void SetReportId(Core.Domain.Models.SocketMessage<String> message)
        {
            Core.Domain.Models.WSReportIdIncommingMessage data = _utilities.Deserialize<Core.Domain.Models.WSReportIdIncommingMessage>(message.Data);

            Guid reportId;

            if (Guid.TryParse(data.ReportUniqueId, out reportId))
            {
                if (message.MessageType == "ReportId")
                {
                    Core.Domain.Models.WebSocketUserId webSocketUserId = _utilities.Deserialize<Core.Domain.Models.WebSocketUserId>(_blowFish.Decrypt_ECB(_utilities.ByteToString(Convert.FromBase64String(message.UserId))));

                    foreach (ProcessWebSocketHandler handler in _procClients)
                    {
                        if (handler.ClientId == message.UserId && handler.UniqueId == message.UniqueId)
                        {
                            handler.ReportId = reportId;

                            break;
                        }
                    }
                }
            }
        }
    }
}
