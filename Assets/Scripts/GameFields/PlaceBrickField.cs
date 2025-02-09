using System.Collections.Generic;
using Bricks;
using DG.Tweening;
using UnityEngine;

namespace GameFields
{
    public class PlaceBrickField : MonoBehaviour
    {
        [SerializeField] private Transform foundationTransform;
        
        private List<Brick> _bricks;
        private Brick _topBrick;
        
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
