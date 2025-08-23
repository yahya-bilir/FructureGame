using Characters.BaseSystem;
using CollectionSystem;
using CommonComponents;
using Factories;
using FlingTamplate.UIParticle;
using Perks;
using UI;
using VContainer;
using VContainer.Unity;

namespace Initialization
{
    public class GameplayLifetimeScope : LifetimeScope
    {
        protected override void Awake()
        {
            base.Awake();
            if (Parent != null) Parent.Container.Inject(this);
        }
        
        protected override void Configure(IContainerBuilder builder)
        {
            base.Configure(builder);
            builder.RegisterComponentInHierarchy<CamerasManager>();
            builder.RegisterComponentInHierarchy<MainBase>();
            builder.Register<MainBaseGetterAsATarget>(Lifetime.Singleton).AsSelf();
            builder.RegisterComponentInHierarchy<EnemyFactoryManager>();
            builder.RegisterComponentInHierarchy<XPManager>();
            builder.RegisterComponentInHierarchy<GameplayUI>();
            builder.RegisterComponentInHierarchy<UIParticleManager>();
            builder.RegisterComponentInHierarchy<CollectionArea>();
        }
        
    }
}