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
        [SerializeField] private DropBrickField dropBrickField;

        public override void InstallBindings()
        {
            BindBrickConfig();
            BindFields();
        }

        private void BindBrickConfig()
        {
            var brickConfiguration = new BrickSoConfiguration(brickSo);
            Container.Bind<BrickConfiguration>().To<BrickSoConfiguration>().FromInstance(brickConfiguration).AsSingle();
        }

        private void BindFields()
        {
            Container.Bind<PlaceBrickField>().FromInstance(placeBrickField).AsSingle();
            Container.Bind<DropBrickField>().FromInstance(dropBrickField).AsSingle();
        }
    }
}