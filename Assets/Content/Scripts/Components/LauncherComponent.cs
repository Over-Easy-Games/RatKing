using System;
using System.Collections;
using System.Collections.Generic;
using Content.ScriptableObjects;
using UnityEngine;

namespace Content.Scripts.Components
{
    public class LaunchParameters
    {
        public Vector3 direction = Vector3.forward;
        public float speed = 5.0f;

        public LaunchParameters( Vector3 direction = new Vector3(), float speed = 5.0f)
        {
            this.direction = direction;
            this.speed = speed;
        }
    }

    public class LauncherComponent : MonoBehaviour
    {
        [SerializeField] protected ProjectileData projectileData;
        [SerializeField] protected Transform spawnLocation;

        public enum LauncherTeam
        {
            Player,
            Enemy
        }

        [SerializeField]
        protected LauncherTeam launcherTeam;
        
        public virtual void Launch(LaunchParameters launchParameters = default)
        {
            if (launchParameters == null)
            {
                launchParameters = new LaunchParameters();
            }
            
            GameObject newProjectile = Instantiate(projectileData.projectilePrefab);
            ProjectileComponent projectileComponent = newProjectile.GetComponent<ProjectileComponent>();
            projectileComponent.Init(projectileData, launchParameters.direction * launchParameters.speed, launcherTeam);
            newProjectile.transform.position = spawnLocation.position;
            newProjectile.transform.rotation = spawnLocation.rotation;
        }

    }
}
