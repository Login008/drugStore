using System;
using System.Collections.Generic;
using System.Drawing; // ��� ������������� Image
using System.Linq;
using System.Windows.Forms;
using Microsoft.EntityFrameworkCore;

namespace Apteka
{
    public partial class Form1 : Form
    {
        private MedicineContext _context;
        private DataGridView _grid;
        private TextBox _searchBox;
        private TreeView _categoryTree;
        private PictureBox _imageBox; // ��������� PictureBox

        public Form1()
        {
            InitializeComponent();
            _context = new MedicineContext();

            this.Text = "�������";
            this.Width = 800;
            this.Height = 600;

            _searchBox = new TextBox { Width = 300, PlaceholderText = "����� ���������..." };
            _searchBox.TextChanged += SearchBox_TextChanged;

            _categoryTree = new TreeView { Width = 300, Dock = DockStyle.Left };
            _categoryTree.AfterSelect += CategoryTree_AfterSelect;

            _grid = new DataGridView { Dock = DockStyle.Fill, ReadOnly = true, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill };

            // ������� PictureBox ��� ����������� �����������
            _imageBox = new PictureBox
            {
                Dock = DockStyle.Right,
                Width = 500,
                SizeMode = PictureBoxSizeMode.StretchImage // ������������� ����� ������������
            };

            var layout = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, RowCount = 2 };
            layout.Controls.Add(_searchBox, 0, 1);
            layout.Controls.Add(_imageBox, 1, 1);
            layout.Controls.Add(_categoryTree, 0, 0);
            layout.Controls.Add(_grid, 1, 0);
            this.Controls.Add(layout);

            LoadData();
        }

        private void LoadData()
        {
            var medicines = _context.Medicines.ToList();
            _grid.DataSource = medicines;

            var categories = medicines.Select(m => m.Category).Distinct().ToList();
            foreach (var category in categories)
            {
                var node = new TreeNode(category);
                _categoryTree.Nodes.Add(node);
            }
        }

        private void SearchBox_TextChanged(object sender, EventArgs e)
        {
            string searchText = _searchBox.Text.ToLower();
            var filtered = _context.Medicines.Where(m => m.Name.ToLower().Contains(searchText)).ToList();
            _grid.DataSource = filtered;

            // �������� ����������� ���������, ���� ��� �������
            if (filtered.Count > 0)
            {
                var selectedMedicine = filtered.FirstOrDefault(); // �������� ������ ��������� ��������
                ShowImage(selectedMedicine.ImagePath);
            }
            else
            {
                _imageBox.Image = null; // �������� �����������, ���� ������ �� �������
            }
        }

        private void CategoryTree_AfterSelect(object sender, TreeViewEventArgs e)
        {
            string selectedCategory = e.Node.Text;
            var filtered = _context.Medicines.Where(m => m.Category == selectedCategory).ToList();
            _grid.DataSource = filtered;

            // �������� ����������� ���������, ���� ��� �������
            if (filtered.Count > 0)
            {
                var selectedMedicine = filtered.FirstOrDefault(); // �������� ������ ��������� ��������
                ShowImage(selectedMedicine.ImagePath);
            }
            else
            {
                _imageBox.Image = null; // �������� �����������, ���� ������ �� �������
            }
        }

        private void ShowImage(string imagePath)
        {
            if (!string.IsNullOrEmpty(imagePath) && System.IO.File.Exists(imagePath))
            {
                _imageBox.Image = Image.FromFile(imagePath); // ��������� �����������
            }
            else
            {
                _imageBox.Image = null; // ������������� ����������� � null, ���� ���� ������������
            }
        }
    }

    public class Medicine
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string Category { get; set; }
        public string ShelfPosition { get; set; } // ����� �� �����
        public string ImagePath { get; set; } // ���� � �����������
    }

    public class MedicineContext : DbContext
    {
        public DbSet<Medicine> Medicines { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseSqlServer("Server=MSI;Database=MedicineDB;Trusted_Connection=True;TrustServerCertificate=True;");
    }
}



