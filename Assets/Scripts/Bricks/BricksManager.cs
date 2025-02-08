using UnityEngine;
using UnityEngine.UI;

namespace Bricks
{
    public class BricksManager : MonoBehaviour
    {
        [SerializeField] private Brick brickPrefab;
        [SerializeField] private Transform bricksListContainer;
        [SerializeField] private Transform bricksContainer;
        [SerializeField] private ScrollRect scrollRect;
        
        private Brick[] _bricks;

        private void Awake()
        {
            Initialize();
        }

        public void Initialize()
        {
            _bricks = new Brick[9];

            for (int i = 0; i < _bricks.Length; i++)
            {
                _bricks[i] = Instantiate(brickPrefab, bricksListContainer);
                _bricks[i].Setup(scrollRect);
                _bricks[i].OnDragBegan += OnBrickBeginDrag;
            }
        }
        
        private void OnBrickBeginDrag(Brick brick)
        {
            brick.transform.SetParent(bricksContainer);
        }
    }
}