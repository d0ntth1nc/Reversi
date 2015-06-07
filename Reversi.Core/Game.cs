namespace Reversi.Core
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading.Tasks;

    public class Game
    {
        public const int DefaultGameSize = 8;

        private bool _isStarted = false;
        private Player _firstPlayer = null;
        private Player _secondPlayer = null;

        public Game(Player firstPlayer, Player secondPlayer, int fieldSize = DefaultGameSize)
        {
            if (fieldSize % 2 != 0)
            {
                throw new ArgumentException("Game size must be divisible by 2!");
            }

            if (firstPlayer == null || secondPlayer == null)
            {
                throw new ArgumentNullException("Both players cannot be null!");
            }

            firstPlayer.DiskType = PlaceType.FirstPlayer;
            secondPlayer.DiskType = PlaceType.SecondPlayer;

            Field = new Field(fieldSize);
            _isStarted = false;
            _firstPlayer = firstPlayer;
            _secondPlayer = secondPlayer;
        }

        public delegate void GameOver();

        /// <summary>
        /// Fired when the game is over
        /// </summary>
        public event GameOver OnGameOver;

        /// <summary>
        /// Returns the game field
        /// </summary>
        public Field Field { get; private set; }

        /// <summary>
        /// Starts the game
        /// </summary>
        public async void Start()
        {
            if (_isStarted)
            {
                throw new InvalidOperationException("Game is already started!");
            }

            _isStarted = true;

            while (_isStarted)
            {
                bool hasFirstPlayerTurned = await PlayerTurn(_firstPlayer);
                bool hasSecondPlayerTurned = await PlayerTurn(_secondPlayer);

                if (!hasFirstPlayerTurned || !hasSecondPlayerTurned)
                {
                    _isStarted = false;
                    break;
                }
            }

            _firstPlayer.Score = Field
                    .Where(x => x.Type == _firstPlayer.DiskType)
                    .Count();

            _secondPlayer.Score = Field
                .Where(x => x.Type == _secondPlayer.DiskType)
                .Count();

            if (OnGameOver != null)
            {
                OnGameOver();
            }
        }

        private async Task<bool> PlayerTurn(Player player)
        {
            var playerAllCaptures = GetAllPlayerCaptures(player);
            if (playerAllCaptures.Count == 0)
            {
                return false;
            }

            var playerSelection = await player.Select(playerAllCaptures);
            Debug.Assert(playerAllCaptures.ContainsKey(playerSelection),
                "Player returned place that cannot be captured!");

            var captures = playerAllCaptures[playerSelection];
            captures.Add(playerSelection);

            foreach (var place in captures)
            {
                place.Type = player.DiskType;
            }

            return true;
        }

        private Dictionary<FieldPlace, List<FieldPlace>> GetAllPlayerCaptures(Player player)
        {
            var captures = new Dictionary<FieldPlace, List<FieldPlace>>();
            var neutralPlaces = Field.Where(x => x.Type == PlaceType.Neutral);

            foreach (var neutralPlace in neutralPlaces)
            {
                var currentCaptures = Field.GetCaptures(neutralPlace, player.DiskType);

                if (currentCaptures.Count == 0)
                {
                    continue;
                }

                if (captures.ContainsKey(neutralPlace))
                {
                    captures[neutralPlace].AddRange(currentCaptures);
                }
                else
                {
                    captures.Add(neutralPlace, currentCaptures);
                }
            }

            return captures;
        }
    }
}