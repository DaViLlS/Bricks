using System.Collections;
using Bricks;
using UnityEngine;

namespace GameFields
{
    public class PlaceBrickField : GameField
    {
        [SerializeField] private RectTransform foundationTransform;
        
        private const float DistanceOffset = 10f;
        
        private Coroutine _fallRoutine;

        protected override void OnBrickDestroyed(Brick destroyedBrick)
        {
            HandleBrickDisappearance(destroyedBrick);
        }

        protected override void OnBrickDroppedInHole(Brick destroyedBrick)
        {
            HandleBrickDisappearance(destroyedBrick);
        }

        private void HandleBrickDisappearance(Brick brick)
        {
            Bricks.Remove(brick);
            
            if (_fallRoutine != null)
                StopCoroutine(_fallRoutine);
            
            _fallRoutine = StartCoroutine(FallRoutine());
        }

        private IEnumerator FallRoutine()
        {
            var index = 1;
            
            if (Bricks.Count <= 0)
                yield break;

            if (Mathf.Abs(foundationTransform.position.y - _bricks[0].transform.position.y) >
                _bricks[0].BrickHeight)
            {
                _bricks[0].FallOnFloor(new Vector2(foundationTransform.position.x,
                    foundationTransform.position.y + foundationTransform.rect.height));
                
                while (!_bricks[0].IsPlaced)
                {
                    yield return null;
                }
            }
            
            while (index < _bricks.Count)
            {
                if (Mathf.Abs(_bricks[index - 1].transform.position.y - _bricks[index].transform.position.y) > _bricks[index].BrickHeight + DistanceOffset)
                {
                    _bricks[index].FallOnBrick(_bricks[index - 1].transform.position);
                
                    while (!_bricks[index].IsPlaced)
                    {
                        yield return null;
                    }
                }
                
                index++;
            }
        } 
    }
}
