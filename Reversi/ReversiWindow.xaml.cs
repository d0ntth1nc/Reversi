namespace Reversi
{
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using System.Windows.Media;
    using Core;

    public partial class ReversiWindow : Window
    {
        private const int FieldSize = 8;
        private const int ImageSize = 50; //pixels

        private readonly ImagePlace[,] Field = null;

        private Game _game = null;
        private int _gameIndex = 0;
        private UserPlayer _player = null;
        private Computer _computer = null;

        public ReversiWindow()
        {
            Field = new ImagePlace[FieldSize, FieldSize];
            InitializeComponent();
            Reset();
        }

        private void Reset()
        {
            GameField.Children.Clear();
            CreateGame();
            CreateField();
            _game.Start();
        }

        private void CreateGame()
        {
            _player = new UserPlayer("Player");
            _player.OnTurn += OnPlayerTurn;
            _computer = new Computer(FieldSize);

            if (_gameIndex++ % 2 == 0)
            {
                _game = new Game(_player, _computer);
            }
            else
            {
                _game = new Game(_computer, _player);
            }

            _game.OnGameOver += OnGameOver;
        }

        private void CreateField()
        {
            foreach (var fieldPlace in _game.Field)
            {
                int y = fieldPlace.Y;
                int x = fieldPlace.X;
                int top = y * ImageSize;
                int left = x * ImageSize;
                var border = new Border()
                {
                    BorderThickness = new Thickness(1),
                    BorderBrush = Brushes.Black,
                    Margin = new Thickness(left, top, 0, 0)
                };

                GameField.Children.Add(border);

                var imagePlace = new ImagePlace(y, x, ImageSize, _player);
                imagePlace.Place = fieldPlace;
                imagePlace.MouseUp += OnMouseClick;
                imagePlace.MouseEnter += OnMouseEnter;
                imagePlace.MouseLeave += OnMouseLeave;

                border.Child = imagePlace;
                Field[y, x] = imagePlace;
                RenderOptions.SetBitmapScalingMode(imagePlace, BitmapScalingMode.Fant);
            }
        }

        private void OnPlayerTurn(UserPlayer sender)
        {
            foreach (var capture in sender.PossibleCaptures)
            {
                var targetPlace = capture.Key;
                var imagePlace = Field[targetPlace.Y, targetPlace.X];
                imagePlace.Captures = capture.Value;
            }
        }

        private void OnGameOver()
        {
            int result = _player.Score.CompareTo(_computer.Score);
            string message = string.Empty;

            if (result < 0)
            {
                message = "You lost! Play again?";
            }
            else if (result > 0)
            {
                message = "You won! Play again?";
            }
            else
            {
                message = "Tied game! Play again?";
            }
            
            var selection = MessageBox.Show(message, "Game over",
                MessageBoxButton.YesNo, MessageBoxImage.Question);
            
            if (selection == MessageBoxResult.No)
            {
                Application.Current.Shutdown();
            }
            else
            {
                Reset();
            }
        }

        private void OnMouseEnter(object sender, MouseEventArgs args)
        {
            var imagePlace = (ImagePlace)sender;
            if (imagePlace.Captures == null || imagePlace.Captures.Count == 0)
            {
                return;
            }

            imagePlace.MarkAsPossibleCapture();
            foreach (var place in imagePlace.Captures)
            {
                Field[place.Y, place.X].MarkAsPossibleCapture();
            }
        }

        private void OnMouseLeave(object sender, MouseEventArgs args)
        {
            var imagePlace = (ImagePlace)sender;
            if (imagePlace.Captures == null || imagePlace.Captures.Count == 0)
            {
                return;
            }

            imagePlace.Update();
            foreach (var place in imagePlace.Captures)
            {
                Field[place.Y, place.X].Update();
            }
        }

        private void OnMouseClick(object sender, MouseEventArgs args)
        {
            var imagePlace = (ImagePlace)sender;
            if (imagePlace.Captures == null || imagePlace.Captures.Count == 0)
            {
                return;
            }
            _player.Select(imagePlace.Place);
            FinishPlayerTurn();
        }

        private void FinishPlayerTurn()
        {
            foreach (var capture in _player.PossibleCaptures)
            {
                var targetPlace = capture.Key;
                var imagePlace = Field[targetPlace.Y, targetPlace.X];
                imagePlace.Captures = null;
            }
        }
    }
}
