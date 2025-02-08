using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Bricks
{
    public class Brick : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerDownHandler, IPointerUpHandler
    {
        public event Action<Brick> OnDragBegan;
        
        [SerializeField] private Image brickImage;
        [SerializeField] private float dragThresholdY;

        private ScrollRect _parentScrollRect;
        private float _startPositionY;
        private bool _isDraggingBegan;
        private bool _canDrag;

        public void Setup(/*Sprite brickSprite, */ScrollRect scrollRect)
        {
            //brickImage.sprite = brickSprite;
            _parentScrollRect = scrollRect;
        }
        
        public void OnPointerDown(PointerEventData eventData)
        {
            _parentScrollRect.OnBeginDrag(eventData);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            _parentScrollRect.OnEndDrag(eventData);
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

        public void OnEndDrag(PointerEventData eventData)
        {
            _isDraggingBegan = false;
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
    }
}
