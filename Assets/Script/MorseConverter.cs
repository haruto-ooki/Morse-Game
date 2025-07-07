// MorseConverter.cs
using System.Collections.Generic;
using UnityEngine;

public class MorseConverter : MonoBehaviour
{
    private Dictionary<string, string> morseDictionary = new Dictionary<string, string>()
    {
        { "EEE", "S" }, { "TTT", "O" }, { "TETE", "C" },
        { "ET", "A" }, { "TEEE", "B" }, { "TEE", "D" },
        { "E", "E" }, { "EETE", "F" }, { "TTE", "G" }, { "EEEE", "H" },
        { "EE", "I" }, { "ETTTT", "J" }, { "TET", "K" }, { "ETEE", "L" },
        { "TT", "M" }, { "TE", "N" }, { "ETTE", "P" }, { "TTET", "Q" },
        { "ETE", "R" }, { "T", "T" }, { "EET", "U" }, { "EEET", "V" },
        { "ETT", "W" }, { "TEET", "X" }, { "TETTT", "Y" }, { "TTEE", "Z" }
    };

    public string ConvertMorseToText(string morseCode)
    {
        if (morseDictionary.ContainsKey(morseCode))
        {
            return morseDictionary[morseCode];
        }
        return "?";
    }
}