using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QLTV
{
    public partial class frmAdmin : Form
    {
        private string tenDN = "Xin chào ";
        private int id;
        Timer timer = new Timer();
        public frmAdmin()
        {
           InitializeComponent();
            timer.Interval = 1;
            timer.Tick += timer_Tick;
            timer.Start();
        }
        public frmAdmin(int _ID)
        {
            InitializeComponent();
            timer.Interval = 1;
            timer.Tick += timer_Tick;
            timer.Start();
            id = _ID;
            DatabaseDataContext db = new DatabaseDataContext();
            NguoiDung ng = db.NguoiDungs.SingleOrDefault(u => u.ID == _ID);
            tenDN += ng.HoTen;
            labXinchaoAdmin.Text = tenDN;
        }
        private void timer_Tick(object sender, EventArgs e)
        {
            labTimeDate.Text = DateTime.Now.ToString("dddd, MMMM dd, yyyy | hh:mm:ss tt");
        }

        private void btnThoat_Click(object sender, EventArgs e)
        {
            this.Close();
            frmHome frmH = new frmHome();
            frmH.Show();
        }
        private void btnNhap_Click(object sender, EventArgs e)
        {
            this.Hide();
            frmSach newfrmTT = new frmSach(id);
            newfrmTT.Show();
            newfrmTT.FormClosed += (s, arge) => this.Show();
        }
        private void btnDocGia_Click(object sender, EventArgs e)
        {
            this.Hide();
            frmDocGia newfrmTT = new frmDocGia(id);
            newfrmTT.Show();
            newfrmTT.FormClosed += (s, arge) => this.Show();
        }

        private void btnThongKe_Click(object sender, EventArgs e)
        {
            this.Hide();
            frmThongKe newfrmTT = new frmThongKe(id);
            newfrmTT.Show();
            newfrmTT.FormClosed += (s, arge) => this.Show();
        }

        private void btnThongTin_Click(object sender, EventArgs e)
        {
            this.Hide();
            frmThongTin newfrmTT = new frmThongTin();
            newfrmTT.Show();
            newfrmTT.FormClosed += (s, arge) => this.Show();
        }

        private void btnTroGiup_Click(object sender, EventArgs e)
        {
            this.Hide();
            frmTroGiup newfrmTT = new frmTroGiup();
            newfrmTT.Show();
            newfrmTT.FormClosed += (s, arge) => this.Show();
        }

        private void btnMuonTra_Click(object sender, EventArgs e)
        {
            this.Close();
            frmMuon_Tra newfrmTT = new frmMuon_Tra(id);
            newfrmTT.Show();
        }
    }
}
