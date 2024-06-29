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
        protected HurtboxComponent _hurtboxComponent;
        protected HitboxComponent _hitboxComponent;
        
        protected float lifetime;
        protected Vector3 gravity;
        protected Vector3 acceleration;
        protected bool destroyOnTimeout;
        protected bool destroyOnPhysicsCollision;
        protected bool destroyOnHurtboxCollision;
        protected bool destroyOnHitboxCollision;
        protected float drag;

        private void Awake()
        {
            Timeout += OnTimeout;
            rb = GetComponent<Rigidbody>();
            
            _hurtboxComponent = GetComponentInChildren<HurtboxComponent>();
            _hurtboxComponent.OnHit += HurtboxComponentOnHit;
            
            _hitboxComponent = GetComponentInChildren<HitboxComponent>();
            _hitboxComponent.OnHit += HitboxComponentOnOnHit;
        }

        public void Init(ProjectileData projectileData, Vector3 startingVelocity, LauncherComponent.LauncherTeam launcherTeam)
        {
            StartCoroutine(LifeTimer(projectileData.lifetime));
            gravity = projectileData.gravity;
            acceleration = projectileData.acceleration;
            destroyOnTimeout = projectileData.destroyOnTimeout;     
            destroyOnPhysicsCollision = projectileData.destroyOnPhysicsCollision;
            destroyOnHurtboxCollision = projectileData.destroyOnHurtboxCollision;
            destroyOnHitboxCollision = projectileData.destroyOnHitboxCollision;
            drag = projectileData.drag;
            
            currentVelocity = startingVelocity;
            rb.AddForce(currentVelocity, ForceMode.VelocityChange);
            rb.drag = drag;

            if (launcherTeam == LauncherComponent.LauncherTeam.Player)
            {
                _hurtboxComponent.SetLayers(LayerMask.GetMask("PlayerHurtbox"), LayerMask.GetMask("EnemyHitbox") );
                _hitboxComponent.SetLayers(LayerMask.GetMask("PlayerHitbox"), LayerMask.GetMask("EnemyHurtbox") );
            }
            else
            {
                _hurtboxComponent.SetLayers(LayerMask.GetMask("EnemyHurtbox"), LayerMask.GetMask("PlayerHitbox") );
                _hitboxComponent.SetLayers(LayerMask.GetMask("EnemyHitbox"), LayerMask.GetMask("PlayerHurtbox") );
            }
        }
        
        private void OnTimeout()
        {
            if (destroyOnTimeout)
                Destroy(gameObject);
        }

        private void HurtboxComponentOnHit(int damage)
        {
            if (destroyOnHurtboxCollision)
                Destroy(gameObject);
        }

        private void HitboxComponentOnOnHit()
        {
            if (destroyOnHitboxCollision)
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

        protected virtual void Tick()
        {
            rb.AddForce(gravity, ForceMode.Force);
        }
    }
}

