using QLThuVien;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using Thư_viện_mượn_trả_mới;

namespace QLTV
{
    public partial class frmSignUp : Form
    {
        public frmSignUp()
        {
            InitializeComponent(); 
        }

        private void labLogin_Click(object sender, EventArgs e)
        {
            frmLogin frmLogin = new frmLogin();
            frmLogin.Show();
            this.Close();
        }
        private void labExit_Click(object sender, EventArgs e)
        {
            frmHome frmH = new frmHome();
            frmH.Show();
            this.Close();
        }
        private bool IsValidEmail(string email)
        {
            string emailPattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
            return Regex.IsMatch(email, emailPattern);
        }
        private void btnRegister_Click(object sender, EventArgs e)
        {    
            if (string.IsNullOrWhiteSpace(txtHoten.Text))
            {
                MessageBox.Show($"Vui lòng nhập Họ và tên!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtHoten.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(txtEmail.Text))
            {
                MessageBox.Show($"Vui lòng nhập Email!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtEmail.Focus();
                return;
            }

            if (!IsValidEmail(txtEmail.Text))
            {
                MessageBox.Show("Email không hợp lệ. Vui lòng nhập lại!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtEmail.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(txtTenDN.Text))
            {
                MessageBox.Show($"Vui lòng nhập Tên đăng nhập!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtTenDN.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(txtPass.Text))
            {
                MessageBox.Show($"Vui lòng nhập Mật khẩu!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtPass.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(txtRePass.Text))
            {
                MessageBox.Show($"Vui lòng nhập lại Mật khẩu!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtRePass.Focus();
                return;
            }

            if (txtRePass.Text != txtPass.Text)
            {
                MessageBox.Show("Mật khẩu không khớp. Vui lòng nhập lại!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtRePass.Focus();
                return;
            }

            DatabaseDataContext db = new DatabaseDataContext();
            // Kiểm tra tên đăng nhập và email trùng lặp
            var existingUser = db.NguoiDungs.SingleOrDefault(nd => nd.TenDangNhap == txtTenDN.Text);
            if (existingUser != null)
            {
                MessageBox.Show("Tên đăng nhập đã được sử dụng. Vui lòng chọn thông tin khác!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            var existingEmail= db.NguoiDungs.SingleOrDefault(nd => nd.Email == txtEmail.Text);
            if (existingEmail != null)
            {
                MessageBox.Show("Email đã được sử dụng. Vui lòng chọn thông tin khác!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            Random rd = new Random();
            string otp = rd.Next(1000, 9999).ToString();

            MD5 md5 = MD5.Create();
            byte[] inputBytes = Encoding.ASCII.GetBytes(txtPass.Text + otp);
            byte[] hashBytes = md5.ComputeHash(inputBytes);

            var newInfor = new NguoiDung
            {
                TenDangNhap = txtTenDN.Text,
                MatKhau = hashBytes,
                HoTen = txtHoten.Text,
                Email = txtEmail.Text,
                RandomKey = otp,
                MaOTP = otp,
                TrangThaiXacThuc = false,
                QuyenHan = false,
            };

            db.NguoiDungs.InsertOnSubmit(newInfor);
            db.SubmitChanges();
            MessageBox.Show("Đăng ký thành công!\nVui lòng xác thực Email để kích hoạt tài khoản!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            frmLogin frmLog = new frmLogin();
            this.Close();
            frmLog.Show();
            
            SendMail.sendMailTo(newInfor.Email, "Mã OTP xác thực là: " + newInfor.RandomKey);
            newInfor.ThoiGianNhanOTP = DateTime.Now;
            db.SubmitChanges();            
            //this.Hide();

            frmXTTK frmXTTK = new frmXTTK(newInfor.TenDangNhap);
            frmXTTK.Show();
        }
        private void cb_ShowPass_CheckedChanged(object sender, EventArgs e)
        {
            if (cb_ShowPass.Checked)
            {
                txtPass.PasswordChar = '\0';
                txtRePass.PasswordChar = '\0';
            }
            else
            {
                txtPass.PasswordChar = '*';
                txtRePass.PasswordChar = '*';
            }
        }
    }
}
