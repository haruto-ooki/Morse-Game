// MorseProcessor.cs
using UnityEngine;
using System.Collections.Generic;

public class MorseProcessor : MonoBehaviour
{
    private List<float> shortPressDurations = new List<float>(); // �Z�_�̎��ԃ��X�g
    private int calibrationCount = 10; // 10��܂Ōv��
    private float shortDotBase = 0.15f; // �f�t�H���g�̒Z�_�����i�����O�j
    private bool isCalibrating = true; // �������蒆������
    private List<string> morseCode = new List<string>();
    private List<string> wordBuffer = new List<string>(); // �P��o�b�t�@
    private MorseConverter morseConverter;
    private float lastInputTime;
    private const float charThreshold = 3f; // 3�Z�_���̎��� �� �����̋�؂�
    private const float wordThreshold = 7f; // 7�Z�_���̎��� �� �P��̋�؂�

    private void Awake()
    {
        morseConverter = GetComponent<MorseConverter>();

        if (morseConverter == null)
        {
            Debug.LogError("MorseConverter��������܂���I�A�^�b�`����Ă��邩�m�F���Ă��������B");
        }
    }

    private void OnEnable()
    {
        InputManager.OnInputReceived += HandleMorseInput;
    }

    private void OnDisable()
    {
        InputManager.OnInputReceived -= HandleMorseInput;
    }

    private void Update()
    {
        float timeSinceLastInput = Time.time - lastInputTime;

        // ������؂蔻��i3�Z�_���j
        if (morseCode.Count > 0 && timeSinceLastInput > charThreshold * shortDotBase)
        {
            ProcessMorseCharacter();
        }

        // �P���؂蔻��i7�Z�_���j
        if (wordBuffer.Count > 0 && timeSinceLastInput > wordThreshold * shortDotBase)
        {
            Debug.Log($"�P�ꊮ��: {string.Join("", wordBuffer)}");
            wordBuffer.Clear();
        }
    }

    private void HandleMorseInput(string mapName, string actionName, float duration)
    {
        if (mapName != "Morse") return;

        if (isCalibrating && shortPressDurations.Count < calibrationCount)
        {
            shortPressDurations.Add(duration);
            Debug.Log($"�Z�_�v�� {shortPressDurations.Count}/10: {duration} �b");

            if (shortPressDurations.Count == calibrationCount)
            {
                shortDotBase = CalculateAverage(shortPressDurations);
                Debug.Log($"�Z�_�̕��ώ��Ԃ�����: {shortDotBase} �b");
                isCalibrating = false;
            }
            return;
        }

        // �v��������A�Z�_����ɒ��_��Ԋu������
        string signal = duration < shortDotBase * 3 ? "E" : "T";
        morseCode.Add(signal);
        lastInputTime = Time.time;
        Debug.Log($"���͔���: {signal} ({duration}�b)");
    }

    private void ProcessMorseCharacter()
    {
        string morseString = string.Join("", morseCode);
        string convertedText = morseConverter.ConvertMorseToText(morseString);

        Debug.Log($"�ϊ�����: {morseString} �� {convertedText}");

        if (convertedText != "?")
        {
            wordBuffer.Add(convertedText);
        }

        morseCode.Clear();
    }

    private float CalculateAverage(List<float> durations)
    {
        float sum = 0;
        foreach (float time in durations) sum += time;
        return sum / durations.Count;
    }
}