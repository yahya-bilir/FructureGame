using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Dreamteck.Splines;
using UnityEngine;

namespace CollectionSystem
{
    public class CollectionArea : MonoBehaviour
    {
        [SerializeField] private CollectionAreaDataHolder dataHolder;
        
        [SerializeField] private SplineComputer conveyorSpline;
        [SerializeField] private Transform destination;
        
        private readonly List<Fragment> _deployedFragments = new();
        private AmmoCreator _ammoCreator;
        private void Inject(AmmoCreator ammoCreator)
        {
            _ammoCreator = ammoCreator;
        }
        public async UniTask RegisterFragments(IEnumerable<GameObject> fragments)
        {
            await UniTask.WaitForSeconds(1.5f);

            foreach (var go in fragments.Where(f => f))
            {
                var frag = go.GetComponent<Fragment>() ?? go.AddComponent<Fragment>();
                frag.Initialize(
                    conveyorSpline,
                    dataHolder.ApproachMaxSpeed,
                    dataHolder.ConveyorSpeed,
                    destination,
                    dataHolder.StopDistance,
                    this
                );
                frag.StartTransportAsync().Forget();
            }
        }
        
        public void AddDeployedFragment(Fragment fragment)
        {
            _deployedFragments.Add(fragment);
            if (_deployedFragments.Count % dataHolder.FragmentCountToCreateAmmo == 0)
            {
                var fragmentsToBeDestroyed = new List<Fragment>();
                var lastIndex = _deployedFragments.Count - 1;
                for (int i = lastIndex; i >= lastIndex - dataHolder.FragmentCountToCreateAmmo; i--)
                {
                    var frag =  _deployedFragments[i];
                    fragmentsToBeDestroyed.Add(frag);
                }

                foreach (var frag in fragmentsToBeDestroyed)
                {
                    Destroy(frag.gameObject);
                }
                
                _ammoCreator.CreateAmmo();
            }
        }
    }
}