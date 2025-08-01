using System;
using FlingTamplate.UIParticle;
using UnityEngine;
using UnityEngine.UIElements;
using VContainer;
using VContainer.Unity;


public class UIParticleState : LifetimeScope
{
    [SerializeField] private UIParticleManager _particleManager;
    [SerializeField] private UIDocument uiDocument;


    protected override void Awake()
    {
        base.Awake();
        if (Parent != null) Parent.Container.Inject(this);
    }


    protected override void Configure(IContainerBuilder builder)
    {
        base.Configure(builder);

        builder.RegisterComponentInHierarchy<UIParticleManager>().DontDestroyOnLoad();
    }


    private void Start()
    {
        var elements = uiDocument.rootVisualElement.Query<UIParticleVisualElement>();
        elements.ForEach(x => Container.Inject(x));
    }
}