using System;
using DG.Tweening;
using UnityEngine;

namespace Bricks
{
    public class BrickAnimator : MonoBehaviour
    {
        [SerializeField] private float jumpIncreaseValue;
        [SerializeField] private float jumpAnimationDuration;
        [SerializeField] private float fallAnimationDuration;
        [SerializeField] private float hideAnimationDuration;

        private Tween _tween;
        private bool _isActive;

        public bool IsActive => _isActive;

        public void Hide(Action action)
        {
            transform.DOScale(0f, hideAnimationDuration).OnComplete(() =>
            {
                action?.Invoke();
            });
        }
        
        public void MoveToPosition(Vector2 target, float duration, Action action = null)
        {
            _isActive = true;
            _tween = transform.DOMove(target, duration).OnComplete(() =>
            {
                _isActive = false;
                action?.Invoke();
            });
        }

        public void AnimatePlacementOnBrick(Vector2 fallPosition, Action action)
        {
            _isActive = true;
            transform.DOMoveY(transform.transform.position.y + Vector2.up.y * jumpIncreaseValue,
                jumpAnimationDuration).OnComplete(() =>
            {
                FallOnBrick(fallPosition, action);
            });
        }
        
        private void FallOnBrick(Vector2 position, Action action)
        {
            _tween = transform.DOMove(position, fallAnimationDuration).OnComplete(() =>
            {
                _isActive = false;
                action?.Invoke();
            });
        }
        
        private void OnDestroy()
        {
            _tween?.Kill();
        }
    }
}