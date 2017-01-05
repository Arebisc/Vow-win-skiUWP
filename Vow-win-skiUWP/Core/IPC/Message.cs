using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vow_win_skiUWP.Core.IPC
{
    public class Message
    {
        private string senderId;
        private string receiverId;
        private string message;

        //===================================================================================================================================
        public Message(string message, string receiverId, string senderId)
        {
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
            return this;
        }
    }
}
