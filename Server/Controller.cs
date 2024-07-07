using Common.Communication;
using Common.Domain;
using DBBroker;
using Server.SystemOperations;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    public class Controller
    {
        private Broker broker;
        private List<ClientHandler> clients = new List<ClientHandler>();
        public List<ClientHandler> Clients { get => this.clients; set => this.clients = value; }
        private List<User> users = new List<User>();
        public List<User> Users { get => users; set => users = value; }
        private FrmServer frmServer;
        public FrmServer FrmServer { get => frmServer; set => frmServer = value; }


        private static Controller instance;
        public static Controller Instance 
        { 
            get 
            { 
                if (instance == null) instance = new Controller();
                return instance;
            } 
        }


        public Controller()
        {
            broker = new Broker();
        }

        public User Login(User user)
        {
            LoginSO so = new LoginSO(user);
            so.ExecuteTemplate();
            if(so.Result != null) 
            {
                if (!users.Contains(so.Result))
                {
                    users.Add(so.Result);
                    frmServer.RefreshTable();
                }
                else
                {
                    return null;
                }  
            }
            return so.Result;
        }

        public object Logout(User user)
        {
            try
            {
                users.Remove(user);
                frmServer.RefreshTable();
                foreach(ClientHandler handler in clients)
                {
                    if(user.Id == handler.LoggedInUser.Id)
                    {
                        handler.Close();
                    }
                }
                return true;

            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return false;
            }
        }

        public bool SendMessage(Message msg)
        {
            try
            {
                broker.OpenConnection();
                broker.BeginTransaction();
                broker.SendMessage(msg);
                broker.Commit();
                return true;
            }catch (Exception ex)
            {
                broker.Rollback();
                throw ex;
            }
            finally { broker.CloseConnection(); }

        }

        public List<Message> GetMessages(User user)
        {
            List<Message> messages = new List<Message>();
            try
            {
                broker.OpenConnection();
                broker.BeginTransaction();
                messages = broker.GetMessages(user);
                broker.Commit();
                return messages;
            }catch (Exception ex)
            {
                broker.Rollback() ;
                throw ex;
            }finally { broker.CloseConnection(); }
        }

        public bool SendAll(Message msg)
        {
            List<Message> msgs = new List<Message>();
            foreach(User user in users)
            {   
                if(msg.Sender.Id != user.Id)
                {
                    Message message = new Message
                    {
                        Text = msg.Text,
                        Sender = msg.Sender,
                        Receiver = user,
                    };
                    Debug.WriteLine(message.Receiver.Username);
                    msgs.Add(message);
                }
            }
            try
            {
                broker.OpenConnection();
                broker.BeginTransaction();
                broker.SendAll(msgs);
                broker.Commit();
                return true; 
            }
            catch (Exception ex)
            {
                broker.Rollback();
                throw ex;
            }
            finally { broker.CloseConnection(); }
        }

        internal bool AddUser(User user)
        {
            try
            {
                broker.OpenConnection();
                broker.BeginTransaction();
                broker.AddUser(user);
                broker.Commit();
                return true;
            }
            catch (Exception ex)
            {
                broker.Rollback();
                Debug.Write(ex.Message);
                return false;
            }
            finally { broker.CloseConnection(); }
        }
    }
}
