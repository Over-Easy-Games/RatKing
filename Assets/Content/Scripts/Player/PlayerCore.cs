using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEngine;
using Content.Scripts.Utilities;

namespace Content.Scripts.Player
{
    public class PlayerCore : MonoBehaviour
    {
        private PlayerMovement _playerMovement;
        private PlayerRatPack _playerRatPack;
        
        private Vector2 _moveInput;

        [SerializeField]
        private Transform displayRoot;
        [SerializeField, Range(0f, 1f)] private float turnSpeed = 0.2f;
        
        private void Awake()
        {
            if (TryGetComponent<PlayerMovement>(out PlayerMovement playerMovement))
                _playerMovement = playerMovement;
            
            if (TryGetComponent<PlayerRatPack>(out PlayerRatPack playerRatPack))
                _playerRatPack = playerRatPack;
        }
        
        void Update()
        {
            // This is temporary to make it so that WASD isn't smoothed.
            // _moveInput = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
            _moveInput = Vector2.zero;
            if (Input.GetKey(KeyCode.W)){
                _moveInput.y += 1f;
            }
            if (Input.GetKey(KeyCode.S)){
                _moveInput.y -= 1f;
            }
            if (Input.GetKey(KeyCode.D)){
                _moveInput.x += 1f;
            }
            if (Input.GetKey(KeyCode.A)){
                _moveInput.x -= 1f;
            }

            _moveInput = _moveInput.normalized * _moveInput.magnitude;
            
            // Temp set up to debug growing and shrinking physics collider.
            if (Input.GetKeyDown(KeyCode.E)){
                _playerRatPack.AddRat();
            }
            if (Input.GetKeyDown(KeyCode.Q)){
                _playerRatPack.RemoveRat();
            }
            
        }

        private void FixedUpdate()
        {
            _playerMovement.Tick( _moveInput );
            displayRoot.rotation = Quaternion.Slerp(displayRoot.rotation, _playerMovement.PhysicsRoot.rotation, turnSpeed );
        }
    }
}