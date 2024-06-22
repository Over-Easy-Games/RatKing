using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEngine;
using Content.Scripts.Utilities;

namespace Content.Scripts.Player
{
    public class PlayerRatPack : MonoBehaviour
    {

        private Vector3 _physicsColliderScale = Vector3.one;

        [SerializeField] private Transform physicsCollider;

        public event Action<int> OnRatsChanged;
        
        private int _ratCount = 0;
        public int RatCount => _ratCount;
        
        public void AddRat(int count = 1)
        {
            SetRats(_ratCount + 1);
            UpdateCollider();
        }
        
        public void RemoveRat(int count = 1)
        {
            if ( _ratCount < 0 )
                return;
            SetRats(_ratCount - 1);
            UpdateCollider();
        }

        public void SetRats(int count = 1)
        {
            if (_ratCount == count)
                return;
            OnRatsChanged?.Invoke(_ratCount);
            _ratCount = count;
        }

        private void UpdateCollider()
        {
            ChangePhysicsCollider(x: (_ratCount + 1) * 1.1f, z: (_ratCount + 1) * 1.1f);
        }
        
        private void ChangePhysicsCollider(object x = null, object y = null, object z = null)
        {
            _physicsColliderScale = _physicsColliderScale.Change(x, y, z);
            physicsCollider.localScale = _physicsColliderScale;
        }
    }
}