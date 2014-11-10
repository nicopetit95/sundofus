using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SunDofus.Utilities
{
    class Loggers
    {
        public static Logger Status { get; set; }
        public static Logger Errors { get; set; }

        public static void InitializeLoggers()
        {
            Status = new Logger("Status", Basic.Locker, ConsoleColor.Green);
            Errors = new Logger("Errors", Basic.Locker, ConsoleColor.Yellow);

            Status.Write(string.Format("--> Loggers loaded and started"));
        }

        public class Logger
        {
            private string name;
            private object locker;
            private bool write;
            private ConsoleColor color;

            public Logger(string text, object locker, ConsoleColor color, bool write = true)
            {
                this.name = text;
                this.locker = locker;
                this.color = color;
                this.write = write;
            }

            public void Write(string text, bool line = true)
            {
                if (!write)
                    return;

                lock (locker)
                {
                    Console.ForegroundColor = color;
                    Console.Write(string.Format("{0} > {1} > ", DateTime.Now.ToString(), name));
                    Console.ResetColor();
                    Console.Write(string.Concat(text, (line ? Environment.NewLine : "")));
                }
            }
        }
    }
}
