using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using WebCamLib;

namespace DIP_part1_sinoyf1
{
    public partial class Form1 : Form
    {
        Bitmap loaded, loadedBackground, processed, fakeImage, b;
        Device[] cameras;
        Image bmap;
        IDataObject data;
        Color pixel, backPixel, greyColor, green, changed;
        int grey, greygreen, threshold, subtractValue;
        int[] histdata;
        double tr, tg, tb;

        public Form1()
        {
            InitializeComponent();
            saveFileDialog1.Filter = "Png (*.png)|*.png";
            saveFileDialog1.DefaultExt = "png";
            saveFileDialog1.AddExtension = true;
        }

        // Dialogs
        private void openFileDialog1_FileOk(object sender, CancelEventArgs e)
        {
            loaded = new Bitmap(openFileDialog1.FileName);
            pictureBox1.Image = loaded;
        }

        private void openFileDialog2_FileOk(object sender, CancelEventArgs e)
        {
            loadedBackground = new Bitmap(openFileDialog2.FileName);
            pictureBox3.Image = loadedBackground;
        }

        private void saveFileDialog1_FileOk(object sender, CancelEventArgs e)
        {
            pictureBox2.Image.Save(saveFileDialog1.FileName);
        }


        // Camera Process
        private void subtractionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            timerSubtraction.Enabled = true;
        }

        private void stopToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            timerSubtraction.Enabled = false;
        }

        // DIP
        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            processed = new Bitmap(loaded.Width, loaded.Height);

            for (int x = 0; x < loaded.Width; x++)
            {
                for (int y = 0; y < loaded.Height; y++)
                {
                    pixel = loaded.GetPixel(x, y);
                    processed.SetPixel(x, y, pixel);
                }
            }
            pictureBox2.Image = processed;
        }
        
        private void greyscaleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            processed = new Bitmap(loaded.Width, loaded.Height);

            for (int x = 0; x < loaded.Width; x++)
            {
                for (int y = 0; y < loaded.Height; y++)
                {
                    pixel = loaded.GetPixel(x, y);
                    grey = (pixel.R + pixel.G + pixel.B) / 3;
                    processed.SetPixel(x, y, Color.FromArgb(grey, grey, grey));
                }
            }
            pictureBox2.Image = processed;
        }

        private void colorInversionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            processed = new Bitmap(loaded.Width, loaded.Height);

            for (int x = 0; x < loaded.Width; x++)
            {
                for (int y = 0; y < loaded.Height; y++)
                {
                    pixel = loaded.GetPixel(x, y);
                    processed.SetPixel(x, y, 
                        Color.FromArgb(255-pixel.R, 255 - pixel.G, 255 - pixel.B));
                }
            }
            pictureBox2.Image = processed;
        }

        private void histogramToolStripMenuItem_Click(object sender, EventArgs e)
        {
            fakeImage =  new Bitmap(loaded);

            for (int x = 0; x < fakeImage.Width; x++)
            {
                for (int y = 0; y < fakeImage.Height; y++)
                {
                    pixel = fakeImage.GetPixel(x, y);
                    grey = ((pixel.R + pixel.G + pixel.B) / 3);
                    greyColor = Color.FromArgb(grey, grey, grey);
                    fakeImage.SetPixel(x, y, greyColor);
                }
            }

            histdata = new int[256];
            for (int x = 0; x < fakeImage.Width; x++)
            {
                for (int y = 0; y < fakeImage.Height; y++)
                {
                    pixel = fakeImage.GetPixel(x, y);
                    histdata[pixel.R]++; 
                }
            }

            processed = new Bitmap(256, 800);
            for (int x = 0; x < processed.Width; x++)
            {
                for (int y = 0; y < processed.Height; y++)
                {
                    processed.SetPixel(x, y, Color.White);
                }
            }
          
            for (int x = 0; x < processed.Width; x++)
            {
                for (int y = 0; y < Math.Min(histdata[x] / 5, processed.Height - 1); y++)
                {
                    processed.SetPixel(x, (processed.Height - 1) - y, Color.Black);
                }
            }
            pictureBox2.Image = processed;
        }
        
        private void sepiaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            processed = new Bitmap(loaded.Width, loaded.Height);

            for (int x = 0; x < loaded.Width; x++)
            {
                for (int y = 0; y < loaded.Height; y++)
                {
                    pixel = loaded.GetPixel(x, y);
                    tr = 0.393 * pixel.R + 0.769 * pixel.G + 0.189 * pixel.B;
                    tg = 0.349 * pixel.R + 0.686 * pixel.G + 0.168 * pixel.B;
                    tb = 0.272 * pixel.R + 0.534 * pixel.G + 0.131 * pixel.B;
                    changed = Color.FromArgb((int)Math.Min(tr, 255), (int)Math.Min(tg, 255), (int)Math.Min(tb, 255));
                    processed.SetPixel(x, y, changed);
                }
            }
            pictureBox2.Image = processed;
        }

        private void buttonSubtract_Click(object sender, EventArgs e)
        {
            processed = new Bitmap(loaded.Width, loaded.Height);

            green = Color.FromArgb(0, 255, 0);
            greygreen = (green.R + green.G + green.B) / 3;
            threshold = 5;

            for (int x = 0; x < loaded.Width; x++)
            {
                for (int y = 0; y < loaded.Height; y++)
                {
                    pixel = loaded.GetPixel(x, y);
                    backPixel = loadedBackground.GetPixel(x, y);
                    grey = (pixel.R + pixel.G + pixel.B) / 3;
                    subtractValue = Math.Abs(grey - greygreen);
                    if (subtractValue > threshold)
                        processed.SetPixel(x, y, pixel);
                    else
                        processed.SetPixel(x, y, backPixel);
                }
            }
            pictureBox2.Image = processed;
        }

        // Buttons
        private void buttonLoadImage_Click(object sender, EventArgs e)
        {
            openFileDialog1.ShowDialog();
        }

        private void buttonLoadedBackground_Click(object sender, EventArgs e)
        {
            openFileDialog2.ShowDialog();
        }

        private void buttonGetCam_Click(object sender, EventArgs e)
        {
            cameras = DeviceManager.GetAllDevices();
        }

        private void buttonShowCam_Click(object sender, EventArgs e)
        {
            cameras[0].ShowWindow(pictureBox1);
        }

        private void buttonClose_Click(object sender, EventArgs e)
        {
            cameras[0].Stop();
        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            saveFileDialog1.ShowDialog();
        }

        // Timers
        private void timerSubtraction_Tick(object sender, EventArgs e)
        {
            cameras[0].Sendmessage();
            data = Clipboard.GetDataObject();
            bmap = (Image)(data.GetData("System.Drawing.Bitmap", true));
            b = new Bitmap(bmap);
            processed = new Bitmap(b.Width, b.Height);
            
            green = Color.FromArgb(0, 255, 0);
            greygreen = (green.R + green.G + green.B) / 3;
            threshold = 10;

            for (int x = 0; x < b.Width; x++)
            {
                for (int y = 0; y < b.Height; y++)
                {
                    pixel = b.GetPixel(x, y);

                    if (x < loadedBackground.Width && x > 0)
                        backPixel = loadedBackground.GetPixel(x, y);

                    grey = (pixel.R + pixel.G + pixel.B) / 3;
                    subtractValue = Math.Abs(grey - greygreen);
                    if (subtractValue > threshold)
                        processed.SetPixel(x, y, pixel);
                    else
                        processed.SetPixel(x, y, backPixel);
                }
            }
            pictureBox2.Image = processed;
        }
    }
}
