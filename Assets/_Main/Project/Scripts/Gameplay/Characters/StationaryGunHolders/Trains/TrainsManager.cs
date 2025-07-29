using System;
using System.Collections.Generic;
using System.Linq;
using EventBusses;
using Events;
using UnityEngine;
using VContainer;

namespace Trains
{
    public class TrainsManager : MonoBehaviour
    {
        private IObjectResolver _resolver;
        private IEventBus _eventBus;
        private List<TrainEngine>  _trainEngines = new List<TrainEngine>();

        [Inject]
        private void Inject(IObjectResolver resolver, IEventBus  eventBus)
        {
            _resolver = resolver;
            _eventBus =  eventBus;
        }

        private void Awake()
        {
            _trainEngines = GetComponentsInChildren<TrainEngine>().ToList();
            
            foreach (var trainEngine in _trainEngines)
            {
                _resolver.Inject(trainEngine);
            }
        }
    }
}