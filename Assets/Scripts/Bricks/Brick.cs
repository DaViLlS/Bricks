using System;
using System.Collections.Generic;
using Configuration.Brick;
using DG.Tweening;
using GameFields;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Zenject;
using Random = UnityEngine.Random;

namespace Bricks
{
    public class Brick : MonoBehaviour, IBeginDragHandler, IDragHandler, IPointerDownHandler, IPointerUpHandler
    {
        public event Action<Brick> OnDragBegan;
        public event Action<Brick> OnBrickDestroyed;

        [SerializeField] private Image brickImage;
        [SerializeField] private float dragThresholdY;
        [SerializeField] private RectTransform brickRect;
        [SerializeField] private Transform raycastOrigin;
        [Header("Animation")]
        [SerializeField] private float jumpIncreaseValue;
        [SerializeField] private float jumpAnimationDuration = 0.2f;
        [SerializeField] private float fallAnimationDuration = 0.2f;

        private PlaceBrickField _placeBrickField;
        private DropBrickField _dropBrickField;
        
        private Tween _tween;
        private BrickColor _brickColor;
        private ScrollRect _parentScrollRect;
        private RaycastHit2D _hit;
        private Vector2 _placePosition;
        private Vector2 _brickPosition;
        
        private float _startPositionY;
        private bool _belongsToPlaceField;
        private bool _belongsToDropField;
        private bool _isDraggingBegan;
        private bool _canDrag;
        private bool _reachedDragThreshold;

        private float BrickHeight => brickRect.rect.height * 2;
        private float BrickWidth => brickRect.rect.width * 2;
        
        public BrickColor BrickColor => _brickColor;
        public bool BelongsToPlaceField => _belongsToPlaceField;
        public bool BelongsToDropField => _belongsToDropField;

        [Inject]
        private void Construct(PlaceBrickField placeBrickField, DropBrickField dropBrickField)
        {
            _placeBrickField = placeBrickField;
            _dropBrickField = dropBrickField;
        }

        public void Setup(BrickColor brickColor, Sprite brickSprite, ScrollRect scrollRect)
        {
            brickImage.sprite = brickSprite;
            _parentScrollRect = scrollRect;
            _brickColor = brickColor;
        }
        
        public void OnPointerUp(PointerEventData eventData)
        {
            HandleDragEnd(eventData);
        }
        
        public void OnPointerDown(PointerEventData eventData)
        {
            _parentScrollRect.OnBeginDrag(eventData);
        }
        
        public void OnBeginDrag(PointerEventData eventData)
        {
            _startPositionY = Input.mousePosition.y;
            _isDraggingBegan = true;
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (!_canDrag)
            {
                _parentScrollRect.OnDrag(eventData);
                return;
            }
            
            UpdatePosition(eventData.position);
        } 

        public void Fall(Brick brick)
        {
            if (Vector2.Distance(transform.position, brick.transform.position) >= BrickHeight * 2)
            {
                _placePosition = brick.transform.position;
                MoveBrickToPosition(_placePosition.y + BrickHeight, AnimatePlacement);
            }
        }
        
        private void UpdatePosition(Vector2 newPosition)
        {
            transform.position = newPosition;
        }
        
        private void HandleDragEnd(PointerEventData eventData)
        {
            _isDraggingBegan = false;
            _belongsToPlaceField = _placeBrickField.Bricks.Contains(this);
            _belongsToDropField = _dropBrickField.Bricks.Contains(this);
            
            if (!_canDrag)
            {
                _parentScrollRect.OnEndDrag(eventData);
                return;
            }
            
            if (!_belongsToPlaceField && !_belongsToDropField)
            {
                DestroyBrick();
                return;
            }

            HandleBrickPlacement();
        }

        private void HandleBrickPlacement()
        {
            if (_hit.collider != null && _hit.collider.TryGetComponent<Brick>(out var hitBrick))
            {
                _placePosition = hitBrick.transform.position;
                MoveBrickToPosition(_placePosition.y + BrickHeight, OnBrickFallenOnBrick);
                return;
            }

            _placePosition = _hit.point;
            MoveBrickToPosition(_placePosition.y, OnBrickFallenOnFloor);
        }
        
        private void MoveBrickToPosition(float targetY, Action action)
        {
            _tween = brickRect.DOMoveY(targetY, 0.2f).OnComplete(() =>
            {
                action?.Invoke();
            });
        }

        private void OnBrickFallenOnBrick()
        {
            foreach (var brick in _placeBrickField.Bricks)
            {
                if (brick == this)
                    continue;
        
                if (brick.transform.position.y >= transform.position.y)
                {
                    transform.position = _brickPosition;
                    return;
                }
            }

            AnimatePlacement();
        }

        private void AnimatePlacement()
        {
            _tween = brickRect.DOMoveY(brickRect.transform.position.y + Vector2.up.y * jumpIncreaseValue,
                    jumpAnimationDuration).OnComplete(PlaceBrickWithRandomOffset);
        }

        private void PlaceBrickWithRandomOffset()
        {
            var randomPositionX = Random.Range(-BrickWidth / 2, BrickWidth / 2);
            _tween = brickRect.DOMove(new Vector2(_placePosition.x + randomPositionX, _placePosition.y + BrickHeight),
            fallAnimationDuration).OnComplete(() => _brickPosition = transform.position);
        }

        private void OnBrickFallenOnFloor()
        {
            if (_placeBrickField.Bricks.Count > 1)
            {
                DestroyBrick();
            }
        }

        private void DestroyBrick()
        {
            OnBrickDestroyed?.Invoke(this);
            Destroy(gameObject);
        }
        
        private void Update()
        {
            if (!_isDraggingBegan)
                return;

            CheckDragThreshold();
        }

        private void FixedUpdate()
        {
            if (!_canDrag && !_isDraggingBegan)
                return; 

            PerformRaycast();
        }
        
        private void CheckDragThreshold()
        {
            if (!_reachedDragThreshold && Input.mousePosition.y - _startPositionY > dragThresholdY)
            {
                _canDrag = true;
                _reachedDragThreshold = true;
                OnDragBegan?.Invoke(this);
            }
        }
        
        private void PerformRaycast()
        {
            var hit = Physics2D.Raycast(raycastOrigin.position,
                -raycastOrigin.transform.up, Mathf.Infinity,
                LayerMask.GetMask("Brick", "Foundation", "Hole"));
            
            if (hit.collider != null && hit.collider.gameObject != gameObject)
                _hit = hit;
                
            Debug.DrawLine(raycastOrigin.position, _hit.point, Color.red);
        }

        private void OnDestroy()
        {
            _tween?.Kill();
        }
    }
}
