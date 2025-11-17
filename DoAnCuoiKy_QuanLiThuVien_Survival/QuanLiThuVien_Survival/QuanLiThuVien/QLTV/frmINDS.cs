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
    public partial class frmINDS : Form
    {
        private int id;
        public frmINDS()
        {
            InitializeComponent();
            DatabaseDataContext db = new DatabaseDataContext();     
            var theLoai = db.TheLoais.Select(p => p.TenTheLoai).ToList();
            cbbTheLoai.Items.Add("Tất cả sách");
            foreach (string i in theLoai)
            {
                cbbTheLoai.Items.Add(i);
            }
        }
        public frmINDS(int _id)
        {
            InitializeComponent();
            DatabaseDataContext db = new DatabaseDataContext();
            var theLoai = db.TheLoais.Select(p => p.TenTheLoai).ToList();
            cbbTheLoai.Items.Add("Tất cả sách");
            foreach (string i in theLoai)
            {
                cbbTheLoai.Items.Add(i);
            }
            id = _id;
        }
        private void btnInSach_Click(object sender, EventArgs e)
        {
            frmINSach frm = new frmINSach(id, cbbTheLoai.Text);
            frm.Show();
        }
    }
}
