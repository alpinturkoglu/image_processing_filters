﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using AForge.Imaging.Filters;

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
            comboBox1.Items.Add(Filter.negative);
            comboBox1.Items.Add(Filter.median);
            comboBox1.Items.Add(Filter.mean);
            comboBox1.Items.Add(Filter.smoothing);
            comboBox1.Items.Add(Filter.blur);

            comboBox2.Items.Add("3x3");
            comboBox2.Items.Add("5x5");
            comboBox2.Items.Add("9x9");
            comboBox2.Items.Add("15x15");
            comboBox2.Visible = false;
            label4.Visible = false;
          
            //Picture Box Settings
            pictureBox1.Height = 200;
            pictureBox1.Width = 150;
            pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox2.Height = 200;
            pictureBox2.Width = 150;
            pictureBox2.SizeMode= PictureBoxSizeMode.StretchImage;

        }

     
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (bitmap != null)
            {

                if (comboBox1.SelectedItem.Equals(Filter.negative))
                {
                    Bitmap bmpBitmap=bitmap;
                    Image img = bitmap;

                    bmpBitmap=CopyAsNegative(img, bmpBitmap);
                   
                    label2.Visible = true;
                    pictureBox2.Image = bmpBitmap;
                    pictureBox2.Visible = true;
                   
                }
                else if (comboBox1.SelectedItem.Equals(Filter.median))
                {
                    if (bitmap != null)
                    {
                        comboBox2.Visible = true;

                        if (comboBox2.SelectedIndex > -1)
                        {
                            selectedSizeFilter(size_option);
                        }

                    }
                }
                else if (comboBox1.SelectedItem.Equals(Filter.mean))
                {
                    Mean mean = new Mean();
                    mean.ApplyInPlace(bitmap);
                    pictureBox2.Image = bitmap;
                    pictureBox2.Visible = true;
                }
                else if (comboBox1.SelectedItem.Equals(Filter.blur))
                {
                    Blur blur=new Blur();
                    blur.ApplyInPlace(bitmap);
                    comboBox2.Visible = false;
                    label4.Visible = false;

                    label2.Visible = true;
                    pictureBox2.Image = bitmap;
                    pictureBox2.Visible = true;
                }
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
            showImage(bitmap);
        }

        private void showImage(Bitmap img)
        {
            pictureBox2.Image = img;
            pictureBox2.Visible = true;
            label2.Visible = true;
        }

        #region Buttons

        private void btnClear_Click(object sender, EventArgs e)
        {
            bitmap = null;
            label1.Visible = false;
            label2.Visible = false;
            pictureBox2.Image = null;
            pictureBox1.Image = null;
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
                bitmap = new Bitmap(image);

                label1.Visible = true;

            }
        }

        #endregion

        #region Filter Functions

        public static Bitmap MedianFilter(Bitmap sourceBitmap, int matrixSize, int bias = 0, bool grayscale = false)
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

        public static Bitmap CopyAsNegative(Image sourceImage, Bitmap bmpNew)
        {
            BitmapData bmpData = bmpNew.LockBits(new Rectangle(0, 0, sourceImage.Width, sourceImage.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);


            IntPtr ptr = bmpData.Scan0;


            byte[] byteBuffer = new byte[bmpData.Stride * bmpNew.Height];


            Marshal.Copy(ptr, byteBuffer, 0, byteBuffer.Length);
            byte[] pixelBuffer = null;


            int pixel = 0;


            for (int k = 0; k < byteBuffer.Length; k += 4)
            {
                pixel = ~BitConverter.ToInt32(byteBuffer, k);
                pixelBuffer = BitConverter.GetBytes(pixel);


                byteBuffer[k] = pixelBuffer[0];
                byteBuffer[k + 1] = pixelBuffer[1];
                byteBuffer[k + 2] = pixelBuffer[2];
            }


            Marshal.Copy(byteBuffer, 0, ptr, byteBuffer.Length);


            bmpNew.UnlockBits(bmpData);


            bmpData = null;
            byteBuffer = null;


            return bmpNew;
        }

     
        #endregion

        #region Histogram

      

        #endregion


    }
}
