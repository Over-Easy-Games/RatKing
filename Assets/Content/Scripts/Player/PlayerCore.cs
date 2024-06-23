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
    public class PlayerCore : MonoBehaviour
    {
        private PlayerMovement _playerMovement;
        private PlayerRatPack _playerRatPack;
        private HealthComponent _healthComponent;
        private HurtboxComponent _hurtboxComponent;
        private PlayerRatGun _ratGun;
        
        private Vector2 _moveInput;

        [SerializeField]
        private Transform displayRoot;
        [SerializeField, Range(0f, 1f)] private float meshTurnSpeed = 0.2f;
        
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
            
            _hurtboxComponent = GetComponentInChildren<HurtboxComponent>(); 
            _hurtboxComponent.OnHit += HurtboxComponentOnOnHit;
            
            if (TryGetComponent<PlayerRatGun>(out PlayerRatGun ratGun))
                _ratGun = ratGun;
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

        private void HurtboxComponentOnOnHit(int damage)
        {
            if (damage > 0)
                _playerRatPack.RemoveRat(damage);
            else{
                _playerRatPack.AddRat( Math.Abs(damage) );
            }
        }

        private void FixedUpdate()
        {
            _playerMovement.Tick();
            displayRoot.rotation = Quaternion.Slerp(displayRoot.rotation, _playerMovement.PhysicsRoot.rotation, meshTurnSpeed );
        }
    }
}