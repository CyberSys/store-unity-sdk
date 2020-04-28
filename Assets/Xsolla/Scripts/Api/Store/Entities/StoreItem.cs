﻿using System;
using Newtonsoft.Json;
using Xsolla.Core;

namespace Xsolla.Store
{
	[Serializable]
	public class StoreItem
	{
		[Serializable]
		public class Group
		{
			public string external_id;
			public string name;
		}
		[Serializable]
		public class VirtualPrice
		{
			public string sku;
			public string name;
			public string type;
			public string description;
			public string image_url;
			public string amount;
			public string amount_without_discount;
			public bool is_default;
		}
		[Serializable]
		public class InventoryOptions
		{
			[Serializable]
			public class ConsumableOption
			{
				public int? usages_count;
			}
			public ConsumableOption consumable;
		}
		
		public string sku;
		public string name;
		public StoreItem.Group[] groups;
		public string[] attributes;
		public string type;
		public string description;
		public string image_url;
		public bool is_free;
		public Price price;
		public StoreItem.VirtualPrice[] virtual_prices;
		public StoreItem.InventoryOptions inventory_options;
		
		public StoreItem DeepClone()
		{
			string json = JsonConvert.SerializeObject(this);
			return JsonConvert.DeserializeObject<StoreItem>(json);
		}

		public bool IsConsumable()
		{
			return inventory_options.consumable != null;
		}
	}
}
