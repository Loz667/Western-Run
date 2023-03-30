using UnityEngine;

namespace WesternRun
{
    public enum TileType
    {
        Straight,
        Left,
        Right,
        Sideways
    }

    public class Tile : MonoBehaviour
    {
        public TileType type;
        public Transform pivot;
    }
}

