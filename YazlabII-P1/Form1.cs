using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace Basic_Puzzle
{
    public partial class Form1 : Form
    {
        List<Button> puzzlePieces = new List<Button>();
        List<int> shuffle = new List<int>();
        static Bitmap puzzleImg;
        int pieceH;
        int pieceW;
        int OyunPuan =100;
        public Form1()
        {
            InitializeComponent();
            puzzlePieces = new List<Button> { button1, button2, button3, button4, button5, button6, button7, button8, button9, button10, button11, button12, button13, button14, button15, button16 };

            for (int i = 0; i < 16; i++)
            {
                puzzlePieces[i].Visible = false;
            }
            bt_shuffle.Enabled = false;

            isimBox.Enabled = false;
            isimButon.Enabled = false;
            txtOku();
        }

        public void txtOku()
        {
            string dosya_yolu = "puanTablosu.txt";
            FileStream fs = new FileStream(dosya_yolu, FileMode.Open, FileAccess.Read);
            StreamReader sw = new StreamReader(fs);
            string yazi = sw.ReadLine();
            while (yazi != null)
            {
                puanList.Items.Add(yazi);
                yazi = sw.ReadLine();
            }
            sw.Close();
            fs.Close();
        }
        public void txtYaz()
        {
            string dosya_yolu = "puanTablosu.txt";
            FileStream fs = new FileStream(dosya_yolu, FileMode.Open, FileAccess.Write);
            StreamWriter sw = new StreamWriter(fs);
            //puanList   listBox1.Items[listBox1.SelectedIndex].ToString();

            int sayac=0;
            while (sayac < puanList.Items.Count)
            {
                sw.WriteLine(puanList.Items[sayac].ToString());
                sayac++;
            }
            sw.Close();
            fs.Close();
        }

        private Bitmap ölçekle()
        {
            int target_height = 420;
            int target_width = 750;

            Bitmap rsm = puzzleImg ;
            Image target_image;
            target_image = (Image)rsm;
            Rectangle dest_rect = new Rectangle(0, 0, target_width, target_height);
            Bitmap destImage = new Bitmap(target_width, target_height);
            destImage.SetResolution(target_image.HorizontalResolution, target_image.VerticalResolution);
            using (var g = Graphics.FromImage(destImage))
            {
                g.CompositingMode = CompositingMode.SourceCopy;
                g.CompositingQuality = CompositingQuality.HighQuality;
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.SmoothingMode = SmoothingMode.HighQuality;
                g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                using (var wrapmode = new ImageAttributes())
                {
                    wrapmode.SetWrapMode(WrapMode.TileFlipXY);
                    g.DrawImage(target_image, dest_rect, 0, 0, target_image.Width, target_image.Height, GraphicsUnit.Pixel, wrapmode);
                }
            }
            return destImage;
        }

        private void dosyaIslem()
        {
            OpenFileDialog dosya = new OpenFileDialog();
            dosya.Filter = "Resim Dosyası |*.jpg;*.nef;*.png| Video|*.avi| Tüm Dosyalar |*.*";

            dosya.ShowDialog();
            string DosyaYolu = dosya.FileName;
            puzzleImg = new Bitmap(DosyaYolu);
            if (puzzleImg.Height > 418 || puzzleImg.Width > 740)
            {
                puzzleImg=ölçekle();
            }

            pieceW = puzzleImg.Width / 4;
            pieceH = puzzleImg.Height / 4;

            for (int k = 0; k < 4; k++)
                for (int l = 0; l < 4; l++)
                    puzzlePieces[k * 4 + l].SetBounds(5 + k * pieceW, 5 + l * pieceH, pieceW, pieceH);

            for (int i = 0; i < 16; i++)
            {
                puzzlePieces[i].Visible = true;
                puzzlePieces[i].Enabled = false;
            }
            bt_shuffle.Enabled = true;


            karistir();
            ShowPuzzle();
            puanLabel.Text = OyunPuan.ToString();


        }
        private void ShowPuzzle()
        {
            for (int j = 0; j < 4; j++)
            {
                for (int i = 0; i < 4; i++)
                {
                    Rectangle pieceSize = new Rectangle(i * pieceW, j * pieceH, pieceW, pieceH);
                    Bitmap piece = puzzleImg.Clone(pieceSize, puzzleImg.PixelFormat);
                    puzzlePieces[shuffle[j * 4 + i]].BackgroundImage = piece;
                }
            }

            if (bt_shuffle.Enabled)
            {
                for (int c = 0; c < 16; c++)
                {
                    if (Compare(c))
                    {
                        bt_shuffle.Enabled = false;
                        for (int i = 0; i < 16; i++)
                        {
                            puzzlePieces[i].Enabled = true;
                        }

                        break;
                    }

                }
            }

        }
        int change1 = 0;
        int change2 = 0;
        private void degistir(int c1, int c2)
        {
            Image pieceCh1 = puzzlePieces[c1 - 1].BackgroundImage;
            Image pieceCh2 = puzzlePieces[c2 - 1].BackgroundImage;
            puzzlePieces[c1 - 1].BackgroundImage = puzzlePieces[c2 - 1].BackgroundImage;
            puzzlePieces[c2 - 1].BackgroundImage = pieceCh1;
            bool finish = true;
            if (Compare(c1 - 1) && Compare(c2 - 1))
            {
                for (int c = 0; c < 16; c++)
                {
                    if (!Compare(c))
                    {
                        finish = false;
                        break;
                    }
                  
                }
                
                if (finish == true)
                {
                    isimBox.Enabled = true;
                    isimButon.Enabled = true;
                    
                    MessageBox.Show("TEBRİKLER !!! İsminizi Giriniz.");
                    for (int c = 0; c < 16; c++)
                    {
                        puzzlePieces[c].Enabled = false;
                    }
                }

            }
            else if (Compare(c1 - 1))
            {
                puzzlePieces[c1 - 1].Enabled = false;
                //puan değişmedi
            }
                
            else if (Compare(c2 - 1))
            {
                puzzlePieces[c2 - 1].Enabled = false;
                //puan değişmedi
            }
            else if ((Compare(c1 - 1) && Compare(c2 - 1)) == false)
            {
                OyunPuan = OyunPuan - 5;
                puanLabel.Text = OyunPuan.ToString();
                if (OyunPuan <= 0)
                {
                    MessageBox.Show("Oyunu Kaybettiniz");
                    
                    OyunPuan = 100;
                    for (int i = 0; i < 16; i++)
                    {
                        puzzlePieces[i].Enabled = true;
                    }
                    bt_shuffle.Enabled = true;
                    puanLabel.Text = OyunPuan.ToString();
                    
                }
            }


            change1 = 0;
            change2 = 0;
            pieceCh1 = null;
            pieceCh2 = null;
        }

        private void bt_shuffle_Click(object sender, EventArgs e)
        {
            karistir();
            ShowPuzzle();
        }
        private void karistir()
        {
            Random rnd = new Random();
            shuffle.Clear();
            while (shuffle.Count != 16)
            {
                int rand = rnd.Next(0, 16);
                if (!shuffle.Contains(rand))
                    shuffle.Add(rand);
                else
                    continue;
            }
        }
        private bool Compare(int posButton)
        {
            int i = posButton / 4;
            int j = posButton % 4;
            int equalScore = 0;
            int diffScore = 0;
            int Score = 0;
            Rectangle pieceSize = new Rectangle(i * pieceW, j * pieceH, pieceW, pieceH);
            Bitmap orjPiece = puzzleImg.Clone(pieceSize, puzzleImg.PixelFormat);
            Bitmap puzzPiece = (Bitmap)puzzlePieces[posButton].BackgroundImage;
            for (int x = 0; x < orjPiece.Width - 1; x++)
            {
                for (int y = 0; y < puzzPiece.Height - 1; y++)
                {
                    if (orjPiece.GetPixel(x, y) == puzzPiece.GetPixel(x, y))
                    {
                        equalScore++;
                    }
                    else
                        diffScore++;
                }
            }
            Score = (equalScore / (equalScore + diffScore)) * 100;
            if (Score > 10)
                return true;
            else
                return false;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (change1 == 0)
                change1 = 1;
            else
                degistir(change1, 1);
        }
        private void button2_Click(object sender, EventArgs e)
        {
            if (change1 == 0)
                change1 = 2;
            else
                degistir(change1, 2);
        }
        private void button3_Click(object sender, EventArgs e)
        {
            if (change1 == 0)
                change1 = 3;
            else
                degistir(change1, 3);
        }
        private void button4_Click(object sender, EventArgs e)
        {
            if (change1 == 0)
                change1 = 4;
            else
                degistir(change1, 4);
        }
        private void button5_Click(object sender, EventArgs e)
        {
            if (change1 == 0)
                change1 = 5;
            else
                degistir(change1, 5);
        }
        private void button6_Click(object sender, EventArgs e)
        {
            if (change1 == 0)
                change1 = 6;
            else
                degistir(change1, 6);
        }
        private void button7_Click(object sender, EventArgs e)
        {
            if (change1 == 0)
                change1 = 7;
            else
                degistir(change1, 7);
        }
        private void button8_Click(object sender, EventArgs e)
        {
            if (change1 == 0)
                change1 = 8;
            else
                degistir(change1, 8);
        }
        private void button9_Click(object sender, EventArgs e)
        {
            if (change1 == 0)
                change1 = 9;
            else
                degistir(change1, 9);
        }
        private void button10_Click(object sender, EventArgs e)
        {
            if (change1 == 0)
                change1 = 10;
            else
                degistir(change1, 10);
        }
        private void button11_Click(object sender, EventArgs e)
        {
            if (change1 == 0)
                change1 = 11;
            else
                degistir(change1, 11);
        }
        private void button12_Click(object sender, EventArgs e)
        {
            if (change1 == 0)
                change1 = 12;
            else
                degistir(change1, 12);
        }
        private void button13_Click(object sender, EventArgs e)
        {
            if (change1 == 0)
                change1 = 13;
            else
                degistir(change1, 13);
        }
        private void button14_Click(object sender, EventArgs e)
        {
            if (change1 == 0)
                change1 = 14;
            else
                degistir(change1, 14);
        }
        private void button15_Click(object sender, EventArgs e)
        {
            if (change1 == 0)
                change1 = 15;
            else
                degistir(change1, 15);
        }
        private void button16_Click(object sender, EventArgs e)
        {
            if (change1 == 0)
                change1 = 16;
            else
                degistir(change1, 16);
        }
        private void button17_Click(object sender, EventArgs e)
        {
            dosyaIslem();
            OyunPuan = 100;
            for (int i = 0; i < 16; i++)
            {
                puzzlePieces[i].Enabled = true;
            }
            bt_shuffle.Enabled = true;
            puanLabel.Text = OyunPuan.ToString();

        }
        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void isimButon_Click(object sender, EventArgs e)
        {
                puanList.Items.Add(isimBox.Text +":"+ OyunPuan.ToString());
                isimButon.Enabled = false;
                isimBox.Enabled = false;
                txtYaz();
        }
    }
}
