using System;
using System.Collections.Generic;
using UnityEngine;

namespace _193396
{
	public class CustomRegionsJoin : MonoBehaviour
	{
		public GridPreprocessor preprocessor;

		public Grid grid { get; private set; }
		public TilemapHelper.Region region { get; private set; }


		private void Start()
		{
			grid = preprocessor.GetComponent<Grid>();

			List<Vector3Int> triggeredCoords = new List<Vector3Int>{ grid.WorldToCell(transform.position) };

			region = preprocessor.customRegions.Find(r => r.contains(triggeredCoords));

			if (region == null)
				throw new ApplicationException("Failed to find custom region for: " + name);
		}
	}
}