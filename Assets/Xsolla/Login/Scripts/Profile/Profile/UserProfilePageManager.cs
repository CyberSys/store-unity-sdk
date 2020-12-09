﻿using System;
using System.Collections;
using System.Text.RegularExpressions;
using UnityEngine;
using Xsolla.Core;
using Xsolla.Core.Popup;
using Xsolla.Login;

namespace Xsolla.Demo
{
	public class UserProfilePageManager : MonoBehaviour
	{
		public UserProfileEntryUI[] UserProfileEntries;

		private bool? _isProfileUpdated;
		private Action<Error> _commonErrorCallback;
		private  Func<bool> _commonWaitCondition;
		private bool _isUpdateInProgress;

		private const int POPUP_VALUE_LIMIT = 20;

		private void Awake()
		{
			UserProfileEntryUI.UserEntryEdited += OnUserEntryEdited;

			_commonErrorCallback = error =>
			{
				_isProfileUpdated = false;
				Debug.LogError(error.errorMessage);
				StoreDemoPopup.ShowError(error);
			};

			_commonWaitCondition = () => _isProfileUpdated == null;
		}

		private void OnDestroy()
		{
			UserProfileEntryUI.UserEntryEdited -= OnUserEntryEdited;
		}

		private IEnumerator Start()
		{
			UserInfo userInfo = null;
			var token = DemoController.Instance.LoginDemo.Token;

			DemoController.Instance.LoginDemo.GetUserInfo(token,
				onSuccess: info =>
				{
					userInfo = info;
					_isProfileUpdated = true;
				},
				onError: _commonErrorCallback);

			ShowWaiting(waitWhile: _commonWaitCondition);
			yield return new WaitWhile(_commonWaitCondition);

			if (userInfo == null)
			{
				UserProfileEntryUI.UserEntryEdited -= OnUserEntryEdited;
				yield break;
			}
			else
				InitializeEntries(userInfo);
		}

		private void InitializeEntries(UserInfo userInfo)
		{
			for (int index = 0; index < UserProfileEntries.Length; index++)
			{
				var entryType = (UserProfileEntryType)index;
				string entryValue = null;

				switch (entryType)
				{
					case UserProfileEntryType.Email:
						entryValue = userInfo.email;
						break;
					case UserProfileEntryType.Password:
						entryValue = "**********";
						break;
					case UserProfileEntryType.Username:
						entryValue = userInfo.username;
						break;
					case UserProfileEntryType.Nickname:
						entryValue = userInfo.nickname;
						break;
					case UserProfileEntryType.PhoneNumber:
						entryValue = userInfo.phone;
						break;
					case UserProfileEntryType.FirstName:
						entryValue = userInfo.first_name;
						break;
					case UserProfileEntryType.LastName:
						entryValue = userInfo.last_name;
						break;
					case UserProfileEntryType.DateOfBirth:
						entryValue = userInfo.birthday;
						break;
					case UserProfileEntryType.Gender:
						entryValue = userInfo.gender;
						break;
				}

				UserProfileEntries[index].InitializeEntry(entryType, entryValue);
			}
		}

		private void OnUserEntryEdited(UserProfileEntryUI sender, UserProfileEntryType entryType, string oldValue, string newValue)
		{
			if (newValue == null)
			{
				Debug.LogError($"New value of entryType {entryType} is null. Can not update");
				sender.InitializeEntry(entryType, oldValue);
				return;
			}

			if (_isUpdateInProgress)
			{
				Debug.LogWarning("Can not update new entry while another update is in progress");
				sender.InitializeEntry(entryType, oldValue);
				return;
			}

			_isUpdateInProgress = true;
			_isProfileUpdated = null;
			ShowWaiting(waitWhile: _commonWaitCondition);
			StartCoroutine(UnsetIsInProgressOnCompletion());
			StartCoroutine(RevertValueOnError(sender, entryType, oldValue));

			switch (entryType)
			{
				case UserProfileEntryType.DateOfBirth:
				case UserProfileEntryType.FirstName:
				case UserProfileEntryType.Gender:
				case UserProfileEntryType.LastName:
				case UserProfileEntryType.Nickname:
					UpdateCommonEntries(entryType, newValue);
					break;
				case UserProfileEntryType.PhoneNumber:
					if (!string.IsNullOrEmpty(newValue))
						UpdateUserPhoneNumber(newValue);
					else
						StartCoroutine(DeleteUserPhoneNumber());
					break;
				default:
					Debug.LogWarning($"Update of {entryType} is not supported");
					_isProfileUpdated = false;
					return;
			}
		}

