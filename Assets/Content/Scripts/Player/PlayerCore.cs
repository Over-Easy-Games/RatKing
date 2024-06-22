using System;
using System.Collections;
using System.Collections.Generic;
using Content.Scripts.Components;
using UnityEditor.UIElements;
using UnityEngine;
using Content.Scripts.Utilities;

namespace Content.Scripts.Player
{
    public class PlayerCore : MonoBehaviour
    {
        private PlayerMovement _playerMovement;
        private PlayerRatPack _playerRatPack;
        private HealthComponent _healthComponent;
        
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
            _playerRatPack.OnRatsChanged += PlayerRatPackOnOnRatsChanged;
            
            if (TryGetComponent<HealthComponent>(out HealthComponent healthComponent))
                _healthComponent = healthComponent;
            _healthComponent.OnHealthChanged += HealthComponentOnOnHealthChanged;
            _healthComponent.OnHealthEmpty += HealthComponentOnOnHealthEmpty;
        }

        private void PlayerRatPackOnOnRatsChanged(int newRatCount, int ratDelta)
        {
            _playerMovement.SetRatCount(newRatCount);
            
            if (ratDelta > 0)
                _healthComponent.AddHealth(ratDelta);
            else
                _healthComponent.RemoveHealth( Math.Abs(ratDelta) );
        }

        private void HealthComponentOnOnHealthEmpty()
        {
            Debug.Log("Handle what happens when health is empty.");
        }

        private void HealthComponentOnOnHealthChanged(int newHealth)
        {
            Debug.Log($"Health: {newHealth}");
        }

        private void FixedUpdate()
        {
            _playerMovement.Tick();
            displayRoot.rotation = Quaternion.Slerp(displayRoot.rotation, _playerMovement.PhysicsRoot.rotation, turnSpeed );
        }
    }
}