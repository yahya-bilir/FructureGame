using DataSave.Runtime;
using EventBusses;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Initialization
{
    public class InitialLifetimeScope : LifetimeScope
    {
        [SerializeField] private GameData gameData;
        [SerializeField] private Database.GameDatabase gameDatabase;

        protected override void Awake()
        {
            base.Awake();
            DontDestroyOnLoad(gameObject);
        }
        
        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterInstance(gameData).AsSelf();
            builder.RegisterInstance(gameDatabase).AsSelf();
            builder.RegisterEntryPoint<GameManager>();
            builder.RegisterEntryPoint<AddressableStartupLoader>();
            builder.Register<IEventBus, EventBus>(Lifetime.Singleton);
        }

        private void Start()
        {
            var resolver = Container;
            var characterResources = gameData.CharacterResource;

            resolver.Inject(characterResources);
            
            Debug.unityLogger.logEnabled = false;
        }
    }
}