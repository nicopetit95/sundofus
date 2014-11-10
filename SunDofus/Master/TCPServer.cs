using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SilverSock;

namespace SunDofus.Master
{
    class TCPServer
    {
        private SilverServer server;
        private string remote;

        protected delegate void AcceptSocketHandler(SilverSocket socket);
        protected AcceptSocketHandler SocketClientAccepted;

        private void OnSocketClientAccepted(SilverSocket socket)
        {
            var evnt = SocketClientAccepted;
            if (evnt != null)
                evnt(socket);
        }

        protected delegate void ListeningServerHandler(string remote);
        protected ListeningServerHandler ListeningServer;

        private void OnListeningServer(string remote)
        {
            var evnt = ListeningServer;
            if (evnt != null)
                evnt(remote);
        }

        protected delegate void ListeningServerFailedHandler(Exception e);
        protected ListeningServerFailedHandler ListeningServerFailed;

        private void OnListeningServerFailed(Exception exception)
        {
            var evnt = ListeningServerFailed;
            if (evnt != null)
                evnt(exception);
        }

        public TCPServer(string ip, int port)
        {
            remote = string.Format("{0}:{1}", ip, port);

            server = new SilverServer(ip, port);
            {
                server.OnAcceptSocketEvent += new SilverEvents.AcceptSocket(this.AcceptSocket);
                server.OnListeningEvent += new SilverEvents.Listening(this.OnListen);
                server.OnListeningFailedEvent += new SilverEvents.ListeningFailed(this.OnListenFailed);
            }
        }

        public void Start()
        {
            server.WaitConnection();
        }

        #region Events

        private void AcceptSocket(SilverSocket socket)
        {
            OnSocketClientAccepted(socket);
        }

        private void OnListen()
        {
            OnListeningServer(remote);
        }

        private void OnListenFailed(Exception exception)
        {
            OnListeningServerFailed(exception);
        }

        #endregion
    }
}
