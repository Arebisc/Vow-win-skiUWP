﻿using System;
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
           
            WaitingMessages.Add(new Message(message, receiver, sender));
        }


        public bool ReadMessage(string receiver)
        {
            if (WaitingMessages.All(x => x.GetReceiverId != receiver))
            {
                return false;
            }
            else
            {
                foreach (var item in WaitingMessages)
                {
                    if (item.GetReceiverId == receiver)
                    {
                        ReceivedMessages.Add(item);
                        WaitingMessages.Remove(item);
                        break;
                    }
                }             
                return true;
            }
        }
    }
}
