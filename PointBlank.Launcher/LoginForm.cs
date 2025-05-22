
using System;
using System.Windows.Forms;
using System.Net;
using System.Text;
using System.Diagnostics;
using System.IO;

namespace PointBlank.Launcher
{
    public partial class LoginForm : Form
    {
        private const string AUTH_SERVER = "http://0.0.0.0:39190";
        private const string GAME_PATH = "game/PointBlank.exe";
        
        public LoginForm()
        {
            InitializeComponent();
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            try 
            {
                string username = txtUsername.Text;
                string password = txtPassword.Text;

                if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                {
                    MessageBox.Show("Por favor preencha todos os campos.", "Aviso");
                    return;
                }

                // Send login request
                using (WebClient client = new WebClient())
                {
                    string data = $"username={username}&password={password}";
                    byte[] postData = Encoding.UTF8.GetBytes(data);
                    
                    client.Headers.Add("Content-Type", "application/x-www-form-urlencoded");
                    byte[] response = client.UploadData($"{AUTH_SERVER}/auth", postData);
                    string token = Encoding.UTF8.GetString(response);

                    if (!string.IsNullOrEmpty(token))
                    {
                        StartGame(token);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao conectar: {ex.Message}", "Erro");
            }
        }

        private void StartGame(string token)
        {
            try
            {
                if (!File.Exists(GAME_PATH))
                {
                    MessageBox.Show("Cliente do jogo não encontrado.", "Erro");
                    return;
                }

                ProcessStartInfo psi = new ProcessStartInfo();
                psi.FileName = GAME_PATH;
                psi.Arguments = $"/token:{token}";
                Process.Start(psi);
                
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao iniciar o jogo: {ex.Message}", "Erro");
            }
        }
    }
}
