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
        
        public event Action OnHit;

        [SerializeField] 
        private int damage = 5;

        public int Damage => damage;
        
        protected override void Awake()
        {
            base.Awake();
            triggerCollider.includeLayers = triggerCollider.includeLayers | hurtboxMask;
            gameObject.layer = StaticHelpers.GetLayerIndex(hurtboxLayer);
        }

        protected override void OnTriggerEnter(Collider other)
        {
            base.OnTriggerEnter(other);
            OnHit?.Invoke();
        }
    }
}