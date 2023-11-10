#if UNITY_EDITOR
using UnityEngine;

namespace UnityEditor
{
	[CustomEditor(typeof(AdvancedRuleTile))]
	[CanEditMultipleObjects]
	public class AdvancedRuleTileEditor : RuleTileEditor
	{
		public Texture2D AnyIcon;
		public Texture2D SpecifiedIcon;
		public Texture2D NothingIcon;

		public override void RuleOnGUI(Rect rect, Vector3Int position, int neighbor)
		{
			switch (neighbor)
			{
				case AdvancedRuleTile.Neighbor.Any:
					if (AnyIcon != null)
					{
						GUI.DrawTexture(rect, AnyIcon);
						return;
					}
					break;
				case AdvancedRuleTile.Neighbor.Specified:
					if (SpecifiedIcon != null)
					{
						GUI.DrawTexture(rect, SpecifiedIcon);
						return;
					}
					break;
				case AdvancedRuleTile.Neighbor.Nothing:
					if (NothingIcon != null)
					{
						GUI.DrawTexture(rect, NothingIcon);
						return;
					}
					break;
			}

			base.RuleOnGUI(rect, position, neighbor);
		}
	}
}
#endif