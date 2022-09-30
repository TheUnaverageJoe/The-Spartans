using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Pregame : MonoBehaviour
{
    private static int GUI_BUTTON_WIDTH = 465;
    private static int GUI_BUTTON_HEIGHT = 400;
    // private Rect GUI_AREA = new Rect(Screen.width/2 - GUI_BUTTON_WIDTH/2,
    //                                  Screen.height/2-50,
    //                                  GUI_BUTTON_WIDTH,
    //                                  GUI_BUTTON_HEIGHT);
    private Rect GUI_AREA = new Rect(0,0, GUI_BUTTON_WIDTH, GUI_BUTTON_HEIGHT);
    void OnGUI()
        {
            GUIStyle customButton = new GUIStyle("button");
            customButton.fontSize = 40;
            GUILayout.BeginArea(GUI_AREA);
            GUILayout.BeginHorizontal("box");
            if (!NetworkManager.Singleton.IsClient && !NetworkManager.Singleton.IsServer){
                StartButtons(customButton);
            }else{
                StatusLabels(customButton);
            }
            GUILayout.EndHorizontal();
            GUILayout.EndArea();
        }

        static void StartButtons(GUIStyle customButton)
        {

            if (GUILayout.Button("Host", customButton, GUILayout.Width(150), GUILayout.Height(100))) NetworkManager.Singleton.StartHost();
            if (GUILayout.Button("Client", customButton, GUILayout.Width(150), GUILayout.Height(100))) NetworkManager.Singleton.StartClient();
            if (GUILayout.Button("Server", customButton, GUILayout.Width(150), GUILayout.Height(100))) NetworkManager.Singleton.StartServer();
        }

        static void StatusLabels(GUIStyle customButton){
            customButton.fontSize = 24;
            var mode = NetworkManager.Singleton.IsHost ?
                "Host" : NetworkManager.Singleton.IsServer ? "Server" : "Client";

            GUILayout.Label("Transport: " +
                NetworkManager.Singleton.NetworkConfig.NetworkTransport.GetType().Name, customButton);
            GUILayout.Label("Mode: " + mode, customButton);
        }
}
