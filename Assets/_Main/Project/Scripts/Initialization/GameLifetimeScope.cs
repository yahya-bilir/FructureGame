using DataSave.Runtime;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Initialization
{
    public class GameLifetimeScope : LifetimeScope
    {
        [SerializeField] private GameData gameData;

        protected override void Awake()
        {
            base.Awake();
            DontDestroyOnLoad(gameObject);
        }
        
        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterInstance(gameData).AsSelf();
            builder.RegisterEntryPoint<AddressableStartupLoader>();
        }
    }
}