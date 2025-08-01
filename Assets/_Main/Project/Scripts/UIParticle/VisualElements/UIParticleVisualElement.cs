using UnityEngine;
using UnityEngine.UIElements;
using System;
using System.Runtime.CompilerServices;
using VContainer;

namespace FlingTamplate.UIParticle
{
    [UxmlElement]
    public partial class UIParticleVisualElement : VisualElement
    {
        [Inject] private UIParticleManager _particleManager;

        [UxmlAttribute] public string ParticleName;
        [UxmlAttribute] public ParticleType type;
        [UxmlAttribute] public bool IsSingleUsage;
        private string _id;

        public UIParticleVisualElement()
        {
            RegisterCallback<GeometryChangedEvent>(OnGeometryChanged);
            _id = Guid.NewGuid().ToString();
        }

        private void OnGeometryChanged(GeometryChangedEvent evt)
        {
            bool isVisible = this.worldBound.width > 0 && this.worldBound.height > 0;

            ParticleActivate(isVisible);
        }

        public void ParticleActivate(bool isVisiable)
        {
            if (_particleManager is null) return;
            if (isVisiable)
            {
                var rt = _particleManager.Activate(type, new ParticleAdditionData()
                {
                    IsSingleUsage = this.IsSingleUsage,
                    VisualElementName = _id
                });
             
                style.backgroundImage = new StyleBackground(Background.FromRenderTexture(rt));
            }
            else
            {
                _particleManager.DeActivate(true, type, new ParticleAdditionData()
                {
                    IsSingleUsage = this.IsSingleUsage,
                    VisualElementName = _id
                });
                style.backgroundImage = null;
            }
        }
    }
}