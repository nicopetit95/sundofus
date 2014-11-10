using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SilverSock;

namespace SunDofus.Network.Realm
{
    class RealmServer : Master.TCPServer
    {
        public List<RealmClient> Clients { get; private set; }

        public RealmServer()
            : base(Program.Config.RealmAddress, Program.Config.RealmPort)
        {
            Clients = new List<RealmClient>();

            this.SocketClientAccepted += new AcceptSocketHandler(this.OnAcceptedClient);
            this.ListeningServer += new ListeningServerHandler(this.OnListeningServer);
            this.ListeningServerFailed += new ListeningServerFailedHandler(this.OnListeningFailedServer);
        }

        private void OnAcceptedClient(SilverSocket socket)
        {
            if (socket == null) 
                return;

            lock (Clients)
                Clients.Add(new RealmClient(socket));
        }

        private void OnListeningServer(string remote)
        {
            Utilities.Loggers.Status.Write(string.Format("AuthServer starded on <{0}> !", remote));
        }

        private void OnListeningFailedServer(Exception exception)
        {
            Utilities.Loggers.Errors.Write(string.Format("AuthServer can't start : {0}", exception.ToString()));
        }
    }
}
