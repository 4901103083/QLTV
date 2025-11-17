using FRM;
using QLThuVien;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Thư_viện_mượn_trả_mới;

namespace QLTV
{
    public partial class frmUser : Form
    {
        private string Xinchao = "Xin chào ";
        private int ID;
        public frmUser()
        {
            InitializeComponent();
            lab_Xinchao.Text = Xinchao;
        }
        public frmUser(int _id)
        {
            InitializeComponent();
            
            DatabaseDataContext db = new DatabaseDataContext();
            NguoiDung ng = db.NguoiDungs.SingleOrDefault(u => u.ID == _id);
            ID = ng.ID;
            Xinchao += ng.HoTen;
            lab_Xinchao.Text = Xinchao;
        }
        private void btnDangKy_Click(object sender, EventArgs e)
        {
            this.Hide();
            frmXemSach newfrmTT = new frmXemSach(ID);
            newfrmTT.Show();
            newfrmTT.FormClosed += (s, arge) => this.Show();
        }

        private void btnLSMuon_Click(object sender, EventArgs e)
        {
            this.Hide();
            frmLichSuMuon newfrmTT = new frmLichSuMuon(ID);
            newfrmTT.Show();
            newfrmTT.FormClosed += (s, arge) => this.Show();
        }

        private void btnTroGiup_Click(object sender, EventArgs e)
        {
            this.Hide();
            frmTroGiupDG newfrmTT = new frmTroGiupDG();
            newfrmTT.Show();
            newfrmTT.FormClosed += (s, arge) => this.Show();
        }

        private void btnXem_Click(object sender, EventArgs e)
        {
            this.Hide();
            frmDoiThongTin newfrmTT = new frmDoiThongTin(ID);
            newfrmTT.Show();
            newfrmTT.FormClosed += (s, arge) => this.Show();
        }

        private void btnDangXuat_Click(object sender, EventArgs e)
        {
            this.Close();
            frmHome frmHome = new frmHome();
            frmHome.Show();            
        }
    }
}
