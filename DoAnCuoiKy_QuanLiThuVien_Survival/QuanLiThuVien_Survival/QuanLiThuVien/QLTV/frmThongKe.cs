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

namespace QLTV
{
    public partial class frmThongKe : Form
    {
        private int id;
        public frmThongKe()
        {
            InitializeComponent();
        }
        public frmThongKe(int _id)
        {
            InitializeComponent();
            id = _id;
        }
        private void frmThongKe_Load(object sender, EventArgs e)
        {
            cbbThongKe.Items.Clear();
            cbbThongKe.Items.Add("Top 4 độc giả mượn nhiều nhất");
            cbbThongKe.Items.Add("Top 4 sách được mượn nhiều nhất");
        }
        private void btnQuayLai_Click(object sender, EventArgs e)
        {

            this.Hide(); // Ẩn form hiện tại
            frmAdmin frmMain = new frmAdmin();
            frmMain.Show();

            //// Khi frmMain đóng, hiển thị lại frmThongKe
            //frmMain.FormClosed += (s, args) => this.Show();
        }

        private void cbbThongKe_SelectedIndexChanged(object sender, EventArgs e)
        {
            //if(checked)
            string value = cbbThongKe.SelectedItem.ToString();
            if (value == "Top 4 độc giả mượn nhiều nhất")
            {
                this.Hide(); // Ẩn form hiện tại
                frmThongKeDocGia frmDocGia = new frmThongKeDocGia(id, value);
                frmDocGia.Show();

                // Khi frmMain đóng, hiển thị lại frmThongKe
                frmDocGia.FormClosed += (s, args) => this.Show();
            }
            else if (value == "Top 4 sách được mượn nhiều nhất")
            {
                this.Hide(); // Ẩn form hiện tại
                frmThongKeSach frmSach = new frmThongKeSach(id, value);
                frmSach.Show();

                // Khi frmMain đóng, hiển thị lại frmThongKe
                frmSach.FormClosed += (s, args) => this.Show();
            }
        }

        private void guna2ControlBox1_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
