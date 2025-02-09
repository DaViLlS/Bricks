using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Bricks
{
    public class Brick : MonoBehaviour, IBeginDragHandler, IDragHandler, IPointerDownHandler, IPointerUpHandler
    {
        public event Action<Brick> OnDragBegan;
        public event Action<Brick> PointerUp;
        
        [SerializeField] private Image brickImage;
        [SerializeField] private float dragThresholdY;
        [SerializeField] private RectTransform brickRect;
        [SerializeField] private Transform raycastOrigin;

        private ScrollRect _parentScrollRect;
        private RaycastHit2D _hit;
        private float _startPositionY;
        private bool _isDraggingBegan;
        private bool _canDrag;
        
        public RectTransform BrickRect => brickRect;
        public RaycastHit2D Hit => _hit;

        public void Setup(Sprite brickSprite, ScrollRect scrollRect)
        {
            brickImage.sprite = brickSprite;
            _parentScrollRect = scrollRect;
        }
        
        public void OnPointerDown(PointerEventData eventData)
        {
            _parentScrollRect.OnBeginDrag(eventData);
        }
        
        public void OnPointerUp(PointerEventData eventData)
        {
            _isDraggingBegan = false;
            _parentScrollRect.OnEndDrag(eventData);
            PointerUp?.Invoke(this);
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
            
            if (Input.mousePosition.y - _startPositionY > dragThresholdY)
            {
                _canDrag = true;
                OnDragBegan?.Invoke(this);
            }
        }

        private void FixedUpdate()
        {
            if (_canDrag)
            {
                var results = Physics2D.RaycastAll(raycastOrigin.position,
                    -raycastOrigin.transform.up, Mathf.Infinity, LayerMask.GetMask("Brick"));
                
                Debug.DrawLine(raycastOrigin.position, _hit.point, Color.red);

                if (results != null && results.Length > 0)
                {
                    if (results[0].collider != null && results[0].collider.gameObject != gameObject)
                    {
                        _hit = results[0];
                        Debug.DrawLine(raycastOrigin.position, _hit.point, Color.red);
                        return;
                    }
                
                    if (results.Length > 1 && results[1].collider != null && results[1].collider.gameObject != gameObject)
                    {
                        _hit = results[1];
                        Debug.DrawLine(raycastOrigin.position, _hit.point, Color.red);
                    }
                }
            }
        }
    }
}
