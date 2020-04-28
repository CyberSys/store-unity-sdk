﻿using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Xsolla.Core;
using Xsolla.Login;

public class AuthController : MonoBehaviour
{
    [SerializeField] private GameObject resetPassword_Panel;
    [SerializeField] private GameObject loginSignUp_Panel;
    [SerializeField] private GameObject signUp_Panel;
    [SerializeField] private GameObject login_Panel;
    [SerializeField] private GameObject popUp_Controller;

    [SerializeField] private Button openChangePassw_Btn;
    [SerializeField] private Button openSignUp_Btn;
    [SerializeField] private Button openLogin_Btn;
    [SerializeField] private Button closeChangePassword_Btn;

    private List<GameObject> opened_Pages = new List<GameObject>();

    private void Awake()
    {
        PagesController();
        PagesEvents();
    }

    private void PagesEvents()
    {
        signUp_Panel.GetComponent<ISignUp>().OnSuccessfulSignUp = () => OpenPopUp("Account successfully created", string.Format("Please check {0} and confirm your email", signUp_Panel.GetComponent<ISignUp>().SignUpEmail));
        signUp_Panel.GetComponent<ISignUp>().OnUnsuccessfulSignUp = OnError;
        resetPassword_Panel.GetComponent<IResetPassword>().OnSuccessfulResetPassword = () => {
			OpenPopUp("Password successfully reset", "Please check your email and change the password"); 
        };
        resetPassword_Panel.GetComponent<IResetPassword>().OnUnsuccessfulResetPassword = OnError;
        login_Panel.GetComponent<ILogin>().OnUnsuccessfulLogin = OnError;
        login_Panel.GetComponent<ILogin>().OnSuccessfulLogin = () => OpenPopUp("You have successfully logged in", PopUpWindows.Success);
    }
    
    private void PagesController()
    {
        OpenPage(loginSignUp_Panel);
        openLogin_Btn.GetComponent<IPanelVisualElement>().Select();
        OpenPage(login_Panel);
        
        var returnToTheMain = new UnityAction(() => ReturnToTheLogIn());
        popUp_Controller.GetComponent<IPopUpController>().OnReturnToLogin = returnToTheMain;
        closeChangePassword_Btn.onClick.AddListener(new UnityAction(() => ReturnToTheLogIn()));
        openChangePassw_Btn.onClick.AddListener(() => { CloseAll(); OpenPage(resetPassword_Panel); });
        openSignUp_Btn.onClick.AddListener(() =>
        {
            CloseAll();
            OpenPage(signUp_Panel);
            OpenPage(loginSignUp_Panel);
            openSignUp_Btn.GetComponent<IPanelVisualElement>().Select();
            openLogin_Btn.GetComponent<IPanelVisualElement>().Deselect();
        });
        openLogin_Btn.onClick.AddListener(() =>
        {
            CloseAll();
            OpenPage(login_Panel);
            OpenPage(loginSignUp_Panel);
            openLogin_Btn.GetComponent<IPanelVisualElement>().Select();
            openSignUp_Btn.GetComponent<IPanelVisualElement>().Deselect();
        });
    }

	private void ReturnToTheLogIn()
	{
        CloseAll();
        OpenPage(loginSignUp_Panel);
        OpenPage(login_Panel);
        openLogin_Btn.GetComponent<IPanelVisualElement>().Select();
        openSignUp_Btn.GetComponent<IPanelVisualElement>().Deselect();
    }

    private void OpenPage(GameObject page)
    {
        page.GetComponent<IPage>().Open();
        opened_Pages.Add(page);
    }
    private void OpenSaved()
    {
        foreach (var page in opened_Pages)
        {
            page.GetComponent<IPage>().Open();
        }
    }

    private void CloseAndSave()
    {
        foreach (var page in opened_Pages)
        {
            page.GetComponent<IPage>().Close();
        }
    }

    private void CloseAll()
    {
        foreach (var page in opened_Pages)
        {
            page.GetComponent<IPage>().Close();
        }
        opened_Pages = new List<GameObject>();
    }

    private void Start()
    {
        if (XsollaSettings.LoginId == string.Empty)
        {
            OpenPopUp("Xsolla Login settings not completed", PopUpWindows.Warning);
            Debug.Log("Please register Xsolla Publisher Account, and fill the Login ID form. For more details read documentation.\nhttps://github.com/xsolla/login-unity-sdk/blob/master/README.md");
        }
        else if ((XsollaLogin.Instance.Token != null) && !XsollaLogin.Instance.Token.IsExpired())
            Debug.Log($"Your token {XsollaLogin.Instance.Token} is active");
    }

    private void OpenPopUp(string message, PopUpWindows popUp)
    {
        CloseAndSave();
        popUp_Controller.GetComponent<IPopUpController>().OnClosePopUp = OpenSaved;
        popUp_Controller.GetComponent<IPopUpController>().ShowPopUp(message, popUp);
        Debug.Log(message);
    }
    private void OpenPopUp(string header, string message)
    {
        CloseAndSave();
        popUp_Controller.GetComponent<IPopUpController>().OnClosePopUp = new UnityAction(ReturnToTheLogIn);
        popUp_Controller.GetComponent<IPopUpController>().ShowPopUp(header, message);
        Debug.Log(message);
    }
    private void OnError(Error error)
    {
        switch (error.ErrorType)
        {
            case ErrorType.UsernameIsTaken:
                OpenPopUp("Username Is Taken.", PopUpWindows.Error);
                break;
            case ErrorType.EmailIsTaken:
                OpenPopUp("Email Is Taken", PopUpWindows.Error);
                break;
            case ErrorType.InvalidLoginOrPassword:
                OpenPopUp("Wrong username or password", PopUpWindows.Error);
                break;
            case ErrorType.InvalidToken:
                OpenPopUp("Invalid Token", PopUpWindows.Error);
                break;
            case ErrorType.NetworkError:
                OpenPopUp($"Network Error: {error.errorMessage}", PopUpWindows.Error);
                break;
			default:
				string errorMessage =
					!string.IsNullOrEmpty(error.errorMessage) ? error.errorMessage :
					!string.IsNullOrEmpty(error.errorCode) ? error.errorCode :
					!string.IsNullOrEmpty(error.statusCode) ? error.statusCode :
					"Unknown error";
				OpenPopUp(errorMessage, PopUpWindows.Error);
				break;
        }
    }
}
