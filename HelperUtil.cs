using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Collections;

namespace HGHelperUtilities
{
    public static class HelperUtil
    {
        public static Bitmap CropImage(Image orgImg, Rectangle sRect, Rectangle destRect)
        {
            if (destRect == null)
            {
                destRect = new Rectangle(Point.Empty, sRect.Size);
            }
            var cropImage = new Bitmap(destRect.Width, destRect.Height);
            using (var graphics = Graphics.FromImage(cropImage))
            {
                graphics.DrawImage(orgImg, destRect, sRect, GraphicsUnit.Pixel);
            }
            return cropImage;
        }

        public static Bitmap ResizeBitmap(Bitmap sourceBMP, int scale)
        {
            return ResizeBitmap(sourceBMP, sourceBMP.Width * scale, sourceBMP.Height * scale);
        }

        public static Bitmap ResizeBitmap(Bitmap sourceBMP, int width, int height)
        {
            Bitmap result = new Bitmap(width, height);
            using (Graphics g = Graphics.FromImage(result))
            {
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
                g.SmoothingMode = SmoothingMode.None;
                g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                g.CompositingQuality = CompositingQuality.AssumeLinear;
                g.DrawImage(sourceBMP, 0, 0, width, height);
            }
            return result;
        }

        public static T Clamp<T>(this T val, T min, T max) where T : IComparable<T>
        {
            if (val.CompareTo(min) < 0) return min;
            else if (val.CompareTo(max) > 0) return max;
            else return val;
        }

        public static void CopyRegionIntoImage(Bitmap srcBitmap, Rectangle srcRegion, ref Bitmap destBitmap, Rectangle destRegion)
        {
            using (Graphics g = Graphics.FromImage(destBitmap))
            {
                g.CompositingMode = CompositingMode.SourceOver;
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
                g.SmoothingMode = SmoothingMode.None;
                g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                g.CompositingQuality = CompositingQuality.AssumeLinear;
                g.DrawImage(srcBitmap, destRegion, srcRegion, GraphicsUnit.Pixel);
            }
        }
        public static int to1D(int x, int y,int xMax)
        {
            return (y * xMax) + x;
        }
        public static int to1D(int x, int y, int z, int xMax, int yMax)
        {
            return (z * xMax * yMax) + (y * xMax) + x;
        }

        public static int[] to3D(int idx, int xMax, int yMax)
        {
            int z = idx / (xMax * yMax);
            idx -= (z * xMax * yMax);
            int y = idx / xMax;
            int x = idx % xMax;
            return new int[] { x, y, z };
        }

        //by Tim Lloyd via https://stackoverflow.com/questions/4791202/most-efficient-way-to-reverse-the-order-of-a-bitarray
        public static void BitArrayReverse(BitArray array)
        {
            int length = array.Length;
            int mid = (length / 2);

            for (int i = 0; i < mid; i++)
            {
                bool bit = array[i];
                array[i] = array[length - i - 1];
                array[length - i - 1] = bit;
            }
        }

        public static int GetIntFromBitArray(BitArray bitArray)
        {

            if (bitArray.Length > 32)
                throw new ArgumentException("Argument length shall be at most 32 bits.");

            int[] array = new int[1];
            bitArray.CopyTo(array, 0);
            return array[0];

        }
        //WordWrap by honey the codewitch via https://www.codeproject.com/Tips/5271389/Simple-Word-Wrapping-in-Csharp
        public static string WordWrap(this string text, int width, params char[] wordBreakChars)
        {
            char[] _WordBreakChars = new char[] { ' ', '_', '\t', '.', '+', '-', '(', ')', '[', ']', '\"', /*'\'',*/ '{', '}', '!', '<', '>', '~', '`', '*', '$', '#', '@', '!', '\\', '/', ':', ';', ',', '?', '^', '%', '&', '|', '\n', '\r', '\v', '\f', '\0' };

            if (string.IsNullOrEmpty(text) || 0 == width || width >= text.Length)
                return text;
            if (null == wordBreakChars || 0 == wordBreakChars.Length)
                wordBreakChars = _WordBreakChars;
            var sb = new StringBuilder();
            var sr = new StringReader(text);
            string line;
            var first = true;
            while (null != (line = sr.ReadLine()))
            {
                var col = 0;
                if (!first)
                {
                    sb.AppendLine();
                    col = 0;
                }
                else
                    first = false;
                var words = line.Split(wordBreakChars);

                for (var i = 0; i < words.Length; i++)
                {
                    var word = words[i];
                    if (0 != i)
                    {
                        sb.Append(" ");
                        ++col;
                    }
                    if (col + word.Length > width)
                    {
                        sb.AppendLine();
                        col = 0;
                    }
                    sb.Append(word);
                    col += word.Length;
                }
            }
            return sb.ToString();
        }
    }
}
