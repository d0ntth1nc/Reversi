namespace Reversi.Core
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public abstract class Player
    {
        public Player(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException("Name cannot be null or empty!");
            }

            Name = name;
            Score = 0;
            DiskType = PlaceType.Neutral;
        }

        /// <summary>
        /// Player name
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Player score
        /// </summary>
        public int Score { get; internal set; }

        /// <summary>
        /// Player disk type
        /// </summary>
        public PlaceType DiskType { get; internal set; }

        internal abstract Task<FieldPlace> Select(Dictionary<FieldPlace, List<FieldPlace>> allPossibleCaptures);
    }
}
