using System.Collections.Generic;
using Bricks;
using UnityEngine;
using Zenject;

namespace GameFields
{
    public abstract class GameField : MonoBehaviour
    {
        protected List<Brick> _bricks;
        protected BricksManager _bricksManager;
        
        public List<Brick> Bricks => _bricks;

        [Inject]
        private void Construct(BricksManager bricksManager)
        {
            _bricksManager = bricksManager;
        }

        private void Awake()
        {
            _bricks = new List<Brick>();
            _bricksManager.OnBrickDestroyed += OnBrickDestroyed;
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
        
        protected abstract void OnBrickDestroyed(Brick destroyedBrick);
    }
}