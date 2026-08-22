using System;
using System.Windows.Forms;

namespace _22_48044_2_LoginSystem
{
    public class LoginForm : Form
    {
        private Label lblTitle;
        private Label lblUsername;
        private Label lblPassword;
        private TextBox txtUsername;
        private TextBox txtPassword;
        private Button btnLogin;
        private Button btnGoToRegister;
        private Label lblStatus;

        private int failedAttempts = 0;
        private const int MaxAttempts = 3;

        public LoginForm()
        {
            InitializeComponent();
            this.Load += LoginForm_Load;
        }

        private void InitializeComponent()
        {
            this.Text = "Login";
            this.Size = new System.Drawing.Size(360, 300);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;

            lblTitle = new Label
            {
                Text = "Login",
                Font = new System.Drawing.Font("Segoe UI", 16, System.Drawing.FontStyle.Bold),
                Location = new System.Drawing.Point(120, 20),
                AutoSize = true
            };

            lblUsername = new Label { Text = "Username:", Location = new System.Drawing.Point(40, 80), AutoSize = true };
            txtUsername = new TextBox { Name = "txtUsername", Location = new System.Drawing.Point(140, 77), Width = 160 };

            lblPassword = new Label { Text = "Password:", Location = new System.Drawing.Point(40, 115), AutoSize = true };
            txtPassword = new TextBox { Name = "txtPassword", Location = new System.Drawing.Point(140, 112), Width = 160, UseSystemPasswordChar = true };

            btnLogin = new Button { Text = "Login", Location = new System.Drawing.Point(140, 155), Width = 100 };
            btnLogin.Click += BtnLogin_Click;

            btnGoToRegister = new Button { Text = "Register", Location = new System.Drawing.Point(140, 190), Width = 100 };
            btnGoToRegister.Click += BtnGoToRegister_Click;

            lblStatus = new Label
            {
                Text = "",
                ForeColor = System.Drawing.Color.Red,
                Location = new System.Drawing.Point(40, 230),
                AutoSize = true,
                MaximumSize = new System.Drawing.Size(280, 0)
            };

            this.Controls.Add(lblTitle);
            this.Controls.Add(lblUsername);
            this.Controls.Add(txtUsername);
            this.Controls.Add(lblPassword);
            this.Controls.Add(txtPassword);
            this.Controls.Add(btnLogin);
            this.Controls.Add(btnGoToRegister);
            this.Controls.Add(lblStatus);
        }

        // Task 2: confirm the DB is reachable as soon as the form loads.
        private void LoginForm_Load(object sender, EventArgs e)
        {
            if (!DatabaseHelper.TestConnection(out string error))
            {
                MessageBox.Show(
                    "Could not connect to the database.\n\n" + error,
                    "Connection Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            txtUsername.Focus();
        }

        private void BtnLogin_Click(object sender, EventArgs e)
        {
            string username = txtUsername.Text.Trim();
            string password = txtPassword.Text;

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                lblStatus.Text = "Please enter both username and password.";
                return;
            }

            try
            {
                string passwordHash = DatabaseHelper.HashPassword(password);
                string fullName = DatabaseHelper.ValidateLogin(username, passwordHash);

                if (fullName != null)
                {
                    failedAttempts = 0;
                    lblStatus.Text = "";
                    OpenHomeForm(fullName);
                }
                else
                {
                    failedAttempts++;
                    lblStatus.Text = $"Invalid username or password. Attempt {failedAttempts}/{MaxAttempts}.";

                    if (failedAttempts >= MaxAttempts)
                    {
                        btnLogin.Enabled = false;
                        lblStatus.Text = "Too many failed attempts. Login disabled.";
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Login failed: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void OpenHomeForm(string fullName)
        {
            this.Hide();
            HomeForm home = new HomeForm(fullName);
            // When HomeForm closes (logout), clear this form and show it again.
            // This keeps the app alive without leaving orphan forms.
            home.FormClosed += (s, e) =>
            {
                ClearFields();
                this.Show();
            };
            home.Show();
        }

        private void ClearFields()
        {
            txtUsername.Clear();
            txtPassword.Clear();
            lblStatus.Text = "";
            failedAttempts = 0;
            btnLogin.Enabled = true;
            txtUsername.Focus();
        }

        private void BtnGoToRegister_Click(object sender, EventArgs e)
        {
            RegisterForm registerForm = new RegisterForm();
            registerForm.ShowDialog(); // modal - blocks this form until registration closes
        }
    }
}
