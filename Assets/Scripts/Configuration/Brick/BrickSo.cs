using System.Collections.Generic;
using UnityEngine;

namespace Configuration.Brick
{
    [CreateAssetMenu(fileName = "BrickSo", menuName = "Game/BrickSo")]
    public class BrickSo : ScriptableObject
    {
        [SerializeField] private List<BrickColor> brickColors;

        public List<BrickColor> BrickColors => brickColors;
    }
}