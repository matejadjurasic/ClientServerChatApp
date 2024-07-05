using Common.Domain;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBBroker
{
    public class Broker
    {
        private DbConnection connection;

        public Broker() 
        {
            connection = new DbConnection();      
        }

        public void OpenConnection()
        {
            connection.OpenConnection();
        }
        public void CloseConnection() 
        {
            connection.CloseConnection();
        }
        public void Commit()
        {
            connection.Commit();
        }
        public void Rollback()
        {
            connection.Rollback();
        }
        public void BeginTransaction()
        {
            connection.BeginTransaction();
        }

        public User Login(User user)
        {
            SqlCommand command = connection.CreateCommand();
            command.CommandText = $"select * from users where username='{user.Username}' and password='{user.Password}'";
            SqlDataReader reader = command.ExecuteReader();

            try
            {
                if (reader.Read())
                {
                    user.Id = (int)reader["id"];
                    return user;
                }
            }
            finally
            {
                reader.Close();
            }
           
            return null;
        }

        public bool SendMessage(Message msg)
        {
            SqlCommand command = connection.CreateCommand();
            command.CommandText = $"insert into message(text,senderID,receiverID,dateTime) values ('{msg.Text}',{msg.Sender.Id},{msg.Receiver.Id},CURRENT_TIMESTAMP)";
            try
            {
                command.ExecuteNonQuery();
                return true;
            }
            catch(Exception ex) 
            {
                return false;
            }
        }

        public List<Message> GetMessages(User user)
        {
            List<Message> msgs = new List<Message>();
            SqlCommand command = connection.CreateCommand();
            command.CommandText = $"select * from users u join message m on u.id = m.senderID where m.receiverID = {user.Id} order by m.dateTime desc";
            SqlDataReader reader = command.ExecuteReader();

            try
            {
                while(reader.Read())
                {
                    msgs.Add(new Message
                    {
                        Receiver = user,
                        Sender = new User
                        {
                            Username = reader["username"].ToString(),
                        },
                        Text = reader["text"].ToString(),
                        DateTime = (DateTime)reader["dateTime"]
                    });
                }
                return msgs;
            }catch(Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw ex;
            }finally { reader.Close(); }
        }
    }
}
