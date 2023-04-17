using System;

namespace Xsolla.UserAccount
{
	/// <summary>
	/// Public user info entity.
	/// </summary>
	/// <see href="https://go-xsolla-login.doc.srv.loc/login-api/users/get-public-user-profile"/>
	[Serializable]
	public class UserPublicInfo
	{
		/// <summary>
		/// The Xsolla Login user ID. You can find it in Publisher Account > your Login project > Users > Username/ID.
		/// </summary>
		public string user_id;
		/// <summary>
		/// Date of user registration in the RFC3339 format.
		/// </summary>
		public string registered;
		/// <summary>
		/// Date of the last user login in the RFC3339 format.
		/// </summary>
		public string last_login;
		/// <summary>
		/// URL of the user avatar.
		/// </summary>
		public string avatar;
		/// <summary>
		/// User nickname.
		/// </summary>
		public string nickname;
		/// <summary>
		/// User tag without "#" at the beginning. Can have no unique value and can be used in the Search users by nickname call.
		/// </summary>
		public string tag;
	}
}
