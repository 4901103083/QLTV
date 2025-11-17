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
    public partial class frmXTTK : Form
    {
        public frmXTTK()
        {
            InitializeComponent();
        }

        string taikhoan;
        public frmXTTK(string _taikhoan)
        {
            InitializeComponent();
            taikhoan = _taikhoan;
            labTenTK.Text = taikhoan;
        }

        private void btnXacThuc_Click(object sender, EventArgs e)
        {
            DatabaseDataContext db = new DatabaseDataContext();
            NguoiDung ng = db.NguoiDungs.SingleOrDefault(p => p.TenDangNhap == taikhoan);

            if (ng != null)
            {
                if (ng.RandomKey == txtOTP.Text)
                {
                    if (ng.ThoiGianNhanOTP != null && (DateTime.Now - ng.ThoiGianNhanOTP).Value.TotalMinutes <= 5)
                    {
                        ng.NgayDangKi = DateTime.Now;
                        ng.TrangThaiXacThuc = true;
                        ng.RandomKey = txtOTP.Text;
                        db.SubmitChanges();
                        MessageBox.Show("Đã kích hoạt thành công", "Thông Báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        this.Close();
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
            NguoiDung ng = db.NguoiDungs.SingleOrDefault(p => p.TenDangNhap == taikhoan);

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

        private void btnThoat_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
