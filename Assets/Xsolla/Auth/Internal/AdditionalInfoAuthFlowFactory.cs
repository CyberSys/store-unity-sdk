using System;
using Xsolla.Core;

namespace Xsolla.Auth
{
	internal static class AdditionalInfoAuthFlowFactory
	{
		public static IAdditionalInfoAuthFlow Create(Action onSuccess, Action<Error> onError, Action onCancel)
		{
#if UNITY_STANDALONE || UNITY_EDITOR
			return XsollaSettings.InAppBrowserEnabled
				? new StandaloneInAppBrowserAdditionalInfoAuthFlow(onSuccess, onError, onCancel)
				: new StandaloneSystemBrowserAdditionalInfoAuthFlow(onSuccess, onError);
#else
			return null;
#endif
		}
	}
}
