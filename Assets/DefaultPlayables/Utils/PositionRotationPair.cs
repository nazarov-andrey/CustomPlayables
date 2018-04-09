using System;
using UnityEngine;

namespace TimelineExtensions
{
    [Serializable]
    public class PositionRotationPair
    {
        public Vector3 Position;
        public Vector3 Rotation;

        public PositionRotationPair ()
        {
        }

        public PositionRotationPair (Vector3 position, Vector3 rotation)
        {
            Position = position;
            Rotation = rotation;
        }

        public override string ToString ()
        {
            return $"PositionRotationPair {nameof (Position)}: {Position}, {nameof (Rotation)}: {Rotation}";
        }
    }
}