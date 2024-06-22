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
    public class PlayerMovement : MonoBehaviour
    {

        private Rigidbody _rigidbody;

        [SerializeField] private float speed = 10f;

        [SerializeField] private float minSpeed = 1f;

        [SerializeField, Range(0f, 1f)] private float accelerationRate = 0.3f;

        [SerializeField, Range(0f, 1f)] private float decelerationRate = 0.3f;

        [SerializeField] private Transform cameraTransform;

        [SerializeField] private Transform physicsRoot;
        public Transform PhysicsRoot => physicsRoot;
        

        [SerializeField, Range(0f, 1f)] private float turnSpeed = 1.0f;

        
        private int _LocalRatCount = 0;

        public InputActionReference moveActionRef;
        private Vector2 _moveInput;
        
        private void Awake()
        {
            if (TryGetComponent<Rigidbody>(out Rigidbody rb))
                _rigidbody = rb;
        }

        private void Update()
        {
            _moveInput = moveActionRef.action.ReadValue<Vector2>();
            _moveInput = _moveInput.normalized * _moveInput.magnitude;
        }

        public void Tick()
        {
            Vector3 forward = cameraTransform.forward.ProjectOntoPlane(Vector3.up).normalized;
            Vector3 right = cameraTransform.right.ProjectOntoPlane(Vector3.up).normalized;

            Vector3 targetDirection = (forward * _moveInput.y + right * _moveInput.x).normalized;

            if (_moveInput != Vector2.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(targetDirection, Vector3.up);
                physicsRoot.rotation = Quaternion.Slerp(physicsRoot.rotation, targetRotation, turnSpeed);
            }

            float speedMultiplier = speed * ( 1.0f / Mathf.Max(1.0f, _LocalRatCount * 0.5f) );
            speedMultiplier = Mathf.Max(minSpeed, speedMultiplier);
            Vector3 currentForward = physicsRoot.forward * _moveInput.magnitude * speedMultiplier;

            float decelerationRateMultiplied = decelerationRate; // / Mathf.Max(1.0f, _LocalRatCount * 0.5f);
            float accelerationRateMultiplied = accelerationRate; //  / Mathf.Max(1.0f, _LocalRatCount * 0.5f);
            float rate = (_moveInput == Vector2.zero) ? decelerationRateMultiplied : accelerationRateMultiplied;

            Vector3 targetSpeed = currentForward;
            Vector3 deltaVelocity = (targetSpeed - _rigidbody.velocity) * rate;
            _rigidbody.AddForce(deltaVelocity, ForceMode.VelocityChange);
        }

        public void SetRatCount(int count)
        {
            _LocalRatCount = count;
        }
        
        private void OnEnable()
        {
            moveActionRef.action.Enable();
        }

        private void OnDisable()
        {
            moveActionRef.action.Disable();
        }
    }
}
