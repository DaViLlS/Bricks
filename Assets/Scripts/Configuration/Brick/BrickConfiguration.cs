using System.Collections.Generic;

namespace Configuration.Brick
{
    public abstract class BrickConfiguration
    {
        public abstract List<BrickColor> Bricks { get; }
        
        public abstract void LoadConfiguration();
        public abstract void SaveConfiguration();
    }

    public enum BrickColor
    {
        Red,
        Yellow,
        Blue,
        Green,
        Purple,
        Orange,
        Turquoise,
        Brown,
        Gray
    }
}