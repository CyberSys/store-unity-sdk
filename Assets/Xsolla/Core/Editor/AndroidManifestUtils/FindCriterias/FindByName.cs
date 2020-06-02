﻿using System;
using System.Xml;

namespace Xsolla.Core
{
	public class FindByName : IFindCriteria<XmlNode>
	{
		private readonly BaseManifestNode node;

		public FindByName(BaseManifestNode node)
		{
			this.node = node;
		}

		public bool MatchesCriteria(XmlNode xmlNode)
		{
			if (xmlNode != null)
			{
				if (xmlNode.Name.Equals(node.Tag, StringComparison.InvariantCultureIgnoreCase))
				{
					if (xmlNode.Attributes != null)
					{
						var attributeNode = xmlNode.Attributes.GetNamedItem(AndroidManifestConstants.NameAttribute);
						return attributeNode != null
							&& node.Attributes.ContainsKey(AndroidManifestConstants.NameAttribute)
							&& attributeNode.Value.Equals(node.Attributes[AndroidManifestConstants.NameAttribute]);
					}
				}
			}

			return false;
		}
	}
}