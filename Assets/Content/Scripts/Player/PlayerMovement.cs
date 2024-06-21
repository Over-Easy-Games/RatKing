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

        [SerializeField] private Transform _cameraTransform;
        
        private void Awake()
        {
            if (TryGetComponent<Rigidbody>(out Rigidbody rb))
                _rigidbody = rb;
        }

        public void Tick(Vector2 moveInput)
        {
            Vector3 forwardMovement = _cameraTransform.forward.ProjectOntoPlane( Vector3.up ).normalized * moveInput.y * speed;
            Vector3 rightMovement = _cameraTransform.right.ProjectOntoPlane( Vector3.up ).normalized * moveInput.x * speed;
            Vector3 targetSpeed = forwardMovement + rightMovement;
            
            float rate = (moveInput == Vector2.zero) ? decelerationRate : accelerationRate;
            
            Vector3 deltaVelocity = ( targetSpeed - _rigidbody.velocity) * rate;
            _rigidbody.AddForce(deltaVelocity, ForceMode.VelocityChange);
        }
    }
}