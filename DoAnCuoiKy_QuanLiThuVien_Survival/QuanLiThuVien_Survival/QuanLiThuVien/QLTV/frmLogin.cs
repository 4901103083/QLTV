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
using QLThuVien;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace QLTV
{
    public partial class frmLogin : Form
    {
        public frmLogin()
        {
            InitializeComponent();
        }

        private void lbResigter_Click(object sender, EventArgs e)
        {
            this.Hide();
            frmSignUp frmSign = new frmSignUp();
            frmSign.ShowDialog();
            this.Show();
        }

        private void guna2ControlBox1_Click(object sender, EventArgs e)
        {           
            frmHome frmH = new frmHome();
            frmH.Show();
            this.Close();
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtTDN.Text))
            {
                MessageBox.Show($"Vui lòng nhập tên đăng nhập!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtTDN.Focus();
                return;
            }
            if (string.IsNullOrEmpty(txtPass.Text))
            {
                MessageBox.Show($"Vui lòng nhập mật khẩu!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtPass.Focus();
                return;
            }

            DatabaseDataContext db = new DatabaseDataContext();
            NguoiDung ng = db.NguoiDungs.SingleOrDefault(u => u.TenDangNhap == txtTDN.Text);

            if (ng != null)
            {
                if (ng.TrangThaiXacThuc == false)
                {
                    MessageBox.Show("Bạn chưa xác thực!\nVui lòng xác thực!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    Random rd = new Random();
                    ng.RandomKey = rd.Next(1000, 9999).ToString();
                    SendMail.sendMailTo(ng.Email, "Mã OTP xác thực là: " + ng.RandomKey);
                    ng.ThoiGianNhanOTP = DateTime.Now;
                    db.SubmitChanges();
                    frmXTTK frmXTTK = new frmXTTK(ng.TenDangNhap);
                    frmXTTK.Show();
                    this.Hide();
                    return;
                }

                MD5 md5 = MD5.Create();
                byte[] inputBytes = Encoding.ASCII.GetBytes(txtPass.Text + ng.MaOTP);
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                if (ng.MatKhau == hashBytes)
                {
                    if (ng.QuyenHan == true)
                    {
                        MessageBox.Show($"Chào mừng Admin {ng.HoTen} đã quay lại!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        frmAdmin frmAdmin = new frmAdmin(ng.ID);
                        frmAdmin.Show();
                        this.Close();
                        return;
                    }
                    else
                    {
                        MessageBox.Show($"Chào mừng bạn đọc {ng.HoTen} đã quay lại!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        frmUser frmUser = new frmUser(ng.ID);
                        frmUser.Show();
                        this.Close();
                        return;
                    }
                }
                else
                {
                    MessageBox.Show("Sai tên đăng nhập hoặc mật khẩu\nVui lòng kiểm tra lại", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtTDN.Focus();
                    return;
                }
            }
            else
            {
                MessageBox.Show("Tài khoản không tồn tại!\nVui lòng kiểm tra lại!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtTDN.Focus();
                return;
            }
        }

        private void cb_ShowPass_CheckedChanged(object sender, EventArgs e)
        {
            if (cb_ShowPass.Checked)
            {
                txtPass.PasswordChar = '\0';
            }
            else
            {
                txtPass.PasswordChar = '*';
            }
        }

        private void labFogotPass_Click(object sender, EventArgs e)
        {
            DatabaseDataContext db = new DatabaseDataContext();
            NguoiDung ng = db.NguoiDungs.SingleOrDefault(u => u.TenDangNhap == txtTDN.Text);

            Random rd = new Random();
            string otp = rd.Next(1000, 9999).ToString();

            if (ng != null)
            {
                MessageBox.Show("Bạn cần xác thực tài khoản trước!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                SendMail.sendMailTo(ng.Email, "Mã OTP xác thực là: " + otp);
                ng.RandomKey = otp;
                ng.ThoiGianNhanOTP = DateTime.Now;
                db.SubmitChanges();

                frmDoiPass frmDP = new frmDoiPass(ng.TenDangNhap);
                frmDP.Show();
            }
            else
            {
                MessageBox.Show("Tài khoản không tồn tại!\nVui lòng kiểm tra lại!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtTDN.Focus();
                return;
            }
        }
    }
}
