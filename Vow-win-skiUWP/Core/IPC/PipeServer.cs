using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vow_win_skiUWP.Views;

namespace Vow_win_skiUWP.Core.IPC
{



    public class PipeServer
    {
        private static PipeServer _instance;
        private List<Message> Messages;
        private List<Message> History;

        private ObservableCollection<Message> WaitingMessages;
        private ObservableCollection<Message> ReceivedMessages;


        public static void InitServer()
        {
            if (_instance == null)
            {
                _instance = new PipeServer();
            }

        }
        public static PipeServer GetServer => _instance;

        public PipeServer()
        {
            Build();
        }

        public void Build()
        {
            Messages = new List<Message>();
            History = new List<Message>();
            WaitingMessages = new ObservableCollection<Message>();
            ReceivedMessages = new ObservableCollection<Message>();
        }


        public ObservableCollection<Message> ShowWaiting()
        {
            return WaitingMessages;
        }

        public ObservableCollection<Message> ShowHistory()
        {
            return ReceivedMessages;
        }


        public void SendMessage(string message, string receiver, string sender)
        {
            Messages.Add(new Message(message, receiver, sender));
            History.Add(new Message(message, receiver, sender));
            WaitingMessages.Add(new Message(message, receiver, sender));
        }


        public bool ReadMessage(string receiver)
        {
            if (Messages.All(x => x.GetReceiverId != receiver))
            {
                return false;
            }
            else
            {
                ReceivedMessages.Add(Messages.Find(x => x.GetReceiverId == receiver));
                WaitingMessages.Remove(Messages.Find(x => x.GetReceiverId == receiver));
                Messages.Remove(Messages.Find(x => x.GetReceiverId == receiver).PrintMessage());
                return true;
            }
        }
    }
}
