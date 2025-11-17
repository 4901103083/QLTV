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
using QLThuVien;
using QLTV;
using static System.Net.WebRequestMethods;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace FRM
{
    public partial class frmDoiThongTin : Form
    {
        private int id;
        private string OTP = "";
        private DateTime ThoiGianNhanOTPMail = new DateTime(); 
        public frmDoiThongTin()
        {
            InitializeComponent();
        }
        public frmDoiThongTin(int _id)
        {
            InitializeComponent();
            id = _id;
            DatabaseDataContext data = new DatabaseDataContext();
            NguoiDung nd = data.NguoiDungs.Where(p => p.ID == id).SingleOrDefault();
            txtHoTen.PlaceholderText = nd.HoTen.ToString();
            txtEmail.PlaceholderText = nd.Email.ToString();
            txtTenTK.PlaceholderText = nd.TenDangNhap.ToString();
            txtMKOld.PlaceholderText = "Nhập mật khẩu hiện tại";
            txtMKNew.PlaceholderText = "Nhập mật khẩu mới";
            txtReMKNew.PlaceholderText = "Nhập lại mật khẩu mới";

            txtMKNew.Hide();
            txtReMKNew.Hide();
            pnOTP.Hide();
            Random rd = new Random();
            OTP = rd.Next(1000, 9999).ToString();
        }

        private void lblThoat_Click(object sender, EventArgs e)
        {
            frmUser frmUser = new frmUser();
            frmUser.Show();
            this.Close();
        }
        private void cbTenTK_CheckedChanged(object sender, EventArgs e)
        {
            if (cbTenTK.Checked)
            {
                txtTenTK.Enabled = true;
            }
            else
            {
                txtTenTK.Enabled= false;
            }      
        }
        private void cbMK_CheckedChanged(object sender, EventArgs e) 
        {
            if (cbMK.Checked)
            {
                txtMKOld.Enabled = true;
                txtMKNew.Enabled = true;
                txtReMKNew.Enabled = true;
                txtMKNew.Show();
                txtReMKNew.Show();
            }
            else
            {
                txtMKOld.Enabled = false;
                txtMKNew.Enabled = false;
                txtReMKNew.Enabled = false;
                txtMKNew.Hide();
                txtReMKNew.Hide();
            }
        }
        private void cbEmail_CheckedChanged(object sender, EventArgs e)
        {
            if (cbEmail.Checked)
            {
                txtEmail.Enabled = true;
                pnOTP.Hide();
            }
            else
            {
                txtEmail.Enabled = false;
                pnOTP.Hide();
            }
        }
        private void cbHoTen_CheckedChanged(object sender, EventArgs e)
        {
            if (cbHoTen.Checked)
            {
                txtHoTen.Enabled = true;
            }
            else
            {
                txtHoTen.Enabled = false;
            }
        }
        private bool IsValidEmail(string email)
        {
            string emailPattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
            return Regex.IsMatch(email, emailPattern);
        }
        private void lblDoiTT_Click(object sender, EventArgs e)
        {
            DatabaseDataContext data = new DatabaseDataContext();
            NguoiDung nd = data.NguoiDungs.Where(p => p.ID == id).SingleOrDefault();
            if(nd != null)
            {
                if (cbHoTen.Checked)
                {
                    if (string.IsNullOrEmpty(txtHoTen.Text))
                    {
                        MessageBox.Show("Vui lòng nhập họ và tên mới!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        txtHoTen.Focus();
                        return;
                    }
                    else nd.HoTen = txtHoTen.Text;
                }
                if (cbTenTK.Checked)
                {
                    if (string.IsNullOrEmpty(txtTenTK.Text))
                    {
                        MessageBox.Show("Vui lòng nhập tên tài khoản!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        txtTenTK.Focus();
                        return;
                    }
                    if (nd.TenDangNhap != txtTenTK.Text)
                    {
                        NguoiDung check = data.NguoiDungs.Where(p => p.TenDangNhap == txtTenTK.Text).SingleOrDefault();
                        if (check != null)
                        {
                            MessageBox.Show("Tên tài khoản đã được sử dụng!\nVui lòng nhập lại", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            txtTenTK.Focus();
                            return;
                        }
                        else
                        {
                            nd.TenDangNhap = txtTenTK.Text;
                        }
                    }
                }
                
                if (cbMK.Checked)
                {
                    if (string.IsNullOrEmpty(txtMKOld.Text))
                    {
                        MessageBox.Show("Vui lòng nhập mật khẩu cũ!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        txtMKOld.Focus();
                        return;
                    }
                    else if (string.IsNullOrEmpty(txtMKNew.Text))
                    {
                        MessageBox.Show("Vui lòng nhập mật khẩu mới!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        txtMKNew.Focus();
                        return;
                    }
                    else if (string.IsNullOrEmpty(txtReMKNew.Text))
                    {
                        MessageBox.Show("Vui lòng nhập lại mật khẩu mới!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        txtReMKNew.Focus();
                        return;
                    }
                    else if (txtMKNew.Text != txtReMKNew.Text)
                    {
                        MessageBox.Show("Mật khẩu nhập lại không khớp!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        txtReMKNew.Focus();
                        return;
                    }
                    else
                    {
                        MD5 md5 = MD5.Create();
                        byte[] inputBytes = Encoding.ASCII.GetBytes(txtMKOld.Text + nd.MaOTP);
                        byte[] hashBytes = md5.ComputeHash(inputBytes);
                        if (nd.MatKhau == hashBytes)
                        {
                            byte[] newBytes = Encoding.ASCII.GetBytes(txtMKNew.Text + nd.MaOTP);
                            byte[] hashnewBytes = md5.ComputeHash(newBytes);
                            nd.MatKhau = hashnewBytes;
                            cbMK.Checked = false;
                        }
                        else
                        {
                            MessageBox.Show("Mật khẩu không chính xác", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            txtMKOld.Focus();
                            return;
                        }
                    }
                }
                data.SubmitChanges();

                if (cbEmail.Checked)
                {
                    if (string.IsNullOrEmpty(txtEmail.Text))
                    {
                        MessageBox.Show("Vui lòng nhập Email mới!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        txtEmail.Focus();
                        return;
                    }
                    else if (!IsValidEmail(txtEmail.Text))
                    {
                        MessageBox.Show("Email không hợp lệ!\nVi lòng nhập lại!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        txtEmail.Focus();
                        return;
                    }
                    else
                    {
                        if (txtEmail.Text != nd.Email)
                        {
                            NguoiDung check = data.NguoiDungs.Where(p => p.Email == txtEmail.Text).SingleOrDefault();
                            if (check != null)
                            {
                                MessageBox.Show("Email đã được sử dụng!\nVui lòng nhập lại", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                txtTenTK.Focus();
                                return;
                            }
                            else
                            {
                                MessageBox.Show("Bạn cần xác thực Email mới trước khi đổi!\nVui lòng kiểm tra Email mới để lấy mã OTP", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                pnOTP.Show();
                                SendMail.sendMailTo(txtEmail.Text, "Mã OTP xác thực là: " + OTP);
                                ThoiGianNhanOTPMail = DateTime.Now;
                            }
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Đổi thông tin thành công", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
            }
            else
            {
                MessageBox.Show("Tài Khoản không tồn tại!", "Thông Báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }

        private void btnThoatt_Click(object sender, EventArgs e)
        {
            pnOTP.Hide();
        }

        private void btnXacNhan_Click(object sender, EventArgs e)
        {
            if (txtOTP.Text == OTP)
            {
                if (ThoiGianNhanOTPMail != null && (DateTime.Now - ThoiGianNhanOTPMail).TotalMinutes <= 5)
                {
                    DatabaseDataContext db = new DatabaseDataContext();
                    NguoiDung ng = db.NguoiDungs.SingleOrDefault(u => u.ID == id);
                    ng.Email = txtEmail.Text;
                    db.SubmitChanges();
                    MessageBox.Show("Xác nhận thành công\nĐã đổi Email hoàn tất", "Thông Báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    MessageBox.Show("Đổi thông tin thành công", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    pnOTP.Hide();
                }
                else
                {
                    MessageBox.Show("Mã OTP hết hiệu lực. Vui lòng nhấn Gửi lại để nhận mã mới.", "Thông Báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }
            else
            {
                MessageBox.Show("Mã OTP không chính xác. Vui lòng thử lại.", "Thông Báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }

        private void btnGuiLai_Click(object sender, EventArgs e)
        {
            btnGuiLai.Enabled = false; // Vô hiệu hóa nút "Gửi lại" để tránh spam
            
            Random rd = new Random();
            OTP = rd.Next(1000, 9999).ToString();

            try
            {
                SendMail.sendMailTo(txtEmail.Text, "Mã OTP xác thực là: " + OTP);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Không thể gửi email. Vui lòng kiểm tra kết nối mạng.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                btnGuiLai.Enabled = true; // Bật lại nút nếu xảy ra lỗi
                return;
            }

            ThoiGianNhanOTPMail = DateTime.Now;
            MessageBox.Show("Đã gửi OTP", "Thông Báo");

            // Đợi 30 giây trước khi bật lại nút "Gửi lại"
            Task.Delay(30000).ContinueWith(t =>
            {
                btnGuiLai.Invoke((MethodInvoker)(() => btnGuiLai.Enabled = true));
            });
        }
    }
}
