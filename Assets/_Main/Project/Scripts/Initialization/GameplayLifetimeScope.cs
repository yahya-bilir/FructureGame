using Characters;
using Characters.Transforming;
using CommonComponents;
using Factories;
using IslandSystem;
using PropertySystem;
using UI;
using UI.PerksAndDraggables.PerkManagers;
using UnityEngine;
using UnityEngine.Serialization;
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
            builder.RegisterComponentInHierarchy<EnemyManager>();
            builder.RegisterComponentInHierarchy<GameplayUI>();
            builder.RegisterComponentInHierarchy<PerkCreator>();
            builder.Register<EnemyTargetingManager>(Lifetime.Singleton);
        }
    }
}