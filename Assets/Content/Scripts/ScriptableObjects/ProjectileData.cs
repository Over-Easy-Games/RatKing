using UnityEngine;

namespace Content.ScriptableObjects
{
    [CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/ProjectileData", order = 1)]
    public class ProjectileData : ScriptableObject
    {
        public GameObject projectilePrefab;

        // negative for no lifetime
        public float lifetime;
        public Vector3 gravity;
        public Vector3 acceleration;
        public bool destroyOnTimeout;
        public bool destroyOnPhysicsCollision;
        public bool destroyOnHitboxCollision;
        public bool destroyOnHurtboxCollision;
    }
    
}