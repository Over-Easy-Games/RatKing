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

        private void Awake()
        {
            shootActionRef.action.performed += context => { Launch(); };
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