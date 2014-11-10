using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SilverSock;
using SunDofus.Entities.Models.Clients;
using SunDofus.Network.Game;

namespace SunDofus.Network.Realm
{
    class RealmClient : Master.TCPClient
    {
        public AccountModel Account { get; set; }
        public AccountState State { get; set; }

        private object locker;
        private string key;

        public RealmClient(SilverSocket socket) : base(socket)
        {
            locker = new object();
            key = Utilities.Basic.RandomString(32);

            this.DisconnectedSocket += new DisconnectedSocketHandler(this.Disconnected);
            this.ReceivedDatas += new ReceiveDatasHandler(this.PacketReceived);

            Send(string.Concat("HC", key));
        }

        public void SendInformations()
        {
            Send(string.Concat("Ad", Account.Pseudo));
            Send("Ac0");

            RefreshHosts();

            Send(string.Concat("AlK", Account.Level));
            Send(string.Concat("AQ", Account.Question));
        }

        public void RefreshHosts()
        {
            Send(string.Format("AH{0};{1};{2};1",
                Program.Config.GameID, (int)Program.GameServer.State, (Program.Config.GameID * 75)));
        }

        public void Send(string message)
        {
            lock (locker)
                this.SendDatas(message);
        }

        public void Send(string format, params object[] args)
        {
            Send(string.Format(format, args));
        }

        private void PacketReceived(string datas)
        {
            lock (locker)
                Parse(datas);
        }

        private void Disconnected()
        {
            lock (Program.RealmServer.Clients)
                Program.RealmServer.Clients.Remove(this);
        }

        private void Parse(string datas)
        {
            switch (State)
            {
                case AccountState.OnCheckingVersion:
                    ParseVersionPacket(datas);
                    return;

                case AccountState.OnCheckingAccount:
                    CheckAccount(datas);
                    return;

                case AccountState.OnServersList:
                    ParseListPacket(datas);
                    return;
            }
        }

        private void CheckAccount(string datas)
        {
            if (!datas.Contains("#1"))
                return;

            var infos = datas.Split('#');
            var username = infos[0];
            var password = infos[1];

            var requestedAccount = Entities.Requests.AccountsRequests.LoadAccount(username);

            if (requestedAccount != null && Utilities.Basic.Encrypt(requestedAccount.Password, key) == password)
            {
                Account = requestedAccount;

                if (Program.GameServer.Clients.Count >= Program.Config.MaxPlayer)
                {
                    Send("M00\0");
                    Disconnect();
                }
                else
                {
                    SendInformations();
                    State = AccountState.OnServersList;
                }
            }
            else
            {
                Send("AlEx");
                this.Disconnect();
            }
        }

        private void ParseVersionPacket(string datas)
        {
            if (datas.Contains("1.29.1"))
                State = AccountState.OnCheckingAccount;
            else
            {
                Send("AlEv1.29.1");
                this.Disconnect();
            }
        }

        private void ParseListPacket(string datas)
        {
            if (datas.Substring(0, 1) != "A")
                return;

            var packet = string.Empty;

            switch (datas[1])
            {
                case 'F':

                    Send(string.Concat("AF", Program.Config.GameID));
                    return;

                case 'x':

                    Send("AxK{0}|{1},{2}", Account.SubscriptionTime(),
                        Program.Config.GameID, Account.Characters[Program.Config.GameID].Count);
                    return;

                case 'X':

                    var id = 0;

                    if (!int.TryParse(datas.Substring(2), out id))
                        return;

                    if (Program.Config.GameID != id)
                    {
                        Send("BN");
                        Disconnect();
                    }

                    var key = Utilities.Basic.RandomString(16);

                    lock (Program.GameServer.Tickets)
                        Program.GameServer.Tickets.Add(new GameServer.GameTicket(this, key));

                    Send("AYK{0}:{1};{2}", Program.Config.GameAddress, Program.Config.GamePort, key);
                    return;
            }
        }

        public enum AccountState
        {
            OnCheckingVersion,
            OnCheckingAccount,
            OnCheckingQueue,
            OnServersList,
        }
    }
}
