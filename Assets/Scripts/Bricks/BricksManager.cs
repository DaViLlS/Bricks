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

                _bricks[i] = _instantiator.InstantiatePrefabForComponent<Brick>(brickPrefab, bricksListContainer);
                _bricks[i].Setup(brickSpriteColorPair.brickSprite, scrollRect);
                _bricks[i].OnDragBegan += OnBrickBeginDrag;
            }
        }
        
        private void OnBrickBeginDrag(Brick brick)
        {
            brick.transform.SetParent(bricksContainer);
        }
    }

    [Serializable]
    public class BrickColorPair
    {
        public BrickColor brickColor;
        public Sprite brickSprite;
    }
}