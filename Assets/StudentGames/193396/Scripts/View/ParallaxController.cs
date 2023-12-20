using UnityEngine;

namespace _193396
{
    public class ParallaxController : MonoBehaviour
    {
        public float parallaxEffect = 1f;

        private Transform cameraTransform;
        private float length;
        private float startPosition;

        void Start()
        {
            cameraTransform = transform.parent;

            length = GetComponent<SpriteRenderer>().bounds.size.x;
            startPosition = transform.position.x;
        }

        void LateUpdate()
        {
            float temp = cameraTransform.position.x * (1 - parallaxEffect);
            float dist = cameraTransform.position.x * parallaxEffect;

            transform.position = new Vector3(startPosition + dist, transform.position.y, transform.position.z);

            if (temp > startPosition + length)
                startPosition += length;

            if (temp < startPosition - length)
                startPosition -= length;
        }
    }
}