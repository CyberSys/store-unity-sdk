﻿using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SimpleButtonLockDecorator : SimpleButton
{
	bool isLocked = false;

	public void Lock()
	{
		isLocked = true;
	}

	public void Unlock()
	{
		isLocked = false;
	}

	public bool IsLocked() => isLocked;

	public override void OnPointerDown(PointerEventData eventData)
	{
		if (!IsLocked()) {
			base.OnPointerDown(eventData);
		}
	}

	public override void OnPointerUp(PointerEventData eventData)
	{
		if (!IsLocked()) {
			base.OnPointerUp(eventData);
		}
	}

	public override void OnPointerEnter(PointerEventData eventData)
	{
		if (!IsLocked()) {
			base.OnPointerEnter(eventData);
		}
	}

	public override void OnPointerExit(PointerEventData eventData)
	{
		if (!IsLocked()) {
			base.OnPointerExit(eventData);
		}
	}

	public override void OnDrag(PointerEventData eventData)
	{
		if (!IsLocked()) {
			base.OnDrag(eventData);
		}
	}
}