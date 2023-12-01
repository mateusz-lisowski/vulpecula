using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(menuName = "Data/Layers")]
public class LayerManager : ScriptableObject
{
	public enum Layer
	{ 
		Default = 0,
		TransparentFX = 1,
		IgnoreRaycast = 2,
		Water = 4,
		UI = 5,
		Ground = 8,
		PlatformPassable = 9,
		PlayerPlatformPassable = 10,
		Player = 12,
		PlayerInvulnerable = 13,
		Enemy = 16,
		EnemyInvulnerable = 17,
		Detection = 20,
		GroundDamaging = 21,
		WallJumpable = 22,
		GroundDroppable = 23,
		GroundBreakable = 24,
		Slope = 25,
		Collectible = 28,
		Editor = 31
	}

	[SerializeField]
	public bool[] collisionMatrix = new bool[32 * 32];
}


[CustomEditor(typeof(LayerManager))]
public class LayerManagerEditor : Editor
{
	LayerManager manager;

	private static Type layerType { get { return typeof(LayerManager.Layer); } }
	private static class Styles
	{
		public static GUIStyle window
		{
			get
			{
				var style = new GUIStyle(GUI.skin.window);
				style.padding = new RectOffset(12, 6, 6, 6);
				style.margin = new RectOffset(12, 6, 6, 6);
				return style;
			}
		}
		public static GUIStyle leftLabel
		{
			get
			{
				var style = new GUIStyle(GUI.skin.label);
				style.alignment = TextAnchor.LowerRight;
				return style;
			}
		}

		public static float denseItemHeight = 16f;
		public static List<GUIContent> layerLabel;

		public static string SeparateCammelCase(string text)
		{
			if (string.IsNullOrWhiteSpace(text))
				return string.Empty;
			StringBuilder newText = new StringBuilder(text.Length * 2);
			newText.Append(text[0]);
			for (int i = 1; i < text.Length; i++)
			{
				if (char.IsUpper(text[i]))
					if (text[i - 1] != ' ' && !char.IsUpper(text[i - 1])
						&& (i >= text.Length || !char.IsUpper(text[i + 1])))
						newText.Append(' ');
				newText.Append(text[i]);
			}
			return newText.ToString();
		}
	}

	private void OnEnable()
	{
		manager = target as LayerManager;

		Styles.layerLabel = new List<GUIContent>();
		for (int i = 0; i < 32; i++)
			if (Enum.IsDefined(layerType, i))
				Styles.layerLabel.Add(new GUIContent(Styles.SeparateCammelCase(Enum.GetName(layerType, i))));
	}
	
	bool showLayers = false;
	bool showCollisionMatrix = false;
	Vector2 scrollCollisionMatrix = Vector2.zero;

	private void InspectorLayers()
	{
		showLayers = EditorGUILayout.BeginFoldoutHeaderGroup(showLayers, "Layers");
		if (showLayers)
		{
			EditorGUILayout.BeginVertical(Styles.window);
			EditorGUI.BeginDisabledGroup(true);

			for (int i = 0; i < 32; i++)
			{
				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.LabelField(String.Format("Layer {0}", i), GUILayout.Width(80f));
				EditorGUILayout.TextField(Styles.SeparateCammelCase(Enum.GetName(layerType, i)));
				EditorGUILayout.EndHorizontal();
			}

			EditorGUI.EndDisabledGroup();
			EditorGUILayout.EndVertical();
		}
		EditorGUILayout.EndFoldoutHeaderGroup();
	}

	private void InspectorCollisionMatrix()
	{
		showCollisionMatrix = EditorGUILayout.BeginFoldoutHeaderGroup(showCollisionMatrix, "Collision Matrix");
		if (showCollisionMatrix)
		{
			scrollCollisionMatrix = EditorGUILayout.BeginScrollView(scrollCollisionMatrix, Styles.window);

			float maxLabelWidth = Styles.layerLabel.Max(c => GUI.skin.label.CalcSize(c).x);
			float labelsHeight = Styles.layerLabel.Count * Styles.denseItemHeight;
			Rect rect = GUILayoutUtility.GetRect(maxLabelWidth + labelsHeight, maxLabelWidth + labelsHeight);

			for (int i = 0; i < Styles.layerLabel.Count; i++)
			{
				var label = Styles.layerLabel[Styles.layerLabel.Count - i - 1];

				Vector2 labelSize = GUI.skin.label.CalcSize(label);
				Vector2 labelCenter = new Vector2(
					rect.x + maxLabelWidth + (i + 0.5f) * Styles.denseItemHeight,
					rect.y + maxLabelWidth - labelSize.x / 2);

				GUIUtility.RotateAroundPivot(90f, labelCenter);
				GUI.Label(new Rect(labelCenter - labelSize / 2, labelSize),
					label, Styles.leftLabel);
				GUIUtility.RotateAroundPivot(-90f, labelCenter);
			}

			for (int i = 0; i < Styles.layerLabel.Count; i++)
				GUI.Label(new Rect(
					rect.x,
					rect.y + maxLabelWidth + i * Styles.denseItemHeight,
					maxLabelWidth, Styles.denseItemHeight), Styles.layerLabel[i], Styles.leftLabel);

			Rect currentRect = new Rect(rect.x + maxLabelWidth, rect.y + maxLabelWidth, 
				Styles.denseItemHeight, Styles.denseItemHeight);

			for (int i = 0; i < 32; i++)
				if (Enum.IsDefined(layerType, i))
				{
					for (int j = 31; j >= i; j--)
						if (Enum.IsDefined(layerType, j))
						{
							bool lastCollision = manager.collisionMatrix[i * 32 + j];

							bool collision = GUI.Toggle(currentRect, lastCollision,
								new GUIContent("", String.Format("{0}/{1}",
								Styles.SeparateCammelCase(Enum.GetName(layerType, i)),
								Styles.SeparateCammelCase(Enum.GetName(layerType, j))
								)));

							if (lastCollision != collision)
							{
								manager.collisionMatrix[i * 32 + j] = collision;
								manager.collisionMatrix[j * 32 + i] = collision;
							}

							currentRect.position = new Vector2(
								currentRect.x + Styles.denseItemHeight, currentRect.y);
						}
					currentRect.position = new Vector2(
						rect.x + maxLabelWidth, currentRect.y + Styles.denseItemHeight);
				}
			EditorGUILayout.EndScrollView();
		}
		EditorGUILayout.EndFoldoutHeaderGroup();
	}
	
	public override void OnInspectorGUI()
	{
		InspectorLayers();
		InspectorCollisionMatrix();
	}

}

