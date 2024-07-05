using Common.Communication;
using Common.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Message = Common.Domain.Message;

namespace Client
{
    public partial class FrmMain : Form
    {
        private User user;
        private User selectedUser;
        private FrmLogin frmLogin;

        public User User { get => user; set => user = value; }

        public FrmMain(User user,FrmLogin frm)
        {
            InitializeComponent();
            this.user = user;
            frmLogin = frm;
            lstMessages.View = View.Details;
            lstMessages.Columns.Add("Username", 150, HorizontalAlignment.Left);
            lstMessages.Columns.Add("Text", 250, HorizontalAlignment.Left);
            lstMessages.Columns.Add("DateTime", 150, HorizontalAlignment.Left);
            Communication.Instance.StartListening(this);
        }

        private void lstUsers_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(lstUsers.SelectedIndex != -1)
            {
                selectedUser = lstUsers.SelectedItem as User;
            }
        }

        public void SetUsers()
        {
            if (InvokeRequired)
            {
                Invoke(new Action(SetUsers));
                return;
            }
            lstUsers.DataSource = null;
            lstUsers.DataSource = Communication.Instance.Users;
            lstUsers.DisplayMember = "Username";
            lstUsers.SelectedIndex = -1;
        }

        private void btnLogout_Click(object sender, EventArgs e)
        {
            Response r = Communication.Instance.Logout(user);
            if (r.Exception == null)
            {
                Dispose();
                frmLogin.Visible = true;
                Communication.Instance.StopListening();
            }
            else
            {
                MessageBox.Show("Error logging out");
            }
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            Message msg = new Message();
            msg.Text = txtMessage.Text;
            msg.Sender = user;
            msg.Receiver = selectedUser;
            Response r = Communication.Instance.SendMessage(msg);
            if (r.Exception == null && (bool)r.Result == true) 
            {
                MessageBox.Show("Success!");
                txtMessage.Text = "";
            }
            else
            {
                MessageBox.Show("Something went wrong");
            }
        }

        public void SetMessages()
        {
            if (InvokeRequired)
            {
                Invoke(new Action(SetMessages));
                return;
            }
            lstMessages.Items.Clear();
            if(Communication.Instance.Messages != null)
            {
                foreach (Message msg in Communication.Instance.Messages)
                {
                    ListViewItem item = new ListViewItem(msg.Sender.Username);
                    item.SubItems.Add(msg.Text);
                    item.SubItems.Add(msg.DateTime.ToString());
                    lstMessages.Items.Add(item);
                }
                txtLastMessage.Text = Communication.Instance.Messages[0].Sender.Username + "\n"+ Communication.Instance.Messages[0].Text;
            }

        }

        public void CloseForm()
        {
            if(InvokeRequired)
            {
                Invoke(new Action(CloseForm));
                return;
            }
            Close();
        }

        private void FrmMain_FormClosed(object sender, FormClosedEventArgs e)
        {
            Communication.Instance.Logout(user);
            Communication.Instance.StopListening();
        }
    }
}
