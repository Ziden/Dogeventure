using System;
using UnityEngine;

namespace Src.MonoComponent
{
    public class SignalListener : MonoBehaviour
    {
        [SerializeField] private SignalSource source;
        private ISignalActionable[] _signalActionables;

        private void Start()
        {
            _signalActionables = GetComponents<ISignalActionable>();
        }

        private void Awake()
        {
            source.OnSignal += SourceOnOnSignal;
        }

        private void SourceOnOnSignal()
        {
            foreach (var signalActionable in _signalActionables)
            {
                signalActionable.OnSignal();
            }
        }
    }
}