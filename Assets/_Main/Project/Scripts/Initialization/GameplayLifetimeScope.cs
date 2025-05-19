using System;
using Characters;
using Characters.Player;
using CommonComponents;
using DataSave.Runtime;
using Factories;
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

        private void Start()
        {
            Application.targetFrameRate = 60;
            Debug.unityLogger.logEnabled = false;

        }
    }
}