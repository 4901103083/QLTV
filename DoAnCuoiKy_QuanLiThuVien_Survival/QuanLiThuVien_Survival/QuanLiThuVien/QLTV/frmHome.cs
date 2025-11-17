using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QLTV
{
    public partial class frmHome : Form
    {
        public frmHome()
        {
            InitializeComponent();
        }

        private void btnThoat_Click(object sender, EventArgs e)
        {      
            Application.Exit();
        }

        private void btnDN_Click(object sender, EventArgs e)
        {
            frmLogin frmLogin = new frmLogin();
            frmLogin.Show();
            this.Hide();
        }

        private void btnDK_Click(object sender, EventArgs e)
        {
            
            frmSignUp frmSignUp = new frmSignUp();
            frmSignUp.Show();
            this.Hide();
        }

        private void frmHome_Load(object sender, EventArgs e)
        {

        }
    }
}