		private IEnumerator UnsetIsInProgressOnCompletion()
		{
			yield return new WaitWhile(_commonWaitCondition);

			_isUpdateInProgress = false;
		}

		private IEnumerator RevertValueOnError(UserProfileEntryUI sender, UserProfileEntryType entryType, string oldValue)
		{
			yield return new WaitWhile(_commonWaitCondition);

			if(_isProfileUpdated == false)
				sender.InitializeEntry(entryType, oldValue);
		}

		private void UpdateCommonEntries(UserProfileEntryType entryType, string newValue)
		{
			var infoUpdatePack = new UserInfoUpdate();
			var isValueValid = newValue.Length <= 255;

			switch (entryType)
			{
				case UserProfileEntryType.DateOfBirth:
					if (DateTime.TryParse(newValue, out DateTime birthday))
						infoUpdatePack.birthday = newValue;
					else
						isValueValid = false;
					break;
				case UserProfileEntryType.FirstName:
					infoUpdatePack.first_name = newValue;
					break;
				case UserProfileEntryType.Gender:
					if (newValue == UserProfileGender.MALE_SHORT || newValue == UserProfileGender.FEMALE_SHORT)
						infoUpdatePack.gender = newValue;
					else
						isValueValid = false;
					break;
				case UserProfileEntryType.LastName:
					infoUpdatePack.last_name = newValue;
					break;
				case UserProfileEntryType.Nickname:
					StartCoroutine(UpdateUpperRightCornerInfoOnCompletion());
					infoUpdatePack.nickname = newValue;
					break;
			}

			if (!isValueValid)
			{
				ShowInvalidValue(entryType, newValue);
				_isProfileUpdated = false;
				return;
			}

		
			var token = DemoController.Instance.LoginDemo.Token;
			DemoController.Instance.LoginDemo.UpdateUserInfo(token, infoUpdatePack,
				onSuccess: newInfo =>
				{
					InitializeEntries(newInfo);
					_isProfileUpdated = true;
				},
				onError: _commonErrorCallback);
		}

		private IEnumerator UpdateUpperRightCornerInfoOnCompletion()
		{
			yield return new WaitWhile(_commonWaitCondition);
			FindObjectOfType<UserInfoDrawer>()?.Refresh();
		}

		private void UpdateUserPhoneNumber(string newValue)
		{
			var isValid = Regex.IsMatch(newValue, @"^\+(\d){5,25}$");

			if (!isValid)
			{
				ShowInvalidValue(UserProfileEntryType.PhoneNumber, newValue);
				_isProfileUpdated = false;
				return;
			}

			var token = DemoController.Instance.LoginDemo.Token;

			DemoController.Instance.LoginDemo.GetUserInfo(token,
				onSuccess: info => info.phone = newValue);

			DemoController.Instance.LoginDemo.ChangeUserPhoneNumber(token, newValue,
				onSuccess: () => _isProfileUpdated = true,
				onError: _commonErrorCallback);
		}

		private IEnumerator DeleteUserPhoneNumber()
		{
			var token = DemoController.Instance.LoginDemo.Token;
			UserInfo userInfo = null;

			DemoController.Instance.LoginDemo.GetUserInfo(token,
				onSuccess: info =>
				{
					userInfo = info;
				},
				onError: _commonErrorCallback);

			yield return new WaitWhile(() => _isProfileUpdated == null && userInfo == null);

			if (userInfo != null)
			{
				var oldPhoneNumber = userInfo.phone;
				userInfo.phone = null;

				DemoController.Instance.LoginDemo.DeleteUserPhoneNumber(token, oldPhoneNumber,
					onSuccess: () => _isProfileUpdated = true,
					onError: _commonErrorCallback);
			}
		}

		private void ShowWaiting(Func<bool> waitWhile)
		{
			PopupFactory.Instance.CreateWaiting().SetCloseCondition(() => !waitWhile.Invoke());
		}

		private void ShowInvalidValue(UserProfileEntryType entryType, string value)
		{
			if (value.Length > POPUP_VALUE_LIMIT)
				value = $"{value.Substring(0, POPUP_VALUE_LIMIT)}...";

			var errorMessage = $"Incorrect new value for {entryType}: '{value}'";
			var error = new Error(ErrorType.InvalidData, errorMessage: errorMessage);
			StoreDemoPopup.ShowError(error);
		}
	}
}
