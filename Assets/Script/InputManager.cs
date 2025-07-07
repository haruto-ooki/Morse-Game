// InputManager.cs
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    private PlayerInput playerInput;
    public static event System.Action<string, string, float> OnInputReceived;
    private float pressStartTime;

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();

        foreach (var actionMap in playerInput.actions.actionMaps)
        {
            foreach (var action in actionMap.actions)
            {
                string mapName = actionMap.name;
                string actionName = action.name;

                if (mapName == "Morse") // Morse マップ限定
                {
                    action.started += ctx => StartPress();
                    action.canceled += ctx => EndPress(mapName, actionName);
                }
            }
        }

        Debug.Log("InputManagerが正常にセットアップされました！");
    }

    private void StartPress()
    {
        pressStartTime = Time.time;
        Debug.Log("入力開始！");
    }

    private void EndPress(string mapName, string actionName)
    {
        float pressDuration = Time.time - pressStartTime;
        Debug.Log($"入力終了: {mapName}, {actionName}, {pressDuration}秒");
        OnInputReceived?.Invoke(mapName, actionName, pressDuration);
    }
}