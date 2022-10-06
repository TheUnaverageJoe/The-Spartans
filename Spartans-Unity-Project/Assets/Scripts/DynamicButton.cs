using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Spartans;

[RequireComponent(typeof(Button))]
public class DynamicButton : MonoBehaviour
{
    private Button _thisButton;
    // Start is called before the first frame update
    void Start()
    {
        _thisButton = GetComponent<Button>();
        _thisButton.onClick.AddListener(GameManagerDisconnect);
    }

    private void GameManagerDisconnect(){
        if(GameManager.Instance != null)
            GameManager.Instance.StopConnection();
        else
            print("Game Manager has no Instance");
    }
}
