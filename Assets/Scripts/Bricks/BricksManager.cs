using System;
using System.Collections.Generic;
using System.Linq;
using Configuration.Brick;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Bricks
{
    public class BricksManager : MonoBehaviour
    {
        public event Action<Brick> OnBrickDestroyed;
        
        [SerializeField] private Brick brickPrefab;
        [SerializeField] private Transform bricksListContainer;
        [SerializeField] private Transform bricksContainer;
        [SerializeField] private ScrollRect scrollRect;
        [SerializeField] private List<BrickColorPair> brickColorPairs;
        
        private BrickConfiguration _brickConfiguration;
        private IInstantiator _instantiator;
        private Brick[] _bricks;

        [Inject]
        public void Construct(BrickConfiguration brickConfiguration, IInstantiator instantiator)
        {
            _brickConfiguration = brickConfiguration;
            _instantiator = instantiator;
        }

        private void Awake()
        {
            Initialize();
        }

        public void Initialize()
        {
            _bricks = new Brick[_brickConfiguration.Bricks.Count];

            for (int i = 0; i < _bricks.Length; i++)
            {
                var brickColor = _brickConfiguration.Bricks[i];
                var brickSpriteColorPair = brickColorPairs.FirstOrDefault(x => x.brickColor == brickColor);

                if (brickSpriteColorPair == null)
                {
                    Debug.LogError($"Brick Color {brickColor} not found");
                    continue;
                }

                CreateBrick(i, brickColor, brickSpriteColorPair.brickSprite);
            }
        }
        
        private void OnBrickBeginDrag(Brick brick)
        {
            brick.transform.SetParent(bricksContainer);
            var brickSpriteColorPair = brickColorPairs.FirstOrDefault(x => x.brickColor == brick.BrickColor);
            var brickIndex = 0;

            for (int i = 0; i < _bricks.Length; i++)
            {
                if (_bricks[i] == brick)
                {
                    brickIndex = i;
                    break;
                }
            }

            CreateBrick(brickIndex, brick.BrickColor, brickSpriteColorPair.brickSprite);
        }
        
        private void CreateBrick(int brickIndex, BrickColor brickColor, Sprite brickSprite)
        {
            _bricks[brickIndex] = _instantiator.InstantiatePrefabForComponent<Brick>(brickPrefab, bricksListContainer);
            _bricks[brickIndex].Setup(brickColor, brickSprite, scrollRect);
            _bricks[brickIndex].OnDragBegan += OnBrickBeginDrag;
            _bricks[brickIndex].OnBrickDestroyed += HandleBrickDestroy;
            _bricks[brickIndex].OnBrickDestroyed += UnsubscribeFromBrick;
        }

        private void UnsubscribeFromBrick(Brick destroyedBrick)
        {
            destroyedBrick.OnDragBegan -= OnBrickBeginDrag;
            destroyedBrick.OnBrickDestroyed -= HandleBrickDestroy;
            destroyedBrick.OnBrickDestroyed -= UnsubscribeFromBrick;
        }

        private void HandleBrickDestroy(Brick destroyedBrick)
        {
            OnBrickDestroyed?.Invoke(destroyedBrick);
        }
    }

    [Serializable]
    public class BrickColorPair
    {
        public BrickColor brickColor;
        public Sprite brickSprite;
    }
}