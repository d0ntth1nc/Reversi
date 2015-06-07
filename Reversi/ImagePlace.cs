namespace Reversi
{
    using Core;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;

    internal class ImagePlace : Image
    {
        private FieldPlace _place = null;
        private UserPlayer _player = null;

        public ImagePlace(int y, int x, int size, UserPlayer player)
        {
            Y = y;
            X = x;
            Width = size;
            Height = size;
            HorizontalAlignment = HorizontalAlignment.Left;
            VerticalAlignment = VerticalAlignment.Top;
            Stretch = Stretch.Fill;
            Source = ImageSources.TransparentBall;

            if (player == null)
            {
                throw new ArgumentNullException("Player cannot be null!");
            }

            _player = player;
        }

        public int Y { get; private set; }

        public int X { get; private set; }

        public List<FieldPlace> Captures { get; set; }

        public FieldPlace Place
        {
            get
            {
                return _place;
            }

            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException();
                }

                _place = value;
                _place.OnTypeChanged += (sender) => Update();
                Update();
            }
        }

        public void MarkAsPossibleCapture()
        {
            Debug.Assert(_player.DiskType != PlaceType.Neutral);

            var imgSource = _player.DiskType == PlaceType.FirstPlayer ?
                ImageSources.BlueBall : ImageSources.BlackBall;

            Opacity = 0.3;
            Source = imgSource;
        }

        public void Update()
        {
            Opacity = 1;

            switch (Place.Type)
            {
                case PlaceType.FirstPlayer:
                    Source = ImageSources.BlueBall;
                    break;
                case PlaceType.SecondPlayer:
                    Source = ImageSources.BlackBall;
                    break;
                case PlaceType.Neutral:
                    Source = ImageSources.TransparentBall;
                    break;
            }
        }
    }
}