using Common.Communication;
using Common.Domain;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.Remoting.Channels;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Client
{
    public class Communication
    {
        private Receiver receiver;
        private Sender sender;
        private Socket socket;
        private bool isRunning;
        private Thread updateThread;
        private FrmLogin frmLogin;
        private FrmMain frmMain;
        private List<User> users = new List<User>();
        public List<User> Users { get => users; set => users = value; }
        private List<Message> messages = new List<Message>();
        public List<Message> Messages { get => messages; set => messages = value; }

        private static Communication instance;


        public static Communication Instance 
        { get 
            { 
                if (instance == null) instance = new Communication();
                return instance;
            } 
        }



        public Communication()
        {

        }
        public void Connect(FrmLogin frm)
        {
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.Connect("127.0.0.1", 9999);
            sender = new Sender(socket);
            receiver = new Receiver(socket);
            frmLogin = frm;
        }

        public void Close()
        {
            StopListening();
            socket.Close();
        }

        public void StartListening(FrmMain frm)
        {
            frmMain = frm;
            isRunning = true;
            updateThread = new Thread(SendRequest);
            updateThread.Start();
        }

        public void StopListening()
        {
            isRunning = false;
        }

        private void SendRequest()
        {
            while(isRunning)
            {
                try
                {
                    Users = (List<User>)UpdateTable().Result;
                    frmMain.SetUsers();
                    Messages = (List<Message>)GetMessages(frmMain.User).Result;
                    frmMain.SetMessages();
                    Thread.Sleep(5000);
                }
                catch(Exception e) 
                {
                    Debug.WriteLine(e.Message);
                }
            }
        }


        internal Response GetMessages(User user)
        {
            Request req = new Request
            {
                Argument = user,
                Operation = Operation.GetMessages
            };
            sender.Send(req);
            Response r = (Response)receiver.Receive();
            return r;
        }

        internal Response UpdateTable()
        {
            Request req = new Request
            {
                Operation = Operation.UpdateTable
            };
            sender.Send(req);
            Response r = (Response)receiver.Receive();
            return r;
        }

        internal Response Login(User user)
        {
            Request req = new Request
            {
                Argument = user,
                Operation = Operation.Login
            };
            sender.Send(req);
            Response r = (Response)receiver.Receive();
            return r;
        }

        internal Response Logout(User user)
        {
            Request req = new Request
            {
                Argument = user,
                Operation = Operation.Logout
            };
            sender.Send(req);
            Response r = (Response)receiver.Receive();
            return r;
        }

        internal Response SendMessage(Message msg)
        {
            Request req = new Request
            {
                Argument = msg,
                Operation = Operation.SendMessage
            };
            sender.Send(req);
            Response r = (Response)receiver.Receive();
            return r;
        }

        internal Response SendAll(Message msg)
        {
            Request req = new Request
            {
                Argument = msg,
                Operation = Operation.SendAll
            };
            sender.Send(req);
            Response r = (Response)receiver.Receive();
            return r;
        }
    }
}
