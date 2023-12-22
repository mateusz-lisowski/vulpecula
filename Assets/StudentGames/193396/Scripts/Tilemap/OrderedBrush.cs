using System;
using System.Linq;
using UnityEngine;
using UnityEditor.Tilemaps;
using UnityEditor;

namespace _193396
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

				targetsOrdered.RemoveAll(t => isTargetAutogen(t));
				targets = targetsOrdered.ToArray();

				return targets;
			}
		}

		private bool isTargetGround(GameObject tilemap)
		{
			return tilemap.transform.parent.name == "Ground";
		}
		private bool isTargetDecoration(GameObject tilemap)
		{
			return tilemap.transform.parent.name == "Environment";
		}
		private bool isTargetField(GameObject tilemap)
		{
			return tilemap.transform.parent.name == "Fields";
		}
		private bool isTargetEntity(GameObject tilemap)
		{
			return tilemap.transform.parent.name == "Grid";
		}

		private bool isTargetAutogen(GameObject tilemap)
		{
			return tilemap.transform.parent.name == "Autogen";
		}
	}
}
