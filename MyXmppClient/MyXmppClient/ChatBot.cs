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
            string domain = "conf.hipchat.com";

            _xmppClient.SetUsername(botUserName);
            _xmppClient.SetXmppDomain(domain);
            _xmppClient.Password = "bottheeva";
            //_xmppClient.Show = Matrix.Xmpp.Show.chat;

            _xmppClient.OnLogin += new EventHandler<Matrix.EventArgs>(_xmppClient_OnLogin);
            _xmppClient.OnMessage += new EventHandler<Matrix.Xmpp.Client.MessageEventArgs>(_xmppClient_OnMessage);
            _xmppClient.OnPresence += new EventHandler<Matrix.Xmpp.Client.PresenceEventArgs>(_xmppClient_OnPresence);
            _xmppClient.AutoRoster = false;

            try
            {
                _xmppClient.Open();
                
                Thread.Sleep(10000);
            }
            catch
            {
                Console.WriteLine("Login Failed");
            }
            

            if (_loggedIn)
            {
                MucManager manager = new MucManager(_xmppClient);
                Jid roomJid = new Jid("114716_tavisca@conf.hipchat.com");
                manager.EnterRoom(roomJid, "lucy");
                _xmppClient.Send(new Message(roomJid,"This is test"));
                 _presenceManager = new PresenceManager(_xmppClient);
                Jid sub_jid = new Jid("114716_1163344@chat.hipchat.com");
                _presenceManager.Subscribe(sub_jid);
                do
                {
                    Thread.Sleep(100);
                } while (true);
            }
            _xmppClient.Close();
        }

        void _xmppClient_OnMessage(object sender, Matrix.Xmpp.Client.MessageEventArgs e)
        {
            string user = e.Message.From.User;
            string message = e.Message.Body;
            if(message!=null)
                if (IsAddressedToBot(message))
                {
                    message = message.Replace("@lucy","");
                    message = message.TrimStart().TrimEnd();
                    _xmppClient.Send(new Message(user + e.Message.From.Server, MessageType.chat, message));
                    Console.WriteLine(message);
                }
        }

        private string ParseMessage(string message)
        {
            var tempMessage = message.Replace("@lucy", " ");
            tempMessage = message.TrimStart().TrimEnd();
            Console.WriteLine(tempMessage);
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
            /*int i = 0;
            while (i < 10)
            {
                Console.Write(".");
                i++;
                Thread.Sleep(500);
            }*/
            _loggedIn = true;
            Console.WriteLine("Successfully logged in!!");
        }
    }
}
