using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using Xsolla.Core;

public class UserAttributes : MonoSingleton<UserAttributes>
{
	public Action<List<UserAttributeModel>> AttributesChangedEvent;
	public List<UserAttributeModel> Attributes { get; private set; }

	private ILoginImplementation _implementation;
	
	public void Init(ILoginImplementation implementation)
	{
		_implementation = implementation;
		Attributes = new List<UserAttributeModel>();
	}
	
	public void GetAttributes([CanBeNull] Action<List<UserAttributeModel>> onSuccess = null, [CanBeNull] Action<Error> onError = null)
	{
		if (!IsDemoImplemented(() => onSuccess?.Invoke(Attributes))) return;
		InvokeDemoMethod(() => _implementation.GetAttributes(attributes =>
		{
			Attributes = attributes;
			AttributesChangedEvent?.Invoke(Attributes);
			onSuccess?.Invoke(attributes);
		}, onError));
	}
	
	public void SetAttributes(List<UserAttributeModel> attributes, [CanBeNull] Action onSuccess = null, 
		[CanBeNull] Action<Error> onError = null)
	{
		if (!IsDemoImplemented(onSuccess))
		{
			Attributes = attributes;
			return;
		}
		InvokeDemoMethod(() => _implementation.SetAttributes(attributes, onSuccess, onError));
	}

	public void RemoveAttributes(List<string> attributes, [CanBeNull] Action onSuccess = null, [CanBeNull] Action<Error> onError = null)
	{
		void Callback()
		{
			attributes.ForEach(a => Attributes.RemoveAll(r => r.key.Equals(a)));
			AttributesChangedEvent?.Invoke(Attributes);
			onSuccess?.Invoke();
		}
		if (!IsDemoImplemented(onSuccess))
			Callback();
		else
			InvokeDemoMethod(() => _implementation.RemoveAttributes(attributes, Callback, onError));
	}

	public void UpdateAttributes(List<UserAttributeModel> setAttributes, List<string> removeAttributes,
		[CanBeNull] Action onSuccess = null, [CanBeNull] Action<Error> onError = null)
	{
		SetAttributes(setAttributes, () => RemoveAttributes(removeAttributes, onSuccess, onError), onError);
	}
	
	private bool IsDemoImplemented(Action notImplementedCallback)
	{
		if (_implementation != null) return true;
		Debug.LogWarning("UserAttributes: IStoreDemoImplementation is null");
		notImplementedCallback?.Invoke();
		return false;
	}

	private void InvokeDemoMethod(Action method)
	{
		try
		{
			method?.Invoke();
		}
		catch (Exception e)
		{
			Debug.LogException(e);
		}
	}
}