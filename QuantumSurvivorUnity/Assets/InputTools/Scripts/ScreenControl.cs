using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.OnScreen;

namespace InputTools
{
	public abstract class ScreenControl : OnScreenControl, IPointerDownHandler, IPointerUpHandler, IDragHandler
	{
		public abstract void OnPointerUp(PointerEventData eventData);

		public abstract void OnPointerDown(PointerEventData eventData);

		public abstract void OnDrag(PointerEventData eventData);
	}
}