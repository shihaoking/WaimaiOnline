using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Drawing;

namespace ZsfProject.Tools
{
    public static class PhoneToImage
    {
        public static MemoryStream DrawImage(string phoneNumber)
        {
            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            phoneNumber = phoneNumber.Replace(" / ", "/").Replace(" /", "/").Replace("/ ", "/").Replace("/", " / ");
            using (Bitmap image = new Bitmap(phoneNumber.Length * 15, 35))
            {
                Graphics g = Graphics.FromImage(image);

                g.Clear(Color.FromArgb(153, 204, 0));

                Font font = new Font(FontFamily.GenericSansSerif, 15, FontStyle.Bold, GraphicsUnit.Pixel);
                Brush brush = new SolidBrush(Color.White);
                SizeF size = g.MeasureString(phoneNumber, font);

                Rectangle rec = new Rectangle(0, 0, (int)Math.Ceiling(size.Width) + 20 , 35);
                StringFormat sf = new StringFormat(StringFormat.GenericDefault);
                sf.Alignment = StringAlignment.Center;
                sf.LineAlignment = StringAlignment.Center;

                g.DrawString(phoneNumber, font, brush, rec, sf);

                rec.Width = rec.Width + 1;
                Bitmap newImg = image.Clone(rec, image.PixelFormat);
                newImg.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
                newImg.Dispose(); g.Dispose();
                ms.Seek(0, System.IO.SeekOrigin.Begin);
            }
            return ms;
        }
    }
}