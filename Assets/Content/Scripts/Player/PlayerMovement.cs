using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine.Utility;
using UnityEditor.UIElements;
using UnityEngine;
using Content.Scripts.Utilities;

namespace Content.Scripts.Player
{
    public class PlayerMovement : MonoBehaviour
    {

        private Rigidbody _rigidbody;

        [SerializeField] private float speed = 10f;

        [SerializeField, Range(0f, 1f)] private float accelerationRate = 0.3f;

        [SerializeField, Range(0f, 1f)] private float decelerationRate = 0.3f;

        [SerializeField] private Transform cameraTransform;

        [SerializeField] private Transform physicsRoot;
        public Transform PhysicsRoot => physicsRoot;
        

        [SerializeField, Range(0f, 1f)] private float turnSpeed = 1.0f;

        
        private int _LocalRatCount = 0;
        
        private void Awake()
        {
            if (TryGetComponent<Rigidbody>(out Rigidbody rb))
                _rigidbody = rb;
        }

        public void Tick(Vector2 moveInput)
        {
            Vector3 forward = cameraTransform.forward.ProjectOntoPlane(Vector3.up).normalized;
            Vector3 right = cameraTransform.right.ProjectOntoPlane(Vector3.up).normalized;

            Vector3 targetDirection = (forward * moveInput.y + right * moveInput.x).normalized;

            if (moveInput != Vector2.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(targetDirection, Vector3.up);
                physicsRoot.rotation = Quaternion.Slerp(physicsRoot.rotation, targetRotation, turnSpeed);
            }

            float speedMultiplier = speed * ( 1.0f / Mathf.Max(1.0f, (float)_LocalRatCount) );
            Vector3 currentForward = physicsRoot.forward * moveInput.magnitude * speedMultiplier;

            float rate = (moveInput == Vector2.zero) ? decelerationRate : accelerationRate;

            Vector3 targetSpeed = currentForward;
            Vector3 deltaVelocity = (targetSpeed - _rigidbody.velocity) * rate;
            _rigidbody.AddForce(deltaVelocity, ForceMode.VelocityChange);
        }

        public void SetRatCount(int count)
        {
            _LocalRatCount = count;
        }
    }
}
