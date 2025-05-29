using System.Collections.Generic;
using UnityEngine;

namespace Characters.Tree
{
    public class Tree : Character
    {
        [SerializeField] private List<GameObject> treeParts;
        
        protected override void Awake()
        {
            base.Awake();
            CharacterCombatManager = new TreeCombatManager(CharacterPropertyManager, CharacterVisualEffects, this, treeParts);
        }
    }
}