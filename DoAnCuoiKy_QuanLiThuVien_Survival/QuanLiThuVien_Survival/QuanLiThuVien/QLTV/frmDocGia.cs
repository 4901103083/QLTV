using QLTV;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QLTV
{
    public partial class frmDocGia : Form
    {
        private int id;
        public frmDocGia()
        {
            InitializeComponent();
            cbbQuyen.Items.Add("Người dùng");
            cbbQuyen.Items.Add("Quản trị");
        }
        public frmDocGia(int _id)
        {
            InitializeComponent();
            cbbQuyen.Items.Add("Người dùng");
            cbbQuyen.Items.Add("Quản trị");
            id = _id;
        }
        #region Hiển thị
        private void frmDocGia_Load(object sender, EventArgs e)
        {
            loadDuLieu();
        }
        private void loadDuLieu()
        {
            DatabaseDataContext db = new DatabaseDataContext();
            var docGiaList = db.NguoiDungs.Select(d => new
            {
                d.ID,
                d.HoTen,
                d.Email,
                d.NgayDangKi,
            }).OrderBy(p => p.ID).ToList();

            dataGVDg.DataSource = docGiaList;
            foreach (DataGridViewRow row in dataGVDg.Rows)
            {
                if (row.Cells["ID"].Value != null)
                {
                    string idBanDoc = row.Cells["ID"].Value.ToString();

                    // Sử dụng LINQ để lấy tổng số lượng sách mượn cho độc giả
                    int tongSoLuongMuon = LayTongSoLuongMuon(idBanDoc);

                    // Cập nhật giá trị vào cột "Tổng Số Lượng"
                    row.Cells["TongSoLuong"].Value = tongSoLuongMuon;
                }
            }
            if (docGiaList.Count > 0)
            {
                HienDuLieuDong(0);
            }
        }

        private int LayTongSoLuongMuon(string idBanDoc)
        {
            int tongSoLuongMuon = 0;
            using (DatabaseDataContext db = new DatabaseDataContext())
            {
                // LINQ truy vấn tổng số lượng sách mượn cho một độc giả
                var query = from p in db.PhieuMuons
                            join s in db.Saches on p.IDSach equals s.ID
                            where p.IDBanDoc.ToString() == idBanDoc && (p.TrangThai == 1 || p.TrangThai == 2)
                            group p by p.IDBanDoc into g
                            select new
                            {
                                SoLuongMuon = g.Sum(p => p.SoLuong)
                            };

                var result = query.FirstOrDefault();

                if (result != null)
                {
                    // Chuyển đổi từ int? sang int, sử dụng 0 nếu giá trị là null
                    tongSoLuongMuon = result.SoLuongMuon ?? 0;
                }
            }
            return tongSoLuongMuon;
        }
        private void dataGVDg_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {               
                int indexRow = e.RowIndex;                
                HienDuLieuDong(indexRow);
            }
        }

        private void HienDuLieuDong(int index)
        {
            DatabaseDataContext db = new DatabaseDataContext();
            string IDtmp = dataGVDg.Rows[index].Cells["ID"].Value.ToString();
            NguoiDung nguoi = db.NguoiDungs.Where(p => p.ID.ToString() == IDtmp).FirstOrDefault();

            if (nguoi != null)
            {
                txtID.Text = nguoi.ID.ToString();
                txtHoTen.Text = nguoi.HoTen;
                txtNgayDK.Text = nguoi.NgayDangKi.ToString();
                txtEmail.Text = nguoi.Email;
                txtSoLuong.Text = LayTongSoLuongMuon(nguoi.ID.ToString()).ToString();
                if (nguoi.QuyenHan == false)
                {
                    cbbQuyen.Text = cbbQuyen.Items[0].ToString();
                }
                else
                {
                    cbbQuyen.Text = cbbQuyen.Items[1].ToString();
                }
            }
            else
            {
                MessageBox.Show("Không tìm thấy độc giả với mã này.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private bool IsValidEmail(string email)
        {
            string emailPattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
            return Regex.IsMatch(email, emailPattern);
        }

        #endregion

        #region Chức năng

        private void btnThem_Click(object sender, EventArgs e)
        {
            if(btnThem.Text == "Thêm")
            {
                lblUser.Visible = true;
                lblPass.Visible = true;
                txtPass.Visible = true; 
                txtUser.Visible = true;

                txtHoTen.ReadOnly = false;
                txtEmail.ReadOnly = false;

                DatabaseDataContext data = new DatabaseDataContext();
                var MaxID = data.NguoiDungs.Max(d => d.ID);
                
                txtID.Text = (MaxID + 1) .ToString();  
                txtNgayDK.Text = DateTime.Now.ToString();
                txtSoLuong.Text = "0";
                txtHoTen.Text = "";
                txtEmail.Text = "";
                btnThem.Text = "Lưu";

            }
            else if(btnThem.Text == "Lưu")
            {
                if (string.IsNullOrEmpty(txtHoTen.Text))
                {
                    MessageBox.Show("Vui lòng nhập họ và tên!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtHoTen.Focus();
                    return;
                }
                if (string.IsNullOrEmpty(txtEmail.Text))
                {
                    MessageBox.Show("Vui lòng nhập Email!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtEmail.Focus();
                    return;
                }
                else if (!IsValidEmail(txtEmail.Text))
                {
                    MessageBox.Show("Email không hợp lệ!!\nVui lòng nhập lại!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtEmail.Focus();
                    return;
                }
                if (string.IsNullOrEmpty(txtUser.Text))
                {
                    MessageBox.Show("Vui lòng nhập tên tài khoản!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtUser.Focus();
                    return;
                }
                if (string.IsNullOrEmpty(txtPass.Text))
                {
                    MessageBox.Show("Vui lòng nhập mật khẩu!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtPass.Focus();
                    return;
                }
                DatabaseDataContext data = new DatabaseDataContext();

                var existingUser = data.NguoiDungs.SingleOrDefault(nd => nd.TenDangNhap == txtUser.Text);
                if (existingUser != null)
                {
                    MessageBox.Show("Tên đăng nhập đã được sử dụng. Vui lòng chọn thông tin khác!", "Thông Báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                var existingEmail = data.NguoiDungs.SingleOrDefault(nd => nd.Email == txtEmail.Text);
                if (existingEmail != null)
                {
                    MessageBox.Show("Email đã được sử dụng. Vui lòng chọn thông tin khác!", "Thông Báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
             
                NguoiDung NewND = new NguoiDung();

                MD5 md5 = MD5.Create();
                Random rd = new Random();
                int OTP = rd.Next(1000, 9999);
                byte[] inputBytes = Encoding.ASCII.GetBytes(txtPass.Text + OTP);
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                NewND.TenDangNhap = txtUser.Text;
                NewND.MatKhau = hashBytes;
                NewND.HoTen = txtHoTen.Text;
                NewND.Email = txtEmail.Text.ToString();
                NewND.QuyenHan = false;
                NewND.MaOTP = OTP.ToString();
                NewND.NgayDangKi = DateTime.Now;
                NewND.TrangThaiXacThuc = false;
                NewND.ThoiGianNhanOTP = null;
                NewND.RandomKey = OTP.ToString();

                data.NguoiDungs.InsertOnSubmit(NewND); 
                data.SubmitChanges();

                MessageBox.Show("Thêm Thành Công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                btnThem.Text = "Thêm";

                lblUser.Visible = false ;
                lblPass.Visible = false;
                txtPass.Visible = false;
                txtUser.Visible = false;

                txtHoTen.ReadOnly = true;
                txtEmail.ReadOnly = true;
                loadDuLieu();
            }
        }

        private void btnThoat_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnXoa_Click(object sender, EventArgs e)
        {
            try
            {
                using (DatabaseDataContext db = new DatabaseDataContext())
                {
                    int IDdg = int.Parse(txtID.Text); 

                    // Kiểm tra xem độc giả có tồn tại không
                    var docGia = db.NguoiDungs.SingleOrDefault(d => d.ID == IDdg);
                    if (docGia == null)
                    {
                        MessageBox.Show("Mã độc giả không tồn tại!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    // Kiểm tra xem độc giả có sách mượn chưa trả không
                    var query = from p in db.PhieuMuons
                                join s in db.Saches on p.IDSach equals s.ID
                                where p.IDBanDoc == IDdg && p.TrangThai == 1
                                group p by p.IDBanDoc into g
                                select new
                                {
                                    SoLuongMuon = g.Sum(p => p.SoLuong)
                                };

                    var result = query.FirstOrDefault();
                    if (result != null)
                    {
                        MessageBox.Show("Không thể xóa vì độc giả này còn mượn sách.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    // Xóa phiếu mượn và độc giả
                    var phieuMuon = db.PhieuMuons.Where(m => m.IDBanDoc == IDdg).ToList();
                    db.PhieuMuons.DeleteAllOnSubmit(phieuMuon);
                    db.NguoiDungs.DeleteOnSubmit(docGia);

                    db.SubmitChanges();

                    MessageBox.Show("Xóa độc giả thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    loadDuLieu();
                    return;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi: {ex.Message}", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        private void btnSua_Click(object sender, EventArgs e)
        {
            DatabaseDataContext db = new DatabaseDataContext();
            var nd = db.NguoiDungs.Where(p => p.ID.ToString() == txtID.Text).FirstOrDefault();
            if (nd != null)
            {
                if (nd.ID == id)
                {
                    MessageBox.Show("Bạn không thể đổi quyền chính bạn!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                if (nd.QuyenHan == true)
                {
                    nd.QuyenHan = false;
                    MessageBox.Show($"Đã cập nhật quyền truy cập cho bạn đọc {nd.HoTen} thành người dùng", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    nd.QuyenHan = true;
                    MessageBox.Show($"Đã cấp quyền quản trị cho bạn đọc {nd.HoTen}", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                db.SubmitChanges();
                loadDuLieu();
            }
        }
    }
}
