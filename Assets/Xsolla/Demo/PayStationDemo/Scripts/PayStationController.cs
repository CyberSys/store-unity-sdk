﻿using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Xsolla.Core;
using Xsolla.Core.Popup;
using Xsolla.Login;
using Xsolla.PayStation;
using Xsolla.Store;

public class PayStationController : MonoBehaviour
{
	// SKU of crystals virtual currency
	const string VirtualCurrencyCrystal = "crystal";
	// SKU of virtual currency package that contains 100 crystals
	const string CrystalPack = "crystal_pack_1";

	[SerializeField] SimpleTextButton buyCrystalsButton;
	[SerializeField] Text purchaseStatusText;
	
	[SerializeField] GameObject loadingScreen;
	[SerializeField] GameObject purchaseStatusWidget;
	
	[SerializeField] VirtualCurrencyContainer virtualCurrencyBalanceWidget;

	void Awake()
	{
		// PayStation demo setup
		Init();
	}

	void Init()
	{		
		// Obtain PayStation token to query store API
		GetToken(token =>
		{
			XsollaStore.Instance.Token = new Xsolla.Core.Token(token.token, true);
			
			UpdateVirtualCurrencies();
		});

		// Demo UI setup
		AddListeners();
	}

	void GetToken(Action<TokenEntity> onComplete)
	{
		XsollaPayStation.Instance.RequestToken(onComplete, ShowError);
	}

	void UpdateVirtualCurrencies()
	{
		XsollaStore.Instance.GetVirtualCurrencyList(
			XsollaSettings.StoreProjectId,
			currencies =>
			{
				// filter virtual currencies to display only crystals
				var filteredCurrencies = new VirtualCurrencyItems
				{
					items = new[] {currencies.items.ToList().SingleOrDefault(item => item.sku == VirtualCurrencyCrystal)}
				};

				virtualCurrencyBalanceWidget.SetCurrencies(filteredCurrencies.items.ToList());

				UpdateVirtualCurrenciesBalance(() =>
				{
					loadingScreen.SetActive(false);
				});
			}, ShowError);
	}

	void UpdateVirtualCurrenciesBalance(Action onComplete = null)
	{
		UserInventory.Instance.UpdateVirtualCurrencyBalance(balance =>
		{
			virtualCurrencyBalanceWidget.SetCurrenciesBalance(balance);
			onComplete?.Invoke();
		}, ShowError);
		// XsollaStore.Instance.GetVirtualCurrencyBalance(
		// 	XsollaSettings.StoreProjectId,
		// 	(balance) =>
		// 	{
		// 		virtualCurrencyBalanceWidget.SetCurrenciesBalance(balance);
		// 		onComplete?.Invoke();
		// 	},
		// 	ShowError);
	}

	void AddListeners()
	{
		buyCrystalsButton.onClick = () =>
		{
			// Launch purchase process
			XsollaStore.Instance.ItemPurchase(XsollaSettings.StoreProjectId, CrystalPack, data =>
			{
				XsollaStore.Instance.OpenPurchaseUi(data);
				ProcessOrder(data.order_id, () =>
				{
					UpdateVirtualCurrenciesBalance(() =>
					{
						// Hide widget that displays current order status
						purchaseStatusWidget.SetActive(false);
						// Show 'buy' button
						buyCrystalsButton.gameObject.SetActive(true);
						// Show VC balance
						virtualCurrencyBalanceWidget.gameObject.SetActive(true);
					});
				});
			}, ShowError);
		};
	}
	
	void ProcessOrder(int orderId, Action onOrderPaid = null)
	{
		// Begin order status polling
		StartCoroutine(CheckOrderStatus(orderId, onOrderPaid));
		
		// Activate widget that displays current order status
		UpdateOrderStatusDisplayText(OrderStatusType.New);
		purchaseStatusWidget.SetActive(true);
		// Hide 'buy' button
		buyCrystalsButton.gameObject.SetActive(false);
		// Hide VC balance
		virtualCurrencyBalanceWidget.gameObject.SetActive(false);
	}

	IEnumerator CheckOrderStatus(int orderId, Action onOrderPaid = null)
	{
		yield return new WaitForSeconds(3.0f);
		
		XsollaStore.Instance.CheckOrderStatus(XsollaSettings.StoreProjectId, orderId,status =>
		{
			UpdateOrderStatusDisplayText(status.Status);
			
			if ((status.Status != OrderStatusType.Paid) && (status.Status != OrderStatusType.Done))
			{
				print(string.Format("Waiting for order {0} to be processed...", orderId));
				StartCoroutine(CheckOrderStatus(orderId, onOrderPaid));
			}
			else
			{
				print(string.Format("Order {0} was successfully processed!", orderId));
				PopupFactory.Instance.CreateSuccess();
				onOrderPaid?.Invoke();
			}
		}, ShowError);
	}
	
	void UpdateOrderStatusDisplayText(OrderStatusType status)
	{
		purchaseStatusText.text = string.Format("PURCHASE STATUS: {0}", status.ToString().ToUpper());
	}

	public void ShowError(Error error)
	{
		print(error);
		PopupFactory.Instance.CreateError().SetMessage(error.ToString());
	}
}