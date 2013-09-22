//----------------------------------------------------------------------------------
// Sub Socket Sample
// Author: Manar Ezzadeen
// Blog  : http://idevhawk.phonezad.com
// Email : idevhawk@gmail.com
//----------------------------------------------------------------------------------

namespace PAServer
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading;
    using CommandLine;
    using ZeroMQ;
    using System.Threading.Tasks;
    using Disruptor;

    class Program
    {
        private const int RingBufferSize = 1 << 11; // 2048
        private const int ObjectsCount = RingBufferSize / 2;

        static void Main(string[] args)
        {
            try
            {
                var disruptor = new Disruptor.Dsl.Disruptor<EventMessage>(() => new EventMessage(), RingBufferSize, TaskScheduler.Default);
                var storageLocation = System.IO.Path.Combine(Environment.CurrentDirectory, "test.log"); 
                var options = new Options();
                var parser = new CommandLineParser(new CommandLineParserSettings(Console.Error));
                if (!parser.ParseArguments(args, options))
                    Environment.Exit(1);

                using(var ctx = ZmqContext.Create())
                {
                    using (var socket = ctx.CreateSocket(SocketType.SUB))
                    {
                        if (options.subscriptionPrefixes.Count() == 0)
                            socket.SubscribeAll();
                        else
                            foreach (var subscriptionPrefix in options.subscriptionPrefixes)
                                socket.Subscribe(Encoding.UTF8.GetBytes(subscriptionPrefix));

                        foreach (var endPoint in options.connectEndPoints)
                            socket.Connect(endPoint);
            
                        using (var persistHandler = new ObjectPersistHandler(storageLocation))
                        {
                            disruptor.HandleEventsWith(persistHandler).Then(new MessageHandler());
                            var ringBuffer = disruptor.Start();
                            while (true)
                            {
                                Thread.Sleep(options.delay);
                                var msg = socket.Receive(Encoding.UTF8);
                                string eventMessage = msg;
                                if (!string.IsNullOrWhiteSpace(eventMessage))
                                {
                                    long sequenceNo = ringBuffer.Next();
                                    EventMessage entry = ringBuffer[sequenceNo];
                                    entry.Message = eventMessage;
                                    ringBuffer.Publish(sequenceNo);
                                    Console.WriteLine("Published entry {0}, value {1}", sequenceNo, entry.Message);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception exp)
            {
                Console.WriteLine(exp.Message);
            }
        }
    }
}
