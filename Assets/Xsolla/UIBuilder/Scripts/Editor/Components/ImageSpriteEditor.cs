﻿using System.Linq;
using UnityEditor;

namespace Xsolla.UIBuilder
{
	[CustomEditor(typeof(ImageSpriteDecorator))]
	public class ImageSpriteEditor : ThemeDecoratorEditor
	{
		private SerializedProperty imageProp;

		protected override void FindProperties()
		{
			imageProp = serializedObject.FindProperty("_image");
		}

		protected override void DrawProperties()
		{
			EditorGUILayout.PropertyField(imageProp);

			var colors = ThemesLibrary.Current.Sprites.ToArray();
			var guids = colors.Select(x => x.Id).ToList();
			var index = guids.IndexOf(propId.stringValue);
			var names = colors.Select(x => x.Name).ToArray();

			var selectedIndex = EditorGUILayout.Popup("Sprite", index, names);
			if (selectedIndex != index)
			{
				propId.stringValue = guids[selectedIndex];
			}
		}
	}
}