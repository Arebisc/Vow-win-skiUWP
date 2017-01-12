using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vow_win_skiUWP.Log;

namespace Vow_win_skiUWP.Core.IPC
{
    public class Message
    {
        private Reporter reporter;
        private string senderId;
        private string receiverId;
        private string message;

        //===================================================================================================================================
        public Message(string message, string receiverId, string senderId)
        {
            reporter = new Reporter();
            this.receiverId = receiverId;
            this.message = message;
            this.senderId = senderId;
        }
        //===================================================================================================================================
        public string GetReceiverId()
        {
            return receiverId;
        }
        public string GetMessage()
        {
            return message;
        }
        public string GetSenderId()
        {
            return senderId;
        }
        //===================================================================================================================================
        public override string ToString()
        {
            return receiverId + " received from " + senderId + " : " + message;
        }

        public Message PrintMessage()
        {
            Console.WriteLine(ToString());
            reporter.AddLog(ToString());
            return this;
        }
    }
}
