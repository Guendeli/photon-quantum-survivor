#pragma warning disable CS0162
using UnityEngine;

namespace InputTools
{
	public class MobileButtonsManager : MonoBehaviour
	{
		public bool ShowOnEditor = false;

		void Start()
		{
#if UNITY_STANDALONE || UNITY_WEBGL
#if UNITY_EDITOR
			if (ShowOnEditor == false)
			{
				gameObject.SetActive(false);
			}
			return;
#endif
			gameObject.SetActive(false);
#endif
		}
	}
}