using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PrimitiveExtensions
{
    public static float RoundDecimalPlaces(this float value, int decimalPlaces)
    {
        float multiplier = Mathf.Pow(10f, decimalPlaces);
        return Mathf.Round(value * multiplier) / multiplier;
    }
}
