using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DIP_part1_sinoyf1
{
    public partial class Form1 : Form
    {
        Bitmap loaded, loadedBackground, processed;
        public Form1()
        {
            InitializeComponent();
            saveFileDialog1.Filter = "Png (*.png)|*.png";
            saveFileDialog1.DefaultExt = "png";
            saveFileDialog1.AddExtension = true;
        }

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

        private void greyscaleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            processed = new Bitmap(loaded.Width, loaded.Height);

            for(int x = 0; x < loaded.Width; x++)
            {
                for(int y = 0; y < loaded.Height; y++)
                {
                    Color pixel = loaded.GetPixel(x, y);
                    int grey = (pixel.R + pixel.G + pixel.B) / 3;
                    processed.SetPixel(x, y, Color.FromArgb(grey, grey, grey));
                }
            }
            pictureBox2.Image = processed;
        }

        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            processed = new Bitmap(loaded.Width, loaded.Height);

            for (int x = 0; x < loaded.Width; x++)
            {
                for (int y = 0; y < loaded.Height; y++)
                {
                    Color pixel = loaded.GetPixel(x, y);
                    processed.SetPixel(x, y, pixel);
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
                    Color pixel = loaded.GetPixel(x, y);
                    processed.SetPixel(x, y, 
                        Color.FromArgb(255-pixel.R, 255 - pixel.G, 255 - pixel.B));
                }
            }
            pictureBox2.Image = processed;
        }

        private void histogramToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Color pixel;
            Bitmap fakeImage =  new Bitmap(loaded);

            // Greyscale Convertion;
            for (int x = 0; x < fakeImage.Width; x++)
            {
                for (int y = 0; y < fakeImage.Height; y++)
                {
                    pixel = fakeImage.GetPixel(x, y);
                    int greyData = ((pixel.R + pixel.G + pixel.B) / 3);
                    Color grey = Color.FromArgb(greyData, greyData, greyData);
                    fakeImage.SetPixel(x, y, grey);
                }
            }

            // Histogram 1d Data;
            int[] histdata = new int[256]; // array from 0 to 255
            for (int x = 0; x < fakeImage.Width; x++)
            {
                for (int y = 0; y < fakeImage.Height; y++)
                {
                    pixel = fakeImage.GetPixel(x, y);
                    histdata[pixel.R]++; // can be any color property r,g or b
                }
            }

            // Bitmap Graph Generation
            // Setting empty Bitmap with background color
            processed = new Bitmap(256, 800);
            for (int x = 0; x < processed.Width; x++)
            {
                for (int y = 0; y < processed.Height; y++)
                {
                    processed.SetPixel(x, y, Color.White);
                }
            }
            // Plotting points based from histdata
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
                    Color pixel = loaded.GetPixel(x, y);
                    double tr = 0.393 * pixel.R + 0.769 * pixel.G + 0.189 * pixel.B;
                    double tg = 0.349 * pixel.R + 0.686 * pixel.G + 0.168 * pixel.B;
                    double tb = 0.272 * pixel.R + 0.534 * pixel.G + 0.131 * pixel.B;
                    Color changed = Color.FromArgb((int)Math.Min(tr, 255), (int)Math.Min(tg, 255), (int)Math.Min(tb, 255));
                    processed.SetPixel(x, y, changed);
                }
            }
            pictureBox2.Image = processed;
        }

        private void buttonLoadImage_Click(object sender, EventArgs e)
        {
            openFileDialog1.ShowDialog();
        }

        private void buttonLoadedBackground_Click(object sender, EventArgs e)
        {
            openFileDialog2.ShowDialog();
        }
     
        private void buttonSubtract_Click(object sender, EventArgs e)
        {
            processed = new Bitmap(loaded.Width, loaded.Height);

            Color green = Color.FromArgb(0, 255, 0);
            int greygreen = (green.R + green.G + green.B) / 3;
            int threshold = 5;

            for (int x = 0; x < loaded.Width; x++)
            {
                for (int y = 0; y < loaded.Height; y++)
                {
                    Color pixel = loaded.GetPixel(x, y);
                    Color backPixel = loadedBackground.GetPixel(x, y);
                    int grey = (pixel.R + pixel.G + pixel.B) / 3;
                    int subtractValue = Math.Abs(grey - greygreen);
                    if (subtractValue > threshold)
                        processed.SetPixel(x, y, pixel);
                    else
                        processed.SetPixel(x, y, backPixel);
                }
            }
            pictureBox2.Image = processed;
        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            saveFileDialog1.ShowDialog();
        }

    }
}
