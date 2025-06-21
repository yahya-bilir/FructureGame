using Characters.Player;
using Characters.Transforming;
using CommonComponents;
using DataSave.Runtime;
using Factories;
using PropertySystem;
using UI;
using UI.Screens;
using UnityEngine;
using UnityEngine.Serialization;
using VContainer;
using VContainer.Unity;

namespace Initialization
{
    public class GameplayLifetimeScope : LifetimeScope
    {
        [FormerlySerializedAs("playerProperties")] [SerializeField] private CharacterPropertiesSO playerPropertiesSo;
        
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
            builder.RegisterComponentInHierarchy<EnemyFactoryManager>();
            builder.RegisterComponentInHierarchy<TreeFactoryManager>();
            builder.RegisterComponentInHierarchy<GameplayUI>();
            builder.RegisterComponentInHierarchy<WeaponChanging>();
            builder.Register<CharacterTransformManager>(Lifetime.Singleton);
            builder.RegisterInstance(playerPropertiesSo).AsSelf();
        }
    }
}