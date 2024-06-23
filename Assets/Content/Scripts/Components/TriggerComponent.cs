using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Content.Scripts.Utilities;

namespace Content.Scripts.Components
{
    public class TriggerComponent : MonoBehaviour
    {
        public event Action<Collider> TriggerStay;
        public event Action<Collider> TriggerEnter;
        public event Action<Collider> TriggerExit;

        protected Collider triggerCollider;
        
        protected virtual void Awake()
        {
            if (TryGetComponent<Collider>(out Collider triggerColliderOut))
                triggerCollider = triggerColliderOut;
            triggerCollider.isTrigger = true;
        }

        protected virtual void OnTriggerStay(Collider other)
        {
            TriggerStay?.Invoke(other);
        }

        protected virtual void OnTriggerEnter(Collider other)
        {
            Debug.Log($"{name} hit {other.name}");
            TriggerEnter?.Invoke(other);
        }

        protected virtual void OnTriggerExit(Collider other)
        {
            TriggerExit?.Invoke(other);
        }

        public void SetLayers(LayerMask layer, LayerMask mask)
        {
            gameObject.layer = StaticHelpers.GetLayerIndex(layer);
            triggerCollider.includeLayers = mask;
        }
        
    }
}