using System;
using UnityEngine;

namespace Src.MonoComponent
{
    public class SignalSource : MonoBehaviour
    {
        public event Action OnSignal;


        public void Signal()
        {
            OnSignal?.Invoke();
        }
    }
}