﻿using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Xsolla.UIBuilder
{
	public class ThemesLibrary : ScriptableObject
	{
		[HideInInspector]
		[SerializeField] private string _currentThemeId = default;

		[HideInInspector]
		[SerializeField] private List<Theme> _themes = new List<Theme>();

		public static Theme Current
		{
			get
			{
				var id = Instance._currentThemeId;
				if (string.IsNullOrEmpty(id))
				{
					return null;
				}

				return Themes.FirstOrDefault(x => x.Id == id);
			}
			set
			{
				Instance._currentThemeId = value != null ? value.Id : string.Empty;
				CurrentChanged?.Invoke(value);
			}
		}

		public static event Action<Theme> CurrentChanged;

		public static List<Theme> Themes
		{
			get => Instance._themes;
			set => Instance._themes = value;
		}

		public static void SetCurrentThemeById(string id)
		{
			var theme = Themes.FirstOrDefault(x => x.Id == id);
			if (theme != null)
			{
				Current = theme;
			}
		}

		public static void SetCurrentThemeByName(string name)
		{
			var theme = Themes.FirstOrDefault(x => x.Name == name);
			if (theme != null)
			{
				Current = theme;
			}
		}

		#region Singleton

		private const string AssetPath = "UIBuilder/ThemesLibrary";

		private static ThemesLibrary _instance;

		public static ThemesLibrary Instance
		{
			get
			{
				if (!_instance)
				{
					_instance = Resources.Load<ThemesLibrary>(AssetPath);
					Debug.Assert(_instance, $"Can't load instance of ThemesLibrary by path: {AssetPath}");
				}

				return _instance;
			}
		}

		#endregion
	}
}