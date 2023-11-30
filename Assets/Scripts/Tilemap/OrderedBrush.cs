using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor.Tilemaps;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace UnityEditor
{
	[CustomGridBrush(true, true, true, "Default Brush")]
	public class OrderedBrush : GridBrush { }

	[CustomEditor(typeof(OrderedBrush))]
	public class OrderedBrushEditor : GridBrushEditor
	{
		public override GameObject[] validTargets
		{
			get
			{
				var targets = base.validTargets;
				Array.Sort(targets, (go1, go2) => String.Compare(go1.name, go2.name));

				var targetsOrdered = targets.Where(t => isTargetGround(t)).ToList();
				targetsOrdered = targetsOrdered.Concat(targets.Where(t => isTargetDecoration(t))).ToList();
				targetsOrdered = targetsOrdered.Concat(targets.Where(t => isTargetField(t))).ToList();
				targetsOrdered = targetsOrdered.Concat(targets.Where(t => isTargetEntity(t))).ToList();
				
				var targetsRest = targets.Where(t => !targetsOrdered.Contains(t)).ToList();
				targetsOrdered = targetsOrdered.Concat(targetsRest).ToList();

				targets = targetsOrdered.ToArray();

				return targets;
			}
		}

		private bool isTargetGround(GameObject tilemap)
		{
			return tilemap.transform.parent.name == "Environment"
				&& tilemap.GetComponent<TilemapCollider2D>() != null;
		}
		private bool isTargetDecoration(GameObject tilemap)
		{
			return tilemap.transform.parent.name == "Environment" 
				&& tilemap.GetComponent<TilemapCollider2D>() == null;
		}
		private bool isTargetField(GameObject tilemap)
		{
			return tilemap.transform.parent.name == "Fields";
		}
		private bool isTargetEntity(GameObject tilemap)
		{
			return tilemap.transform.parent.name == "Grid";
		}
	}
}
