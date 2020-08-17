using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using Photon.Pun;

public class Login : MonoBehaviourPunCallbacks
{
    public InputField usernameField;
    public InputField passwordField;

    public Button loginBtn;

    public Manager manager;

    public void CallLogin()
    {
        Connect();
        //StartCoroutine(LoginUser());
    }

    IEnumerator LoginUser()
    {
        WWWForm form = new WWWForm();
        form.AddField("username", usernameField.text);
        form.AddField("password", passwordField.text);
        UnityWebRequest www = UnityWebRequest.Post("http://85.214.107.230/gathertogether/login.php", form);

        yield return www.SendWebRequest();

        if (www.downloadHandler.text == "0")
        {
            Connect();
        }
        else
        {
            Debug.Log("User login failed. Error #" + www.downloadHandler.text);
        }
    }

    public void VerifyInputs()
    {
        loginBtn.interactable = (usernameField.text.Length >= 4 && passwordField.text.Length >= 4);
    }

    public override void OnConnectedToMaster()
    {
        Join();

        base.OnConnectedToMaster();
    }

    public override void OnJoinedRoom()
    {
        StartGame();

        base.OnJoinedRoom();
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Create();

        base.OnJoinRandomFailed(returnCode, message);
    }

    public void Connect()
    {
        PhotonNetwork.NickName = "admin";
        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.GameVersion = "0.1";
        //PhotonNetwork.ConnectToMaster("85.214.107.230", 5055, "0.1");
        PhotonNetwork.ConnectUsingSettings();
    }

    public void Join()
    {
        PhotonNetwork.JoinRandomRoom();
        Debug.Log("Trying to join random room..");
    }

    public void Create()
    {
        PhotonNetwork.CreateRoom("");
        Debug.Log("Room created..");
        //StartGame();
    }

    public void StartGame()
    {
        PhotonNetwork.LoadLevel(0);
        Debug.Log("Level loaded..");
    }
}
