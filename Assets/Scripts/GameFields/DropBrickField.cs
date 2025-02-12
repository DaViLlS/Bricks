using Bricks;
using UnityEngine;

namespace GameFields
{
    public class DropBrickField : GameField
    {
        [SerializeField] private Transform dropBrickTarget;
        [SerializeField] private GameObject holeForward;
        
        public Transform DropBrickTarget => dropBrickTarget;

        public void EnableHole()
        {
            holeForward.SetActive(true);
        }

        public void DisableHole()
        {
            holeForward.SetActive(false);
        }

        protected override void OnBrickDestroyed(Brick destroyedBrick)
        {
            
        }

        protected override void OnBrickDroppedInHole(Brick destroyedBrick)
        {
            
        }
    }
}
