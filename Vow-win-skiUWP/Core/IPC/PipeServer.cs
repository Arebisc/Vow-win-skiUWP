using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vow_win_skiUWP.Core.IPC
{
    public class PipeServer
    {
        private static PipeServer _instance;
        private List<Message> Messages;
        private List<Message> History;


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
            Console.WriteLine("Tworzenie Serwera IPC.");
            Build();
        }

        public void Build()
        {
            Messages = new List<Message>();
            History = new List<Message>();
        }


        public void Show()
        {
            Console.WriteLine("Waiting messages for receive: ");
            foreach (var x in Messages)
            {
                Console.WriteLine(x.GetSenderId() + " to " + x.GetReceiverId() + " " + x.GetMessage());
            }
        }

        public void ShowHistory()
        {
            Console.WriteLine("Communication history: ");
            foreach (var x in History)
            {
                Console.WriteLine(x.GetSenderId() + " to " + x.GetReceiverId() + " " + x.GetMessage());
            }
        }


        public void SendMessage(string message, string receiver, string sender)
        {
            Messages.Add(new Message(message, receiver, sender));
            History.Add(new Message(message, receiver, sender));
        }


        public bool ReadMessage(string receiver)
        {
            if (Messages.All(x => x.GetReceiverId() != receiver))
            {
                return false;
            }
            else
            {
                Messages.Remove(Messages.Find(x => x.GetReceiverId() == receiver).PrintMessage());
                return true;
            }
        }
    }
}
