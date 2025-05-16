using CommonComponents;
using DataSave.Runtime;
using Factories;
using Player;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Initialization
{
    public class GameplayLifetimeScope : LifetimeScope
    {
        [SerializeField] private CharacterProperties playerProperties;
        
        protected override void Awake()
        {
            base.Awake();
            if (Parent != null) Parent.Container.Inject(this);
        }
        
        protected override void Configure(IContainerBuilder builder)
        {
            base.Configure(builder);
            
            builder.RegisterComponentInHierarchy<CamerasManager>();
            builder.RegisterComponentInHierarchy<PlayerController>();
            builder.RegisterComponentInHierarchy<DynamicJoystick>();
            builder.RegisterComponentInHierarchy<EnemyFactoryManager>();

            builder.RegisterInstance(playerProperties).AsSelf();
        }        
    }
}