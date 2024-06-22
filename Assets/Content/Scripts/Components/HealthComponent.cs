using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Content.Scripts.Components
{
    public class HealthComponent : MonoBehaviour
    {
        [SerializeField]
        protected int maxHealth = 10;
        private int _currentHealth;
        
        public int MaxHealth => maxHealth;
        public int Health => _currentHealth;
        
        public event Action<int> OnHealthChanged;
        public event Action OnHealthEmpty;

        protected void Awake()
        {
            _currentHealth = maxHealth;
        }

        public void AddHealth(int amount = 1)
        {
            if (amount < 0){
                Debug.LogError("Cannot use negative numbers in AddHealth()");
                return;
            }

            // If added amount overflows max health
            if (_currentHealth + amount >= maxHealth)
                SetHealth(maxHealth);
            else
                SetHealth(_currentHealth + amount);
        }

        public void RemoveHealth(int amount = 1)
        {
            if (amount < 0){
                Debug.LogError("Cannot use negative numbers in RemoveHealth()");
                return;
            }
            
            if ( _currentHealth - amount > 0 )
                SetHealth(_currentHealth - amount);
            else
                SetHealth(0);
        }

        private void SetHealth(int newHealth)
        {
            if (newHealth == _currentHealth)
                return;

            _currentHealth = newHealth;
            OnHealthChanged?.Invoke(newHealth);

            if (newHealth == 0)
                OnHealthEmpty?.Invoke();
        } 
    }
}