using System.Collections.Generic;
using UnityEngine;

namespace _193396
{
	[CreateAssetMenu(menuName = "193396/Data/Behavior/ShockwaveAttack")]
	public class ShockwaveAttackBehaviorData : BaseAttackBehaviorData
	{
		[Space(10)]

		[Tooltip("Layers that can provoke an attack")]
		public RuntimeSettings.LayerMaskInput provokeLayers;
		[Tooltip("Object to instantiate on every wave of an attack")]
		public GameObject wavePrefab;
		[Tooltip("Direction toward which to send an attack")]
		public List<Vector2> directions;

		[Space(5)]

		[Tooltip("Speed of an attack")]
		public float speed = 6f;
		[Tooltip("Time after which the projectile is destroyed")]
		public float maxLifetime = 3f;

		[Space(5)]

		[Tooltip("Number of waves spawned from the projectile per second")]
		public float spawnFrequency = 6f;
	}
}