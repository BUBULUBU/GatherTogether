using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class Registration : MonoBehaviour
{
    public InputField usernameField;
    public InputField passwordField;

   // public InputField nextField;
    //InputField myField;

    public Button registerBtn;

    private void Start()
    {
        /*if (nextField == null)
        {
            Destroy(this);
            return;
        }
        myField = GetComponent<InputField>();*/
    }

    private void Update()
    {
        /*if(myField.isFocused && Input.GetKey(KeyCode.Tab))
        {
            nextField.ActivateInputField();
        }*/
    }

    public void CallRegister()
    {
        StartCoroutine(Register());
    }

    IEnumerator Register()
    {
        WWWForm form = new WWWForm();
        form.AddField("username", usernameField.text);
        form.AddField("password", passwordField.text);
        UnityWebRequest www = UnityWebRequest.Post("http://85.214.107.230/gathertogether/register.php", form);

        yield return www.SendWebRequest();

        if (www.downloadHandler.text == "0")
        {
            Debug.Log("User successfully created.");
        }
        else
        {
            Debug.Log("User creation failed. Error #" + www.downloadHandler.text);
        }
    }
}
