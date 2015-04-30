// Simple Player sample application
// AForge.NET framework
// http://www.aforgenet.com/framework/
//
// Copyright © AForge.NET, 2006-2011
// contacts@aforgenet.com
//

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.Net.Sockets;
using AForge.Video;
using AForge.Video.DirectShow;

namespace Player
{
    public partial class MainForm : Form
    {
        //Int32 port = 23;
        TcpClient client = new TcpClient();
        private Stopwatch stopWatch = null;
        private int start = Environment.TickCount;


        // Class constructor
        // Initialized form and starts video stream
        public MainForm()
        {
            InitializeComponent();
 //           label1.Text = charCount.ToString();
       
            // create video source
            MJPEGStream mjpegSource = new MJPEGStream("http://192.168.0.115:99/videostream.cgi?user=admin&pwd=");
            // open it
            OpenVideoSource(mjpegSource);
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            CloseCurrentVideoSource();
        }

        // "Exit" menu item clicked
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        // Capture 1st display in the system
        private void capture1stDisplayToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenVideoSource(new ScreenCaptureStream(Screen.AllScreens[0].Bounds, 100));
        }

        // Open video source
        private void OpenVideoSource(IVideoSource source)
        {
            // set busy cursor
            this.Cursor = Cursors.WaitCursor;

            // stop current video source
            CloseCurrentVideoSource();

            // start new video source
            videoSourcePlayer.VideoSource = source;
            videoSourcePlayer.Start();

            // reset stop watch
            stopWatch = null;

            // start timer
            timer.Start();

            this.Cursor = Cursors.Default;
        }

        // Close video source if it is running
        private void CloseCurrentVideoSource()
        {
            if (videoSourcePlayer.VideoSource != null)
            {
                videoSourcePlayer.SignalToStop();

                // wait ~ 3 seconds
                for (int i = 0; i < 30; i++)
                {
                    if (!videoSourcePlayer.IsRunning)
                        break;
                    System.Threading.Thread.Sleep(100);
                }

                if (videoSourcePlayer.IsRunning)
                {
                    videoSourcePlayer.Stop();
                }

                videoSourcePlayer.VideoSource = null;
            }
        }

        // New frame received by the player
        private void videoSourcePlayer_NewFrame(object sender, ref Bitmap image)
        {
           
            DateTime now = DateTime.Now;
            Graphics g = Graphics.FromImage(image);

            // paint current time
            SolidBrush brush = new SolidBrush(Color.Red);
            g.DrawString(now.ToString(), this.Font, brush, new PointF(5, 5));
            brush.Dispose();

            g.Dispose();
        }

        // On timer event - gather statistics
        private void timer_Tick(object sender, EventArgs e)
        {
            IVideoSource videoSource = videoSourcePlayer.VideoSource;

            if (videoSource != null)
            {
                // get number of frames since the last timer tick
                int framesReceived = videoSource.FramesReceived;

                if (stopWatch == null)
                {
                    stopWatch = new Stopwatch();
                    stopWatch.Start();
                }
                else
                {
                    stopWatch.Stop();

                    float fps = 1000.0f * framesReceived / stopWatch.ElapsedMilliseconds;
                    fpsLabel.Text = fps.ToString("F2") + " fps";

                    stopWatch.Reset();
                    stopWatch.Start();
                }
            }
        }

        // Connect to wifi shield s
        private void connect_button_Click(object sender, EventArgs e)
        {
            // Create a TcpClient.
            // Note, for this client to work you need to have a TcpServer 
            // connected to the same address as specified by the server, port
            // combination.

            client.Connect("192.168.0.101", 23);
            lbl_connection_status.Text = "Client Socket Program - Server Connected ...";

        }

        private void up_button_Click(object sender, EventArgs e)
        {
            label2.Text = "Up";
            byte[] buf = System.Text.Encoding.ASCII.GetBytes("U");
            client.GetStream().Write(buf, 0, buf.Length);
        }

        private void right_button_Click(object sender, EventArgs e)
        {
            label2.Text = "Right";
            byte[] buf = System.Text.Encoding.ASCII.GetBytes("R");
            client.GetStream().Write(buf, 0, buf.Length);
        }

        private void down_button_Click(object sender, EventArgs e)
        {
            label2.Text = "Down";
            byte[] buf = System.Text.Encoding.ASCII.GetBytes("D");
            client.GetStream().Write(buf, 0, buf.Length);
        }

        private void left_button_Click(object sender, EventArgs e)
        {
            label2.Text = "Left";
            byte[] buf = System.Text.Encoding.ASCII.GetBytes("L");
            client.GetStream().Write(buf, 0, buf.Length);
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {

            byte[] buf = System.Text.Encoding.ASCII.GetBytes("None");
            if ( (Environment.TickCount - start) > 300) 
            {
                switch (keyData)
                {
                    case Keys.Up:
                        start = Environment.TickCount;
                        label2.Text = "Up";
                        buf = System.Text.Encoding.ASCII.GetBytes("U");
                        client.GetStream().Write(buf, 0, buf.Length);
                        break;
                    case Keys.Down:
                        start = Environment.TickCount;
                        label2.Text = "Down";
                        buf = System.Text.Encoding.ASCII.GetBytes("D");
                        client.GetStream().Write(buf, 0, buf.Length);
                        break;
                    case Keys.Left:
                        start = Environment.TickCount;
                        label2.Text = "Left";
                        buf = System.Text.Encoding.ASCII.GetBytes("L");
                        client.GetStream().Write(buf, 0, buf.Length);
                        break;
                    case Keys.Right:
                        start = Environment.TickCount;
                        label2.Text = "Right";
                        buf = System.Text.Encoding.ASCII.GetBytes("R");
                        client.GetStream().Write(buf, 0, buf.Length);
                        break;
                }
            }
            return true;
        }
   
    }
}
