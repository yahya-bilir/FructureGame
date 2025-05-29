using System.Collections.Generic;
using UnityEngine;
using VContainer;
using Tree = Characters.Tree.Tree;

namespace Factories
{
    public class TreeFactoryManager : MonoBehaviour
    {
        [SerializeField] private List<Tree> trees;

        [Inject]
        private void Inject(IObjectResolver objectResolver)
        {
            foreach (var tree in trees)
            {
                objectResolver.Inject(tree);
            }
        }
    }
}