//----------------------------------------------------------------------------------
// Pub Socket Sample
// Author: Manar Ezzadeen
// Blog  : http://idevhawk.phonezad.com
// Email : idevhawk@gmail.com
//----------------------------------------------------------------------------------

namespace PAInterface
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading;
    using CommandLine;
    using ZeroMQ;

    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                var options = ProcessCommandLineArgs(args);

                SendLog(options);
            }
            catch (Exception exp)
            {
                Console.WriteLine(exp.Message);
            }
        }

        private static Options ProcessCommandLineArgs(string[] args)
        {
            var options = new Options();
            var parser = new CommandLineParser(new CommandLineParserSettings(Console.Error));
            if (!parser.ParseArguments(args, options))
                Environment.Exit(1);
            return options;
        }

        private static void SendLog(Options options)
        {
            using (var ctx = ZmqContext.Create())
            {
                using (var socket = ctx.CreateSocket(SocketType.PUB))
                {
                    foreach (var endPoint in options.bindEndPoints)
                        socket.Bind(endPoint);
                        var msg = options.altMessages[0];
                        Thread.Sleep(options.delay);
                        socket.Send(msg, Encoding.UTF8);
                        Thread.Sleep(options.delay);
                    }
                }
            }

        }
    }

