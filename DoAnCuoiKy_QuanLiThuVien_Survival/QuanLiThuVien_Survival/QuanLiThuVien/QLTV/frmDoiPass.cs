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
    public partial class frmDoiPass : Form
    {
        private string tenTK = "";
        public frmDoiPass()
        {
            InitializeComponent();
            labTenTK.Text = tenTK;
            panelXacThuc.Hide();
            panelOTP.Show(); 
        }
        public frmDoiPass(string _tenTK)
        {
            InitializeComponent();
            tenTK = _tenTK;
            labTenTK.Text = tenTK;
            panelXacThuc.Hide();
            panelOTP.Show();
        }
        private void btnThoat_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnXacnhan_Click(object sender, EventArgs e)
        {
            string pass = txtPass.Text;
            string repass = txtRePass.Text;
            if (pass == "" || repass == "")
            {
                MessageBox.Show("Vui lòng nhập đầy đủ thông tin!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (pass != repass)
            {
                MessageBox.Show("Mật khẩu không khớp\nVui lòng nhập lại", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            DatabaseDataContext db = new DatabaseDataContext();
            NguoiDung ng = db.NguoiDungs.SingleOrDefault(u => u.TenDangNhap == tenTK);

            MD5 md5 = MD5.Create(); //MD5 là Một chiều = bạn chỉ có thể mã hóa, không thể giải mã lại.
            byte[] inputBytes = Encoding.ASCII.GetBytes(pass + ng.RandomKey); // chuyển mật khẩu thành mã
            byte[] hashBytes = md5.ComputeHash(inputBytes); // mã hóa ra 16 byte

            ng.MatKhau = hashBytes; // lưu mã hashByte vào cột Mật khẩu trong csdl
            ng.MaOTP = ng.RandomKey;
            db.SubmitChanges();

            MessageBox.Show("Đổi mật khẩu thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            this.Close();
        }

        private void btnXacThuc_Click(object sender, EventArgs e)
        {
            DatabaseDataContext db = new DatabaseDataContext();
            NguoiDung ng = db.NguoiDungs.SingleOrDefault(p => p.TenDangNhap == tenTK);

            if (ng != null)
            {
                if (ng.RandomKey == txtOTP.Text)
                {
                    if (ng.ThoiGianNhanOTP != null && (DateTime.Now - ng.ThoiGianNhanOTP).Value.TotalMinutes <= 5)
                    {
                        ng.RandomKey = txtOTP.Text;
                        db.SubmitChanges();
                        MessageBox.Show("Xác thực thành công", "Thông Báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        panelOTP.Hide();
                        panelXacThuc.Show();
                    }
                    else
                    {
                        MessageBox.Show("Mã OTP hết hiệu lực. Vui lòng nhấn Gửi lại để nhận mã mới.", "Thông Báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        btnGuiLai_Click(sender, e);
                    }
                }
                else
                {
                    MessageBox.Show("Mã OTP không chính xác. Vui lòng thử lại.", "Thông Báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }
        }

        private void btnGuiLai_Click(object sender, EventArgs e)
        {
            btnGuiLai.Enabled = false; // Vô hiệu hóa nút "Gửi lại" để tránh spam
            DatabaseDataContext db = new DatabaseDataContext();
            NguoiDung ng = db.NguoiDungs.SingleOrDefault(p => p.TenDangNhap == tenTK);

            if (ng != null)
            {
                Random rd = new Random();
                ng.RandomKey = rd.Next(1000, 9999).ToString();

                try
                {
                    SendMail.sendMailTo(ng.Email, "Mã OTP xác thực là: " + ng.RandomKey);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Không thể gửi email. Vui lòng kiểm tra kết nối mạng.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    btnGuiLai.Enabled = true; // Bật lại nút nếu xảy ra lỗi
                    return;
                }

                ng.ThoiGianNhanOTP = DateTime.Now;
                db.SubmitChanges();
                MessageBox.Show("Đã gửi OTP", "Thông Báo");

                // Đợi 30 giây trước khi bật lại nút "Gửi lại"
                Task.Delay(30000).ContinueWith(t =>
                {
                    btnGuiLai.Invoke((MethodInvoker)(() => btnGuiLai.Enabled = true));
                });
            }
        }

        private void btnThoatOTP_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
