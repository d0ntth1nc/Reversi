namespace Reversi.Core
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public class UserPlayer : Player
    {
        private TaskCompletionSource<FieldPlace> _tcs = null;

        public UserPlayer(string username)
            : base(username)
        { }

        public delegate void Turn(UserPlayer sender);

        /// <summary>
        /// Fired when the player is in turn
        /// </summary>
        public event Turn OnTurn;

        /// <summary>
        /// Returns dictionary with all possible captures for the player
        /// </summary>
        public Dictionary<FieldPlace, List<FieldPlace>> PossibleCaptures { get; private set; }

        /// <summary>
        /// Captures place. Can only be called when the player is in turn!
        /// </summary>
        /// <param name="place">Place to be captured</param>
        public void Select(FieldPlace place)
        {
            if (PossibleCaptures == null || _tcs == null)
            {
                throw new InvalidOperationException("Player is not in turn!");
            }

            if (place == null)
            {
                throw new ArgumentNullException("Selected place cannot be null!");
            }

            if (!PossibleCaptures.ContainsKey(place))
            {
                throw new ArgumentException("Selected place cannot be captured!");
            }

            _tcs.SetResult(place);
        }

        internal override Task<FieldPlace> Select(Dictionary<FieldPlace, List<FieldPlace>> allPossibleCaptures)
        {
            PossibleCaptures = allPossibleCaptures;
            _tcs = new TaskCompletionSource<FieldPlace>();
            
            if (OnTurn != null)
            {
                OnTurn(this);
            }

            return _tcs.Task;
        }
    }
}
