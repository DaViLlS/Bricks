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
                _bricks.Add(brick);
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.TryGetComponent<Brick>(out var brick))
            {
                _bricks.Remove(brick);
            }
        }
    }
}