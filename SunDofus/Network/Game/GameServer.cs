using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SilverSock;
using SunDofus.Entities.Models.Clients;
using SunDofus.Network.Realm;

namespace SunDofus.Network.Game
{
    class GameServer : Master.TCPServer
    {
        public GameState State = GameState.Offline;
        public List<GameClient> Clients { get; private set; }
        public List<GameTicket> Tickets { get; private set; }

        public GameServer()
            : base(Program.Config.GameAddress, Program.Config.GamePort)
        {
            Clients = new List<GameClient>();
            Tickets = new List<GameTicket>();

            this.SocketClientAccepted += new AcceptSocketHandler(this.OnAcceptedClient);
            this.ListeningServer += new ListeningServerHandler(this.OnListeningServer);
            this.ListeningServerFailed += new ListeningServerFailedHandler(this.OnListeningFailedServer);
        }

        public void OnAcceptedClient(SilverSocket socket)
        {
            if (socket == null) 
                return;

            lock(Clients)
                Clients.Add(new GameClient(socket));
        }

        public void OnListeningServer(string remote)
        {
            State = GameState.Online;
            Utilities.Loggers.Status.Write(string.Format("RealmServer started on <{0}> !", remote));
        }

        public void OnListeningFailedServer(Exception exception)
        {
            Utilities.Loggers.Errors.Write(string.Concat("Cannot start the RealmServer because : ", exception.ToString()));
        }

        public class GameTicket
        {
            public AccountModel Model { get; private set; }
            public string Key { get; private set; }
            public GameTicket(RealmClient client, string key)
            {
                Model = client.Account;
                Key = key;
            }
        }

        public enum GameState
        {
            Offline = 0,
            Online = 1,
            Maintenance = 2
        }
    }
}
