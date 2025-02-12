using System.Collections;
using Bricks;
using UnityEngine;

namespace GameFields
{
    public class PlaceBrickField : GameField
    {
        private const float DistanceOffset = 10f;
        
        private Coroutine _fallRoutine;

        protected override void OnBrickDestroyed(Brick destroyedBrick)
        {
            Bricks.Remove(destroyedBrick);
            
            if (_fallRoutine != null)
                StopCoroutine(_fallRoutine);
            
            _fallRoutine = StartCoroutine(FallRoutine());
        }

        private IEnumerator FallRoutine()
        {
            var index = 1;

            while (index < _bricks.Count)
            {
                if (Mathf.Abs(_bricks[index - 1].transform.position.y - _bricks[index].transform.position.y) > _bricks[index].BrickHeight + DistanceOffset)
                {
                    _bricks[index].Fall(_bricks[index - 1]);
                
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
