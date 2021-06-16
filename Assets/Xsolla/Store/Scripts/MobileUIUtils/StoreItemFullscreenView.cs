using UnityEngine;

namespace Xsolla.Demo
{
	public class StoreItemFullscreenView : MonoBehaviour
    {
		[SerializeField] private GameObject[] RegularUI = default;
		[SerializeField] private GameObject FullscreenUI = default;

		private static StoreItemFullscreenView _instance;

		public static void ShowItem(CatalogItemModel itemModel)
		{
			if (_instance == null)
			{
				Debug.LogError("Instance not set");
				return;
			}

			_instance.ShowUI(true);
			_instance.FullscreenUI.GetComponent<StoreItemUI>().Initialize(itemModel, DemoController.Instance.StoreDemo);
		}

		private void Awake()
		{
			_instance = this;

			var closeFullscreenButton = FullscreenUI.GetComponent<ButtonProvider>()?.Button;
			if (closeFullscreenButton != null)
				closeFullscreenButton.onClick += () => ShowUI(false);
		}

		private void OnDestroy()
		{
			if (_instance == this)
				_instance = null;
		}

		private void ShowUI(bool fullscreen)
		{
			foreach (var item in RegularUI)
				item.SetActive(!fullscreen);

			FullscreenUI.SetActive(fullscreen);
		}
	}
}
