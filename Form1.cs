using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.IO;
namespace Youtube_picture_downloader
{
    //image = dlClient.DownloadData("http://i.ytimg.com/vi/" + vidId + "/maxresdefault.jpg");
    //ytUrl = "http://i.ytimg.com/vi/" + vidId + "/maxresdefault.jpg";
    //image = dlClient.DownloadData("http://i.ytimg.com/vi/" + vidId + "/hqdefault.jpg");
    //ytUrl = "http://i.ytimg.com/vi/" + vidId + "/hqdefault.jpg";
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            /* Download THE BYTES (don't store a local file) of the picture */
            WebClient dlClient = new WebClient();
            byte[] image = null;
            string ytUrl = "";
            string vidId = GetVideoID(textBox1.Text);

            lblStatus.Text = "Getting preview..."; Refresh();
            HttpWebResponse response = null;
            HttpWebRequest[] requests = new HttpWebRequest[2] {(HttpWebRequest)WebRequest.Create("http://i.ytimg.com/vi/" + vidId + "/maxresdefault.jpg"),
                                                              (HttpWebRequest)WebRequest.Create("http://i.ytimg.com/vi/" + vidId + "/hqdefault.jpg")};
            foreach (HttpWebRequest request in requests)
            {
                try
                {
                    response = (HttpWebResponse)request.GetResponse();

                    image = dlClient.DownloadData(request.RequestUri);
                    ytUrl = request.RequestUri.AbsoluteUri;
                }
                catch (WebException ex)
                { /* Do nothing and continue to the next request */ }
                finally
                {
                    if (response != null)
                    {
                        response.Close();
                    }
                }
            }
            MemoryStream mem = new MemoryStream(image);
            Image preview = Image.FromStream(mem);

            /* Update the preview */
            pictureBox1.Image = preview;
            lblSize.Text = preview.Size.Width + "x" + preview.Size.Height;
            lblStatus.Text = "Idle";

            /* Ask the user if everything is OK and download the image */
            if (MessageBox.Show("Does the image look OK?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                /* Download the file */
                using (SaveFileDialog sfd = new SaveFileDialog())
                {
                    sfd.Title = "Where do you want to save the image?";
                    sfd.Filter = "JPG Image (*.jpg)|*.jpg";
                    sfd.AddExtension = true;
                    if (sfd.ShowDialog() == DialogResult.OK)
                    {
                        lblStatus.Text = "Downloading image..."; Refresh();
                        dlClient.DownloadFile(ytUrl, sfd.FileName);

                        MessageBox.Show("Done!", "Yay", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        lblStatus.Text = "Done!";
                    }
                }
            }
        }

        private string GetVideoID(string url)
        {
            return url.Substring(url.IndexOf("=") + 1, 11);
        }

        private bool IsValidYtURL(string url)
        {
            /* I might not need the OR "http" since I think all YT links are now https */
            return url.StartsWith("https://www.youtube.com/watch?v=") && GetVideoID(url).Length == 11;
        }

        private void textBox1_MouseEnter(object sender, EventArgs e)
        {
            textBox1.Text = Clipboard.ContainsText() ? IsValidYtURL(Clipboard.GetText()) ? Clipboard.GetText() : textBox1.Text : textBox1.Text;
        }
    }
}
