using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine.Utility;
using UnityEditor.UIElements;
using UnityEngine;
using Content.Scripts.Utilities;
using UnityEngine.InputSystem;

namespace Content.Scripts.Player
{
    public class PlayerCamera : MonoBehaviour
    {

        [SerializeField] private Transform cameraTarget;
        [SerializeField] private float cameraSpeed = 5.0f;

        public InputActionReference lookActionRef;
        private Vector2 _lookInput;

        private void FixedUpdate()
        {
            _lookInput = lookActionRef.action.ReadValue<Vector2>();
            _lookInput = _lookInput.normalized * _lookInput.magnitude;
            cameraTarget.Rotate(Vector3.up, _lookInput.x * cameraSpeed);
        }
        
        private void OnEnable()
        {
            lookActionRef.action.Enable();
        }

        private void OnDisable()
        {
            lookActionRef.action.Disable();
        }
    }
}
