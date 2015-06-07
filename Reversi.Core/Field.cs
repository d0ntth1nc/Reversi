namespace Reversi.Core
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    public class Field : IEnumerable<FieldPlace>
    {
        private readonly dynamic[] Directions =
        {
            new { Y = -1, X = -1 },
            new { Y = -1, X = 0 },
            new { Y = -1, X = 1 },
            new { Y = 0, X = -1 },
            new { Y = 0, X = 1 },
            new { Y = 1, X = -1 },
            new { Y = 1, X = 0 },
            new { Y = 1, X = 1 },
        };

        private FieldPlace[,] _field = null;

        internal Field(int size)
        {
            if (size % 2 != 0)
            {
                throw new ArgumentException("Field size must be divisible by 2!");
            }

            _field = new FieldPlace[size, size];

            for (int row = 0; row < size; row++)
            {
                for (int col = 0; col < size; col++)
                {
                    this[row, col] = new FieldPlace(row, col, PlaceType.Neutral);
                }
            }

            this[size / 2 - 1, size / 2 - 1].Type = PlaceType.FirstPlayer;
            this[size / 2 - 1, size / 2].Type = PlaceType.SecondPlayer;
            this[size / 2, size / 2 - 1].Type = PlaceType.SecondPlayer;
            this[size / 2, size / 2].Type = PlaceType.FirstPlayer;
        }

        public FieldPlace this[int row, int col]
        {
            get
            {
                return _field[row, col];
            }

            private set
            {
                _field[row, col] = value;
            }
        }

        public IEnumerator<FieldPlace> GetEnumerator()
        {
            for (int row = 0; row < _field.GetLength(0); row++)
            {
                for (int col = 0; col < _field.GetLength(1); col++)
                {
                    yield return this[row, col];
                }
            }
        }

        internal List<FieldPlace> GetCaptures(FieldPlace disk, PlaceType playerDiskType)
        {
            if (disk == null)
            {
                throw new ArgumentNullException();
            }

            if (disk.Type != PlaceType.Neutral)
            {
                throw new ArgumentException("Selected disk must be neutral!");
            }

            if (playerDiskType == PlaceType.Neutral)
            {
                throw new ArgumentException("Player disk type cannot be neutral!");
            }

            var allCaptures = new List<FieldPlace>();

            foreach (var direction in Directions)
            {
                int row = disk.Y + direction.Y;
                int col = disk.X + direction.X;
                var captures = new List<FieldPlace>();
                bool isValid = false;

                while (row >= 0 && row < _field.GetLength(0) && col >= 0 && col < _field.GetLength(1))
                {
                    var currentDisk = this[row, col];

                    if (currentDisk.Type == PlaceType.Neutral)
                    {
                        break;
                    }
                    else if (currentDisk.Type == playerDiskType)
                    {
                        isValid = true;
                        break;
                    }
                    else
                    {
                        captures.Add(currentDisk);
                    }

                    row += direction.Y;
                    col += direction.X;
                }

                if (isValid)
                {
                    allCaptures.AddRange(captures);
                }
            }

            return allCaptures;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }
}
