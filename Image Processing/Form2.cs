using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WebCamLib;

namespace Image_Processing
{
    public partial class Form2 : Form
    {
        Bitmap imageA, imageB;
        Device[] mgaDevice;

        public Form2()
        {
            InitializeComponent();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Subtract();
        }

        private void Subtract()
        {
            if (imageA == null)
            {
                MessageBox.Show("Please load an image first.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (imageB == null)
            {
                MessageBox.Show("Please load a background first.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Make sure the images are the same size
            if (imageA.Width != imageB.Width || imageA.Height != imageB.Height)
            {   
                if (!webcamLoop.Enabled)
                {
                    DialogResult result = MessageBox.Show("Images must have the same dimensions. Do you want to resize them?", "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                    if (result == DialogResult.No)
                    {
                        return;
                    }
                }
                
                BasicDIP.Scale(ref imageA, imageB.Width, imageB.Height);
            }

            Bitmap output = new Bitmap(imageA.Width, imageA.Height);
            BasicDIP.GreenScreen(ref imageA, ref imageB, ref output, 5);
            pictureBox3.Image = output;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            openFileDialog1.ShowDialog();
        }

        private void openFileDialog1_FileOk(object sender, CancelEventArgs e)
        {
            imageA = new Bitmap(openFileDialog1.FileName);
            pictureBox1.Image = imageA;
            if (webcamLoop.Enabled)
            {
                mgaDevice[0].Stop();
                webcamLoop.Enabled = false;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            openFileDialog2.ShowDialog();
        }

        private void openFileDialog2_FileOk(object sender, CancelEventArgs e)
        {
            imageB = new Bitmap(openFileDialog2.FileName);
            pictureBox2.Image = imageB;
        }

        private void webcamLoop_Tick(object sender, EventArgs e)
        {
            IDataObject data;
            Image bmap;

            mgaDevice[0].Sendmessage();
            data = Clipboard.GetDataObject();

            bmap = (Image)(data.GetData("System.Drawing.Bitmap", true));

            if (bmap != null)
            {
                Bitmap b = new Bitmap(bmap);
                imageA = b;
                Subtract();
            }
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            mgaDevice = DeviceManager.GetAllDevices();
        }

        private void onToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mgaDevice[0].ShowWindow(pictureBox1);
            webcamLoop.Enabled = true;
        }

        private void offToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mgaDevice[0].Stop();
            webcamLoop.Enabled = false;
        }
    }
}
