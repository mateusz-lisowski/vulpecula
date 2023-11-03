using System.Collections;
using UnityEngine;

public class Effects
{
    public static IEnumerator Flashing(GameObject target, float time)
    {
        float flashingFreq = 4f;

		SpriteRenderer renderer = target.GetComponent<SpriteRenderer>();
        Color col = renderer.color;

		int times = (int)Mathf.Round(time * flashingFreq);
        float waitTime = 0.5f / flashingFreq;

        for (int i = 0; i < times; i++)
        {
            renderer.material.color = new Color(col.r, col.g, col.b, 0.5f);
			yield return new WaitForSeconds(waitTime);

			renderer.material.color = col;
			yield return new WaitForSeconds(waitTime);
        }
    }
}
