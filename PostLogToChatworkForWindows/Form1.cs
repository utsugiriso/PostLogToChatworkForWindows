using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Net.Http;
using System.Runtime.Serialization.Json;

namespace PostLogToChatworkForWindows
{
    public partial class Form1 : Form
    {
        private delegate void DelegateRead(string message);
        private bool watching = false;
        Process powerShellProcess;
        private static HttpClient client = null;

        public Form1()
        {
            InitializeComponent();

            client = new HttpClient();

            textBoxLogFilePath.Text = Properties.Settings.Default.logFilePath;
            textBoxFilter.Text = Properties.Settings.Default.filter;
            textBoxApiToken.Text = Properties.Settings.Default.apiToken;
            textBoxRoomId.Text = Properties.Settings.Default.roomId;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                textBoxLogFilePath.Text = openFileDialog1.FileName;
            }
        }

        private void button2_Click(object sender2, EventArgs e2)
        {
            if (watching)
            {
                watching = false;
                button2.Text = "ウォッチを開始";

                textBoxLogFilePath.ReadOnly = false;
                textBoxApiToken.ReadOnly = false;
                textBoxRoomId.ReadOnly = false;

                if (powerShellProcess != null & !powerShellProcess.HasExited)
                    powerShellProcess.CloseMainWindow();
            }
            else
            {
                if (string.IsNullOrEmpty(textBoxLogFilePath.Text))
                {
                    MessageBox.Show("ログファイルパスを入力してください", "入力漏れ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                if (string.IsNullOrEmpty(textBoxApiToken.Text))
                {
                    MessageBox.Show("APIトークンを入力してください", "入力漏れ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                if (string.IsNullOrEmpty(textBoxRoomId.Text))
                {
                    MessageBox.Show("ルームIDを入力してください", "入力漏れ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                if (!File.Exists(textBoxLogFilePath.Text))
                {
                    MessageBox.Show("ログファイルが存在しません", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                watching = true;
                button2.Text = "ウォッチを停止";

                textBoxLogFilePath.ReadOnly = true;
                textBoxApiToken.ReadOnly = true;
                textBoxRoomId.ReadOnly = true;

                client.DefaultRequestHeaders.Remove("X-ChatWorkToken");
                client.DefaultRequestHeaders.Add("X-ChatWorkToken", textBoxApiToken.Text);

                powerShellProcess = new Process();
                powerShellProcess.StartInfo.FileName = "powershell.exe";
                powerShellProcess.StartInfo.UseShellExecute = false;
                powerShellProcess.StartInfo.Arguments = $"& Get-Content -Path '{textBoxLogFilePath.Text}' -Tail 0 -Wait";
                powerShellProcess.StartInfo.RedirectStandardOutput = true;
                powerShellProcess.OutputDataReceived += new DataReceivedEventHandler(async (sender, e) =>
                {
                    try
                    {
                        if (!string.IsNullOrEmpty(e.Data) && (string.IsNullOrEmpty(textBoxFilter.Text) || Regex.IsMatch(e.Data, textBoxFilter.Text)))
                        {
                            string requestUri = $"https://api.chatwork.com/v2/rooms/{textBoxRoomId.Text}/messages";
                            HttpResponseMessage response = await client.PostAsync(requestUri, new FormUrlEncodedContent(new Dictionary<string, string> { { "body", e.Data } }));
                            Stream stream = await response.Content.ReadAsStreamAsync();
                            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(MessagesResponce));
                            MessagesResponce messagesResponce = (MessagesResponce)serializer.ReadObject(stream);

                            requestUri = $"https://api.chatwork.com/v2/rooms/{textBoxRoomId.Text}/messages/unread";
                            await client.PostAsync(requestUri, new FormUrlEncodedContent(new Dictionary<string, string> { { "message_id", messagesResponce.messageID } }));
                        }
                    }
                    catch (Exception exception)
                    {
                        MessageBox.Show(exception.Message, "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                });
                powerShellProcess.Start();
                powerShellProcess.BeginOutputReadLine();
            }

        }

        private void SaveSettings()
        {
            Properties.Settings.Default.logFilePath = textBoxLogFilePath.Text;
            Properties.Settings.Default.filter = textBoxFilter.Text;
            Properties.Settings.Default.apiToken = textBoxApiToken.Text;
            Properties.Settings.Default.roomId = textBoxRoomId.Text;
            Properties.Settings.Default.Save();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            SaveSettings();
            if (powerShellProcess != null && !powerShellProcess.HasExited)
                powerShellProcess.CloseMainWindow();
        }
    }
}
