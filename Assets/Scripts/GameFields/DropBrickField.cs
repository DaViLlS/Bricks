using Bricks;
using UnityEngine;

namespace GameFields
{
    public class DropBrickField : GameField
    {
        [SerializeField] private Transform dropBrickTarget;
        [SerializeField] private GameObject holeForward;
        
        public Transform DropBrickTarget => dropBrickTarget;

        protected override void BrickTriggeredField(Brick brick)
        {
            holeForward.SetActive(true);
        }

        protected override void BrickExitFromField(Brick brick)
        {
            holeForward.SetActive(false);
        }

        protected override void OnBrickDestroyed(Brick destroyedBrick)
        {
            
        }
    }
}
