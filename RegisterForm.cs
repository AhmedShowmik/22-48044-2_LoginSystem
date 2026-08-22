using System;
using System.Windows.Forms;

namespace _22_48044_2_LoginSystem
{
    public class RegisterForm : Form
    {
        private Label lblTitle;
        private TextBox txtUsername, txtPassword, txtConfirmPassword, txtEmail, txtFullName;
        private Button btnRegister, btnBackToLogin;
        private Label lblStatus;

        public RegisterForm()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = "Register";
            this.Size = new System.Drawing.Size(380, 420);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;

            lblTitle = new Label
            {
                Text = "Create Account",
                Font = new System.Drawing.Font("Segoe UI", 14, System.Drawing.FontStyle.Bold),
                Location = new System.Drawing.Point(90, 15),
                AutoSize = true
            };

            int y = 70;
            (Label lbl1, txtUsername) = MakeRow("Username:", y); y += 40;
            (Label lbl2, txtFullName) = MakeRow("Full Name:", y); y += 40;
            (Label lbl3, txtEmail) = MakeRow("Email:", y); y += 40;
            (Label lbl4, txtPassword) = MakeRow("Password:", y, isPassword: true); y += 40;
            (Label lbl5, txtConfirmPassword) = MakeRow("Confirm Password:", y, isPassword: true); y += 50;

            btnRegister = new Button { Text = "Register", Location = new System.Drawing.Point(150, y), Width = 110 };
            btnRegister.Click += BtnRegister_Click;
            y += 40;

            btnBackToLogin = new Button { Text = "Back to Login", Location = new System.Drawing.Point(150, y), Width = 110 };
            btnBackToLogin.Click += (s, e) => this.Close();
            y += 45;

            lblStatus = new Label
            {
                Text = "",
                ForeColor = System.Drawing.Color.Red,
                Location = new System.Drawing.Point(30, y),
                AutoSize = true,
                MaximumSize = new System.Drawing.Size(320, 0)
            };

            this.Controls.Add(lblTitle);
            this.Controls.Add(lbl1); this.Controls.Add(txtUsername);
            this.Controls.Add(lbl2); this.Controls.Add(txtFullName);
            this.Controls.Add(lbl3); this.Controls.Add(txtEmail);
            this.Controls.Add(lbl4); this.Controls.Add(txtPassword);
            this.Controls.Add(lbl5); this.Controls.Add(txtConfirmPassword);
            this.Controls.Add(btnRegister);
            this.Controls.Add(btnBackToLogin);
            this.Controls.Add(lblStatus);
        }

        private (Label, TextBox) MakeRow(string labelText, int y, bool isPassword = false)
        {
            Label lbl = new Label { Text = labelText, Location = new System.Drawing.Point(30, y), AutoSize = true };
            TextBox txt = new TextBox
            {
                Location = new System.Drawing.Point(160, y - 3),
                Width = 180,
                UseSystemPasswordChar = isPassword
            };
            return (lbl, txt);
        }

        // Task 3: validation + duplicate check + hashed, parameterized insert
        private void BtnRegister_Click(object sender, EventArgs e)
        {
            string username = txtUsername.Text.Trim();
            string fullName = txtFullName.Text.Trim();
            string email = txtEmail.Text.Trim();
            string password = txtPassword.Text;
            string confirmPassword = txtConfirmPassword.Text;

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(fullName) ||
                string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password) ||
                string.IsNullOrWhiteSpace(confirmPassword))
            {
                lblStatus.Text = "All fields are required.";
                return;
            }

            if (password.Length < 6)
            {
                lblStatus.Text = "Password must be at least 6 characters.";
                return;
            }

            if (password != confirmPassword)
            {
                lblStatus.Text = "Passwords do not match.";
                return;
            }

            if (!email.Contains("@"))
            {
                lblStatus.Text = "Please enter a valid email address.";
                return;
            }

            try
            {
                if (DatabaseHelper.UsernameExists(username))
                {
                    lblStatus.Text = "Username already taken.";
                    return;
                }

                string passwordHash = DatabaseHelper.HashPassword(password);
                DatabaseHelper.RegisterUser(username, passwordHash, email, fullName);

                MessageBox.Show("Registration successful! You can now log in.", "Success",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                ClearForm();
                this.Close(); // back to login
            }
            catch (Exception ex)
            {
                // Catches things like a UNIQUE constraint violation from a race condition
                lblStatus.Text = "Registration failed: " + ex.Message;
            }
        }

        private void ClearForm()
        {
            txtUsername.Clear();
            txtFullName.Clear();
            txtEmail.Clear();
            txtPassword.Clear();
            txtConfirmPassword.Clear();
            lblStatus.Text = "";
        }
    }
}
