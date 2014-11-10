using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SunDofus.World.Characters
{
    class CharacterChannels
    {
        public List<Channel> Channels { get; set; }
        public Character Client { get; set; }

        public CharacterChannels(Character client)
        {
            Client = client;
            Channels = new List<Channel>();

            AddChannel('*', true);
            AddChannel('#', true);
            AddChannel('$', true);
            AddChannel('p', true);
            AddChannel('%', true);
            AddChannel('i', true);
            AddChannel(':', true);
            AddChannel('?', true);
            AddChannel('!', true);
            AddChannel('^', true);
        }

        public void AddChannel(char head, bool state)
        {
            lock (Channels)
                Channels.Add(new Channel(head, state));
        }

        public void SendChannels()
        {
            Client.NClient.Send(string.Concat("cC+", string.Join("", from c in Channels select c.Head.ToString())));
        }

        public void ChangeChannelState(char head, bool state)
        {
            if (Channels.Any(x => x.Head == head))
            {
                Channels.First(x => x.Head == head).On = state;
                Client.NClient.Send(string.Format("cC{0}{1}", (state ? "+" : "-"), head.ToString()));
            }
        }

        public class Channel
        {
            public char Head { get; set; }
            public bool On { get; set; }

            public Channel(char head, bool on)
            {
                Head = head;
                On = on;
            }
        }
    }
}
