using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ReleaseFileBackupTool
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// ウィンドウ初期化
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();

            this.txtRemoteHost.Text = Properties.Settings.Default.RemoteHost;
            this.txtRemotePort.Text = Properties.Settings.Default.RemotePort;
            this.txtRemoteUser.Text = Properties.Settings.Default.RemoteUser;
            this.txtRemoteBasePath.Text = Properties.Settings.Default.RemoteBasePath;

            this.txtSshPrivateKeyPath.Text = Properties.Settings.Default.SshPrivateKeyPath;

            this.txtReleasePath.Text = Properties.Settings.Default.ReleasePath;
            this.txtBackupPath.Text = Properties.Settings.Default.BackupPath;
        }

        /// <summary>
        /// 実行ボタン押下
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnExecute_Click(object sender, RoutedEventArgs e)
        {
            // 必須チェック
            if (this.pwdPassword.Password == "")
            {
                MessageBox.Show(@"Password is empty.");
                return;
            }

            // ディレクトリ名の末尾を補完
            if (this.txtReleasePath.Text.Substring(this.txtReleasePath.Text.Length - 1) != @"\")
            {
                this.txtReleasePath.Text = this.txtReleasePath.Text + @"\";
            }
            if (this.txtBackupPath.Text.Substring(this.txtBackupPath.Text.Length - 1) != @"\")
            {
                this.txtBackupPath.Text = this.txtBackupPath.Text + @"\";
            }
           
            var sftp = new SftpFileBackup
            {
                RemoteHost = this.txtRemoteHost.Text,
                RemotePort = int.Parse(this.txtRemotePort.Text),
                RemoteUser = this.txtRemoteUser.Text,
                RemoteBasePath = this.txtRemoteBasePath.Text,
                SshPrivateKeyPath = this.txtSshPrivateKeyPath.Text,
                SshPassword = this.pwdPassword.Password,
                ReleasePath = this.txtReleasePath.Text,
                BackupPath = this.txtBackupPath.Text
            };
            sftp.execute();

            foreach (var message in sftp.MessageList)
            {
                this.txtResult.Text = this.txtResult.Text + message + "\n";
            }
            MessageBox.Show(@"complate");
        }

        /// <summary>
        /// クローズ処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Properties.Settings.Default.RemoteHost = this.txtRemoteHost.Text;
            Properties.Settings.Default.RemotePort = this.txtRemotePort.Text;
            Properties.Settings.Default.RemoteUser = this.txtRemoteUser.Text;
            Properties.Settings.Default.RemoteBasePath = this.txtRemoteBasePath.Text;

            Properties.Settings.Default.SshPrivateKeyPath = this.txtSshPrivateKeyPath.Text;

            Properties.Settings.Default.ReleasePath = this.txtReleasePath.Text;
            Properties.Settings.Default.BackupPath = this.txtBackupPath.Text;

            Properties.Settings.Default.Save();
        }
    }
}
