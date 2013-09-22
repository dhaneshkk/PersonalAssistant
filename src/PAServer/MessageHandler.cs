//-----------------------------------------------------------------------
// <copyright file="LogMessageHandler.cs" company="">
//     Copyright (c) dhanesh, . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace PAServer
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.IO;
    using System.Threading.Tasks;
    using Disruptor;

    public class MessageEntity
    {
        public string Type { get; set; }
        public string DateAndTime { get; set; }
        public string Message { get; set; }
        public MessageEntity(string type, string dateTime, string message)
        {
            Type = type;
            DateAndTime = dateTime;
            Message = message;
        }
    }
    public interface MessageProcessor
    {
        void Processor(MessageEntity message);
    }
    class QuoteProcessor:MessageProcessor
    {
        private readonly StreamWriter writer;

        public QuoteProcessor(string path)
        {
            writer = new StreamWriter(path, true);
        }

        public void Processor(MessageEntity data)
        {
            AppendData(data);
            writer.Flush();
            
        }

        private void AppendData(MessageEntity data)
        {
            writer.Write(data.Message);
            writer.Write(Environment.NewLine);
        }

        public void Dispose()
        {
            writer.Dispose();
        }
    }
    class DictionaryProcessor : MessageProcessor
    {
        private readonly StreamWriter writer;

        public DictionaryProcessor(string path)
        {
            writer = new StreamWriter(path, true);
        }

        public void Processor(MessageEntity data)
        {
            AppendData(data);
            writer.Flush();

        }

        private void AppendData(MessageEntity data)
        {
            writer.Write(data.Message);
            writer.Write(Environment.NewLine);
        }

        public void Dispose()
        {
            writer.Dispose();
        }
    }
    /// <summary>
    /// TODO: Provide summary section in the documentation header.
    /// </summary>
    public class MessageHandler : IEventHandler<EventMessage>
    {
        private readonly StreamWriter _writer;
       
        public void OnNext(EventMessage data, long sequence, bool endOfBatch)
        {
            process(data.Message);
        }

        private void process(string msg)
        {
            if (!string.IsNullOrWhiteSpace(msg))
            {
                MessageEntity msgEntity = ParseMessage(msg);
                MessageProcessor msgP = ProcessorFactory(msgEntity.Type);
                msgP.Processor(msgEntity);
            }
        }

        private static MessageEntity ParseMessage(string msg)
        {
            string[] msgParts = msg.Split(new char[] { '|' });
            return (new MessageEntity(msgParts[0], msgParts[1], msgParts[2]));
        }

        private MessageProcessor ProcessorFactory(string processId)
        {
            MessageProcessor MsgProcessor = null;
            switch (processId)
            {
                case "d":
                    MsgProcessor = new DictionaryProcessor("dictionary.txt");
                    break;
                case "q":
                    MsgProcessor = new QuoteProcessor("Quote.txt");
                    break;
            }
            return MsgProcessor;
        }
        
    }

    public class ObjectPersistHandler : IEventHandler<EventMessage>, IDisposable
    {
        private readonly StreamWriter _writer;

        public ObjectPersistHandler(string path)
        {
            _writer = new StreamWriter(path, true);
        }

        public void OnNext(EventMessage data, long sequence, bool endOfBatch)
        {
            AppendData(data);
            if (endOfBatch)
            {
                _writer.Flush();
            }
        }

        private void AppendData(EventMessage data)
        {
            _writer.Write(data.Message);
            _writer.Write(Environment.NewLine);
        }

        public void Dispose()
        {
            _writer.Dispose();
        }
    }
}
