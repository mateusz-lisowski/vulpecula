using UnityEngine;

namespace _193396
{
    public class ForcePosition : MonoBehaviour
    {
        public bool forceX = false;
        public float positionX = 0f;
        public bool forceY = false;
        public float positionY = 0f;

        void Update()
        {
            transform.position = new Vector3(
                forceX ? positionX : transform.position.x,
                forceY ? positionY : transform.position.y,
                transform.position.z);
        }
    }
}