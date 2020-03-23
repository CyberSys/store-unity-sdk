﻿using System;
using UnityEngine;
using UnityEngine.UI;
using Xsolla.Login;

public class AttributeItemUI : MonoBehaviour
{
	[SerializeField]
	InputField inputKey;
	[SerializeField]
	InputField inputValue;
	[SerializeField]
	SimpleButton removeButton;

	UserAttribute _itemInformation;

	AttributesItemsContainer _attributesContainer;

	public Action<UserAttribute> onRemove;

	private void Awake()
	{
		_attributesContainer = GetComponentInParent<AttributesItemsContainer>();
	}

	public void Initialize(UserAttribute itemInformation)
	{
		_itemInformation = itemInformation;

		inputKey.text = itemInformation.key;
		inputValue.text = itemInformation.value;

		removeButton.onClick = () =>
		{
			onRemove?.Invoke(_itemInformation);
			Destroy(gameObject);
		};
	}

	public void OnKeyEdited()
	{
		if (inputKey.text == _itemInformation.key)
		{
			return;
		}
		
		if (_attributesContainer.ContainsAttribute(inputKey.text))
		{
			inputKey.text = _itemInformation.key;
			Debug.LogError("Attribute with specified key already exists");
		}
		else
		{
			_attributesContainer.MarkAttributeToRemove(_itemInformation.key);
			_itemInformation.key = inputKey.text;
		}
	}

	public void OnValueEdited()
	{
		_itemInformation.value = inputValue.text;
	}
}