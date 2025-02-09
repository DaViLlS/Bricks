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

        private void Awake()
        {
            _bricks = new List<Brick>();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.TryGetComponent<Brick>(out var brick))
            {
                brick.PointerUp += OnPointerUp;
                _bricks.Add(brick);
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.TryGetComponent<Brick>(out var brick))
            {
                brick.PointerUp -= OnPointerUp;
                _bricks.Remove(brick);
            }
        }

        private void OnPointerUp(Brick brick)
        {
            if (brick.Hit.collider != null && brick.Hit.collider.TryGetComponent<Brick>(out var hitBrick))
            {
                brick.BrickRect.DOMoveY(hitBrick.transform.position.y + 165, 0.2f);
                return;
            }
            
            brick.BrickRect.DOMoveY(foundationTransform.position.y + 20, 0.2f);
        }
    }
}
