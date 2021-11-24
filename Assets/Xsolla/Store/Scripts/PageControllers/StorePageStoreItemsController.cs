using System.Collections.Generic;
using System.Linq;

namespace Xsolla.Demo
{
	public class StorePageStoreItemsController : BaseStorePageStoreItemsController
	{
		protected override bool IsShowContent
		{
			get
			{
				return UserCatalog.Instance.HasVirtualItems || UserCatalog.Instance.HasBundles;
			}
		}

		protected override List<ItemModel> GetItemsByGroup(string groupName)
		{
			var isGroupAll = groupName.Equals(GROUP_ALL);

			var items = UserCatalog.Instance.AllItems.Where(item =>
			{
				if (item.IsVirtualCurrency())
					return false;
				else
				{
					var itemGroups = SdkCatalogLogic.Instance.GetCatalogGroupsByItem(item);

					if (itemGroups.Contains(BattlePassConstants.BATTLEPASS_GROUP))
						return false; //This is battlepass exclusive item or battlepass util

					if (base.CheckHideInAttribute(item, HideInFlag.Store))
						return false; //This item must be hidden by attribute

					return isGroupAll || itemGroups.Contains(groupName);
				}
			}).ToList();

			return items.Cast<ItemModel>().ToList();
		}

		protected override List<string> GetGroups()
		{
			var items = UserCatalog.Instance.AllItems;
			var groups = new List<string>();

			items.ForEach(i => groups.AddRange(SdkCatalogLogic.Instance.GetCatalogGroupsByItem(i)));
			groups = groups.Distinct().ToList();
			groups.Remove(GROUP_ALL);

			return groups;
		}
	}
}
