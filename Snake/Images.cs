using System;
using System.Windows.Media.Imaging;
using System.Windows.Media;


namespace Snake
{
    public static class Images
    {
        public static readonly Dictionary<string, (ImageSource Body, ImageSource Head, ImageSource DeadBody, ImageSource DeadHead)> SnakeColors =
            new Dictionary<string, (ImageSource Body, ImageSource Head, ImageSource DeadBody, ImageSource DeadHead)>
        {
        { "Green", (LoadImage("BodyGreen.png"), LoadImage("HeadGreen.png"), LoadImage("DeadBodyGreen.png"), LoadImage("DeadHeadGreen.png")) },
        { "Blue", (LoadImage("BodyBlue.png"), LoadImage("HeadBlue.png"), LoadImage("DeadBodyBlue.png"), LoadImage("DeadHeadBlue.png")) },
        { "Red", (LoadImage("BodyRed.png"), LoadImage("HeadRed.png"), LoadImage("DeadBodyRed.png"), LoadImage("DeadHeadRed.png")) },
        { "Yellow", (LoadImage("BodyYellow.png"), LoadImage("HeadYellow.png"), LoadImage("DeadBodyYellow.png"), LoadImage("DeadHeadYellow.png")) }
        };

        public static string SelectedColor = "Green";

        public static ImageSource Empty = LoadImage("Empty.png");
        public static ImageSource Food = LoadImage("Food.png");

        private static ImageSource LoadImage(string fileName)
        {
            return new BitmapImage(new Uri($"Assets/{fileName}", UriKind.Relative));
        }
    }

}
