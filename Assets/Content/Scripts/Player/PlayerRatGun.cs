using System;
using System.Collections;
using System.Collections.Generic;
using Content.Scripts.Components;
using UnityEditor.UIElements;
using UnityEngine;
using Content.Scripts.Utilities;
using UnityEngine.InputSystem;

namespace Content.Scripts.Player
{
    public class PlayerRatGun : LauncherComponent
    {

        public InputActionReference shootActionRef;

        [SerializeField] private float launchSpeed = 20.0f;

        [SerializeField] private float launchAngle = 45.0f;

        public event Action OnTryShoot;

        private void Awake()
        {
            shootActionRef.action.performed += OnShootPerformed;
        }

        public void OnShootPerformed(InputAction.CallbackContext context)
        {
            OnTryShoot?.Invoke();
        }

        public override void Launch(LaunchParameters launchParameters = default)
        {
            base.Launch( new LaunchParameters( direction: Quaternion.AngleAxis(-launchAngle, spawnLocation.right) * spawnLocation.forward, speed: launchSpeed ) );
        }

        private void OnEnable()
        {
            shootActionRef.action.Enable();
        }

        private void OnDisable()
        {
            shootActionRef.action.Disable();
        }
    }
}