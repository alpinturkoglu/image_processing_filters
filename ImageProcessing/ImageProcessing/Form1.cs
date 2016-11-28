using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ImageProcessing
{
    public partial class Form1 : Form
    {
        private Bitmap bitmap;
        private int size_option;
        public Form1()
        {
            InitializeComponent();
            
            //Combo Box Settings
            comboBox1.Items.Add(Filter.minFilter);
            comboBox1.Items.Add(Filter.maxFilter);
            comboBox1.Items.Add(Filter.medianFilter);
            comboBox1.Items.Add(Filter.smoothing);
            comboBox1.Items.Add(Filter.thresholding);

            comboBox2.Items.Add("3x3");
            comboBox2.Items.Add("5x5");
            comboBox2.Items.Add("9x9");
            comboBox2.Items.Add("15x15");
          
            //Picture Box Settings
            pictureBox1.Height = 200;
            pictureBox1.Width = 150;
            pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox2.Height = 200;
            pictureBox2.Width = 150;
            pictureBox2.SizeMode= PictureBoxSizeMode.StretchImage;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var activationContext = Type.GetTypeFromProgID("matlab.application.single");
            var matlab = (MLApp.MLApp)Activator.CreateInstance(activationContext);
            Console.WriteLine(matlab.Execute("1+2"));
        }

        private void button2_Click(object sender, EventArgs e)
        {
            
            DialogResult result = openFileDialog1.ShowDialog(); // Show the dialog.
            if (result == DialogResult.OK) // Test result.
            {
                string file = openFileDialog1.FileName;
                Image image = Image.FromFile(file);
                pictureBox1.Image = image;
                bitmap=new Bitmap(image);
                
                label1.Visible = true;

            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
           
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            //İşlemler bitince picturebox ve label2 visible olacak
            if (comboBox1.SelectedItem.Equals(Filter.minFilter))
            {
                
            }
            else if (comboBox1.SelectedItem.Equals(Filter.maxFilter))
            {
                
            }
            else if (comboBox1.SelectedItem.Equals(Filter.medianFilter))
            {
                if (bitmap != null)
                {
                    if (comboBox2.SelectedIndex > -1)
                    {
                        selectedSizeFilter(size_option);
                    }
                   
                }
               
              
            }
            else if (comboBox1.SelectedItem.Equals(Filter.smoothing))
            {
                
            }
            else if (comboBox1.SelectedItem.Equals(Filter.averageFilter))
            {
                
            }
            else if (comboBox1.SelectedItem.Equals(Filter.thresholding))
            {
                
            }
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox2.SelectedItem.Equals("3x3"))
            {
                size_option = 3;
                
            }
            else if (comboBox2.SelectedItem.Equals("5x5"))
            {
                size_option = 5;
               
            }
            else if (comboBox2.SelectedItem.Equals("9x9"))
            {
                size_option = 9;
            }
            else if (comboBox2.SelectedItem.Equals("15x15"))
            {
                size_option = 15;
            }

            comboBox1_SelectedIndexChanged(sender,e);
        }

        private void selectedSizeFilter(int size)
        {

            bitmap = MedianFilter(bitmap, size, 0, true);
            pictureBox2.Image = bitmap;
            pictureBox2.Visible = true;
            label2.Visible = true;
        }

        public static Bitmap MedianFilter(Bitmap sourceBitmap,
                                            int matrixSize,
                                              int bias = 0,
                                    bool grayscale = false)
        {
            BitmapData sourceData =
                       sourceBitmap.LockBits(new Rectangle(0, 0,
                       sourceBitmap.Width, sourceBitmap.Height),
                       ImageLockMode.ReadOnly,
                       PixelFormat.Format32bppArgb);


            byte[] pixelBuffer = new byte[sourceData.Stride *
                                          sourceData.Height];


            byte[] resultBuffer = new byte[sourceData.Stride *
                                           sourceData.Height];


            Marshal.Copy(sourceData.Scan0, pixelBuffer, 0,
                                       pixelBuffer.Length);


            sourceBitmap.UnlockBits(sourceData);


            if (grayscale == true)
            {
                float rgb = 0;


                for (int k = 0; k < pixelBuffer.Length; k += 4)
                {
                    rgb = pixelBuffer[k] * 0.11f;
                    rgb += pixelBuffer[k + 1] * 0.59f;
                    rgb += pixelBuffer[k + 2] * 0.3f;


                    pixelBuffer[k] = (byte)rgb;
                    pixelBuffer[k + 1] = pixelBuffer[k];
                    pixelBuffer[k + 2] = pixelBuffer[k];
                    pixelBuffer[k + 3] = 255;
                }
            }


            int filterOffset = (matrixSize - 1) / 2;
            int calcOffset = 0;


            int byteOffset = 0;

            List<int> neighbourPixels = new List<int>();
            byte[] middlePixel;


            for (int offsetY = filterOffset; offsetY <
                sourceBitmap.Height - filterOffset; offsetY++)
            {
                for (int offsetX = filterOffset; offsetX <
                    sourceBitmap.Width - filterOffset; offsetX++)
                {
                    byteOffset = offsetY *
                                 sourceData.Stride +
                                 offsetX * 4;


                    neighbourPixels.Clear();


                    for (int filterY = -filterOffset;
                        filterY <= filterOffset; filterY++)
                    {
                        for (int filterX = -filterOffset;
                            filterX <= filterOffset; filterX++)
                        {


                            calcOffset = byteOffset +
                                         (filterX * 4) +
                                (filterY * sourceData.Stride);


                            neighbourPixels.Add(BitConverter.ToInt32(
                                             pixelBuffer, calcOffset));
                        }
                    }


                    neighbourPixels.Sort();

                    middlePixel = BitConverter.GetBytes(
                                       neighbourPixels[filterOffset]);


                    resultBuffer[byteOffset] = middlePixel[0];
                    resultBuffer[byteOffset + 1] = middlePixel[1];
                    resultBuffer[byteOffset + 2] = middlePixel[2];
                    resultBuffer[byteOffset + 3] = middlePixel[3];
                }
            }


            Bitmap resultBitmap = new Bitmap(sourceBitmap.Width,
                                             sourceBitmap.Height);


            BitmapData resultData =
                       resultBitmap.LockBits(new Rectangle(0, 0,
                       resultBitmap.Width, resultBitmap.Height),
                       ImageLockMode.WriteOnly,
                       PixelFormat.Format32bppArgb);


            Marshal.Copy(resultBuffer, 0, resultData.Scan0,
                                       resultBuffer.Length);


            resultBitmap.UnlockBits(resultData);


            return resultBitmap;
        }

       
    }
}
