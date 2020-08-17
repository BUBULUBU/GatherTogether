using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Launcher2 : MonoBehaviour
{
    public Login login;
    // Start is called before the first frame update
    void Start()
    {
        login.Connect();
    }

}
