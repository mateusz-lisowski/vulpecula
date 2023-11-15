using UnityEngine;

public class PathData : MonoBehaviour
{
    public enum Type 
    {
        Cycle, // After the last move towards the first
        Repeat, // After the last teleport to the first
        Bounce, // After the last move back in reverse order
    }

    public float speed = 2f;
    public Type type = Type.Cycle;
}
