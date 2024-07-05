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

namespace Client
{
    public partial class FrmLogin : Form
    {
        public FrmLogin()
        {
            InitializeComponent();
            Communication.Instance.Connect(this);
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            User user = new User
            {
                Username = txtUsername.Text,
                Password = txtPassword.Text
            };
            Response r = Communication.Instance.Login(user);
            if (r.Exception == null)
            {
                Visible = false;
                FrmMain frmMain = new FrmMain((User)r.Result, this);
                frmMain.ShowDialog();
            }
            else
            {
                MessageBox.Show("Error logging in");
            }
        }
    }
}
