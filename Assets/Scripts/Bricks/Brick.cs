using System;
using DG.Tweening;
using GameFields;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Zenject;

namespace Bricks
{
    public class Brick : MonoBehaviour, IBeginDragHandler, IDragHandler, IPointerDownHandler, IPointerUpHandler
    {
        public event Action<Brick> OnDragBegan;

        [SerializeField] private Image brickImage;
        [SerializeField] private float dragThresholdY;
        [SerializeField] private RectTransform brickRect;
        [SerializeField] private Transform raycastOrigin;

        private PlaceBrickField _placeBrickField;
        private ScrollRect _parentScrollRect;
        private RaycastHit2D _hit;
        private float _startPositionY;
        private bool _isDraggingBegan;
        private bool _canDrag;

        [Inject]
        private void Construct(PlaceBrickField placeBrickField)
        {
            _placeBrickField = placeBrickField;
        }

        public void Setup(Sprite brickSprite, ScrollRect scrollRect)
        {
            brickImage.sprite = brickSprite;
            _parentScrollRect = scrollRect;
        }
        
        public void OnPointerUp(PointerEventData eventData)
        {
            _isDraggingBegan = false;
            _parentScrollRect.OnEndDrag(eventData);

            if (!_placeBrickField.Bricks.Contains(this))
            {
                Destroy(gameObject);
                return;
            }
            
            if (_hit.collider != null && _hit.collider.TryGetComponent<Brick>(out var hitBrick))
            {
                brickRect.DOMoveY(hitBrick.transform.position.y + 165, 0.2f).OnComplete(OnBrickFallenOnBrick);
                return;
            }
            
            brickRect.DOMoveY(_hit.point.y, 0.2f).OnComplete(OnBrickFallenOnFloor);
        }

        private void OnBrickFallenOnBrick()
        {
            foreach (var brick in _placeBrickField.Bricks)
            {
                if (brick == this)
                    continue;
                
                if (brick.transform.position.y > transform.position.y || Mathf.Approximately(brick.transform.position.y, transform.position.y))
                {
                    Destroy(gameObject);
                    break;
                }
            }
        }

        private void OnBrickFallenOnFloor()
        {
            if (_placeBrickField.Bricks.Count > 1)
            {
                Destroy(gameObject);
            }
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
            
            transform.position = eventData.position;
        }

        private void Update()
        {
            if (!_isDraggingBegan)
                return;

            if (!(Input.mousePosition.y - _startPositionY > dragThresholdY))
                return;
            
            _canDrag = true;
            OnDragBegan?.Invoke(this);
        }

        private void FixedUpdate()
        {
            if (!_canDrag)
                return;
            
            var hit = Physics2D.Raycast(raycastOrigin.position,
                -raycastOrigin.transform.up, Mathf.Infinity, LayerMask.GetMask("Brick"));
                
            Debug.DrawLine(raycastOrigin.position, _hit.point, Color.red);

            if (hit.collider != null && hit.collider.gameObject != gameObject)
                _hit = hit;
        }
    }
}
