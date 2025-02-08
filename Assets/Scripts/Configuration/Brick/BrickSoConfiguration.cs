using System.Collections.Generic;

namespace Configuration.Brick
{
    public class BrickSoConfiguration : BrickConfiguration
    {
        private BrickSo _brickSo;

        public override List<BrickColor> Bricks => _brickSo.BrickColors;

        public BrickSoConfiguration(BrickSo brickSo)
        {
            _brickSo = brickSo;
        }
        
        public override void LoadConfiguration()
        {
            
        }

        public override void SaveConfiguration()
        {
            
        }
    }
}