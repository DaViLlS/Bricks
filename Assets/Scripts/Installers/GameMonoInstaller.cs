using Configuration.Brick;
using GameFields;
using UnityEngine;
using Zenject;

namespace Installers
{
    public class GameMonoInstaller : MonoInstaller
    {
        [SerializeField] private BrickSo brickSo;
        [SerializeField] private PlaceBrickField placeBrickField;

        public override void InstallBindings()
        {
            BindBrickConfig();
            BindPlaceBrickField();
        }

        private void BindBrickConfig()
        {
            var brickConfiguration = new BrickSoConfiguration(brickSo);
            Container.Bind<BrickConfiguration>().To<BrickSoConfiguration>().FromInstance(brickConfiguration).AsSingle();
        }

        private void BindPlaceBrickField()
        {
            Container.Bind<PlaceBrickField>().FromInstance(placeBrickField).AsSingle();
        }
    }
}