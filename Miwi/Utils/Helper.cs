using System;
using System.Windows.Media;

namespace Miwi.Utils
{
    internal static class Helper
    {
        public static Color InvertColor(Color color)
        {
            color.R = (byte)Math.Abs(255 - color.R);
            color.B = (byte)Math.Abs(255 - color.B);
            color.G = (byte)Math.Abs(255 - color.G);
            return color;
        }

        public static double GetLuminace(Color color)
        {
            double R = color.ScR;
            double G = color.ScG;
            double B = color.ScB;
            if (R <= 0.03928)
            {
                R /= 12.92;
            }
            else
            {
                R = Math.Pow(((R + 0.055) / 1.055), 2.4);
            }
            if (G <= 0.03928)
            {
                G /= 12.92;
            }
            else
            {
                G = Math.Pow(((G + 0.055) / 1.055), 2.4);
            }
            if (B <= 0.03928)
            {
                B /= 12.92;
            }
            else
            {
                B = Math.Pow(((B + 0.055) / 1.055), 2.4);
            }

            return (0.2126 * R) + (0.7152 * G) + (0.0722 * B);
        }

        public static SolidColorBrush AdaptColor1ToColor2(Color color1, Color color2)
        {
            SolidColorBrush cb = new SolidColorBrush(color1);
            if (GetLuminace(color2) >= 0.175)
            {
                if (GetLuminace(color1) >= 0.5)
                {
                    cb = new SolidColorBrush(InvertColor(cb.Color));
                }
            }
            else if (GetLuminace(color2) <= 0.1833)
            {
                if (GetLuminace(color1) <= 0.5)
                {
                    cb = new SolidColorBrush(InvertColor(cb.Color));
                }
            }
            return cb;
        }
    }
}
