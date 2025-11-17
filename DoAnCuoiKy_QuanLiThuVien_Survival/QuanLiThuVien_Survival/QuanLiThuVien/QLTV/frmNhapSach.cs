using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QLTV
{
    public partial class frmNhapSach : Form
    {
        public frmNhapSach()
        {
            InitializeComponent();
        }
        #region Hiển thị dữ liệu
        private void FrmNhapSach_Load(object sender, EventArgs e)
        {
            // TODO: This line of code loads data into the 'qLTVDataSet.Sach' table. You can move, or remove it, as needed.
            //this.sachTableAdapter.Fill(this.qLTVDataSet.Sach);
            //// TODO: This line of code loads data into the 'qLTVDataSet.TheLoai' table. You can move, or remove it, as needed.
            //this.theLoaiTableAdapter.Fill(this.qLTVDataSet.TheLoai);
            //// TODO: This line of code loads data into the 'qLTVDataSet.NhaXuatBan' table. You can move, or remove it, as needed.
            //this.nhaXuatBanTableAdapter.Fill(this.qLTVDataSet.NhaXuatBan);
            //TODO: This line of code loads data into the 'qLTVDataSet.TacGia' table. You can move, or remove it, as needed.
            //this.tacGiaTableAdapter.Fill(this.qLTVDataSet.TacGia);
            DatabaseDataContext db = new DatabaseDataContext();
            LoadTacGiaToComboBox();
            LoadNXBToComboBox();
            LoadTheLoaiToComboBox();
            int tongSoSach = db.Saches.Count();
            lbTongSach.Text = $"Tổng số sách có trong thư viện: {tongSoSach} - Quyển sách";
            loadDuLieu();
        }
        private void LoadTacGiaToComboBox()
        {
            using (var db = new DatabaseDataContext())
            {
                var tacGiaList = db.TacGias
                                   .Select(tg => new { tg.MaTG, tg.TenTG })
                                   .ToList();

                cbbMaTG.DataSource = tacGiaList;
                cbbMaTG.DisplayMember = "TenTG"; // Hiển thị Tên Tác Giả
                cbbMaTG.ValueMember = "MaTG";   // Lưu ID Tác Giả
            }
        }
        private void LoadTheLoaiToComboBox()
        {
            using (var db = new DatabaseDataContext())
            {
                var theLoaiList = db.TheLoais
                                    .Select(tl => new { tl.MaTheLoai, tl.TenTheLoai })
                                    .ToList();

                cbbMaTheLoai.DataSource = theLoaiList;
                cbbMaTheLoai.DisplayMember = "TenTheLoai"; // Hiển thị Tên Thể Loại
                cbbMaTheLoai.ValueMember = "MaTheLoai";   // Lưu ID Thể Loại
            }
        }

        private void LoadNXBToComboBox()
        {
            using (var db = new DatabaseDataContext())
            {
                var nxbList = db.NhaXuatBans
                                .Select(nxb => new { nxb.MaNXB, nxb.TenNXB })
                                .ToList();

                cbbMaNXB.DataSource = nxbList;
                cbbMaNXB.DisplayMember = "TenNXB"; // Hiển thị Tên Nhà Xuất Bản
                cbbMaNXB.ValueMember = "MaNXB";   // Lưu ID Nhà Xuất Bản
            }
        }

        private void loadDuLieu()
        {
            DatabaseDataContext db = new DatabaseDataContext();
            var data = db.Saches
            .Join(db.TacGias, sach => sach.MaTG, tacGia => tacGia.MaTG, (sach, tacGia) => new { sach, tacGia })
            .Join(db.NhaXuatBans, temp => temp.sach.MaNXB, nxb => nxb.MaNXB, (temp, nxb) => new { temp.sach, temp.tacGia, nxb })
            .Join(db.TheLoais, temp => temp.sach.MaTheLoai, tl => tl.MaTheLoai, (temp, tl) => new
            {
                ID = temp.sach.ID,
                TenSach = temp.sach.TenSach,
                MaTG = temp.tacGia.TenTG,
                MaNXB = temp.nxb.TenNXB,
                MaTheLoai = tl.TenTheLoai,
                SoLuong = temp.sach.SoLuong,
                MoTa = temp.sach.MoTa
            })
            .OrderBy(s => s.ID)
            .ToList();

            dgvSach.DataSource = data;
            if (data.Count > 0)
            {
                hienThiDuLieu(0);
            }
        }
        private void dgvSach_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            //xac dinh row dang chon
            int idrow = e.RowIndex;
            if (idrow == -1) return;
            hienThiDuLieu(idrow);
        }

        private void hienThiDuLieu(int idrow)
        {
            DatabaseDataContext db = new DatabaseDataContext();
            // Lấy dòng dữ liệu từ DataGridView
            DataGridViewRow row = dgvSach.Rows[idrow];
            int idSach = Convert.ToInt32(dgvSach.Rows[idrow].Cells["ID"].Value);
            //hien thi du lieu
            Sach sach = db.Saches.Where(p => p.ID == idSach).SingleOrDefault();
            if (sach != null)
            {
                txtTen.Text = sach.TenSach;
                cbbMaTG.Text = row.Cells["MaTG"].Value.ToString();
                cbbMaNXB.Text = row.Cells["MaNXB"].Value.ToString();
                cbbMaTheLoai.Text = row.Cells["MaTheLoai"].Value.ToString();
                txtSoLuong.Text = sach.SoLuong.ToString();
                txtMoTa.Text=sach.MoTa.ToString();
            }
        }

        #endregion

        #region Chức năng
        private void btnDau_Click(object sender, EventArgs e)
        {
            if (dgvSach.Rows.Count > 0)
            {
                dgvSach.CurrentCell = dgvSach.Rows[0].Cells[0];
            }
            else
            {
                MessageBox.Show("Danh sách sách hiện tại không có dữ liệu.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
        private void btnThem_Click(object sender, EventArgs e)
        {
            string tensach = txtTen.Text;
            string soluong = txtSoLuong.Text;
            string moTa = txtMoTa.Text;

            if (string.IsNullOrWhiteSpace(tensach) || string.IsNullOrWhiteSpace(soluong))
            {
                MessageBox.Show("Vui lòng điền đầy đủ thông tin.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!int.TryParse(soluong, out int soLuong) || soLuong <= 0)
            {
                MessageBox.Show("Số lượng sách phải là số nguyên dương.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DatabaseDataContext db = new DatabaseDataContext();
            var Sach = db.Saches.Any(s => s.TenSach == tensach && s.MoTa == moTa); 
            if (Sach)
            {
                MessageBox.Show("Sách này đã tồn tại trong hệ thống. Không thể thêm trùng tên.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            try
            {
                int nextID = db.Saches.Max(sach => sach.ID) + 1;
                Sach s = new Sach
                {
                    ID = nextID,
                    TenSach = tensach,
                    SoLuong = soLuong,
                    MaTG = Convert.ToInt32(cbbMaTG.SelectedValue),
                    MaNXB = Convert.ToInt32(cbbMaNXB.SelectedValue),
                    MaTheLoai = Convert.ToInt32(cbbMaTheLoai.SelectedValue),
                    MoTa = moTa
                };

                db.Saches.InsertOnSubmit(s);   
                db.SubmitChanges();
                int tongSoSach = db.Saches.Count();
                lbTongSach.Text = $"Tổng số sách có trong thư viện: {tongSoSach} - Quyển sách";

                loadDuLieu();
                MessageBox.Show("Thêm sách mới thành công", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Đã xảy ra lỗi khi thêm sách: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void btnTruoc_Click(object sender, EventArgs e)
        {
            if (dgvSach.Rows.Count > 0)
            {
                int currentIndex = dgvSach.CurrentCell.RowIndex;
                if (currentIndex > 0)
                {
                    dgvSach.CurrentCell = dgvSach.Rows[currentIndex - 1].Cells[0];
                }
                else
                {
                    MessageBox.Show("Đây là dòng đầu tiên.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            else
            {
                MessageBox.Show("Danh sách sách hiện tại không có dữ liệu.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
        private void btnXoa_Click(object sender, EventArgs e)
        {
            DatabaseDataContext db = new DatabaseDataContext();
            string tensach = txtTen.Text;
            string moTa = txtMoTa.Text;

            var sachList = db.Saches.Where(p => p.TenSach == tensach && p.MoTa == moTa).ToList();
            DialogResult result = MessageBox.Show("Bạn có chắc chắn muốn xóa sách này?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (result == DialogResult.Yes)
            {
                if (sachList.Count == 0)
                {
                    MessageBox.Show("Không tìm thấy sách cần xóa.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                try
                {
                    foreach (var sach in sachList)
                    {
                        // Kiểm tra nếu có phiếu mượn liên kết với sách này
                        var phieuMuon = db.PhieuMuons.Where(pm => pm.IDSach == sach.ID).FirstOrDefault();
                        if (phieuMuon != null)
                        {
                            // Nếu sách đang bị mượn, không cho phép xóa sách này, nhưng vẫn tiếp tục kiểm tra các sách còn lại
                            MessageBox.Show($"Không thể xóa sách '{sach.TenSach}' vì sách này đang được mượn.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }
                        db.Saches.DeleteOnSubmit(sach);
                    }
                    db.SubmitChanges();

                    int tongSoSach = db.Saches.Count();
                    lbTongSach.Text = $"Tổng số sách có trong thư viện: {tongSoSach} - Quyển sách";

                    loadDuLieu();
                    MessageBox.Show("Xóa sách thành công", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Đã xảy ra lỗi khi xóa sách: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                return;
            }
        }
        private void btnTiep_Click(object sender, EventArgs e)
        {
            if (dgvSach.Rows.Count > 0)
            {
                int currentIndex = dgvSach.CurrentCell.RowIndex;
                if (currentIndex < dgvSach.Rows.Count - 1)
                {
                    dgvSach.CurrentCell = dgvSach.Rows[currentIndex + 1].Cells[0];
                }
                else
                {
                    MessageBox.Show("Đây là dòng cuối cùng.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            else
            {
                MessageBox.Show("Danh sách sách hiện tại không có dữ liệu.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
        private void btnCuoi_Click(object sender, EventArgs e)
        {
            // Kiểm tra xem DataGridView có dòng nào hay không
            if (dgvSach.Rows.Count > 0)
            {             
                dgvSach.CurrentCell = dgvSach.Rows[dgvSach.Rows.Count - 1].Cells[0];
            }
            else
            {
                MessageBox.Show("Danh sách sách hiện tại không có dữ liệu.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
        private void btnThoat_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        private void btnSua_Click(object sender, EventArgs e)
        {
            string tensach = txtTen.Text;
            string soluong = txtSoLuong.Text;
            string moTa = txtMoTa.Text;

            if (string.IsNullOrWhiteSpace(tensach) || string.IsNullOrWhiteSpace(soluong) || string.IsNullOrWhiteSpace(moTa))
            {
                MessageBox.Show("Vui lòng điền đầy đủ thông tin.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!int.TryParse(soluong, out int soLuong) || soLuong <= 0)
            {
                MessageBox.Show("Số lượng sách phải là số nguyên dương.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                /*DatabaseDataContext db = new DatabaseDataContext();
                var sach = db.Saches.FirstOrDefault(s => s.TenSach == tensach);
                sach.SoLuong = soLuong;
                sach.MaTG = Convert.ToInt32(cbbMaTG.SelectedValue);
                sach.MaNXB = Convert.ToInt32(cbbMaNXB.SelectedValue);
                sach.MaTheLoai = Convert.ToInt32(cbbMaTheLoai.SelectedValue);
                sach.MoTa = moTa;

                db.SubmitChanges();
                MessageBox.Show("Sửa thông tin sách thành công.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                loadDuLieu();*/

                using (var db = new DatabaseDataContext())
                {
                    // 3. TÌM SÁCH – NẾU KHÔNG TÌM THẤY → DỪNG LẠI
                    var sach = db.Saches.FirstOrDefault(s => s.TenSach == tensach);
                    if (sach == null)
                    {
                        MessageBox.Show("Không tìm thấy sách có tên: '" + tensach + "' để sửa!",
                                        "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    // 4. BÂY GIỜ MỚI GÁN (AN TOÀN)
                    sach.SoLuong = soLuong;
                    sach.MoTa = moTa;

                    // Cẩn thận với ComboBox (có thể null nếu chưa chọn)
                    sach.MaTG = cbbMaTG.SelectedValue != null ? Convert.ToInt32(cbbMaTG.SelectedValue) : 0;
                    sach.MaNXB = cbbMaNXB.SelectedValue != null ? Convert.ToInt32(cbbMaNXB.SelectedValue) : 0;
                    sach.MaTheLoai = cbbMaTheLoai.SelectedValue != null ? Convert.ToInt32(cbbMaTheLoai.SelectedValue) : 0;

                    db.SubmitChanges();

                    MessageBox.Show("Sửa thông tin sách thành công.", "Thông báo",
                                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                    loadDuLieu();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Đã xảy ra lỗi khi sửa thông tin sách: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void btnTim_Click(object sender, EventArgs e)
        {
            string keyword = txtTen.Text.ToString();

            if (string.IsNullOrWhiteSpace(keyword))
            {
                MessageBox.Show("Vui lòng nhập từ khóa tìm kiếm.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            try
            {
                bool found = false;

                foreach (DataGridViewRow row in dgvSach.Rows)
                {
                    if (row.Cells["TenSach"].Value != null && row.Cells["TenSach"].Value.ToString().Contains(keyword))
                    {
                        dgvSach.ClearSelection(); // Bỏ chọn các dòng trước
                        row.Selected = true; // Chọn dòng hiện tại
                        dgvSach.CurrentCell = row.Cells[0]; // Đặt con trỏ vào ô đầu tiên của dòng được chọn
                        dgvSach.FirstDisplayedScrollingRowIndex = row.Index; // Cuộn đến dòng được chọn
                        found = true;
                        break; // Thoát vòng lặp sau khi tìm thấy dòng
                    }
                }

                if (!found)
                {
                    MessageBox.Show("Không tìm thấy sách phù hợp với từ khóa.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("Đã tìm thấy sách phù hợp với từ khóa.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Đã xảy ra lỗi trong quá trình tìm kiếm: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            //try
            //{
            //    DatabaseDataContext db = new DatabaseDataContext();
            //    var ketQua = db.Saches
            //        .Where(s => s.TenSach.Contains(keyword))
            //        .Select(s => new
            //        {
            //            ID = s.ID,
            //            TenSach = s.TenSach,
            //            MaTG = s.MaTG,
            //            MaNXB = s.MaNXB,
            //            MaTheLoai = s.MaTheLoai,
            //            SoLuong = s.SoLuong,
            //            MoTa = s.MoTa
            //        }).ToList();

            //    if (ketQua.Count == 0)
            //    {
            //        MessageBox.Show("Không tìm thấy sách phù hợp với từ khóa.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            //    }
            //    else
            //    {
            //        dgvSach.DataSource = ketQua;
            //        MessageBox.Show($"Tìm thấy {ketQua.Count} sách phù hợp.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            //    }
            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show("Đã xảy ra lỗi trong quá trình tìm kiếm: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //}
        }
        #endregion
    }
}
