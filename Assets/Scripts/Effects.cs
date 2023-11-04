using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "Effects Data")]
public class Effects : ScriptableObject
{
    public static Effects instance;

	[Space(5)]

	[Header("Flashing")]
	public float flashingFrequency;
	[Range(0f, 1f)] public float flashingAlpha;


	Effects()
	{
		instance = this;
	}


	public IEnumerator Flashing(GameObject target, float time)
    {
		SpriteRenderer renderer = target.GetComponent<SpriteRenderer>();
        Color col = renderer.color;

		int times = (int)Mathf.Round(time * flashingFrequency);
        float waitTime = 0.5f / flashingFrequency;

        for (int i = 0; i < times; i++)
        {
            renderer.material.color = new Color(col.r, col.g, col.b, flashingAlpha);
			yield return new WaitForSeconds(waitTime);

			renderer.material.color = col;
			yield return new WaitForSeconds(waitTime);
        }
    }
}
