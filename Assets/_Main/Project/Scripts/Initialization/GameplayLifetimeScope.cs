using Characters.Transforming;
using CommonComponents;
using Factories;
using IslandSystem;
using PropertySystem;
using UI;
using UI.PerksAndDraggables;
using UI.PerksAndDraggables.PerkManagers;
using UnityEngine;
using UnityEngine.Serialization;
using VContainer;
using VContainer.Unity;

namespace Initialization
{
    public class GameplayLifetimeScope : LifetimeScope
    {
        [FormerlySerializedAs("playerProperties")] [SerializeField] private CharacterPropertiesSO playerPropertiesSo;
        [SerializeField] private RectTransform bottomHalf;
        protected override void Awake()
        {
            base.Awake();
            if (Parent != null) Parent.Container.Inject(this);
        }
        
        protected override void Configure(IContainerBuilder builder)
        {
            base.Configure(builder);
            
            builder.RegisterComponentInHierarchy<CamerasManager>();
            builder.RegisterComponentInHierarchy<DynamicJoystick>();
            builder.RegisterComponentInHierarchy<IslandManager>();
            builder.RegisterComponentInHierarchy<EnemyManager>();
            builder.RegisterComponentInHierarchy<TreeFactoryManager>();
            builder.RegisterComponentInHierarchy<GameplayUI>();
            builder.RegisterComponentInHierarchy<WeaponChanging>();
            builder.RegisterComponentInHierarchy<BottomPerkManager>();
            builder.RegisterComponentInHierarchy<MiddlePerkManager>();
            builder.RegisterComponentInHierarchy<AstarPath>();
            builder.RegisterComponentInHierarchy<CloudMovementManager>();
            builder.Register<CharacterTransformManager>(Lifetime.Singleton);
            builder.RegisterInstance(playerPropertiesSo).AsSelf();
            builder.RegisterInstance(bottomHalf).AsSelf();
        }
    }
}