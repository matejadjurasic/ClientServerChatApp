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

namespace Server
{
    public partial class FrmAddUser : Form
    {
        public FrmAddUser()
        {
            InitializeComponent();
        }

        private void btnAddUser_Click(object sender, EventArgs e)
        {
            User user = new User
            {
                Username = txtUsername.Text,
                Password = txtPassword.Text
            };
            bool r = Controller.Instance.AddUser(user);
            if (r)
            {
                MessageBox.Show("Success!");
            }
            else
            {
                MessageBox.Show("Something went wrong!");
            }
            this.Dispose();
        }
    }
}
