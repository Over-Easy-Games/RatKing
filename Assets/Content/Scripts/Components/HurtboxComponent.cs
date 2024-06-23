using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Content.Scripts.Utilities;

namespace Content.Scripts.Components
{
    public class HurtboxComponent : TriggerComponent
    {
        [SerializeField] 
        private LayerMask hurtboxLayer;
        [SerializeField] 
        private LayerMask hurtboxMask;
        
        public event Action<int> OnHit;
        
        protected override void Awake()
        {
            base.Awake();
            if (hurtboxLayer.value > 0 && hurtboxMask.value > 0)
                SetLayers(hurtboxLayer, triggerCollider.includeLayers | hurtboxMask);
        }

        protected override void OnTriggerEnter(Collider other)
        {
            base.OnTriggerEnter(other);
            int hurtboxDamage = other.GetComponent<HitboxComponent>().Damage;
            OnHit?.Invoke(hurtboxDamage);
            
        }
    }
}