using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneratedPlatforms : MonoBehaviour
{
    public GameObject platformPrefab;
    public int numberOfPlatforms = 8;
    public float radius = 5f;
    public float rotationSpeed = 20f;

    private GameObject[] platforms;
    private float angleOffset = 0f;

    void Start()
    {
        GenerateCircleOfPlatforms();
    }

    void Update()
    {
        MovePlatformsInCircle();
    }

    void GenerateCircleOfPlatforms()
    {
        float angleStep = 360f / numberOfPlatforms;
        platforms = new GameObject[numberOfPlatforms];

        for (int i = 0; i < numberOfPlatforms; i++)
        {
            float angle = i * angleStep + angleOffset;
            float x = Mathf.Sin(Mathf.Deg2Rad * angle) * radius;
            float y = Mathf.Cos(Mathf.Deg2Rad * angle) * radius;

            Vector3 platformPosition = new Vector3(x, y, 0f);
            platforms[i] = Instantiate(platformPrefab, platformPosition, Quaternion.identity);
        }
    }

    void MovePlatformsInCircle()
    {
        angleOffset += rotationSpeed * Time.deltaTime;

        for (int i = 0; i < numberOfPlatforms; i++)
        {
            float angle = i * (360f / numberOfPlatforms) + angleOffset;
            float x = Mathf.Sin(Mathf.Deg2Rad * angle) * radius;
            float y = Mathf.Cos(Mathf.Deg2Rad * angle) * radius;

            Vector3 platformPosition = new Vector3(x, y, 0f);
            platforms[i].transform.position = platformPosition;
        }
    }
}
 