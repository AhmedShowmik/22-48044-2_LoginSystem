using System;
using System.Windows.Forms;

namespace _22_48044_2_LoginSystem
{
    public class HomeForm : Form
    {
        private Label lblWelcome;
        private DataGridView dataGridUsers;
        private TextBox txtSearch;
        private Button btnSearch, btnShowAll, btnLogout;

        public HomeForm(string fullName)
        {
            InitializeComponent(fullName);
            this.Load += (s, e) => LoadUsers();
        }

        private void InitializeComponent(string fullName)
        {
            this.Text = "Home";
            this.Size = new System.Drawing.Size(650, 480);
            this.StartPosition = FormStartPosition.CenterScreen;

            lblWelcome = new Label
            {
                Text = $"Welcome, {fullName}",
                Font = new System.Drawing.Font("Segoe UI", 14, System.Drawing.FontStyle.Bold),
                Location = new System.Drawing.Point(20, 15),
                AutoSize = true
            };

            // Bonus: search/filter by username
            txtSearch = new TextBox { Location = new System.Drawing.Point(20, 55), Width = 200, PlaceholderText = "Search username..." };
            btnSearch = new Button { Text = "Search", Location = new System.Drawing.Point(230, 53), Width = 80 };
            btnSearch.Click += (s, e) => LoadUsers(txtSearch.Text.Trim());

            btnShowAll = new Button { Text = "Show All", Location = new System.Drawing.Point(320, 53), Width = 80 };
            btnShowAll.Click += (s, e) => { txtSearch.Clear(); LoadUsers(); };

            dataGridUsers = new DataGridView
            {
                Location = new System.Drawing.Point(20, 95),
                Size = new System.Drawing.Size(590, 300),
                ReadOnly = true,
                AllowUserToAddRows = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect
            };

            btnLogout = new Button
            {
                Text = "Logout",
                Location = new System.Drawing.Point(20, 410),
                Width = 100,
                BackColor = System.Drawing.Color.IndianRed,
                ForeColor = System.Drawing.Color.White
            };
            btnLogout.Click += BtnLogout_Click;

            this.Controls.Add(lblWelcome);
            this.Controls.Add(txtSearch);
            this.Controls.Add(btnSearch);
            this.Controls.Add(btnShowAll);
            this.Controls.Add(dataGridUsers);
            this.Controls.Add(btnLogout);
        }

        // Task 7: DataGridView via SqlDataAdapter + DataTable. Never shows PasswordHash.
        private void LoadUsers(string searchTerm = null)
        {
            try
            {
                dataGridUsers.DataSource = string.IsNullOrWhiteSpace(searchTerm)
                    ? DatabaseHelper.GetAllUsers()
                    : DatabaseHelper.SearchUsers(searchTerm);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Could not load users: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Task 5: Logout closes THIS form only (app keeps running via LoginForm),
        // and does not leave this form running in the background.
        private void BtnLogout_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
