using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Renci.SshNet;

namespace ReleaseFileBackupTool
{
    class SftpFileBackup
    {
        public string RemoteHost { set; get; }
        public int RemotePort { set; get; }
        public string RemoteUser { set; get; }
        public string RemoteBasePath { set; get; }

        public string SshPrivateKeyPath { set; get; }
        public string SshPassword { set; get; }

        public string ReleasePath { set; get; }
        public string BackupPath { set; get; }

        public List<string> MessageList { private set; get; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public SftpFileBackup()
        {
            this.MessageList = new List<string>();
        }

        /// <summary>
        /// バックアップ実行
        /// </summary>
        public void execute()
        {
            var authMethod = new PrivateKeyAuthenticationMethod(this.RemoteUser, new PrivateKeyFile(this.SshPrivateKeyPath, this.SshPassword));
            var connectionInfo = new ConnectionInfo(this.RemoteHost, this.RemotePort, this.RemoteUser, authMethod);

            using (var client = new SftpClient(connectionInfo))
            {
                // リリースファイル一覧を探索
                string[] files = Directory.GetFiles(this.ReleasePath, "*", SearchOption.AllDirectories);
                foreach (var file in files)
                {
                    // 絶対パスを相対パスへ変換
                    Uri basePath = new Uri(this.ReleasePath);
                    Uri relativePath = basePath.MakeRelativeUri(new Uri(file));

                    string remoteFile = relativePath.ToString();

                    // バックアップ先ファイル名を決定
                    string backupFilePath = this.BackupPath + relativePath;

                    // ディレクトリが存在しない場合は作成
                    string directoryPath = Path.GetDirectoryName(backupFilePath);
                    if (!Directory.Exists(directoryPath))
                    {
                        Directory.CreateDirectory(directoryPath);
                    }

                    // ダウンロード
                    client.Connect();
                    client.ChangeDirectory(this.RemoteBasePath);
                    if (client.Exists(remoteFile))
                    {
                        using (var fs = File.OpenWrite(backupFilePath))
                        {
                            client.DownloadFile(remoteFile, fs);
                        }
                        this.MessageList.Add("Success : " + relativePath);
                    }
                    else
                    {
                        this.MessageList.Add("FileNotFound : " + relativePath);

                    }
                    client.Disconnect();
                }
            }
        }
    }
}
