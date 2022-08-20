using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace charmap
{
    class Images
    {
        public static Dictionary<string, Bitmap> images = new Dictionary<string, Bitmap>();
    }

    class ImageProc
    {
        public static bool IsMatch(Bitmap _source, Bitmap _template)
        {
            Mat match = new Mat();

            double min = 0;
            double max = 0;
            Point minLoc = new Point();
            Point maxLoc = new Point();

            _source = Resize(_source, 100, 100);
            _template = Resize(_template, 100, 100);

           // _template.Save("C:/aa.jpg");

            CvInvoke.MatchTemplate(new Image<Gray, Byte>(_source), new Image<Gray, Byte>(_template), match, Emgu.CV.CvEnum.TemplateMatchingType.CcoeffNormed);
            CvInvoke.MinMaxLoc(match, ref min, ref max, ref minLoc, ref maxLoc);

            if (max > 0.7)
            {
                return true;
            }

            return false;
        }

        private static Bitmap Resize(Bitmap bmp, int width, int height)
        {
            Bitmap result = new Bitmap(width, height);

            using (Graphics g = Graphics.FromImage(result))
            {
                g.DrawImage(bmp, 0, 0, width, height);
            }

            return result;
        }

        public static Bitmap CaptureScreen(Point topLeft, Size capturesize)
        {
            Bitmap capture = new Bitmap(capturesize.Width, capturesize.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            var gfx = Graphics.FromImage(capture);

            gfx.CopyFromScreen(topLeft.X, topLeft.Y, 0, 0, capturesize, CopyPixelOperation.SourceCopy);

            return capture;
        }
    }
}
