using System.Collections.Generic;
using Bricks;
using UnityEngine;

namespace GameFields
{
    public abstract class GameField : MonoBehaviour
    {
        private List<Brick> _bricks;
        public List<Brick> Bricks => _bricks;
        
        private void Awake()
        {
            _bricks = new List<Brick>();
        }
        
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.TryGetComponent<Brick>(out var brick))
            {
                brick.OnBrickDestroyed += OnBrickDestroyed;
                _bricks.Add(brick);
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.TryGetComponent<Brick>(out var brick))
            {
                brick.OnBrickDestroyed -= OnBrickDestroyed;
                _bricks.Remove(brick);
            }
        }

        private void OnBrickDestroyed(Brick destroyedBrick)
        {
            destroyedBrick.OnBrickDestroyed -= OnBrickDestroyed;
            Bricks.Remove(destroyedBrick);
            
            if (destroyedBrick.BelongsToPlaceField)
            {
                for (var i = _bricks.Count - 1; i >= 0; i--)
                {
                    if (i - 1 < 0)
                        continue;
                    
                    _bricks[i].Fall(_bricks[i - 1]);
                }
            }
        }
    }
}