using Common.Communication;
using Common.Domain;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    public class ClientHandler
    {
        private Sender sender;
        private Receiver receiver;
        private Socket socket;
        private bool kraj = false;
        private User loggedInUser = null;
        public User LoggedInUser { get => this.loggedInUser; set => this.loggedInUser = value; }


        public ClientHandler(Socket socket)
        {
            this.socket = socket;
            sender = new Sender(socket);
            receiver = new Receiver(socket);
        }

        public void HandleRequest()
        {
            while (!kraj)
            {
                Request req = (Request)receiver.Receive();
                Response r = ProcessRequest(req);
                sender.Send(r);
            }
        }

        private Response ProcessRequest(Request req)
        {
            Response r = new Response();

            try
            {
                switch (req.Operation)
                {
                    case Operation.Login:
                        r.Result = Controller.Instance.Login((User)req.Argument);
                        if(r.Result != null)
                        {
                            loggedInUser = (User)r.Result;
                        }
                        else
                        {
                            throw new Exception();
                        }    
                        break;
                    case Operation.Logout:
                        r.Result = Controller.Instance.Logout((User)req.Argument);
                        if((bool)r.Result == true)
                        {
                            //kraj = true;
                            //socket.Close();
                        }
                        else
                        {
                            throw new Exception();
                        }
                        break;
                    case Operation.UpdateTable:
                        r.Result = Controller.Instance.Users;
                        break;
                    case Operation.SendMessage:
                        r.Result = Controller.Instance.SendMessage((Message)req.Argument);
                        break;
                    case Operation.GetMessages:
                        r.Result = Controller.Instance.GetMessages((User)req.Argument);
                        break;
                }
            }
            catch (Exception ex)
            {
                r.Exception = ex;
                Debug.WriteLine(ex.Message);
            }
            return r;
        }

    }
}
