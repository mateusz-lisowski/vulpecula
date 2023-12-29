using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace _193396
{
	[CreateAssetMenu(menuName = "Data/Layers")]
	public class RuntimeSettings : ScriptableObject, ISerializationCallbackReceiver
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
			PlayerTransition = 14,
			Enemy = 16,
			EnemyInvulnerable = 17,
			EnemyFlying = 18,
			EnemyFlyingInvulnerable = 19,
			Detection = 20,
			GroundDamaging = 21,
			WallJumpable = 22,
			GroundDroppable = 23,
			GroundBreakable = 24,
			Slope = 25,
			Collectible = 28,
			Editor = 31
		}
		public enum Tag
		{
			None,
			EditorOnly,
			Player,
			Enemy,
			EnemyFlying,
			Detection,
			Ground,
			Passable,
			PassablePlayer,
			Collectible,
			WallJumpable,
			GroundDamaging,
			GroundDroppable,
			Breakable,
			BreakableExplode,
			Slope,
		}

		[Serializable]
		public struct LayerMaskInput
		{
			public int value;

			public static implicit operator LayerMask(LayerMaskInput m) => m.value;
			public static implicit operator int(LayerMaskInput m) => m.value;
		}

		public struct PhysicsSettings
		{
			public Vector2 gravity;
		}

		private bool installed = false;
		private int[] collisionMatrixBackup = new int[32];
		private PhysicsSettings physicsBackup;
		public void install()
		{
			if (!installed)
			{
				installed = true;

				for (int i = 0; i < 32; i++)
					collisionMatrixBackup[i] = Physics2D.GetLayerCollisionMask(i);

				physicsBackup.gravity = Physics2D.gravity;
			}

			for (int i = 0; i < 32; i++)
			{
				int mask = 0;
				for (int j = 0; j < 32; j++)
					if (collisionMatrix[32 * i + j])
						mask |= 1 << j;

				Physics2D.SetLayerCollisionMask(i, mask);
			}

			Physics2D.gravity = physics.gravity;
		}
		public void uninstall()
		{
			if (!installed)
				return;
			installed = false;

			for (int i = 0; i < 32; i++)
				Physics2D.SetLayerCollisionMask(i, collisionMatrixBackup[i]);

			Physics2D.gravity = physicsBackup.gravity;
		}
		public void mapTagsToLayers(GameObject gameObject)
		{
			Stack<GameObject> objects = new Stack<GameObject>();
			objects.Push(gameObject);

			GameObject currentObject;
			while (objects.TryPop(out currentObject))
			{
				currentObject.layer = (int)tagMapping.layer(currentObject.tag);

				if (currentObject.layer == 0 && currentObject != gameObject)
					currentObject.layer = currentObject.transform.parent.gameObject.layer;

				foreach (Transform child in currentObject.transform)
					objects.Push(child.gameObject);
			}
		}


		public class TagMapper
		{
			public Tag tag(string name)
			{
				if (nameToTag.ContainsKey(name))
					return nameToTag[name];
				else
					return Tag.None;
			}
			public Layer layer(string name)
			{
				if (nameToTag.ContainsKey(name))
					return tagToLayer[(int)nameToTag[name]];
				else
					return Layer.Default;
			}
			public Layer layer(Tag tag)
			{
				return tagToLayer[(int)tag];
			}
			public string name(Tag tag)
			{
				foreach (var pair in nameToTag)
					if (pair.Value == tag)
						return pair.Key;
				return "";
			}

			public Dictionary<string, Tag> nameToTag = new Dictionary<string, Tag>();
			public Layer[] tagToLayer = new Layer[Enum.GetValues(typeof(Tag)).Cast<int>().Max() + 1];
		}
		[NonSerialized] public PhysicsSettings physics = new PhysicsSettings();
		[NonSerialized] public TagMapper tagMapping = new TagMapper();
		[NonSerialized] public bool[] collisionMatrix = new bool[32 * 32];


		[Serializable]
		private struct SerializablePhysicsSettings
		{
			public float gravity;
		}
		[Serializable]
		private struct SerializableTagMappingData
		{
			public string name;
			public Layer layer;
		}
		[SerializeField] private SerializablePhysicsSettings serializablePhysics;
		[SerializeField] private SerializableTagMappingData[] serializableTagMapping;
		[SerializeField] private int[] serializableCollisionMatrix = new int[32];

		public void OnBeforeSerialize()
		{
			serializablePhysics = new SerializablePhysicsSettings();
			serializablePhysics.gravity = physics.gravity.y;

			serializableTagMapping = new SerializableTagMappingData[tagMapping.tagToLayer.Length];
			for (int i = 0; i < serializableTagMapping.Length; i++)
				serializableTagMapping[i] = new SerializableTagMappingData
				{
					name = tagMapping.name((Tag)i),
					layer = tagMapping.layer((Tag)i)
				};

			for (int i = 0; i < 32; i++)
				for (int j = 0; j < 32; j++)
					if (collisionMatrix[32 * i + j])
						serializableCollisionMatrix[i] |= 1 << j;
					else
						serializableCollisionMatrix[i] &= ~(1 << j);
		}
		public void OnAfterDeserialize()
		{
			physics.gravity = new Vector2(0f, serializablePhysics.gravity);

			tagMapping.nameToTag.Clear();

			for (int i = 0; i < serializableTagMapping.Length; i++)
			{
				tagMapping.tagToLayer[i] = serializableTagMapping[i].layer;
				if (!tagMapping.nameToTag.ContainsKey(serializableTagMapping[i].name))
					tagMapping.nameToTag.Add(serializableTagMapping[i].name, (Tag)i);
			}

			for (int i = 0; i < 32; i++)
				for (int j = 0; j < 32; j++)
					collisionMatrix[32 * i + j] = (serializableCollisionMatrix[i] & (1 << j)) != 0;
		}
	}


	[CustomPropertyDrawer(typeof(RuntimeSettings.LayerMaskInput))]
	public class LayerMaskDrawer : PropertyDrawer
	{
		private enum LayerMaskEnum
		{
			Default = 1 << 0,
			TransparentFX = 1 << 1,
			IgnoreRaycast = 1 << 2,
			Water = 1 << 4,
			UI = 1 << 5,
			Ground = 1 << 8,
			PlatformPassable = 1 << 9,
			PlayerPlatformPassable = 1 << 10,
			Player = 1 << 12,
			PlayerInvulnerable = 1 << 13,
			PlayerTransition = 1 << 14,
			Enemy = 1 << 16,
			EnemyInvulnerable = 1 << 17,
			EnemyFlying = 1 << 18,
			EnemyFlyingInvulnerable = 1 << 19,
			Detection = 1 << 20,
			GroundDamaging = 1 << 21,
			WallJumpable = 1 << 22,
			GroundDroppable = 1 << 23,
			GroundBreakable = 1 << 24,
			Slope = 1 << 25,
			Collectible = 1 << 28,
			Editor = 1 << 31
		}

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			string name = property.displayName;
			property.Next(true);

			var newMask = (int)(LayerMaskEnum)EditorGUI.EnumFlagsField(
				position, name, (LayerMaskEnum)property.intValue);

			if (property.intValue != newMask)
			{
				property.intValue = newMask;
				property.serializedObject.ApplyModifiedProperties();
			}
		}
	}

	[CustomEditor(typeof(RuntimeSettings))]
	public class RuntimeSettingsEditor : Editor
	{
		private RuntimeSettings manager;

		private static Type layerType { get { return typeof(RuntimeSettings.Layer); } }
		private static Type tagType { get { return typeof(RuntimeSettings.Tag); } }
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
			manager = target as RuntimeSettings;

			Styles.layerLabel = new List<GUIContent>();
			for (int i = 0; i < 32; i++)
				if (Enum.IsDefined(layerType, i))
					Styles.layerLabel.Add(new GUIContent(Styles.SeparateCammelCase(Enum.GetName(layerType, i))));
		}

		private static bool showPhysics = false;
		private static bool showLayers = false;
		private static bool showTagMappings = false;
		private static bool showCollisionMatrix = false;
		private static Vector2 scrollCollisionMatrix = Vector2.zero;

		private void InspectorPhysics(ref bool dirty)
		{
			showPhysics = EditorGUILayout.BeginFoldoutHeaderGroup(showPhysics, "Physics Settings");
			if (showPhysics)
			{
				EditorGUILayout.BeginVertical(Styles.window);

				float newGravity = EditorGUILayout.FloatField("Gravity", manager.physics.gravity.y);
				if (newGravity != manager.physics.gravity.y)
				{
					manager.physics.gravity.y = newGravity;
					dirty = true;
				}

				EditorGUILayout.EndVertical();
			}
			EditorGUILayout.EndFoldoutHeaderGroup();
		}

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

		private void InspectorTagMappings(ref bool dirty)
		{
			showTagMappings = EditorGUILayout.BeginFoldoutHeaderGroup(showTagMappings, "Tag Mappings");
			if (showTagMappings)
			{
				EditorGUILayout.BeginVertical(Styles.window);

				foreach (RuntimeSettings.Tag tag in Enum.GetValues(tagType))
				{
					var name = manager.tagMapping.name(tag);
					var layer = manager.tagMapping.layer(tag);

					EditorGUILayout.BeginHorizontal();


					EditorGUI.BeginDisabledGroup(true);
					EditorGUILayout.TextField(Styles.SeparateCammelCase(Enum.GetName(tagType, tag)));
					EditorGUI.EndDisabledGroup();

					var newName = EditorGUILayout.TagField(name);
					var newLayer = (RuntimeSettings.Layer)EditorGUILayout.EnumPopup(layer);

					EditorGUILayout.EndHorizontal();

					if (layer != newLayer)
					{
						manager.tagMapping.tagToLayer[(int)tag] = newLayer;
						dirty = true;
					}
					if (name != newName)
					{
						manager.tagMapping.nameToTag.Remove(name);
						manager.tagMapping.nameToTag[newName] = tag;
						dirty = true;
					}
				}

				EditorGUILayout.EndVertical();
			}
			EditorGUILayout.EndFoldoutHeaderGroup();
		}

		private void InspectorCollisionMatrix(ref bool dirty)
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
									dirty = true;
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
			bool dirty = false;

			InspectorPhysics(ref dirty);
			InspectorLayers();
			InspectorTagMappings(ref dirty);
			InspectorCollisionMatrix(ref dirty);

			if (dirty)
				EditorUtility.SetDirty(manager);
		}

	}
}