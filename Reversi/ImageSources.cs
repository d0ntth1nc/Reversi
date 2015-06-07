namespace Reversi
{
    using System.Collections.Generic;
    using System.Reflection;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;

    internal static class ImageSources
    {
        static ImageSources()
        {
            var thisExe = Assembly.GetExecutingAssembly();
            using (var blackBallStream = thisExe.GetManifestResourceStream("Reversi.Resources.blackball.png"))
            using (var blueBallStream = thisExe.GetManifestResourceStream("Reversi.Resources.glossyBall.png"))
            {
                BlueBall = BitmapFrame.Create(blueBallStream);
                BlackBall = BitmapFrame.Create(blackBallStream);
            }

            // Creating transparent image source
            int width = 128;
            int height = width;
            int stride = width / 8;
            byte[] pixels = new byte[height * stride];
            var colors = new List<Color>() { Colors.Transparent };
            var palette = new BitmapPalette(colors);

            TransparentBall =
                BitmapSource.Create(width, height, 96, 96, PixelFormats.Indexed1, palette, pixels, stride);
        }

        public static BitmapSource BlueBall { get; private set; }

        public static BitmapSource BlackBall { get; private set; }

        public static BitmapSource TransparentBall { get; private set; }
    }
}
