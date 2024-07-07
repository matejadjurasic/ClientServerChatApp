using Common.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Schema;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace Server
{
    public partial class FrmServer : Form
    {
        private Server server;
        public String maxClients = ConfigurationManager.AppSettings["max_broj_klijenata"];
        private User selectedUser;
        public FrmServer()
        {
            InitializeComponent();
            Controller.Instance.FrmServer = this;
            txtMaxUsers.Text = maxClients;
            btnStop.Enabled = false;
            txtStart.Text = "Server nije pokrenut!";
            RefreshTable();
        }


        private void btnStart_Click(object sender, EventArgs e)
        {
            server = new Server();
            btnStart.Enabled = false;
            btnStop.Enabled = true;
            txtStart.Text = "Server je pokrenut!";
            server.Start();
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            btnStart.Enabled = true;
            btnStop.Enabled = false;
            txtStart.Text = "Server nije pokrenut!";
            server.Stop();
        }

        public void RefreshTable()
        {
            if (InvokeRequired)
            {
                Invoke(new Action(RefreshTable));
                return;
            }
            lstUsers.DataSource = null;
            lstUsers.DataSource = Controller.Instance.Users;
            lstUsers.DisplayMember = "Username";
        }

        private void btnLogoutUser_Click(object sender, EventArgs e)
        {

        }


        private void lstUsers_SelectedIndexChanged(object sender, EventArgs e)
        {
            selectedUser = lstUsers.SelectedItem as User;
        }

        private void btnAddUser_Click(object sender, EventArgs e)
        {
            FrmAddUser addUser = new FrmAddUser();
            addUser.ShowDialog();
        }

        private void FrmServer_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
        }
    }
}
