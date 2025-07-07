// MorseProcessor.cs
using UnityEngine;
using System.Collections.Generic;

public class MorseProcessor : MonoBehaviour
{
    private List<float> shortPressDurations = new List<float>(); // 短点の時間リスト
    private int calibrationCount = 10; // 10回まで計測
    private float shortDotBase = 0.15f; // デフォルトの短点長さ（調整前）
    private bool isCalibrating = true; // 初期測定中か判定
    private List<string> morseCode = new List<string>();
    private List<string> wordBuffer = new List<string>(); // 単語バッファ
    private MorseConverter morseConverter;
    private float lastInputTime;
    private const float charThreshold = 3f; // 3短点分の時間 → 文字の区切り
    private const float wordThreshold = 7f; // 7短点分の時間 → 単語の区切り

    private void Awake()
    {
        morseConverter = GetComponent<MorseConverter>();

        if (morseConverter == null)
        {
            Debug.LogError("MorseConverterが見つかりません！アタッチされているか確認してください。");
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

        // 文字区切り判定（3短点分）
        if (morseCode.Count > 0 && timeSinceLastInput > charThreshold * shortDotBase)
        {
            ProcessMorseCharacter();
        }

        // 単語区切り判定（7短点分）
        if (wordBuffer.Count > 0 && timeSinceLastInput > wordThreshold * shortDotBase)
        {
            Debug.Log($"単語完成: {string.Join("", wordBuffer)}");
            wordBuffer.Clear();
        }
    }

    private void HandleMorseInput(string mapName, string actionName, float duration)
    {
        if (mapName != "Morse") return;

        if (isCalibrating && shortPressDurations.Count < calibrationCount)
        {
            shortPressDurations.Add(duration);
            Debug.Log($"短点計測 {shortPressDurations.Count}/10: {duration} 秒");

            if (shortPressDurations.Count == calibrationCount)
            {
                shortDotBase = CalculateAverage(shortPressDurations);
                Debug.Log($"短点の平均時間が決定: {shortDotBase} 秒");
                isCalibrating = false;
            }
            return;
        }

        // 計測完了後、短点を基準に長点や間隔を決定
        string signal = duration < shortDotBase * 3 ? "E" : "T";
        morseCode.Add(signal);
        lastInputTime = Time.time;
        Debug.Log($"入力判定: {signal} ({duration}秒)");
    }

    private void ProcessMorseCharacter()
    {
        string morseString = string.Join("", morseCode);
        string convertedText = morseConverter.ConvertMorseToText(morseString);

        Debug.Log($"変換結果: {morseString} → {convertedText}");

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