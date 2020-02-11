using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System;
using System.Text;
using System.Collections;

public class Register : MonoBehaviour
{
    // Cached references
    public InputField emailInputField;
    public InputField passwordInputField;
    public InputField confirmPasswordInputField;
    public Button registerButton;
    public Text messageBoardText;
    public Player playerManager;

    string httpServer;

    private void Start()
    {
        playerManager = FindObjectOfType<Player>();
        httpServer = playerManager.GetHttpServer();
    }

    public void OnRegisterButtonClick()
    {
        StartCoroutine(RegisterNewUser());
    }

    public IEnumerator RegisterNewUser()
    {
        if (string.IsNullOrEmpty(emailInputField.text))
        {
            throw new NullReferenceException("Email can't be void");
        }
        else if (string.IsNullOrEmpty(passwordInputField.text))
        {
            throw new NullReferenceException("Password can't be void");
        }
        else if (passwordInputField.text != confirmPasswordInputField.text)
        {
            throw new Exception("Passwords don't match");
        }

        UnityWebRequest httpClient = new UnityWebRequest(httpServer + "/api/Account/Register", "POST");
        AspNetUserRegister newUser = new AspNetUserRegister();
        newUser.Email = emailInputField.text;
        newUser.Password = passwordInputField.text;
        newUser.ConfirmPassword = confirmPasswordInputField.text;

        string jsonData = JsonUtility.ToJson(newUser);
        byte[] sendData = Encoding.UTF8.GetBytes(jsonData);
        httpClient.uploadHandler = new UploadHandlerRaw(sendData);

        httpClient.SetRequestHeader("Content-Type", "application/json");

        httpClient.certificateHandler = new ByPassCertificate();

        yield return httpClient.SendWebRequest();

        if (httpClient.isNetworkError || httpClient.isHttpError)
        {
            throw new Exception("Register click: Network Error" + httpClient.error);
        }

        messageBoardText.text = "\n" + httpClient.responseCode;
        httpClient.Dispose();
    }

}
