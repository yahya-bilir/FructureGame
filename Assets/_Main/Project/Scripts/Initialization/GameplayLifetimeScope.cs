using System;
using Characters.BaseSystem;
using CommonComponents;
using Factories;
using Trains;
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
            builder.RegisterComponentInHierarchy<TrainsManager>();
            builder.RegisterComponentInHierarchy<EnemyFactoryManager>();
            builder.RegisterComponentInHierarchy<GameplayUI>();
        }
        
    }
}