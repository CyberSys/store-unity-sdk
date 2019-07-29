﻿using System;

namespace Xsolla.Store
{
	[Serializable]
	public class StoreItem
	{
		public string sku;
		public Character character;
		public string name;
		public string[] attributes;
		public string[] variants;
		public string[] media_list;
		public string type;
		public Price[] prices;
		public string[] vc_prices;
		public string[] groups;
		public string long_description;
		public string description;
		public string image_url;
		public string promotion;
		public bool is_free;
		public int order;
		public string purchase_limit;
	}
}