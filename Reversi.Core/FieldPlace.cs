namespace Reversi.Core
{
    public class FieldPlace
    {
        private PlaceType _type;

        internal FieldPlace(int y, int x, PlaceType type)
        {
            Y = y;
            X = x;
            Type = type;
        }

        public delegate void TypeChanged(FieldPlace sender);

        /// <summary>
        /// Fired when the type is changed
        /// </summary>
        public event TypeChanged OnTypeChanged;

        public int Y { get; internal set; }

        public int X { get; internal set; }

        public PlaceType Type
        {
            get
            {
                return _type;
            }

            internal set
            {
                _type = value;

                if (OnTypeChanged != null)
                {
                    OnTypeChanged(this);
                }
            }
        }
    }
}
