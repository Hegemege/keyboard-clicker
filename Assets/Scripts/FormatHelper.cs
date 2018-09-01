using System.Collections;
using System.Collections.Generic;
using Keiwando;
using UnityEngine;

public struct PostfixedNumber
{
    public BigInteger Value; // Actual value for calculations
    public float PostfixValue; // Value that goes along with the postfix
    public string Postfix; // ISO postfix
}

public static class FormatHelper
{
    public static readonly Dictionary<int, string> Postfix = new Dictionary<int, string>()
    {
        {0, ""},
        {3, "k"},
        {6, "M"},
        {9, "G"},
        {12, "T"},
        {15, "P"},
        {18, "E"},
        {21, "Z"},
        {24, "Y"}
    };

    public static Dictionary<int, string> ExponentPresentation;
    private const int Exponents = 999;

    static FormatHelper()
    {
        ExponentPresentation = new Dictionary<int, string>();
        for (int i = 0; i < Exponents; i++)
        {
            AddExponentBase();
        }
    }

    public static PostfixedNumber GetPostfix(BigInteger value)
    {
        var lastUnderThousand = value;
        var tenBaseExponent = 0;

        while (lastUnderThousand > 1000 * 1000)
        {
            lastUnderThousand /= 1000;
            tenBaseExponent += 3;
        }

        // Get the remaining as a float
        float baseValue;
        if (lastUnderThousand > 1000)
        {
            baseValue = BigInteger.ToInt32(lastUnderThousand) / 1000f;
            tenBaseExponent += 3;
        }
        else
        {
            baseValue = BigInteger.ToInt32(lastUnderThousand);
        }

        // Get the postfix. If the exponent is not in ISO postfixes, use the E-base. If too large, add more to the E-base dictionary
        string postfix;
        if (tenBaseExponent <= 24)
        {
            postfix = Postfix[tenBaseExponent];
        }
        else
        {
            // Get the value as a x.y (e.g. 1.2, 4.6), so divide until it's less than 10f
            while (baseValue > 10f)
            {
                baseValue /= 10f;
                tenBaseExponent += 1;
            }

            // Add a few more cached exponent presentations if we ran out of them
            if (tenBaseExponent >= ExponentPresentation.Count)
            {
                AddExponentBase();
                AddExponentBase();
                AddExponentBase();
            }

            postfix = ExponentPresentation[tenBaseExponent];
        }

        return new PostfixedNumber() { Value = value, PostfixValue = baseValue, Postfix = postfix };
    }

    private static void AddExponentBase()
    {
        var i = ExponentPresentation.Count;
        ExponentPresentation[i] = string.Format("E+{0}", i);
    }
}
