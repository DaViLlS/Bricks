using Bricks;
using TMPro;
using UnityEngine;
using Zenject;

namespace UI
{
    public class ActionsInfoPanel : MonoBehaviour
    {
        [SerializeField] private TMP_Text actionsText;
        [SerializeField] private string dragBeginText;
        [SerializeField] private string brickPlacedText;
        [SerializeField] private string brickDroppedText;
        [SerializeField] private string brickDestroyedText;
        
        private BricksManager _bricksManager;
        
        [Inject]
        private void Construct(BricksManager bricksManager)
        {
            _bricksManager = bricksManager;
        }

        private void Start()
        {
            _bricksManager.OnBrickDragBegan += BricksManagerOnOnBrickDragBegan;
            _bricksManager.OnBrickPlaced += BricksManagerOnOnBrickPlaced;
            _bricksManager.OnBrickDroppedInHole += BricksManagerOnOnBrickDroppedInHole;
            _bricksManager.OnBrickDestroyed += BricksManagerOnOnBrickDestroyed;
        }
        
        private void BricksManagerOnOnBrickDragBegan()
        {
            actionsText.text = dragBeginText;
        }
        
        private void BricksManagerOnOnBrickPlaced()
        {
            actionsText.text = brickPlacedText;
        }
        
        private void BricksManagerOnOnBrickDroppedInHole(Brick brick)
        {
            actionsText.text = brickDroppedText;
        }
        
        private void BricksManagerOnOnBrickDestroyed(Brick brick)
        {
            actionsText.text = brickDestroyedText;
        }
    }
}