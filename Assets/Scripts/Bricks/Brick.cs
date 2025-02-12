using System;
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

        [SerializeField] private BrickAnimator brickAnimator;
        [SerializeField] private Image brickImage;
        [SerializeField] private float dragThresholdY;
        [SerializeField] private RectTransform brickRect;
        [SerializeField] private Transform raycastOrigin;
        
        private PlaceBrickField _placeBrickField;
        private DropBrickField _dropBrickField;
        
        private BrickColor _brickColor;
        private ScrollRect _parentScrollRect;
        private RaycastHit2D _hit;
        private Vector2 _placePosition;
        private Vector2 _brickPosition;
        
        private float _mouseBeginDragPositionY;

        private bool _belongsToPlaceField;
        private bool _belongsToDropField;
        private bool _isDraggingBegan;
        private bool _reachedDragThreshold;
        private bool _isPlaced;
        
        private float BrickWidth => brickRect.rect.width * 2;
        
        public BrickColor BrickColor => _brickColor;
        public bool IsPlaced => _isPlaced;
        public float BrickHeight => brickRect.rect.height * 2;

        [Inject]
        private void Construct(PlaceBrickField placeBrickField, DropBrickField dropBrickField)
        {
            _placeBrickField = placeBrickField;
            _dropBrickField = dropBrickField;
        }

        public void Setup(BrickColor brickColor, Sprite brickSprite, ScrollRect scrollRect)
        {
            _brickPosition = Vector2.zero;
            brickImage.sprite = brickSprite;
            _parentScrollRect = scrollRect;
            _brickColor = brickColor;
        }
        
        public void Fall(Brick brick)
        {
            _placePosition = brick.transform.position;
            
            _isPlaced = false;
            brickAnimator.MoveToPosition(new Vector2(transform.position.x, _placePosition.y + BrickHeight), 0.2f, AnimatePlacement);
        }
        
        public void OnPointerUp(PointerEventData eventData)
        {
            if (brickAnimator.IsActive)
                return;
            
            HandleDragEnd(eventData);
        }
        
        public void OnPointerDown(PointerEventData eventData)
        {
            _parentScrollRect.OnBeginDrag(eventData);
        }
        
        public void OnBeginDrag(PointerEventData eventData)
        {
            if (brickAnimator.IsActive)
                return;
            
            _mouseBeginDragPositionY = Input.mousePosition.y;
            _isDraggingBegan = true;
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (brickAnimator.IsActive)
                return;
            
            if (!_reachedDragThreshold)
            {
                _parentScrollRect.OnDrag(eventData);
                return;
            }

            transform.position = eventData.position;
        }
        
        private void HandleDragEnd(PointerEventData eventData)
        {
            _isDraggingBegan = false;
            _belongsToPlaceField = _placeBrickField.Bricks.Contains(this);
            _belongsToDropField = _dropBrickField.Bricks.Contains(this);
            
            if (!_reachedDragThreshold)
            {
                _parentScrollRect.OnEndDrag(eventData);
                return;
            }
            
            if (!_belongsToPlaceField && !_belongsToDropField)
            {
                DestroyBrick();
                return;
            }

            if (_belongsToDropField)
            {
                DropBrickInHole();
            }
            else if (_belongsToPlaceField)
            {
                HandlePlacement();
            }
        }

        private void DropBrickInHole()
        {
            if (_hit.collider.CompareTag("Hole"))
            {
                _dropBrickField.EnableHole();
                
                _isPlaced = false;
                brickAnimator.MoveToPosition(_dropBrickField.DropBrickTarget.position, 0.5f, () =>
                {
                    DestroyBrick();
                    _dropBrickField.DisableHole();
                });
            }
            else if (_brickPosition != Vector2.zero)
            {
                _isPlaced = false;
                brickAnimator.MoveToPosition(_brickPosition, 0.1f);
            }
            else
            {
                DestroyBrick();
            }
        }

        private void HandlePlacement()
        {
            if (_hit.collider != null && _hit.collider.TryGetComponent<Brick>(out var hitBrick))
            {
                if (!hitBrick.IsPlaced)
                {
                    DestroyBrick();
                    return;
                }
                
                _placePosition = hitBrick.transform.position;
                _isPlaced = false;
                brickAnimator.MoveToPosition(new Vector2(transform.position.x, _placePosition.y + BrickHeight), 0.2f, OnFallenOnBrick);
                return;
            }

            _placePosition = _hit.point;
            _isPlaced = false;
            brickAnimator.MoveToPosition(new Vector2(transform.position.x, _placePosition.y), 0.2f, OnFallenOnFloor);
        }

        private void OnFallenOnBrick()
        {
            foreach (var brick in _placeBrickField.Bricks)
            {
                if (brick == this)
                    continue;
        
                if (brick.transform.position.y >= transform.position.y)
                {
                    _isPlaced = false;
                    brickAnimator.MoveToPosition(_brickPosition, 0.1f);
                    return;
                }
            }

            AnimatePlacement();
        }
        
        private void OnFallenOnFloor()
        {
            if (_placeBrickField.Bricks.Count <= 1)
            {
                _isPlaced = true;
                return;
            }
            
            if (_brickPosition == Vector2.zero)
            {
                DestroyBrick();
                return;
            }
           
            _isPlaced = false;
            brickAnimator.MoveToPosition(_brickPosition, 0.1f);
        }
        
        private void AnimatePlacement()
        {
            var randomPositionX = Random.Range(-BrickWidth / 2, BrickWidth / 2);
            
            brickAnimator.AnimatePlacementOnBrick(new Vector2(_placePosition.x + randomPositionX, _placePosition.y + BrickHeight),
                () =>
                {
                    _isPlaced = true;
                    _brickPosition = transform.position;
                });
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
            if (!_reachedDragThreshold && !_isDraggingBegan)
                return; 

            PerformRaycast();
        }
        
        private void CheckDragThreshold()
        {
            if (!_reachedDragThreshold && Input.mousePosition.y - _mouseBeginDragPositionY > dragThresholdY)
            {
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
    }
}
