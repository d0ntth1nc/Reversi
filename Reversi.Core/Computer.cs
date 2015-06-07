namespace Reversi.Core
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public class Computer : Player
    {
        public Computer(int gameFieldSize)
            : base("Computer")
        {
            GameFieldSize = gameFieldSize;
        }

        /// <summary>
        /// Used by computer algorithm to detect the field borders
        /// </summary>
        public int GameFieldSize { get; set; }

        internal override async Task<FieldPlace> Select(Dictionary<FieldPlace, List<FieldPlace>> allPossibleCaptures)
        {
            var possibleSelections = allPossibleCaptures
                .OrderByDescending(selection => selection.Value.Count)
                .Select(selection => selection.Key)
                .ToArray();

            var selected = await Task.Run(() => SelectBestPlace(possibleSelections));
            return selected;
        }

        private FieldPlace SelectBestPlace(IEnumerable<FieldPlace> possibleSelections)
        {
            var borderPlaces = possibleSelections
                .Where(place =>
                    place.Y == 0 || place.Y == GameFieldSize - 1 ||
                    place.X == 0 || place.X == GameFieldSize - 1
                );

            var cornerPlaces = borderPlaces
                .Where(place => place.Y == 0 || place.Y == GameFieldSize - 1)
                .Where(place => place.X == 0 || place.X == GameFieldSize - 1);

            // Corner places are the most important
            if (cornerPlaces.Any())
            {
                return cornerPlaces.First();
            }

            // Border places are second in importance
            if (borderPlaces.Any())
            {
                return borderPlaces.First();
            }

            // Try to select place which is not too bad - near border
            var goodPlaces = possibleSelections
                .Where(place => place.Y != 1 && place.Y != GameFieldSize - 2)
                .Where(place => place.X != 1 && place.X != GameFieldSize - 2);

            if (goodPlaces.Any())
            {
                return goodPlaces.First();
            }

            // There is only bad places
            return possibleSelections.First();
        }
    }
}
