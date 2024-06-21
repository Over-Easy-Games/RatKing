using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEngine;

namespace Content.Scripts.Utilities
{
    public class Helpers
    {
        
    }
    public static class StaticHelpers
    {
        public static Vector3 Change(this Vector3 srcVector, object x = null, object y = null, object z = null) {
            float newX = x == null ? srcVector.x : Convert.ToSingle(x);
            float newY = y == null ? srcVector.y : Convert.ToSingle(y);
            float newZ = z == null ? srcVector.z : Convert.ToSingle(z);

            return new Vector3(newX, newY, newZ);
        }
        
        public static Vector3 ChangeDelta(this Vector3 srcVector, object x = null, object y = null, object z = null) {
            float newX = x == null ? srcVector.x : Convert.ToSingle(x);
            float newY = y == null ? srcVector.y : Convert.ToSingle(y);
            float newZ = z == null ? srcVector.z : Convert.ToSingle(z);

            return new Vector3(srcVector.x + newX, srcVector.y + newY, srcVector.z + newZ);
        }
    }
}