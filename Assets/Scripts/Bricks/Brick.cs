using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Bricks
{
    public class Brick : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler, IEndDragHandler
    {
        public event Action<Brick> OnDragBegan;
        
        [SerializeField] private Image brickImage;
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private float dragThresholdY;

        private float _startPositionY;
        private bool _isDraggingBegan;
        private bool _canDrag;

        public void Setup(Sprite brickSprite)
        {
            brickImage.sprite = brickSprite;
        }
        
        public void OnPointerDown(PointerEventData eventData)
        {
            _startPositionY = Input.mousePosition.y;
            _isDraggingBegan = true;
            canvasGroup.blocksRaycasts = false;
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (!_canDrag)
                return;
            
            transform.position = eventData.position;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            canvasGroup.blocksRaycasts = true;
            _isDraggingBegan = false;
        }
        
        public void OnPointerUp(PointerEventData eventData)
        {
            canvasGroup.blocksRaycasts = true;
            _isDraggingBegan = false;
        }

        private void Update()
        {
            if (!_isDraggingBegan)
                return;

            Debug.Log($"waka waka {Input.mousePosition.y - _startPositionY}");
            
            if (Input.mousePosition.y - _startPositionY > dragThresholdY)
            {
                _canDrag = true;
                OnDragBegan?.Invoke(this);
            }
        }
    }
}
