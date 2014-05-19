using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Drawing;
using System.Web.UI.WebControls;
using System.IO;

namespace ZsfProject.Tools
{
    public class ImageValidate
    {
        public static string CreateCode(int codeLength)
        {

            string so = "1,2,3,4,5,6,7,8,9,0"
                + ",a,b,c,d,e,f,g,h,i,j,k,l,m,n,o,p,q,r,s,t,u,v,w,x,y,z"
                + ",A,B,C,D,E,F,G,H,I,J,K,L,M,N,O,P,Q,R,S,T,U,V,W,X,Y,Z";

            string[] strArr = so.Split(',');
            string code = "";
            Random rand = new Random();
            for (int i = 0; i < codeLength; i++)
                code += strArr[rand.Next(0, strArr.Length)];

            return code;
        }

        /*产生验证图片*/
        public static MemoryStream CreateImage(string code)
        {

            Bitmap image = new Bitmap(code.Length * 15, 30);

            Graphics g = Graphics.FromImage(image);
            g.Clear(Color.FromArgb(236, 226, 226));

            Random random = new Random();
            //画图片的背景噪音线
            for (int i = 0; i < 5; i++)
            {
                int x1 = random.Next(image.Width);
                int x2 = random.Next(image.Width);
                int y1 = random.Next(image.Height);
                int y2 = random.Next(image.Height);

                g.DrawLine(new Pen(Color.LightGray), x1, y1, x2, y2);
            }

            Font font = new Font("Arial", 15, FontStyle.Italic);

            Brush brush = new SolidBrush(Color.FromArgb(random.Next(0, 150), random.Next(0, 150), random.Next(0, 150)));

            char[] codeChars = code.ToCharArray();
            for (int i = 0; i < codeChars.Length; i++)
            {
                g.DrawString(codeChars[i].ToString(), font, brush, i * 12 + code.Length - 2, random.Next(0, 10));
                brush = new SolidBrush(Color.FromArgb(random.Next(0, 150), random.Next(0, 150), random.Next(0, 150)));
            }

            //画图片的前景噪音点
            for (int i = 0; i < 5; i++)
            {
                int x1 = random.Next(image.Width);
                int x2 = random.Next(image.Width);
                int y1 = random.Next(image.Height);
                int y2 = random.Next(image.Height);

                g.DrawLine(new Pen(Color.LightGray), x1, y1, x2, y2);
            }

            MemoryStream ms = new MemoryStream();
            image.Save(ms, System.Drawing.Imaging.ImageFormat.Gif);
            g.Dispose();
            image.Dispose();
            ms.Seek(0, SeekOrigin.Begin);
            return ms;
        }
    }
}