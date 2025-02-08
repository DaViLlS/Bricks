using Configuration.Brick;
using UnityEngine;
using Zenject;

namespace Installers
{
    public class GameMonoInstaller : MonoInstaller
    {
        [SerializeField] private BrickSo brickSo;

        public override void InstallBindings()
        {
            InstallBrickConfig();
        }

        private void InstallBrickConfig()
        {
            var brickConfiguration = new BrickSoConfiguration(brickSo);
            Container.Bind<BrickConfiguration>().To<BrickSoConfiguration>().FromInstance(brickConfiguration).AsSingle();
        }
    }
}