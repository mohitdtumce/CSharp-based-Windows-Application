using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace Uni_Form_Trans_Test_01
{
    public class Canvas
    {
        private Bitmap BitmapObject;
        public int Width, Height;
        Graphics gfx;

        // Constructor for bitmap being passed as default value
        public Canvas(Bitmap bmp)
        {
            BitmapObject = bmp;
            gfx = Graphics.FromImage(BitmapObject);
        }

        // Constructor for only make canvas
        public Canvas(int width, int height)
        {
            BitmapObject = new Bitmap(width, height);
            this.Width = width;
            this.Height = height;
            gfx = Graphics.FromImage(BitmapObject);
        }

        public void Resize(int width, int height)
        {
            if (BitmapObject != null)
                BitmapObject.Dispose();
            BitmapObject = new Bitmap(width, height);
            this.Width = width;
            this.Height = height;
            gfx = Graphics.FromImage(BitmapObject);
        }

        // Returns current bitmap
        public Bitmap GetBitmap()
        {
            return BitmapObject;
        }

        // This is an important function. Overlays the new bitmap over the existing canvas
        public void AddBitmap(ref Bitmap bmpObj, int x, int y)
        {
            gfx.DrawImage(bmpObj, new Point(x, y));
        }

        public void Clear()
        {
            gfx = Graphics.FromImage(BitmapObject);
            gfx.Clear(Color.Transparent);
            //gfx.DrawRectangle(Pens.Red, 0, 0, Width - 1, Height - 1);
        }

        public void FlipBitmap()
        {
            BitmapObject.RotateFlip(RotateFlipType.Rotate180FlipY);
        }

        public void SetTransparency(float alphaLevel)
        {
            Bitmap img = new Bitmap(BitmapObject);
            Clear();
            Graphics graphics = Graphics.FromImage(BitmapObject);
            ColorMatrix colormatrix = new ColorMatrix();
            colormatrix.Matrix33 = alphaLevel;
            ImageAttributes imgAttribute = new ImageAttributes();
            imgAttribute.SetColorMatrix(colormatrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);
            graphics.DrawImage(img, new Rectangle(0, 0, BitmapObject.Width, BitmapObject.Height), 0, 0, img.Width, img.Height, GraphicsUnit.Pixel, imgAttribute);
            graphics.Dispose();   // Releasing all resource used by graphics
            img.Dispose();
            imgAttribute.Dispose();
        }
    }

}
