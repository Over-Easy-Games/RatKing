using System;
using System.Collections;
using System.Collections.Generic;
using Content.ScriptableObjects;
using UnityEngine;

namespace Content.Scripts.Components
{
    [RequireComponent(typeof(Rigidbody))]
    public class ProjectileComponent : MonoBehaviour
    {
        protected Vector3 currentVelocity = Vector3.zero;

        protected event Action Timeout;

        protected Rigidbody rb;
        protected HitboxComponent _hitboxComponent;
        protected HurtboxComponent _hurtboxComponent;
        
        protected float lifetime;
        protected Vector3 gravity;
        protected Vector3 acceleration;
        protected bool destroyOnTimeout;
        protected bool destroyOnPhysicsCollision;
        protected bool destroyOnHitboxCollision;
        protected bool destroyOnHurtboxCollision;

        private void Awake()
        {
            Timeout += OnTimeout;
            rb = GetComponent<Rigidbody>();
            
            _hitboxComponent = GetComponentInChildren<HitboxComponent>();
            _hitboxComponent.OnHit += HitboxComponentOnHit;
            
            _hurtboxComponent = GetComponentInChildren<HurtboxComponent>();
            _hurtboxComponent.OnHit += HurtboxComponentOnOnHit;
        }

        public void Init(ProjectileData projectileData, Vector3 startingVelocity, LauncherComponent.LauncherTeam launcherTeam)
        {
            StartCoroutine(LifeTimer(projectileData.lifetime));
            gravity = projectileData.gravity;
            acceleration = projectileData.acceleration;
            destroyOnTimeout = projectileData.destroyOnTimeout;     
            destroyOnPhysicsCollision = projectileData.destroyOnPhysicsCollision;
            destroyOnHitboxCollision = projectileData.destroyOnHitboxCollision;
            destroyOnHurtboxCollision = projectileData.destroyOnHurtboxCollision;
            
            currentVelocity = startingVelocity;

            if (launcherTeam == LauncherComponent.LauncherTeam.Player)
            {
                _hitboxComponent.SetLayers(LayerMask.GetMask("PlayerHitbox"), LayerMask.GetMask("EnemyHurtbox") );
                _hurtboxComponent.SetLayers(LayerMask.GetMask("PlayerHurtbox"), LayerMask.GetMask("EnemyHitbox") );
            }
            else
            {
                _hitboxComponent.SetLayers(LayerMask.GetMask("EnemyHitbox"), LayerMask.GetMask("PlayerHurtbox") );
                _hurtboxComponent.SetLayers(LayerMask.GetMask("EnemyHurtbox"), LayerMask.GetMask("PlayerHitbox") );
            }
        }
        
        private void OnTimeout()
        {
            if (destroyOnTimeout)
                Destroy(gameObject);
        }

        private void HitboxComponentOnHit(int damage)
        {
            if (destroyOnHitboxCollision)
                Destroy(gameObject);
        }

        private void HurtboxComponentOnOnHit()
        {
            if (destroyOnHurtboxCollision)
                Destroy(gameObject);
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (destroyOnPhysicsCollision)
                Destroy(gameObject);
        }

        protected IEnumerator LifeTimer(float duration)
        {
            if (duration > 0.0f){
                yield return new WaitForSeconds(duration);
                Timeout?.Invoke();
            }
        }

        private void FixedUpdate()
        {
            Tick();
        }

        private void Tick()
        {
            rb.velocity = currentVelocity + gravity;
            // kinematic or physics or raycast
        }
    }
}

