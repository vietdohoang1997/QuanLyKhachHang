using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        SqlConnection conn = null;
        string strConn = "Data Source =DESKTOP-APSTN3N\\SQLEXPRESS; Database=CSDL_QLKH; User Id=vietdohoang; pwd=123456789";

        private void Form1_Load(object sender, EventArgs e)
        {
            HienThiToanBoKhachHang();        
        }
        
        private void OpenConnection()
        {
            if (conn == null)
                conn = new SqlConnection(strConn);
            if (conn.State == ConnectionState.Closed)
                conn.Open();
        }

        private void CloseConnection()
        {
            if (conn != null && conn.State == ConnectionState.Open)
                conn.Close();
        }
        private void HienThiToanBoKhachHang()
        {
            try
            {
                OpenConnection();
                SqlCommand cmd = new SqlCommand();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "select *from KhachHang";
                cmd.Connection = conn;

                SqlDataReader reader = cmd.ExecuteReader();

                lvDanhSach.Items.Clear();

                lvDanhSach.Groups.Clear();
                ListViewGroup lvgVip = new ListViewGroup("Khách hàng Vip");
                lvDanhSach.Groups.Add(lvgVip);
                ListViewGroup lvgNor = new ListViewGroup("Khách hàng thường");
                lvDanhSach.Groups.Add(lvgNor);


                while(reader.Read())
                {
                    int ma = reader.GetInt32(0);
                    string ten = reader.GetString(1);
                    int gioiTinh = reader.GetInt32(2);
                    string phone = reader.GetString(3);
                    string loaiKH = reader.GetString(4);

                    ListViewItem lvi = new ListViewItem((lvDanhSach.Items.Count+1) + "");
                    lvi.SubItems.Add(ma + "");
                    lvi.SubItems.Add(ten);
                    lvi.SubItems.Add(gioiTinh==0 ? "Nam" : "Nữ");
                    lvi.SubItems.Add(phone);
                    lvi.SubItems.Add(loaiKH);

                    lvDanhSach.Items.Add(lvi);

                    if (string.Compare(loaiKH, "vip", true) == 0)
                        lvi.Group = lvgVip;
                    else
                        lvi.Group = lvgNor;
                    if (gioiTinh == 0)
                        lvi.ImageIndex = 0;
                    else if (gioiTinh == 1)
                        lvi.ImageIndex = 1;

                    lvi.Tag = ma;

                }

                reader.Close();
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void lvDanhSach_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lvDanhSach.SelectedItems.Count == 0) return;
            ListViewItem lvi = lvDanhSach.SelectedItems[0];
            int ma = (int)lvi.Tag;
            HienThiChiTiet(ma);
        }

        private void HienThiChiTiet(int ma)
        {
            try
            {
                OpenConnection();
                SqlCommand cmd = new SqlCommand();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "select* from KhachHang where Ma = @ma";
                cmd.Connection = conn;

                cmd.Parameters.Add("@ma", SqlDbType.Int).Value = ma;

                SqlDataReader reader = cmd.ExecuteReader();
                if(reader.Read())
                {
                    string ten = reader.GetString(1);
                    int gioiTinh = reader.GetInt32(2);
                    string phone = reader.GetString(3);
                    string loaiKH = reader.GetString(4);

                    txtMa.Text = ma + "";
                    txtTen.Text = ten;
                    if(gioiTinh==0)
                        rdNam.Checked = true;
                    else
                        rdNu.Checked = true;
                    txtPhone.Text = phone;
                    if(string.Compare(loaiKH, "vip", true)==0)
                        cboLoaiKH.SelectedIndex = 0;
                    else
                        cboLoaiKH.SelectedIndex = 1;
                }
                reader.Close();

            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnTaoMoi_Click(object sender, EventArgs e)
        {
            txtMa.Text = "";
            txtTen.Text = "";
            txtPhone.Text = "";
            //rdNam.Checked = false;
            cboLoaiKH.SelectedIndex = -1;
            txtMa.Focus();
        }

        private void btnLuu_Click(object sender, EventArgs e)
        {
            int ma = int.Parse(txtMa.Text);
            if(KiemTraTonTai(ma) == true)
            {
                //Cap nhat
                CapNhatKhachHang();
            }
            else
            {
                //them moi
                ThemMoiKhachHang();
            }
        }

        private void CapNhatKhachHang()
        {
            try
            {
                OpenConnection();

                SqlCommand cmd = new SqlCommand();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "update KhachHang set Ten=@ten, gioitinh=@gt, phone=@phone, loaiKH=@loai where Ma=@ma";
                cmd.Connection = conn;

                cmd.Parameters.Add("@ma", SqlDbType.Int).Value = txtMa.Text;
                cmd.Parameters.Add("@ten", SqlDbType.NVarChar).Value = txtTen.Text;
                if (rdNam.Checked == true)
                    cmd.Parameters.Add("@gt", SqlDbType.Int).Value = 0;
                else
                    cmd.Parameters.Add("@gt", SqlDbType.Int).Value = 1;
                cmd.Parameters.Add("@phone", SqlDbType.NVarChar).Value = txtPhone.Text;
                cmd.Parameters.Add("@loai", SqlDbType.NVarChar).Value = cboLoaiKH.Text;

                int kq = cmd.ExecuteNonQuery();
                if (kq > 0)
                {
                    HienThiToanBoKhachHang();
                    MessageBox.Show("Sửa thành công");
                    btnTaoMoi.PerformClick();
                }
                else
                    MessageBox.Show("Ko sửa đc");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void ThemMoiKhachHang()
        {
            try
            {
                OpenConnection();

                SqlCommand cmd = new SqlCommand();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "insert into KhachHang values(@ma, @ten, @gioitinh, @phone, @loaiKH)";
                cmd.Connection = conn;

                cmd.Parameters.Add("@ma", SqlDbType.Int).Value = txtMa.Text;
                cmd.Parameters.Add("@ten", SqlDbType.NVarChar).Value = txtTen.Text;
                if(rdNam.Checked==true)
                    cmd.Parameters.Add("@gioitinh", SqlDbType.Int).Value = 0;
                else
                    cmd.Parameters.Add("@gioitinh", SqlDbType.Int).Value = 1;
                cmd.Parameters.Add("@phone", SqlDbType.NVarChar).Value = txtPhone.Text;
                cmd.Parameters.Add("@loaiKH", SqlDbType.NVarChar).Value = cboLoaiKH.Text;

                int kq = cmd.ExecuteNonQuery();
                if (kq > 0)
                {
                    HienThiToanBoKhachHang();
                    MessageBox.Show("Lưu thành công");
                    btnTaoMoi.PerformClick();
                }
                else
                    MessageBox.Show("Ko lưu đc");
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
     
        private bool KiemTraTonTai(int ma)
        {
            try
            {
                OpenConnection();
                SqlCommand cmd = new SqlCommand();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "select* from KhachHang where Ma = @ma";
                cmd.Connection = conn;

                cmd.Parameters.Add("@ma", SqlDbType.Int).Value = ma;

                SqlDataReader reader = cmd.ExecuteReader();

                bool kq = reader.Read();

                reader.Close();

                return kq;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return false;
        }


        private void btnXoa_Click(object sender, EventArgs e)
        {
            if(lvDanhSach.SelectedItems.Count==0)
            {
                MessageBox.Show("Chưa chọn khách hàng để xóa");
                return;
            }
            ListViewItem lvi = lvDanhSach.SelectedItems[0];
            int ma = (int)lvi.Tag;
            DialogResult ret = MessageBox.Show("Muốn xóa mã [" + ma + "]?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if(ret==DialogResult.Yes)
            {
                ThucHienXoa(ma);
            }
        }

        private void ThucHienXoa(int ma)
        {
            try
            {
                OpenConnection();
                SqlCommand cmd = new SqlCommand();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "delete from KhachHang where Ma = @ma";
                cmd.Connection = conn;

                cmd.Parameters.Add("@ma", SqlDbType.Int).Value = ma;

                int kq = cmd.ExecuteNonQuery();

                if (kq > 0)
                {
                    HienThiToanBoKhachHang();
                    MessageBox.Show("Đã xóa");
                    btnTaoMoi.PerformClick();
                }
                else
                    MessageBox.Show("Không xóa đc");                         
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
                 
        }

        private void btnIn_Click(object sender, EventArgs e)
        {
            frmInAn frm = new frmInAn();
            frm.Show();
        }


    }
}
