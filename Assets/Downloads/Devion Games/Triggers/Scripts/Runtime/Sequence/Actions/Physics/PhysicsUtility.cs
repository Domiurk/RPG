using UnityEngine;

namespace DevionGames
{
    public enum Direction
    {
        Forward,
        Backward,
        Up,
        Down,
        Left,
        Right
    }

    public static class PhysicsUtility
    {
        public static Vector3 GetDirection(Transform transform, Direction direction)
        {
            return direction switch{
                Direction.Backward => -transform.forward,
                Direction.Up => transform.up,
                Direction.Down => -transform.up,
                Direction.Left => -transform.right,
                Direction.Right => transform.right,
                _ => transform.forward
            };
        }
    }
}