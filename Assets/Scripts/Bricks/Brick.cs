using System;
using DG.Tweening;
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
        [SerializeField] private RectTransform brickRect;

        private ScrollRect _parentScrollRect;
        private float _startPositionY;
        private bool _isDraggingBegan;
        private bool _canDrag;
        
        private Transform _foundationTransform;

        public void Setup(Sprite brickSprite, ScrollRect scrollRect, Transform foundationTransform)
        {
            brickImage.sprite = brickSprite;
            _parentScrollRect = scrollRect;
            _foundationTransform = foundationTransform;
        }
        
        public void OnPointerDown(PointerEventData eventData)
        {
            _parentScrollRect.OnBeginDrag(eventData);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            _parentScrollRect.OnEndDrag(eventData);
            brickRect.DOMoveY(_foundationTransform.position.y + 20, 0.2f);
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
