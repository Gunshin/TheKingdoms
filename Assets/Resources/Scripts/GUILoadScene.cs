using UnityEngine;
using System.Collections;

public class GUILoadScene : MonoBehaviour {

    public void LoadScene(string name_)
    {
        Application.LoadLevel(name_);
    }

}
