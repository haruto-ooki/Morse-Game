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

                if (mapName == "Morse") // Morse �}�b�v����
                {
                    action.started += ctx => StartPress();
                    action.canceled += ctx => EndPress(mapName, actionName);
                }
            }
        }

        Debug.Log("InputManager������ɃZ�b�g�A�b�v����܂����I");
    }

    private void StartPress()
    {
        pressStartTime = Time.time;
        Debug.Log("���͊J�n�I");
    }

    private void EndPress(string mapName, string actionName)
    {
        float pressDuration = Time.time - pressStartTime;
        Debug.Log($"���͏I��: {mapName}, {actionName}, {pressDuration}�b");
        OnInputReceived?.Invoke(mapName, actionName, pressDuration);
    }
}