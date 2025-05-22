
using System.Windows.Forms;

namespace PointBlank.Launcher
{
    partial class LoginForm
    {
        private System.ComponentModel.IContainer components = null;
        private TextBox txtUsername;
        private TextBox txtPassword;
        private Button btnLogin;
        private Label lblUsername;
        private Label lblPassword;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.txtUsername = new TextBox();
            this.txtPassword = new TextBox();
            this.btnLogin = new Button();
            this.lblUsername = new Label();
            this.lblPassword = new Label();

            // lblUsername
            this.lblUsername.Location = new System.Drawing.Point(12, 15);
            this.lblUsername.Size = new System.Drawing.Size(70, 23);
            this.lblUsername.Text = "Usuário:";

            // txtUsername
            this.txtUsername.Location = new System.Drawing.Point(88, 12);
            this.txtUsername.Size = new System.Drawing.Size(184, 20);

            // lblPassword
            this.lblPassword.Location = new System.Drawing.Point(12, 41);
            this.lblPassword.Size = new System.Drawing.Size(70, 23);
            this.lblPassword.Text = "Senha:";

            // txtPassword
            this.txtPassword.Location = new System.Drawing.Point(88, 38);
            this.txtPassword.Size = new System.Drawing.Size(184, 20);
            this.txtPassword.PasswordChar = '*';

            // btnLogin
            this.btnLogin.Location = new System.Drawing.Point(88, 64);
            this.btnLogin.Size = new System.Drawing.Size(184, 23);
            this.btnLogin.Text = "Entrar";
            this.btnLogin.Click += new System.EventHandler(this.btnLogin_Click);

            // LoginForm
            this.ClientSize = new System.Drawing.Size(284, 101);
            this.Controls.AddRange(new Control[] {
                this.lblUsername,
                this.txtUsername,
                this.lblPassword,
                this.txtPassword,
                this.btnLogin
            });
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Text = "Point Blank - Login";
            this.StartPosition = FormStartPosition.CenterScreen;
        }
    }
}
