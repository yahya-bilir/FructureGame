using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Dreamteck.Splines;
using UnityEngine;
using VContainer;

namespace CollectionSystem
{
    public class CollectionArea : MonoBehaviour
    {
        [SerializeField] private CollectionAreaDataHolder dataHolder;
        [SerializeField] private SplineComputer conveyorSpline;
        
        private readonly List<Fragment> _deployedFragments = new();
        private AmmoCreator _ammoCreator;
        
        [Inject]
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
                    this
                );
                frag.StartTransportAsync().Forget();
            }
        }
        
        public void AddDeployedFragment(Fragment fragment)
        {
            _deployedFragments.Add(fragment);
    
            if (_deployedFragments.Count < dataHolder.FragmentCountToCreateAmmo)
                return;

            if (_deployedFragments.Count % dataHolder.FragmentCountToCreateAmmo == 0)
            {
                var count = dataHolder.FragmentCountToCreateAmmo;
                var startIndex = _deployedFragments.Count - count;

                var fragmentsToBeDestroyed = _deployedFragments.GetRange(startIndex, count);
                _deployedFragments.RemoveRange(startIndex, count);

                foreach (var frag in fragmentsToBeDestroyed)
                {
                    Destroy(frag.gameObject);
                }

                _ammoCreator.CreateAmmo();
            }
        }
    }
}