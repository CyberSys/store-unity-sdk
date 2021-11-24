using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Xsolla.Demo
{
	public class BattlePassPremiumBuyer : BaseBattlePassBuyer
	{
		[SerializeField] private BattlePassCatalogExtractor BattlePassCatalogExtractor;
		[SerializeField] private BattlePassUserPremiumStatusProvider PremiumStatusProvider;
		[SerializeField] private BattlePassPopupFactory BattlePassPopupFactory;

		private IEnumerable<CatalogItemModel> _battlePassItems;
		private bool _isPremiumUser;

		protected override Action<CatalogItemModel> OnSuccessPurchase
		{
			get
			{
				return _ =>
				{
					PremiumStatusProvider.CheckUserPremium(onPurchase: true);
					BattlePassPopupFactory.CreateBuyPremiumSuccessPopup();
				};
			}
		}

		protected override void OnAwake()
		{
			BattlePassCatalogExtractor.BattlePassItemsExtracted += OnBattlePassItemsExtracted;
			PremiumStatusProvider.UserPremiumDefined += isPremium => _isPremiumUser = isPremium;
		}

		public void OnBattlePassDescriptionArrived(BattlePassDescription battlePassDescription)
		{
			var battlePassName = battlePassDescription.Name;
			StartCoroutine(SetBuyDataOnBattlePassItemsArrival(battlePassName));
		}

		private IEnumerator SetBuyDataOnBattlePassItemsArrival(string battlePassName)
		{
			yield return new WaitWhile(() => _battlePassItems == null);

			foreach (var item in _battlePassItems)
			{
				if (item.Name == battlePassName || item.Sku == battlePassName)
				{
					base.ItemToBuy = item;
					base.PriceData = new PriceDataExtractor().ExtractPriceData(item);
					yield break;
				}
			}
			//else
			Debug.LogError(string.Format("Could not find corresponding item for Battle Pass name: '{0}'", battlePassName));
		}

		protected override bool IsShowDialogValid()
		{
			if (base.PriceData == null)
			{
				Debug.LogError("Price data is not provided");
				return false;
			}
			//else
			return true;
		}

		protected override void OnShowDialog<T>(T dialogController)
		{
			if (base.PriceData is RealPriceData)
			{
				var realPriceData = (RealPriceData)base.PriceData;
				var formattedPrice = PriceFormatter.FormatPrice(realPriceData.currency, realPriceData.price);
				dialogController.ShowPrice(formattedPrice);
			}
			else if (base.PriceData is VirtualPriceData)
			{
				var virtualPriceData = (VirtualPriceData)base.PriceData;
				var price = virtualPriceData.price;
				var userCurrencyValue = base.GetUserCurrencyValue(virtualPriceData.currencySku);
				dialogController.ShowPrice(virtualPriceData.currencyImageUrl, virtualPriceData.currencyName, price, userCurrencyValue);
			}
		}

		protected override bool IsBuyValid()
		{
			if (_isPremiumUser)
			{
				Debug.LogWarning("Attempt to buy premium for already premium user");
				return false;
			}
			//else
			return true;
		}

		protected override int GetBuyItemsCount()
		{
			return 1;
		}

		private void OnBattlePassItemsExtracted(IEnumerable<CatalogItemModel> battlePassItems)
		{
			_battlePassItems = battlePassItems;
		}
	}
}
