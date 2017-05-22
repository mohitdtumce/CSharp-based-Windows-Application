using System;
using System.Drawing;
using System.Resources;
using System.IO;

namespace Uni_Form_Trans_Test_01
{
    public class Animate
    {
        #region    Declarations

        private string name;
        private int length;
        private Canvas canvas;
        private int delay, idx;
        private Point[,] eyeLocations;
        private string[] fileNames;
        static public float rescaleFactor = 1f;

        #endregion

        public Animate(string name, int width, int length, int delay, float rescaleFactor, ref Canvas canvas, ref ResourceManager rm)
        {
            this.name = name;
            this.length = length;
            this.canvas = canvas;
            this.delay = delay;
            idx = 0;
            eyeLocations = new Point[length, 2];
            fileNames = new string[length];
            Animate.SetScale(rescaleFactor);

            for (int i = 0; i < length; i++)
            {
                string fileName = name;
                int l = (i.ToString()).Length;
                for (int j = 0; j < width - l; j++)
                    fileName += '0';
                fileName += i.ToString();
                fileNames[i] = fileName;
            }

            //Read Location Here
            string loc = System.Text.Encoding.UTF8.GetString((byte[])Properties.Resources.ResourceManager.GetObject(name));

            string temp = "";
            int flag = 0;
            float p = 0;
            int spCount = 0;
            string xc = "";
            string yc = "";
            for (int i = 0; i < loc.Length; i++)
            {
                if (loc[i] != ' ')
                {
                    temp += loc[i];
                }
                else if (loc[i] == ' ')
                {
                    spCount++;
                    if (spCount == 1)
                    {
                        xc = temp;
                    }
                    if (spCount == 2)
                    {
                        yc = temp;
                        spCount = 0;
                        eyeLocations[(int)p % length, flag].X = int.Parse(xc);
                        eyeLocations[(int)p % length, flag].Y = int.Parse(yc);
                        p = p + 0.5f;
                        flag = 1 - flag;
                    }
                    temp = "";
                }
            }
        }

        public void SetDelay(int delay)
        {
            this.delay = delay;
        }

        public bool Update(long cnt, int x, int y)
        {
            // Returns True If Current Animation Is Over
            idx %= length;
            if (cnt % delay == 0)
            {
                Bitmap tmp;
                tmp = (Bitmap)Properties.Resources.ResourceManager.GetObject(fileNames[idx]);
                Bitmap tmp2 = new Bitmap(tmp, new Size((int)(tmp.Width / rescaleFactor),
                                    (int)(tmp.Height / rescaleFactor)));
                canvas.AddBitmap(ref tmp2, x, y);
                tmp.Dispose();
                tmp2.Dispose();
                idx++;
            }
            else
            {
                Bitmap tmp = (Bitmap)Bitmap.FromFile(fileNames[idx]);
                Bitmap tmp2 = new Bitmap(tmp, new Size((int)(tmp.Width / rescaleFactor),
                                    (int)(tmp.Height / rescaleFactor)));
                canvas.AddBitmap(ref tmp2, x, y);
                tmp.Dispose();
                tmp2.Dispose();
            }
            if(idx == length)
            {
                return true;
            }
            return false;
        }

        public void Reset()
        {
            idx = 0;
        }

        public static void LoadBitmap(string fileName, ref Bitmap bmp,double rescaleFactor=1.0f)
        {
            bmp = (Bitmap)Properties.Resources.ResourceManager.GetObject(fileName);
            Bitmap tmp = new Bitmap(bmp, new Size((int)(bmp.Width / rescaleFactor),(int)(bmp.Height / rescaleFactor)));
            bmp.Dispose();
            bmp = tmp;
        }

        public Point GetLocation(int flag)
        {
            Point ret = eyeLocations[Math.Max(0, idx - 1), flag];
            ret.X = (int)(ret.X / rescaleFactor);
            ret.Y = (int)(ret.Y / rescaleFactor);
            return ret;
        }

        public static void SetScale(float scale)
        {
            rescaleFactor = scale;
        }
    }

}
