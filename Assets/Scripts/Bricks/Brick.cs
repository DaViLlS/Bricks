using System;
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
        
        private ScrollRect _parentScrollRect;
        private RaycastHit2D _hit;
        private Vector2 _placePosition;
        
        private float _startPositionY;
        private bool _isDraggingBegan;
        private bool _canDrag;

        [Inject]
        private void Construct(PlaceBrickField placeBrickField, DropBrickField dropBrickField)
        {
            _placeBrickField = placeBrickField;
            _dropBrickField = dropBrickField;
        }

        public void Setup(Sprite brickSprite, ScrollRect scrollRect)
        {
            brickImage.sprite = brickSprite;
            _parentScrollRect = scrollRect;
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
        
        private void UpdatePosition(Vector2 newPosition)
        {
            transform.position = newPosition;
        }
        
        private void HandleDragEnd(PointerEventData eventData)
        {
            _isDraggingBegan = false;
            _parentScrollRect.OnEndDrag(eventData);

            if (!_placeBrickField.Bricks.Contains(this) && !_dropBrickField.Bricks.Contains(this))
            {
                Destroy(gameObject);
                return;
            }

            HandleBrickPlacement();
        }

        private void HandleBrickPlacement()
        {
            if (_hit.collider != null && _hit.collider.TryGetComponent<Brick>(out var hitBrick))
            {
                _placePosition = hitBrick.transform.position;
                MoveBrickToPosition(_placePosition.y + 165, OnBrickFallenOnBrick);
                return;
            }

            _placePosition = _hit.point;
            MoveBrickToPosition(_placePosition.y, OnBrickFallenOnFloor);
        }
        
        private void MoveBrickToPosition(float targetY, Action action)
        {
            brickRect.DOMoveY(targetY, 0.2f).OnComplete(() =>
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
        
                if (brick.transform.position.y >= transform.position.y || Mathf.Approximately(transform.position.y, _hit.point.y))
                {
                    Destroy(gameObject);
                    return;
                }
            }
            
            AnimatePlacement();
        }

        private void AnimatePlacement()
        {
            brickRect.DOMoveY(brickRect.transform.position.y + Vector2.up.y * jumpIncreaseValue, jumpAnimationDuration)
                .OnComplete(() =>
                {
                    var randomPositionX = Random.Range(-165 / 2, 165 / 2);
                    brickRect.DOMove(new Vector2(_placePosition.x + randomPositionX, _placePosition.y + 165), fallAnimationDuration);
                });
        }

        private void OnBrickFallenOnFloor()
        {
            if (_placeBrickField.Bricks.Count > 1)
            {
                Destroy(gameObject);
            }
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
            if (Input.mousePosition.y - _startPositionY > dragThresholdY)
            {
                _canDrag = true;
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
