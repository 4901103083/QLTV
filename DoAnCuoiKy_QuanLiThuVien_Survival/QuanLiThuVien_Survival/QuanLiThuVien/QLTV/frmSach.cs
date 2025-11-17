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
    public partial class frmSach : Form
    {
        private int id;
        public frmSach()
        {
            InitializeComponent();
        }
        public frmSach(int _id)
        {
            InitializeComponent();
            id = _id;
        }

        private void btnTacGia_Click(object sender, EventArgs e)
        {
            this.Hide();
            frmNhapTG newfrmTT = new frmNhapTG();
            newfrmTT.Show();
            newfrmTT.FormClosed += (s, arge) => this.Show();
        }

        private void btnNXB_Click(object sender, EventArgs e)
        {
            this.Hide();
            frmNhapNXB newfrmTT = new frmNhapNXB();
            newfrmTT.Show();
            newfrmTT.FormClosed += (s, arge) => this.Show();
        }

        private void btnSach_Click(object sender, EventArgs e)
        {
            this.Hide();
            frmNhapSach newfrmTT = new frmNhapSach();
            newfrmTT.Show();
            newfrmTT.FormClosed += (s, arge) => this.Show();
        }

        private void btnTheLoai_Click(object sender, EventArgs e)
        {
            this.Hide();
            frmNhapTL newfrmTT = new frmNhapTL();
            newfrmTT.Show();
            newfrmTT.FormClosed += (s, arge) => this.Show();
        }

        private void btnIn_Click(object sender, EventArgs e)
        {
            this.Hide();
            frmINDS newfrmTT = new frmINDS(id);
            newfrmTT.Show();
            newfrmTT.FormClosed += (s, arge) => this.Show();
        }
    }
}
