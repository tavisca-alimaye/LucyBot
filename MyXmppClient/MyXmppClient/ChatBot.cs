using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Matrix;
using System.Diagnostics;
using Matrix.Xmpp.Client;
using Matrix.Xmpp;
using System.Threading;

namespace MyXmppClient
{
    public class ChatBot
    {
        private Matrix.Xmpp.Client.XmppClient _xmppClient;
        private Matrix.Xmpp.Client.PresenceManager _presenceManager;
        private bool _loggedIn;

        public ChatBot()
        {
            _xmppClient = new Matrix.Xmpp.Client.XmppClient();
            _loggedIn = false;
        }

        public void Start()
        {
            string botUserName = "114716_1167039";
            string domain = "chat.hipchat.com";

            _xmppClient.SetUsername(botUserName);
            _xmppClient.SetXmppDomain(domain);
            _xmppClient.Password = "bottheeva";
            _xmppClient.Show = Matrix.Xmpp.Show.chat;

            try
            {
                _xmppClient.OnLogin += new EventHandler<Matrix.EventArgs>(_xmppClient_OnLogin);
                _xmppClient.Open();
                
                Thread.Sleep(10000);
            }
            catch
            {
                Console.WriteLine("Login Failed");
            }
            

            if (_loggedIn)
            {
                _xmppClient.AutoRoster = true;
                _xmppClient.OnPresence += new EventHandler<Matrix.Xmpp.Client.PresenceEventArgs>(_xmppClient_OnPresence);
                _xmppClient.OnMessage += new EventHandler<Matrix.Xmpp.Client.MessageEventArgs>(_xmppClient_OnMessage);
                _presenceManager = new PresenceManager(_xmppClient);
                Jid sub_jid = "114716_1163344@chat.hipchat.com";
                _presenceManager.Subscribe(sub_jid, "I want to subscribe you", sub_jid.User);
                _presenceManager.OnSubscribe += _presenceManager_OnSubscribe;
                
            }
            _xmppClient.Close();
        }

        private void _xmppClient_OnError(object sender, ExceptionEventArgs e)
        {
            throw new NotImplementedException();
        }

        void _xmppClient_OnClose(object sender, Matrix.EventArgs e)
        {
            _xmppClient.Dispose();
        }

        void _presenceManager_OnSubscribe(object sender, PresenceEventArgs e)
        {
            throw new NotImplementedException();
        }

        void _xmppClient_OnMessage(object sender, Matrix.Xmpp.Client.MessageEventArgs e)
        {
            string user = e.Message.From.User;
            string message = e.Message.Body;
            if (IsAddressedToBot(message))
            {
                string remainingMessage = ParseMessage(message);
                _xmppClient.Send(new Message("114716_1167039@chat.hipchat.com", MessageType.chat, remainingMessage));
                Console.WriteLine(remainingMessage);
            }
        }

        private string ParseMessage(string message)
        {
            var tempMessage = message.Replace("@lucy", " ");
            tempMessage = message.TrimStart().TrimEnd();
            return tempMessage;
        }

        private bool IsAddressedToBot(string message)
        {
            if (message.StartsWith("@lucy"))
                return true;
            return false;
        }

        void _xmppClient_OnPresence(object sender, Matrix.Xmpp.Client.PresenceEventArgs e)
        {
            Console.WriteLine("{0}@{1} -> {2}", e.Presence.From.User, e.Presence.From.Server, e.Presence.Type);
            Console.WriteLine();
        }

        void _xmppClient_OnLogin(object sender, Matrix.EventArgs e)
        {
            int i = 0;
            while (i < 10)
            {
                Console.Write(".");
                i++;
                Thread.Sleep(500);
            }
            _loggedIn = true;
            Console.WriteLine("Successfully logged in!!");
        }
    }
}
