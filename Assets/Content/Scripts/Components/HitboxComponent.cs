using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Content.Scripts.Utilities;

namespace Content.Scripts.Components
{
    public class HitboxComponent : TriggerComponent
    {
        [SerializeField] 
        private LayerMask hitboxLayer;
        [SerializeField] 
        private LayerMask hitboxMask;
        
        public event Action OnHit;

        [SerializeField] 
        private int damage = 5;

        public int Damage => damage;
        
        protected override void Awake()
        {
            base.Awake();
            if (hitboxLayer.value > 0 && hitboxMask.value > 0)
                SetLayers(hitboxLayer, triggerCollider.includeLayers | hitboxMask);
        }

        protected override void OnTriggerEnter(Collider other)
        {
            base.OnTriggerEnter(other);
            OnHit?.Invoke();
        }
    }
}