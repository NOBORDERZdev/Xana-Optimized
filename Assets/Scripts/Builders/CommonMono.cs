using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SimpleJSON;
using UnityEngine.UI;
using System.Text.RegularExpressions;
using System.Linq;
using System.Text;
using System.IO;
using UnityEngine.Networking;
using System;
using System.Reflection;

/// <summary>
/// This monobehaviour is only required for running all the coroutines. Since we need a GameObject for that so it's instance object takes all the load.
/// </summary>
public class CommonMono : MonoBehaviour
{
    static CommonMono _instance;
    public static CommonMono instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = InstantiateNewObject();
            }
            else if (!_instance.gameObject.activeSelf)
            {
                _instance = InstantiateNewObject();
            }
            return _instance;
        }
    }
    static CommonMono InstantiateNewObject()
    {
        GameObject go = new GameObject();
        go.name = "CommonMono";
        go.AddComponent<CommonMono>();
        DontDestroyOnLoad(go);
        return go.GetComponent<CommonMono>();
    }

}

public enum Flag { NotSet, True, False }


public static class NumberFormats
{
    public static System.Globalization.NumberFormatInfo Korean = new System.Globalization.CultureInfo("ko-KR", false).NumberFormat;
}
public static class Extensions1
{
    public static T[] Splice1<T>(this T[] source, int start)
    {
        var listItems = source.ToList<T>();
        var items = listItems.Skip(start).ToList<T>();
        return items.ToArray<T>();
    }

    //Numbers
    public static string ToFormattedString(this double num, bool useCommas = false, bool useK = false, string prefix = "")
    {
        return FormattedString.FromDouble(num, useCommas, useK, prefix);
    }

    public static string ToFormattedString(this int num, bool useCommas = false, bool useK = false, string prefix = "")
    {
        return FormattedString.FromDouble(num, useCommas, useK, prefix);
    }

    public static string ToFormattedString(this double num, bool useCommas, int maxDigits, string prefix = "")
    {
        return FormattedString.FromDouble(num, maxDigits, useCommas, prefix);
    }

    public static string ToFormattedString(this int num, bool useCommas, int maxDigits, string prefix = "")
    {
        return FormattedString.FromDouble(num, maxDigits, useCommas, prefix);
    }

    //public static string ToCurrencyString(this double num)
    //{
    //    if (Localization.CurrentLanguage != null && Localization.CurrentLanguage.name.ToUpper().Contains("KOREA"))
    //    {
    //        return num.ToKoreanCurrencyString();
    //    }
    //    else
    //    {
    //        num = Math.Floor(num);
    //        return num.ToString("N0");
    //    }
    //}

    static char[] koreanCommaReplacements = new char[] { '원', '만', '억', '조', '경' };
    public static string ToKoreanCurrencyString(this double num)
    {
        bool negative = num < 0;
        num = Math.Abs(Math.Floor(num));
        StringBuilder sb = new StringBuilder();
        int cci = 0; //comma char index 
        sb.Append(num.ToString("F0"));

        while (sb.Length % 4 != 0) sb.Insert(0, '0');
        //Debug.Log("sb " + sb.ToString());

        int[] parts = new int[sb.Length / 4];
        for (int i = 0; i < parts.Length; i++)
        {
            parts[i] = int.Parse(sb.ToString().Substring((i * 4), 4));
        }
        //Debug.Log("parts " + parts.GetDump());

        sb = new StringBuilder();
        for (int i = 0; i < parts.Length; i++)
        {
            cci = Mathf.Clamp(parts.Length - i - 1,
                                0,
                                koreanCommaReplacements.Length);
            //Debug.LogFormat("cci {0} part {1}", cci, i);
            if (parts[i] > 0)
            {
                sb.Append(parts[i]);
                sb.Append(koreanCommaReplacements[cci]);
            }
            else if (i == parts.Length - 1)
            {
                if (sb.Length == 0) sb.Append('0');
                sb.Append(koreanCommaReplacements[cci]);
            }
        }
        if (negative)
        {
            sb.Insert(0, '-');
        }

        //for (int i = sb.Length - 1; i >= startIndex; i--)
        //{
        //    if (cci < koreanCommaReplacements.Length && (ii%4 == 0))
        //    {
        //        sb.Insert(i+1, koreanCommaReplacements[cci]);
        //        cci++;
        //    }
        //    ii++;
        //} 

        return sb.ToString();
    }

    public static bool IsInBetween(this int val, int min, int max)
    {
        if (min > max) return false; //u b idiot, i'll b 2
        if (val > max) return false;
        if (val < min) return false;
        return true;
    }
    public static bool IsInBetween(this double val, double min, double max)
    {
        if (min > max) return false; //u b idiot, i'll b 2
        if (val > max) return false;
        if (val < min) return false;
        return true;
    }

    public static double Lerp(double a, double z, float f)
    {
        return a + (z - a) * f;
    }

    public static double Clamp(this double d, double min, double max)
    {
        return Math.Min(Math.Max(d, min), max);
    }


    public static string ToAmountString(this double n, System.Globalization.NumberFormatInfo f)
    {
        return n.ToString("c", f);
    }

    //bool
    public static int ToInt(this bool b, int trueVal = 1, int falseVal = 0)
    {
        return b ? trueVal : falseVal;
    }

    public static float ToFloat(this bool b, float trueVal = 1, float falseVal = 0)
    {
        return b ? trueVal : falseVal;
    }

    //string 
    public static string ToSentenceCase(this string s)
    {
        return FormattedString.ToSentenceCase(s);
    }
    public static string ToCamelCase(this string s)
    {
        return FormattedString.ToCamelCase(s);
    }

    public static string WithColorTag(this string s, Color color)
    {
        return string.Format("<color={0}>{1}</color>", color.ToHex(), s);
    }

    public static string ToRichText(this string s, Color color, bool bold = false, bool italic = false, int size = 0)
    {
        StringBuilder sb = new StringBuilder();

        if (bold) sb.Append("<b>");
        if (italic) sb.Append("<i>");
        if (size > 0) sb.AppendFormat("<size={0}>", size);
        sb.AppendFormat("<color={0}>", color.ToHex());
        sb.Append(s);
        sb.Append("</color>");
        if (size > 0) sb.Append("</size>");
        if (italic) sb.Append("</i>");
        if (bold) sb.Append("</b>");

        return sb.ToString();
    }

    public static int ParseToInt(this string s, int fallback = 0)
    {
        try
        {
            return int.Parse(s);
        }
        catch
        {
            return fallback;
        }
    }
    public static double ParseToDouble(this string s, double fallback = 0)
    {
        try
        {
            return double.Parse(s);
        }
        catch
        {
            return fallback;
        }
    }
    public static float ParseToFloat(this string s, float fallback = 0)
    {
        try
        {
            return float.Parse(s);
        }
        catch
        {
            return fallback;
        }
    }

    public static string RemoveHTMLTags(this string html)
    {
        const string tagWhiteSpace = @"(>|$)(\W|\n|\r)+<";//matches one or more (white space or line breaks) between '>' and '<'
        const string stripFormatting = @"<[^>]*(>|$)";//match any character between '<' and '>', even when end tag is missing
        const string lineBreak = @"<(br|BR)\s{0,1}\/{0,1}>";//matches: <br>,<br/>,<br />,<BR>,<BR/>,<BR />
        var lineBreakRegex = new Regex(lineBreak, RegexOptions.Multiline);
        var stripFormattingRegex = new Regex(stripFormatting, RegexOptions.Multiline);
        var tagWhiteSpaceRegex = new Regex(tagWhiteSpace, RegexOptions.Multiline);

        var text = html;
        //      //Decode html specific characters
        //      text = System.Net.WebUtility.HtmlDecode(text); 
        //Remove tag whitespace/line breaks
        text = tagWhiteSpaceRegex.Replace(text, "><");
        //Replace <br /> with line breaks
        text = lineBreakRegex.Replace(text, "\n");
        //Strip formatting
        text = stripFormattingRegex.Replace(text, string.Empty);

        return text;
    }

    public static string[] SplitToStringArray(this string s, char splitChar = ',')
    {
        string[] sa = s.Split(splitChar);
        string[] sa2 = new string[0];
        //if (sa.Length == 1 && !string.IsNullOrEmpty(sa[0]))
        //{
        //}
        //else
        {
            sa2 = new string[sa.Length];
            for (int i = 0; i < sa.Length; i++)
            {
                sa2[i] = sa[i];
            }
        }
        return sa2;
    }

    public static bool[] SplitToBoolArray(this string s)
    {
        string[] sa = s.Split(',');
        bool[] ba = new bool[0];
        //if (sa.Length == 1 && !string.IsNullOrEmpty(sa[0]))
        //{
        //}
        //else
        {
            ba = new bool[sa.Length];
            for (int i = 0; i < sa.Length; i++)
            {
                ba[i] = sa[i].Equals("true", StringComparison.CurrentCultureIgnoreCase);
            }
        }
        return ba;
    }

    public static int[] SplitToIntArray(this string s)
    {
        string[] sa = s.Split(',');
        int[] ia = new int[0];
        //if (sa.Length == 1 && !string.IsNullOrEmpty(sa[0]))
        //{
        //}
        //else
        {
            ia = new int[sa.Length];
            for (int i = 0; i < sa.Length; i++)
            {
                int.TryParse(sa[i], out ia[i]);
            }
        }
        return ia;
    }

    public static double[] SplitToDoubleArray(this string s, int length = -1, char separator = ',')
    {
        string[] sa = s.Split(separator);
        double[] da = new double[0];

        if (length > -1) da = new double[length];
        else da = new double[sa.Length];

        for (int i = 0; i < da.Length; i++)
        {
            try
            {
                da[i] = double.Parse(sa[i]);
            }
            catch { }
        }

        return da;
    }

    //Color Extensions
    /// <summary>
    /// Returns hex code of a rgb color (eg. #ffffff for Color.white)
    /// </summary>
    /// <param name="color"></param>
    /// <returns></returns>
    public static string ToHex(this Color color)
    {
        Color32 color32 = color;
        return string.Format("#{0:X2}{1:X2}{2:X2}{3:X2}", color32.r, color32.g, color32.b, color32.a);
    }

    /// <summary>
    /// Returns same color with 0 alpha value
    /// Optionally provide transparency for desired alpha. (alpha = 1 - transparency)
    /// </summary> 
    public static Color transparent(this Color color, float transparency = 1)
    {
        color.a = 1 - transparency;
        return color;
    }

    public static Color dark(this Color color, float darkness = 1)
    {
        color.r *= 1 - darkness;
        color.g *= 1 - darkness;
        color.b *= 1 - darkness;
        return color;
    }

    //Transform extensions
    public static Transform Duplicate(this Transform transformToDuplicate)
    {
        return GameObject.Instantiate(transformToDuplicate, transformToDuplicate.parent) as Transform;
    }

    public static Transform Duplicate(this Transform transformToDuplicate, Transform parent)
    {
        return GameObject.Instantiate(transformToDuplicate, parent) as Transform;
    }

    public static void ClearChildren(this Transform transformToClearChildOf)
    {
        //Debug.Log("ClearChildren");

        List<GameObject> children = new List<GameObject>();

        for (int i = 0; i < transformToClearChildOf.childCount; i++)
            children.Add(transformToClearChildOf.GetChild(i).gameObject);

        for (int i = 0; i < children.Count; i++)
        {
            if (Application.isPlaying)
            {
                GameObject.Destroy(children[i]);
            }
            else
            {
                GameObject.DestroyImmediate(children[i]);
            }
        }
    }

    public static Transform[] GetAllChildren(this Transform pt)
    {
        Transform[] ca = new Transform[pt.childCount];
        for (int i = 0; i < ca.Length; i++)
        {
            ca[i] = pt.GetChild(i);
        }
        return ca;
    }

    public static List<Transform> GetAllActiveChildren(this Transform pt)
    {
        List<Transform> ca = new List<Transform>();
        for (int i = 0; i < pt.childCount; i++)
        {
            if (pt.GetChild(i).gameObject.activeSelf) ca.Add(pt.GetChild(i));
        }
        return ca;
    }

    public static RectTransform[] GetAllChildren(this RectTransform pt)
    {
        RectTransform[] ca = new RectTransform[pt.childCount];
        for (int i = 0; i < ca.Length; i++)
        {
            ca[i] = (RectTransform)pt.GetChild(i);
        }
        return ca;
    }

    public static float CalculateHeight(this RectTransform t)
    {
        return t.rect.yMax - t.rect.yMin;
    }

    //GameObject Extensions
    public static void SetActive(this GameObject[] goa, bool active)
    {
        for (int i = 0; i < goa.Length; i++)
        {
            if (goa[i] != null) goa[i].SetActive(active);
        }
    }

    public static void SetActiveCollection(this IEnumerable<GameObject> collection, bool active)
    {
        IEnumerator<GameObject> e = collection.GetEnumerator();
        e.Reset();
        while (e.MoveNext())
        {
            if (e != null && e.Current != null) e.Current.SetActive(active);
        }
    }

    //UI Extensions
    public static void Stretch(this RectTransform rt)
    {
        rt.pivot = Vector2.one / 2;
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.anchoredPosition = Vector2.zero;
        rt.sizeDelta = Vector2.zero;
    }

    public static void SetText(this Text[] tca, string txt)
    {
        for (int i = 0; i < tca.Length; i++)
        {
            tca[i].text = txt;
        }
    }
    public static string ToXYZString(this Vector3 v)
    {
        return string.Format("x:{0}, y:{1}, z:{2}", v.x, v.y, v.z);
    }
}

public class FormattedString
{
    public static string FromDouble(double number, bool useCommas = false, bool useK = false, string prefix = "")
    {
        //number = System.Math.Floor(number);

        string suffix = "";
        if (useK)
        {
            if (number > 99999999)
            {
                number = number / 100000000;
                suffix = "B";
            }
            else if (number > 999999)
            {
                number = number / 1000000;
                suffix = "M";
            }
            else if (number > 999)
            {
                number = number / 1000;
                suffix = "K";
            }
        }

        StringBuilder s = new StringBuilder(prefix);

        if (useCommas)
        {
            s.Append(number.ToString("N3"));
        }
        else
        {
            s.Append(number.ToString("F3"));
        }

        //if (number % 1 > 0)
        //{
        //    s.Append('.');
        //    s.Append(number.ToString().Split('.')[1]);
        //} 

        for (int si = s.Length - 1; si >= 0; si--)
        {
            if (s[si].Equals('0'))
            {
                s.Remove(si, 1);
            }
            else if (s[si].Equals('.'))
            {
                s.Remove(si, 1);
                break;
            }
            else break;
        }

        s.Append(suffix);

        return s.ToString();
    }

    static string[] nSuffs = new string[] { "", "K", "M", "B", "T", "Q", "QN", "S", "SP", "I", "I", "I", "I" }; //http://www.statman.info/conversions/number_scales.html
    public static string FromDouble(double number, int maxDigits, bool useCommas = false, string prefix = "")
    {
        if (maxDigits == 0) maxDigits = 100;
        //number = System.Math.Floor(number);
        //Debug.Log(number);

        maxDigits -= prefix.Length;
        if (useCommas) maxDigits -= Mathf.Clamp((maxDigits - 1) / 3, 0, maxDigits);


        int i = number.ToString().Length;
        //int i = 0;
        ////i is number of digits here
        //while (true) if (number / System.Math.Pow(10, ++i) < 1) break; 

        if (maxDigits < 1) maxDigits = i;

        //i is difference of digits now 
        i = i - maxDigits + 1;

        //i is power of 10 now 
        if (i > 1)
        {
            i = i - i % 3 + 3;
        }
        else i = 0;

        number = number / System.Math.Pow(10, i);
        StringBuilder s = new StringBuilder(prefix);

        if (useCommas)
        {
            s.Append(number.ToString("N3"));
        }
        else
        {
            s.Append(number.ToString("F3"));
        }

        //if (number % 1 > 0)
        //{
        //    s.Append('.');
        //    s.Append(number.ToString().Split('.')[1]);
        //}

        for (int si = s.Length - 1; si >= 0; si--)
        {
            if (s[si].Equals('0'))
            {
                s.Remove(si, 1);
            }
            else if (s[si].Equals('.'))
            {
                s.Remove(si, 1);
                break;
            }
            else break;
        }

        s.Append(nSuffs[i / 3]);

        //Debug.Log(s.ToString());
        return s.ToString();
    }

    public static string ToSentenceCase(string text)
    {
        string txt = "";

        //using the brain of a 5th standard (source: tera bhai)=>
        //string[] sentences = text.Split('.');
        //bool upperCharProcessed = false;
        //for (int i = 0; i < sentences.Length; i++)
        //{
        //    upperCharProcessed = false;
        //    for (int s = 0; s < sentences[i].Length; s++)
        //    {
        //        if (!upperCharProcessed && char.IsLetter(sentences[i][s]))
        //        {
        //            txt += sentences[i][s].ToString().ToUpper();
        //            upperCharProcessed = true;
        //        }
        //        else txt += sentences[i][s].ToString().ToLower();
        //    }
        //    if (i < sentences.Length - 1) txt += '.';
        //}


        //using regex, much powerful (source: stack overflow) =>
        string lowerCase = text.ToLower();
        Regex r = new Regex(@"(^[a-z])|[?!.:]\s+(.)", RegexOptions.ExplicitCapture);
        txt = r.Replace(lowerCase, s => s.Value.ToUpper());

        return txt;
    }

    public static string ToCamelCase(string text)
    {
        string txt = "";

        string lowerCase = text.ToLower();
        Regex r = new Regex(@"(^[a-z])|[?!.:\s]+([a-z])", RegexOptions.ExplicitCapture); //(^[a-z])|[?!.:\s]+(\s|)+([a-z])
        txt = r.Replace(lowerCase, s => s.Value.ToUpper());

        return txt;
    }
}

public static class Validation
{
    public static bool IsNullOrEmpty(this string s)
    {
        return string.IsNullOrEmpty(s);
    }

    public static bool IsNotEmpty(this string s)
    {
        return !string.IsNullOrEmpty(s);
    }

    public static bool HasEmptyChars(this string s)
    {
        return GetRegexMatchCount(s, @"[ \n\f\b]") > 0;
    }

    public static bool HasSpecialChar(this string s)
    {
        return GetRegexMatchCount(s, @"[!#$%&()*+,-/:;<>?@[]{}_|~]") > 0;
    }

    public static bool HasUpperCaseAlphabet(this string s)
    {
        return GetRegexMatchCount(s, @"[A-Z]") > 0;
    }

    public static bool HasLowerCaseAlphabet(this string s)
    {
        return GetRegexMatchCount(s, @"[a-z]") > 0;
    }

    public static bool HasNumber(this string s)
    {
        return GetRegexMatchCount(s, @"[0-9]") > 0;
    }

    public static bool IsEquals(this Vector3 v1, Vector3 v2)
    {
        return (MathF.Round(v1.x, 3) == MathF.Round(v2.x, 3)
            && MathF.Round(v1.y, 3) == MathF.Round(v2.y, 3)
            && MathF.Round(v1.z, 3) == MathF.Round(v2.z, 3));
    }

    public static bool IsEquals(this Quaternion v1, Quaternion v2)
    {
        return (MathF.Round(v1.x, 3) == MathF.Round(v2.x, 3)
            && MathF.Round(v1.y, 3) == MathF.Round(v2.y, 3)
            && MathF.Round(v1.z, 3) == MathF.Round(v2.z, 3)
            && MathF.Round(v1.w, 3) == MathF.Round(v2.w, 3));
    }

    public static bool HasLengthBetween(this string s, int min, int max)
    {
        return s.Length >= min && s.Length <= max;
    }

    public static bool IsEmail(this string s)
    {
        return GetRegexMatchCount(s, @"\w+@\w{2,}\.\w{2,}") == 1;
    }

    public static bool IsValidEmail(string s)
    {
        string MatchEmailPattern =
            @"^(([\w-]+\.)+[\w-]+|([a-zA-Z]{1}|[\w-]{2,}))@"
            + @"((([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\.([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\."
              + @"([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\.([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])){1}|"
        + @"([a-zA-Z]+[\w-]+\.)+[a-zA-Z]{2,4})$";

        return GetRegexMatchCount(s, MatchEmailPattern) == 1;
    }

    public static bool IsStrongPassword(this string s)
    {
        if (s.HasEmptyChars()) return false;

        if (!s.HasLengthBetween(8, 15)) return false;

        if (!s.HasUpperCaseAlphabet()) return false;

        if (!s.HasLowerCaseAlphabet()) return false;

        if (!s.HasNumber()) return false;

        return true;
    }

    public static int GetRegexMatchCount(string s, string r)
    {
        Regex reg = new Regex(r, RegexOptions.ExplicitCapture);
        return reg.Matches(s).Count;
    }
}

public static class CollectionExt
{
    //Arrays
    public static bool Contains<T>(this T[] origArray, T val)
    {
        if (origArray == null) return false;
        for (int i = 0; i < origArray.Length; i++)
        {
            if (origArray[i].Equals(val)) return true;
        }
        return false;
    }

    public static int IndexOf<T>(this T[] origArray, T val)
    {
        for (int i = 0; i < origArray.Length; i++)
        {
            if (origArray[i].Equals(val)) return i;
        }
        return -1;
    }

    public static T[] SubArray<T>(this T[] origArray, int subsetStart)
    {
        return origArray.SubArray(subsetStart, int.MaxValue);
    }
    public static T[] SubArray<T>(this T[] origArray, int subsetStart, int subsetEnd)
    {
        if (subsetStart >= origArray.Length || subsetEnd < 0) return new T[] { };

        subsetStart = Mathf.Clamp(subsetStart, 0, origArray.Length - 1);
        subsetEnd = Mathf.Clamp(subsetEnd, 0, origArray.Length - 1);
        T[] subset = new T[Mathf.Clamp(subsetEnd - subsetStart + 1, 0, origArray.Length)];
        //Debug.Log("l: " + origArray.Length + " ss: " + subsetStart + " se: " + subsetEnd + " sl: " + subset.Length);

        for (int i = 0; i < subset.Length; i++) subset[i] = origArray[i + subsetStart];

        return subset;
    }

    public static T[] ShiftElements<T>(this T[] origArray, int shiftCount)
    {
        shiftCount = -shiftCount;
        if (shiftCount < 0) shiftCount = origArray.Length + (shiftCount % origArray.Length);

        T[] newArray = new T[origArray.Length];
        for (int i = 0; i < newArray.Length; i++)
        {
            newArray[i] = origArray[(i + shiftCount) % origArray.Length];
        }

        return newArray;
    }

    public static T GetSafe<T>(this T[] arr, int i)
    {
        if (arr != null && arr.Length > i && i >= 0)
        {
            return arr[i];
        }
        return default(T);
    }

    public static T GetSafe<T>(this List<T> l, int i)
    {
        if (l != null && l.Count > i && i >= 0)
        {
            return l[i];
        }
        return default(T);
    }

    public static bool HasItem<T>(this T[] l, int i)
    {
        if (l != null && l.Length > i && i >= 0)
        {
            return true;
        }
        return false;
    }

    public static string GetSafe(this string[] arr, int i)
    {
        if (arr == null || arr.Length <= i)
            return string.Empty;
        else
            return arr[i];
    }

    public static string GetSafe(this List<string> arr, int i)
    {
        if (arr == null || arr.Count <= i)
            return string.Empty;
        else
            return arr[i];
    }

    public static void Insert<T>(this T[] arr, T val, int index)
    {
        T[] a = new T[arr.Length + 1];
        bool ins = false;

        for (int i = 0; i < a.Length; i++)
        {
            if (i < index) a[i] = arr[i];
            else if (i == index) a[i] = val;
            else a[i] = arr[i - 1];
        }

        arr = a;
    }

    public static void ForEachItem<T>(this T[] c, System.Action<T> act)
    {
        foreach (var item in c)
        {
            if (item != null) act(item);
        }
    }

    public static void ForEachItem<T>(this List<T> c, System.Action<T> act)
    {
        foreach (var item in c)
        {
            if (item != null) act(item);
        }
    }

    public static void ForEachItem<T1, T2>(this Dictionary<T1, T2> c, System.Action<T1, T2> act)
    {
        foreach (var item in c)
        {
            act(item.Key, item.Value);
        }
    }

    public static List<int> SortedIndexes<T>(this T[] refList) where T : IComparable
    {
        List<int> indexes = new List<int>();

        if (refList == null) return indexes;

        List<T> tempList = new List<T>();
        for (int i = 0; i < refList.Length; i++)
        {
            int insertAt = tempList.Count;
            for (int j = 0; j < tempList.Count; j++)
            {
                if ((refList[i]).CompareTo(tempList[j]) > 0)
                {
                    insertAt = j;
                    break;
                }
            }
            tempList.Insert(insertAt, refList[i]);
            indexes.Insert(insertAt, i);
        }
        return indexes;
    }

    public static int[] ParseToInt(this string[] arr)
    {
        int[] n = new int[arr.Length];
        for (int i = 0; i < n.Length; i++)
        {
            try
            {
                n[i] = int.Parse(arr[i]);
            }
            catch { }
        }
        return n;
    }

    public static double[] ParseToDouble(this string[] arr)
    {
        double[] n = new double[arr.Length];
        for (int i = 0; i < n.Length; i++)
        {
            try
            {
                n[i] = double.Parse(arr[i]);
            }
            catch { }
        }
        return n;
    }

    public static string[] ToStringArray<T>(this T[] a)
    {
        string[] n = new string[a.Length];
        for (int i = 0; i < n.Length; i++)
        {
            n[i] = a[i].ToString();
        }
        return n;
    }

    public static string[] ToStringArray<T>(this List<T> l)
    {
        string[] n = new string[l.Count];
        for (int i = 0; i < n.Length; i++)
        {
            n[i] = l[i].ToString();
        }
        return n;
    }

    public static string ToStringPrint<T>(this List<T> l)
    {
        string n = string.Empty;
        for (int i = 0; i < l.Count; i++)
        {
            n += l[i].ToString() + ", ";
        }
        return n;
    }

    public static string Print<T>(this T[] a)
    {
        string n = string.Empty;
        for (int i = 0; i < a.Length; i++)
        {
            n += a[i].ToString() + ", ";
        }
        return n;
    }

    public static int GetSum(this int[] arr)
    {
        if (arr == null || arr.Length == 0) return 0;
        int sum = 0;
        for (int i = 0; i < arr.Length; i++)
        {
            sum += arr[i];
        }
        return sum;
    }

    public static double GetSum(this double[] arr)
    {
        if (arr == null || arr.Length == 0) return 0;
        double sum = 0;
        for (int i = 0; i < arr.Length; i++)
        {
            sum += arr[i];
        }
        return sum;
    }

    //Dictionary 
    public static void SetOrAdd<Tkey, Tval>(this Dictionary<Tkey, Tval> dictionary, Tkey key, Tval val)
    {
        if (dictionary.ContainsKey(key)) dictionary[key] = val;
        else dictionary.Add(key, val);
    }

    public static Tval TryGet<Tkey, Tval>(Dictionary<Tkey, Tval> dictionary, Tkey key)
    {
        try
        {
            return (Tval)dictionary[key];
        }
        catch
        {
            return default(Tval);
        }
    }

    //IEnumerable Exts
    public static string GetDump(this IEnumerable collection)
    {
        string s = "";
        IEnumerator e = collection.GetEnumerator();
        e.Reset();
        while (e.MoveNext())
        {
            if (e.Current != null)
            {
                s += "\n" + e.Current.ToString();
            }
            else s += "\nNULL";
        }
        return s;
    }
    public static string GetDumpSL(this IEnumerable collection)
    {
        return GetDumpFormatted(",", "[", "]");
    }
    public static string GetDumpFormatted(this IEnumerable collection, string saperator = "\n", string prefix = "", string suffix = "")
    {
        string s = prefix;
        int i = 0;
        if (collection != null)
        {
            IEnumerator e = collection.GetEnumerator();
            e.Reset();
            while (e.MoveNext())
            {
                if (i > 0) s += saperator;

                if (e.Current != null)
                {
                    s += e.Current.ToString();
                }
                else s += "NULL";

                i++;
            }
        }
        s += suffix;
        return s;
    }

    public static string GetDump(this IDictionary dict)
    {
        StringBuilder sb = new StringBuilder();
        foreach (DictionaryEntry item in dict)
        {
            sb.Append("\n");
            sb.Append(item.Key);
            sb.Append(" ");
            sb.Append(item.Value);
        }
        return sb.ToString();
    }
}

public static class EnumExt
{
    public static T[] GetValues<T>()
    {
        return Enum.GetValues(typeof(T)).Cast<T>() as T[];
    }
}

public static class DateTimeExt
{
    static DateTime defDateTime = new DateTime();

    public static string[] monthsShort = new string[] { "", "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec" };

    static TimeSpan localTimeOffsetSpan = TimeSpan.FromDays(99);
    public static TimeSpan LocalTimeOffsetSpan
    {
        get
        {
            if (localTimeOffsetSpan.TotalDays == 99) localTimeOffsetSpan = DateTime.Now.Subtract(DateTime.UtcNow);
            return localTimeOffsetSpan;
        }
        set
        {
            localTimeOffsetSpan = value;
        }
    }

    static string timezoneOffsetString = "";
    internal static TimeSpan systemOffsetSpan = TimeSpan.FromTicks(0);

    public static string TimezoneOffsetString
    {
        get
        {
            if (string.IsNullOrEmpty(timezoneOffsetString))
            {
                double osm = LocalTimeOffsetSpan.TotalMinutes;
                if (osm > 0) timezoneOffsetString = "+";
                timezoneOffsetString += osm.ToString();
            }
            return timezoneOffsetString;
        }
    }

    public static DateTime UtcNow
    {
        get { return DateTime.UtcNow.Subtract(DateTimeExt.systemOffsetSpan); }
    }

    public static DateTime Parse(string timeFromDb)
    {
        try
        {
            timeFromDb = timeFromDb.Replace('T', ' ');
            timeFromDb = timeFromDb.Replace("Z", "");
            return DateTime.Parse(timeFromDb);
        }
        catch
        {
            // Logs.Add.Info(string.Format("Time: {0} could not be parsed.", timeFromDb));
            return defDateTime;
        }
    }

    public static DateTime Now
    {
        get { return DateTime.UtcNow.Add(LocalTimeOffsetSpan).Subtract(systemOffsetSpan); }
    }

    public static void ParseGMTToLocal(ref string timeFromDb, ref DateTime targetDateTime)
    {
        try
        {
            timeFromDb = timeFromDb.Replace('T', ' ');
            timeFromDb = timeFromDb.Replace("Z", string.Empty);
            targetDateTime = DateTime.Parse(timeFromDb).Add(LocalTimeOffsetSpan).Add(systemOffsetSpan);
        }
        catch
        {
            //            Logs.Add.Info(string.Format("Time: {0} could not be parsed.", timeFromDb));
        }
    }

    public static DateTime ParseGMTToLocal(ref string timeFromDb)
    {
        try
        {
            timeFromDb = timeFromDb.Replace('T', ' ');
            timeFromDb = timeFromDb.Replace("Z", "");
            return DateTime.Parse(timeFromDb).Add(LocalTimeOffsetSpan).Add(systemOffsetSpan);
        }
        catch
        {
            //  Logs.Add.Info(string.Format("Time: {0} could not be parsed.", timeFromDb));
            return defDateTime;
        }
    }

    public static DateTime ParseGMTToLocal(string timeFromDb)
    {
        return ParseGMTToLocal(ref timeFromDb);
    }

    public static DateTime RoundUp(this DateTime dt, TimeSpan d)
    {
        var modTicks = dt.Ticks % d.Ticks;
        var delta = modTicks != 0 ? d.Ticks - modTicks : 0;
        return new DateTime(dt.Ticks + delta, dt.Kind);
    }

    public static DateTime RoundDown(this DateTime dt, TimeSpan d)
    {
        var delta = dt.Ticks % d.Ticks;
        return new DateTime(dt.Ticks - delta, dt.Kind);
    }

    public static DateTime RoundToNearest(this DateTime dt, TimeSpan d)
    {
        var delta = dt.Ticks % d.Ticks;
        bool roundUp = delta > d.Ticks / 2;
        var offset = roundUp ? d.Ticks : 0;

        return new DateTime(dt.Ticks + offset - delta, dt.Kind);
    }

    public class FormattingHelp
    {
        //DateTime dt = new DateTime(2008, 3, 9, 16, 5, 7, 123);

        //String.Format("{0:y yy yyy yyyy}",      dt);  // "8 08 008 2008"   year
        //String.Format("{0:M MM MMM MMMM}",      dt);  // "3 03 Mar March"  month
        //String.Format("{0:d dd ddd dddd}",      dt);  // "9 09 Sun Sunday" day
        //String.Format("{0:h hh H HH}",          dt);  // "4 04 16 16"      hour 12/24
        //String.Format("{0:m mm}",               dt);  // "5 05"            minute
        //String.Format("{0:s ss}",               dt);  // "7 07"            second
        //String.Format("{0:f ff fff ffff}",      dt);  // "1 12 123 1230"   sec.fraction
        //String.Format("{0:F FF FFF FFFF}",      dt);  // "1 12 123 123"    without zeroes
        //String.Format("{0:t tt}",               dt);  // "P PM"            A.M. or P.M.
        //String.Format("{0:z zz zzz}",           dt);  // "-6 -06 -06:00"   time zone

        //// month/day numbers without/with leading zeroes
        //String.Format("{0:M/d/yyyy}",           dt);  // "3/9/2008"
        //String.Format("{0:MM/dd/yyyy}",         dt);  // "03/09/2008"

        //// day/month names
        //String.Format("{0:ddd, MMM d, yyyy}",   dt);  // "Sun, Mar 9, 2008"
        //String.Format("{0:dddd, MMMM d, yyyy}", dt);  // "Sunday, March 9, 2008"

        //// two/four digit year
        //String.Format("{0:MM/dd/yy}",           dt);  // "03/09/08"
        //String.Format("{0:MM/dd/yyyy}",         dt);  // "03/09/2008"
    }
}

public static class TransformExt
{
    public static T TryGetComponent<T>(this Transform tr) where T : Component
    {
        try
        {
            return tr.GetComponent<T>();
        }
        catch (Exception e) { Debug.Log(e.Message); }
        return null;
    }

    public static T TryFindComponentInChildren<T>(this Transform parent, string goName) where T : Component
    {
        try
        {
            T[] comps = parent.GetComponentsInChildren<T>(true);
            foreach (T comp in comps)
            {
                if (comp.name.Equals(goName)) return comp;
            }
        }
        catch (Exception e) { Debug.Log(e.Message); }
        return null;
    }
}


public static class Rand
{
    public static System.Random r = new System.Random();

    public static bool GetBool()
    {
        return r.Next(0, 2) == 1;
    }
    /// <summary>
    /// odds 1-50
    /// </summary>
    public static bool GetBool(int odds = 50)
    {
        odds = Mathf.Clamp(odds, 1, 50);
        return r.Next(0, 100) < odds;
    }

    public static int GetInt(int max = 1)
    {
        return r.Next(0, max);
    }

    public static int GetInt(int min, int max)
    {
        return r.Next(min, max);
    }

    public static int[] GetIntArray(int length, int minVal, int maxVal)
    {
        int[] a = new int[length];
        for (int i = 0; i < length; i++)
        {
            a[i] = r.Next(minVal, maxVal);
        }
        return a;
    }

    public static float GetFloat(float max = 1)
    {
        return (float)r.NextDouble() * max;
    }

    public static double GetDouble(double max = 1)
    {
        return (double)r.NextDouble() * max;
    }

    public static double GetDouble(double min, double max)
    {
        return min + (double)r.NextDouble() * (max - min);
    }

    public static float GetFloat(float min, float max)
    {
        return min + GetFloat(max - min);
    }

    public static T GetEnum<T>() where T : struct, IConvertible
    {
        if (!typeof(T).IsEnum)
        {
            throw new ArgumentException("Rand.GetEnum :: T must be an enum");
        }

        T[] a = (T[])Enum.GetValues(typeof(T));
        return a[GetInt(a.Length)];
    }

    public static T GetFromArray<T>(T[] a)
    {
        return a[GetInt(a.Length)];
    }

    public static T GetCopyOf<T>(this Component comp, T other) where T : Component
    {
        Type type = comp.GetType();
        if (type != other.GetType()) return null; // type mis-match
        BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Default | BindingFlags.DeclaredOnly;
        PropertyInfo[] pinfos = type.GetProperties(flags);
        foreach (var pinfo in pinfos)
        {
            if (pinfo.CanWrite)
            {
                try
                {
                    pinfo.SetValue(comp, pinfo.GetValue(other, null), null);
                }
                catch { } // In case of NotImplementedException being thrown. For some reason specifying that exception didn't seem to catch it, so I didn't catch anything specific.
            }
        }
        FieldInfo[] finfos = type.GetFields(flags);
        foreach (var finfo in finfos)
        {
            finfo.SetValue(comp, finfo.GetValue(other));
        }
        return comp as T;
    }

    public static T AddComponent<T>(this GameObject go, T toAdd) where T : Component
    {
        return go.AddComponent<T>().GetCopyOf(toAdd) as T;
    }
}


/// <summary>
/// Perform delayed actions. Explore it's functions to discover what can be done.
/// </summary>
public class Delayed
{
    #region static methods 
    /// <summary>
    /// Pass a function of no parameters here with desired delay.
    /// <para>Remember while passing lambda expressions or anonymous functions that they may loose references to some variables inside them as those variables can change in meantime</para>
    /// </summary>
    /// <param name="function"> function of void return type</param>
    /// <param name="delay">desired delay</param>
    /// <returns></returns>
    public static Action Function(System.Action function, float delay)
    {
        return new Action(function, delay);
    }

    /// <summary>
    /// Pass a function which takes one parameter here with desired delay.
    /// <para>Remember while passing lambda expressions or anonymous functions that they may loose references to some variables inside them as those variables can change in meantime</para>
    /// </summary>
    /// <typeparam name="T">type of parameter(if you want to define)</typeparam>
    /// <param name="function">function that takes one parameter of any type</param>
    /// <param name="param">value of that parameter</param>
    /// <param name="delay">desired delay</param>
    /// <returns></returns>
    public static Action<T> Function<T>(System.Action<T> function, T param, float delay)
    {
        return new Action<T>(function, param, delay);
    }

    public static Action<T1, T2> Function<T1, T2>(System.Action<T1, T2> function, T1 param1, T2 param2, float delay)
    {
        return new Action<T1, T2>(function, param1, param2, delay);
    }

    public static Action<T1, T2, T3> Function<T1, T2, T3>(System.Action<T1, T2, T3> function, T1 param1, T2 param2, T3 param3, float delay)
    {
        return new Action<T1, T2, T3>(function, param1, param2, param3, delay);
    }

    public static Action<T1, T2, T3, T4> Function<T1, T2, T3, T4>(System.Action<T1, T2, T3, T4> function, T1 param1, T2 param2, T3 param3, T4 param4, float delay)
    {
        return new Action<T1, T2, T3, T4>(function, param1, param2, param3, param4, delay);
    }
    #endregion

    #region classes
    public class Action
    {
        Coroutine cr = null;
        System.Action function;
        float delay;

        public Action(System.Action _function, float _delay)
        {
            function = _function;
            delay = _delay;
            Invoke();
        }

        public void Invoke()
        {
            cr = CommonMono.instance.StartCoroutine(Function_c());
        }

        public void CancelAction()
        {
            if (cr != null) CommonMono.instance.StopCoroutine(cr);
            cr = null;
            function = null;
        }

        IEnumerator Function_c()
        {
            if (delay > 0) yield return new WaitForSeconds(delay);
            if (function != null) function();
            cr = null;
        }
    }

    public class Action<T1>
    {
        Coroutine cr = null;
        System.Action<T1> function;
        T1 param;
        float delay;

        public Action(System.Action<T1> _function, T1 _param, float _delay)
        {
            function = _function;
            param = _param;
            delay = _delay;
            Invoke();
        }

        public void Invoke()
        {
            cr = CommonMono.instance.StartCoroutine(Function_c());
        }

        public void CancelAction()
        {
            if (cr != null) CommonMono.instance.StopCoroutine(cr);
            cr = null;
            function = null;
        }

        IEnumerator Function_c()
        {
            if (delay > 0) yield return new WaitForSeconds(delay);
            if (function != null) function(param);
            cr = null;
        }
    }

    public class Action<T1, T2>
    {
        Coroutine cr = null;
        System.Action<T1, T2> function;
        T1 param1;
        T2 param2;
        float delay;

        public Action(System.Action<T1, T2> _function, T1 _param1, T2 _param2, float _delay)
        {
            function = _function;
            param1 = _param1;
            param2 = _param2;
            delay = _delay;
            Invoke();
        }

        public void Invoke()
        {
            cr = CommonMono.instance.StartCoroutine(Function_c());
        }

        public void CancelAction()
        {
            if (cr != null) CommonMono.instance.StopCoroutine(cr);
            cr = null;
            function = null;
        }

        IEnumerator Function_c()
        {
            if (delay > 0) yield return new WaitForSeconds(delay);
            if (function != null) function(param1, param2);
            cr = null;
        }
    }

    public class Action<T1, T2, T3>
    {
        Coroutine cr = null;
        System.Action<T1, T2, T3> function;
        T1 param1;
        T2 param2;
        T3 param3;
        float delay;

        public Action(System.Action<T1, T2, T3> _function, T1 _param1, T2 _param2, T3 _param3, float _delay)
        {
            function = _function;
            param1 = _param1;
            param2 = _param2;
            param3 = _param3;
            delay = _delay;
            Invoke();
        }

        public void Invoke()
        {
            cr = CommonMono.instance.StartCoroutine(Function_c());
        }

        public void CancelAction()
        {
            if (cr != null) CommonMono.instance.StopCoroutine(cr);
            cr = null;
            function = null;
        }

        IEnumerator Function_c()
        {
            if (delay > 0) yield return new WaitForSeconds(delay);
            if (function != null) function(param1, param2, param3);
            cr = null;
        }
    }

    public class Action<T1, T2, T3, T4>
    {
        Coroutine cr = null;
        System.Action<T1, T2, T3, T4> function;
        T1 param1;
        T2 param2;
        T3 param3;
        T4 param4;
        float delay;

        public Action(System.Action<T1, T2, T3, T4> _function, T1 _param1, T2 _param2, T3 _param3, T4 _param4, float _delay)
        {
            function = _function;
            param1 = _param1;
            param2 = _param2;
            param3 = _param3;
            param4 = _param4;
            delay = _delay;
            Invoke();
        }

        public void Invoke()
        {
            cr = CommonMono.instance.StartCoroutine(Function_c());
        }

        public void CancelAction()
        {
            if (cr != null) CommonMono.instance.StopCoroutine(cr);
            cr = null;
            function = null;
        }

        IEnumerator Function_c()
        {
            if (delay > 0) yield return new WaitForSeconds(delay);
            if (function != null) function(param1, param2, param3, param4);
            cr = null;
        }
    }
    #endregion 
}

/// <summary>
/// Animate things here. Explore it's child classes to discover what can be done.
/// </summary>
public class Interpolate
{
    /// <summary>
    /// Scales a transform from one position to another.
    /// <para>You create a new class for each new interpolation(movement will automatically start), save it's reference if you later want to stop the movement.</para>
    /// </summary>
    public class Scale
    {
        Coroutine cr;

        public bool isRunning
        {
            get
            {
                return cr != null;
            }
        }

        public AnimationCurve curve = new AnimationCurve(new Keyframe[] { new Keyframe(0, 0), new Keyframe(1, 1) });

        /// <summary>
        /// Scales a transform from one vector3 to another.
        /// <para>You create a new class for each new interpolation(scaling will automatically start), save it's reference if you later want to stop or modify the animation.</para>
        /// </summary>
        /// <param name="transform">transform to scale</param>
        /// <param name="from">starting scale</param>
        /// <param name="to">ending scale</param>
        /// <param name="duration">time duration to reach the final scale</param> 
        public Scale(Transform transform, Vector3 from, Vector3 to, float duration, AnimationCurve animCurve = null)
        {
            //if (Vector3.Distance(from, to) < .01f) { Debug.Log("wasted"); return; }
            if (animCurve != null) curve = animCurve;
            cr = CommonMono.instance.StartCoroutine(Interpolation(transform, from, to, duration));
        }

        public void Stop()
        {
            if (cr != null) CommonMono.instance.StopCoroutine(cr);
        }

        IEnumerator Interpolation(Transform transform, Vector3 from, Vector3 to, float duration)
        {
            float t = 0;
            float deltaTime = 0, realTimeLastFrame = Time.realtimeSinceStartup;
            while (t < duration)
            {
                deltaTime = Time.realtimeSinceStartup - realTimeLastFrame;
                realTimeLastFrame = Time.realtimeSinceStartup;
                t += deltaTime;
                if (transform != null)
                    transform.localScale = Vector3.Lerp(from, to, curve.Evaluate(t / duration * 1f));
                yield return null;
            }
            if (transform != null)
                transform.localScale = to;
            cr = null;
        }
    }
    /// <summary>
    /// Moves a transform from one position to another.
    /// <para>You create a new class for each new interpolation(movement will automatically start), save it's reference if you later want to stop the movement.</para>
    /// </summary>
    public class Position
    {
        Coroutine cr;

        public bool isRunning
        {
            get
            {
                return cr != null;
            }
        }

        public AnimationCurve curve = new AnimationCurve(new Keyframe[] { new Keyframe(0, 0), new Keyframe(1, 1) });

        /// <summary>
        /// Moves a transform from one position to another.
        /// <para>You create a new class for each new interpolation(movement will automatically start), save it's reference if you later want to stop the movement.</para>
        /// </summary>
        /// <param name="transform">transform to move</param>
        /// <param name="from">starting position</param>
        /// <param name="to">ending position</param>
        /// <param name="duration">time duration to reach the final destination</param>
        /// <param name="local">to modify localPosition or Position instead</param>
		public Position(Transform transform, Vector3 from, Vector3 to, float duration, bool local = false, AnimationCurve animCurve = null)
        {
            //if (Vector3.Distance(from, to) < .01f) { Debug.Log("wasted"); return; }
            if (animCurve != null) curve = animCurve;
            cr = CommonMono.instance.StartCoroutine(Interpolation(transform, from, to, duration, local));
        }

        public void Stop()
        {
            if (cr != null) CommonMono.instance.StopCoroutine(cr);
        }

        IEnumerator Interpolation(Transform transform, Vector3 from, Vector3 to, float duration, bool local = false)
        {
            float t = 0;
            float deltaTime = 0, realTimeLastFrame = Time.realtimeSinceStartup;
            while (t < duration)
            {
                if (transform == null) yield break;
                deltaTime = Time.realtimeSinceStartup - realTimeLastFrame;
                realTimeLastFrame = Time.realtimeSinceStartup;
                t += deltaTime;
                if (local) transform.localPosition = Vector3.Lerp(from, to, curve.Evaluate(t / duration * 1f));
                else transform.position = Vector3.Lerp(from, to, curve.Evaluate(t / duration * 1f));
                yield return null;
            }
            if (local) transform.localPosition = to;
            else transform.position = to;
            cr = null;
        }
    }

    /// <summary>
    /// Rotates a transform from one eulerAngles to another.
    /// <para>You create a new class for each new interpolation(rotation will automatically start), save it's reference if you later want to stop the rotation.</para>
    /// </summary>
    public class Eulers
    {
        Coroutine cr;

        public bool isRunning
        {
            get
            {
                return cr != null;
            }
        }

        public AnimationCurve curve = new AnimationCurve(new Keyframe[] { new Keyframe(0, 0), new Keyframe(1, 1) });


        /// <summary>
        /// Rotates a transform from one eulerAngles to another.
        /// <para>You create a new class for each new interpolation(rotation will automatically start), save it's reference if you later want to stop the rotation.</para>
        /// </summary>
        /// <param name="transform">target transform to rotate</param>
        /// <param name="from">starting eulerAngles</param>
        /// <param name="to">final eulerAngles</param>
        /// <param name="duration">time duration to rotate to final eulerAngles</param>
        /// <param name="local">to modify localEulerAngles or eulerAngles</param>
        public Eulers(Transform transform, Vector3 from, Vector3 to, float duration, bool local = false)
        {
            cr = CommonMono.instance.StartCoroutine(Interpolation(transform, from, to, duration, local));
        }

        public void Stop()
        {
            if (cr != null) CommonMono.instance.StopCoroutine(cr);
        }

        IEnumerator Interpolation(Transform transform, Vector3 from, Vector3 to, float duration, bool local = false)
        {
            float t = 0;
            float deltaTime = 0, realTimeLastFrame = Time.realtimeSinceStartup;
            while (t < duration)
            {
                deltaTime = Time.realtimeSinceStartup - realTimeLastFrame;
                realTimeLastFrame = Time.realtimeSinceStartup;
                t += deltaTime;
                if (local) transform.localEulerAngles = Vector3.Lerp(from, to, curve.Evaluate(t / duration * 1f));
                else transform.eulerAngles = Vector3.Lerp(from, to, curve.Evaluate(t / duration * 1f));
                yield return null;
            }
            if (local) transform.localEulerAngles = to;
            else transform.eulerAngles = to;
            cr = null;
        }
    }



    /// <summary>
    /// Changes color of a UI graphic component.
    /// <para>You create a new class for each new interpolation(animation will automatically start), save it's reference if you later want to stop the animation.</para>
    /// </summary>
    public class UIGraphicColor
    {
        Coroutine cr;

        public bool isRunning
        {
            get
            {
                return cr != null;
            }
        }

        public static AnimationCurve curve = new AnimationCurve(new Keyframe[] { new Keyframe(0, 0), new Keyframe(1, 1) });


        /// <summary>
        /// Changes color of an UI graphic from one color to another.
        /// <para>You create a new class for each new interpolation(animation will automatically start), save it's reference if you later want to stop the animation.</para>
        /// </summary>
        /// <param name="transform">target image</param>
        /// <param name="from">starting color</param>
        /// <param name="to">final color</param>
        /// <param name="duration">time duration to change to final color</param> 
        public UIGraphicColor(MaskableGraphic graphic, Color from, Color to, float duration)
        {
            cr = CommonMono.instance.StartCoroutine(Interpolation(graphic, from, to, duration));
        }

        public void Stop()
        {
            if (cr != null) CommonMono.instance.StopCoroutine(cr);
        }

        IEnumerator Interpolation(MaskableGraphic graphic, Color from, Color to, float duration)
        {
            float t = 0;
            float deltaTime = 0, realTimeLastFrame = Time.realtimeSinceStartup;
            while (t < duration)
            {
                deltaTime = Time.realtimeSinceStartup - realTimeLastFrame;
                realTimeLastFrame = Time.realtimeSinceStartup;
                t += deltaTime;
                graphic.color = Color.Lerp(from, to, curve.Evaluate(t / duration * 1f));
                yield return null;
            }
            graphic.color = to;
            cr = null;
        }
    }

    public class UICanvasGroupFade
    {
        Coroutine cr;

        public bool isRunning
        {
            get
            {
                return cr != null;
            }
        }

        public static AnimationCurve curve = new AnimationCurve(new Keyframe[] { new Keyframe(0, 0), new Keyframe(1, 1) });


        /// <summary>
        /// Changes color of an UI graphic from one color to another.
        /// <para>You create a new class for each new interpolation(animation will automatically start), save it's reference if you later want to stop the animation.</para>
        /// </summary>
        /// <param name="transform">target image</param>
        /// <param name="from">starting color</param>
        /// <param name="to">final color</param>
        /// <param name="duration">time duration to change to final color</param> 
        public UICanvasGroupFade(CanvasGroup canvasGroup, float from, float to, float duration)
        {
            if (canvasGroup == null) return;
            cr = CommonMono.instance.StartCoroutine(Interpolation(canvasGroup, from, to, duration));
        }

        public void Stop()
        {
            if (cr != null) CommonMono.instance.StopCoroutine(cr);
        }

        IEnumerator Interpolation(CanvasGroup canvasGroup, float from, float to, float duration)
        {
            float t = 0;
            float deltaTime = 0, realTimeLastFrame = Time.realtimeSinceStartup;
            while (t < duration)
            {
                deltaTime = Time.realtimeSinceStartup - realTimeLastFrame;
                realTimeLastFrame = Time.realtimeSinceStartup;
                t += deltaTime;
                canvasGroup.alpha = Mathf.Lerp(from, to, curve.Evaluate(t / duration * 1f));
                yield return null;
            }
            canvasGroup.alpha = to;
            cr = null;
        }
    }
}



/// <summary>
/// Finds components in the entire hirarchy under the passed transform
/// <para>Create a new instance of this class and define what component you want to look for, also pass the transform that is grand parent</para>
/// <para>later you can access all components from the list 'comps'</para>
/// </summary> 
public class ComponentsInChildren<T>
{
    public List<T> comps = new List<T>();

    /// <summary>
    /// Finds components in the entire hirarchy under the passed transform
    /// <para>Create a new instance of this class and define what component you want to look for, also pass the transform that is grand parent</para>
    /// <para>later you can access all components from the list 'comps'</para>
    /// </summary> 
    public ComponentsInChildren(Transform t)
    {
        comps.Clear();
        ProcessTransform(t);
    }

    public void ProcessTransform(Transform tx)
    {
        for (int i = 0; i < tx.childCount; i++)
        {
            if (tx.GetChild(i).childCount > 0)
            {
                ProcessTransform(tx.GetChild(i));
            }
            T comp = tx.GetChild(i).GetComponent<T>();
            if (comp != null) comps.Add(comp);
        }
    }
}

public class Download
{
    public class Image
    {
        static Texture2D _whiteTexture;
        public static Texture2D whiteTexture
        {
            get
            {
                if (_whiteTexture == null)
                {
                    _whiteTexture = new Texture2D(1, 1);
                    _whiteTexture.SetPixel(0, 0, Color.black.transparent(1));
                    _whiteTexture.Apply();
                }
                return _whiteTexture;
            }
        }

        static Texture2D _transparentTexture;
        public static Texture2D transparentTexture
        {
            get
            {
                if (_transparentTexture == null)
                {
                    _transparentTexture = new Texture2D(1, 1);
                    _transparentTexture.SetPixel(0, 0, Color.black.transparent(1));
                    _transparentTexture.Apply();
                }
                return _transparentTexture;
            }
        }
        static Sprite _transparentSprite;
        public static Sprite transparentSprite
        {
            get
            {
                if (_transparentSprite == null)
                {
                    _transparentSprite = Sprite.Create(transparentTexture, new Rect(0, 0, 1, 1), new Vector2(0.5f, 0.5f));
                }
                return _transparentSprite;
            }
        }


        public Texture2D tex = new Texture2D(1, 1, TextureFormat.Alpha8, false);
        string webUrl = "";
        string localUrl = "";
        string localDir = "";
        WWW req = null;
        public Coroutine cr;

        void SetUrl(string val)
        {
            webUrl = val;
            try
            {
                localUrl = Application.persistentDataPath + val.Substring(val.IndexOf('/', 8));
            }
            catch
            { }
            StringBuilder sb = new StringBuilder(localUrl);
            for (int i = localUrl.Length - 1; i > 0; i--)
            {
                if (sb[i].Equals('/'))
                {
                    sb.Remove(i, sb.Length - i);
                    localDir = sb.ToString();
                    break;
                }
            }
        }

        Sprite sprite;
        UnityEngine.UI.Image uiImg;
        System.Action<Texture2D> callback;
        System.Action<Sprite> callbackSprite;
        System.Action<string> failureCallback;

        public Image(string _url, UnityEngine.UI.Image _uiImg, bool cache = true, System.Action<string> onFail = null, Sprite fallbakcSprite = null)
        {
            if (string.IsNullOrEmpty(_url)) return;
            failureCallback = onFail;
            uiImg = _uiImg;
            uiImg.preserveAspect = true;
            SetUrl(_url);
            cr = CommonMono.instance.StartCoroutine(Loading(cache));
            if (fallbakcSprite != null) uiImg.sprite = fallbakcSprite;
            else uiImg.sprite = transparentSprite;
        }

        public Image(string _url, Sprite _sprite, bool cache = true, System.Action<string> onFail = null, Sprite fallbakcSprite = null)
        {
            if (string.IsNullOrEmpty(_url)) return;
            failureCallback = onFail;
            sprite = _sprite;
            SetUrl(_url);
            cr = CommonMono.instance.StartCoroutine(Loading(cache));
            if (fallbakcSprite != null) _sprite = fallbakcSprite;
            else _sprite = transparentSprite;
        }

        public Image(string _url, System.Action<Texture2D> _callback, bool cache = true, System.Action<string> onFail = null)
        {
            if (string.IsNullOrEmpty(_url)) return;
            failureCallback = onFail;
            callback = _callback;
            SetUrl(_url);
            cr = CommonMono.instance.StartCoroutine(Loading(cache));
            if (_callback != null) _callback(transparentTexture);
        }

        public Image(string _url, System.Action<Sprite> _callback, bool cache = true, System.Action<string> onFail = null, Sprite fallbakcSprite = null)
        {
            if (string.IsNullOrEmpty(_url)) return;
            failureCallback = onFail;
            callbackSprite = _callback;
            SetUrl(_url);
            cr = CommonMono.instance.StartCoroutine(Loading(cache));
            if (_callback != null)
            {
                if (fallbakcSprite != null) _callback(fallbakcSprite);
                else _callback(transparentSprite);
            }
        }

        IEnumerator Loading(bool cache)
        {
            bool local = false;
            if (cache && File.Exists(localUrl)) local = true;

            req = new WWW(local ? ("file:///" + localUrl) : webUrl);
            //Debug.Log(local ? ("file:///" + localUrl) : webUrl);
            //            UnityWebRequest req = new UnityWebRequest(local ? ("file:///"+localUrl) : webUrl);
            yield return req;
            //            while (!req.isDone || !req.isError)
            //            {
            //                yield return null;
            //                Debug.Log(req.downloadProgress);
            //            }
            //Debug.Log(req.error);
            if (string.IsNullOrEmpty(req.error))
            {
                if (req.texture.height > 9)
                {
                    req.LoadImageIntoTexture(tex);
                    tex.wrapMode = TextureWrapMode.Clamp;
                    tex.filterMode = FilterMode.Bilinear;
                    //                tex = DownloadHandlerTexture.GetContent(req);

                    if (uiImg != null)
                        LoadImage();
                    if (sprite != null)
                        LoadSprite();
                    if (callback != null)
                        callback(tex);
                    if (callbackSprite != null)
                        callbackSprite(Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f)));

                    if (!local && cache)
                    {
                        if (!Directory.Exists(localDir))
                        {
                            Directory.CreateDirectory(localDir);
                        }
                        File.WriteAllBytes(localUrl, tex.EncodeToPNG());
                    }
                }
                else
                {
                    if (failureCallback != null) failureCallback("tooSmall");
                }
            }
            else
            {
                //                Debug.Log(req.error);
                if (failureCallback != null) failureCallback(req.error);
            }

            req.Dispose();
        }

        void LoadImage()
        {
            uiImg.sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
        }

        void LoadSprite()
        {
            sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
        }

        public void Cancel()
        {
            if (req != null) req.Dispose();
            if (cr != null) CommonMono.instance.StopCoroutine(cr);
        }
    }

    public static void SaveImage(string url, Texture2D t)
    {
        string localUrl = Application.persistentDataPath + url.Substring(url.IndexOf('/', 8));
        string localDir = "";
        StringBuilder sb = new StringBuilder(localUrl);
        for (int i = localUrl.Length - 1; i > 0; i--)
        {
            if (sb[i].Equals('/'))
            {
                sb.Remove(i, sb.Length - i);
                localDir = sb.ToString();
                break;
            }
        }

        if (!Directory.Exists(localDir))
        {
            Directory.CreateDirectory(localDir);
        }
        File.WriteAllBytes(localUrl, t.EncodeToPNG());
    }

    public static string GetSavedImageURI(string url)
    {
        try
        {
            string localUrl = Application.persistentDataPath + url.Substring(url.IndexOf('/', 8));
            if (File.Exists(localUrl))
            {
                return "file:///" + localUrl;
            }
        }
        catch { }
        return string.Empty;
    }
}

/// <summary>
/// basically running the coroutine and firing two functions with response, one for error and one for successfull response
/// </summary>
public class RestAPI
{

    public class Get
    {
        string url;
        System.Action<string> successCallback;
        System.Action<string> failureCallback;
        Coroutine cr;
        float t = 0;

        public Get(string _url, System.Action<string> _successCallback = null, System.Action<string> _failureCallback = null)
        {
            url = _url;
            successCallback = _successCallback;
            failureCallback = _failureCallback;
            cr = CommonMono.instance.StartCoroutine(this.WWW_Text());
        }

        public void Cancel()
        {
            CommonMono.instance.StopCoroutine(cr);
        }

        IEnumerator WWW_Text()
        {
            WWW req = new WWW(url);
            Debug.Log(url);

            while (!req.isDone)
            {
                if (t > 5)
                {
                    req.Dispose();
                    if (failureCallback != null) failureCallback("TIMEOUT");
                    yield break;
                }
                t += Time.deltaTime;
                yield return null;
            }

            if (successCallback != null) if (string.IsNullOrEmpty(req.error)) successCallback(req.text);
            if (failureCallback != null) if (!string.IsNullOrEmpty(req.error)) failureCallback(req.error);
            req.Dispose();
        }
    }

    public class Post
    {
        string url;
        WWWForm form;
        System.Action<string> successCallback;
        System.Action<string> failureCallback;
        float t = 0;

        public Post(string _url, Dictionary<string, string> _params, System.Action<string> _successCallback = null, System.Action<string> _failureCallback = null)
        {
            form = new WWWForm();
            foreach (var i in _params)
            {
                form.AddField(i.Key, i.Value);
            }
            url = _url;
            successCallback = _successCallback;
            failureCallback = _failureCallback;
            CommonMono.instance.StartCoroutine(this.WWW_Text());
#if UNITY_EDITOR
            Debug.LogFormat("REST.POST: {0}", url);
#endif
        }
        public Post(string _url, WWWForm _form, System.Action<string> _successCallback = null, System.Action<string> _failureCallback = null)
        {
            url = _url;
            form = _form;
            successCallback = _successCallback;
            failureCallback = _failureCallback;
            CommonMono.instance.StartCoroutine(this.WWW_Text());
#if UNITY_EDITOR
            Debug.LogFormat("REST.POST: {0}", url);
#endif
        }

        IEnumerator WWW_Text()
        {
            WWW req = new WWW(url, form);

            while (!req.isDone)
            {
                if (t > 5)
                {
                    req.Dispose();
                    if (failureCallback != null) failureCallback("TIMEOUT");
                    yield break;
                }
                t += Time.deltaTime;
                yield return null;
            }

#if UNITY_EDITOR
            Debug.LogFormat("REST.POST.RESPONSE: {0}", req.text);
            if (!string.IsNullOrEmpty(req.error)) Debug.LogFormat("REST.POST.ERROR: {0}", req.error);
#endif
            if (successCallback != null) if (string.IsNullOrEmpty(req.error)) successCallback(req.text);
            if (failureCallback != null) if (!string.IsNullOrEmpty(req.error)) failureCallback(req.error);
            req.Dispose();
        }
    }

    public class Post<T>
    {
        string url;
        WWWForm form;
        System.Action<T> successCallback;
        System.Action<string> failureCallback;

        public Post(string _url, WWWForm _form, System.Action<T> _successCallback = null, System.Action<string> _failureCallback = null)
        {
            url = _url;
            form = _form;
            successCallback = _successCallback;
            failureCallback = _failureCallback;
            CommonMono.instance.StartCoroutine(this.WWW_Text());
        }

        IEnumerator WWW_Text()
        {
            WWW req = new WWW(url, form);
            yield return req;
            T response = JsonUtility.FromJson<T>(req.text);
            if (successCallback != null) if (string.IsNullOrEmpty(req.error)) successCallback(response);
            if (failureCallback != null) if (!string.IsNullOrEmpty(req.error)) failureCallback(req.error);
        }
    }

    static Dictionary<string, string> savedStrings = new Dictionary<string, string>();
    public class SaveStringValue
    {
        string url;
        WWWForm form;
        string keyToAccess, keyInResponse;

        public SaveStringValue(string _url, WWWForm _form, string _keyToAccess, string _keyInResponse)
        {
            if (savedStrings.ContainsKey(_keyToAccess)) return;
            url = _url;
            form = _form;
            keyToAccess = _keyToAccess;
            keyInResponse = _keyInResponse;
            savedStrings.Add(keyToAccess, "");
            CommonMono.instance.StartCoroutine(this.WWW_StringVal());
        }

        IEnumerator WWW_StringVal()
        {
            WWW req = new WWW(url, form);
            yield return req;
            var response = JSON.Parse(req.text);
            savedStrings[keyToAccess] = response[keyInResponse].ToString().Replace("\"", string.Empty);
        }
    }
    public static string GetSavedStringValue(string _keyToAccess)
    {
        if (savedStrings.ContainsKey(_keyToAccess)) return "";
        return savedStrings[_keyToAccess];
    }

    public class EncryptedPassword
    {
        string url;
        WWWForm form;
        System.Action<string> successCallback;
        System.Action<string> failureCallback;

        Coroutine mainCoroutine;
        Coroutine timeoutCoroutine;

        public class ReponseFormat
        {
            public string status;
            public string message;
        }

        public EncryptedPassword(string _url, string _rawPassword, System.Action<string> _successCallback = null, System.Action<string> _failureCallback = null)
        {
            url = _url;
            form = new WWWForm();
            form.AddField("pass", _rawPassword);
            successCallback = _successCallback;
            failureCallback = _failureCallback;
            mainCoroutine = CommonMono.instance.StartCoroutine(this.WWW_Pass());
            timeoutCoroutine = CommonMono.instance.StartCoroutine(this.TimeoutCheck());
        }

        IEnumerator WWW_Pass()
        {
            WWW req = new WWW(url, form);
            yield return req;
            Debug.Log(req.text);
            CommonMono.instance.StopCoroutine(timeoutCoroutine);

            ReponseFormat response = JsonUtility.FromJson<ReponseFormat>(req.text);

            if (!string.IsNullOrEmpty(req.error))
            {
                if (failureCallback != null)
                    failureCallback(req.error);
            }
            else if (!response.status.Equals("true") || string.IsNullOrEmpty(response.message))
            {
                if (failureCallback != null)
                    failureCallback("WebServiceFailure: " + response.message);
            }
            else
            {
                if (successCallback != null)
                    successCallback(response.message);
            }
        }

        IEnumerator TimeoutCheck()
        {
            yield return new WaitForSeconds(5);
            CommonMono.instance.StopCoroutine(mainCoroutine);

            if (failureCallback != null)
            {
                failureCallback("WebServiceTimeout");
            }
        }
    }
}

[System.Serializable]
public class uEnumCond<T>
{
    public T cond;
    public GameObject[] objects;
    //    public UISwitch[] switches;
}

//[System.Serializable]
//public class uEnum<T, T2> where T2 : uEnumCond<T>
//{
//    [UnityEngine.SerializeField]
//    T m_value;
//    public T value
//    {
//        get { return m_value; }
//        set
//        {
//            m_value = value;

//            if (conds != null)
//            {
//                List<GameObject> activeObjects = new List<GameObject>();
//                for (int i = 0; i < conds.Length; i++)
//                {
//                    if (conds[i].cond.Equals(value)) activeObjects.AddRange(conds[i].objects);
//                }

//                for (int i = 0; i < conds.Length; i++)
//                {
//                    for (int o = 0; o < conds[i].objects.Length; o++)
//                    {
//                        if (conds[i].objects[o] == null) continue;
//                        if (conds[i].cond.Equals(value) || activeObjects.Contains(conds[i].objects[o]))
//                        {
//                            if (!conds[i].objects[o].activeSelf) conds[i].objects[o].SetActive(true);
//                        }
//                        else
//                        {
//                            if (conds[i].objects[o].activeSelf) conds[i].objects[o].SetActive(false);
//                        }
//                    }
//                    for (int o = 0; o < conds[i].switches.Length; o++)
//                    {
//                        if (conds[i].switches[o] == null) continue;
//                        if (conds[i].cond.Equals(value))
//                        {
//                            if (!conds[i].switches[o].isOn) conds[i].switches[o].Set(true);
//                        }
//                        else
//                        {
//                            if (conds[i].switches[o].isOn) conds[i].switches[o].Set(false);
//                        }
//                    }
//                    //    conds[i].objects.SetActiveCollection(false);
//                    //if (conds[i].cond.Equals(value))
//                    //{
//                    //    conds[i].objects.SetActiveCollection(true);
//                    //}
//                }
//            }
//            if (Updated != null) Updated();
//        }
//    }

//    public event Action Updated;
//    public T2[] conds;


//    public virtual void SetValue(string s)
//    {
//        T[] vals = EnumExt.GetValues<T>();
//        if (!string.IsNullOrEmpty(s))
//        {
//            foreach (var val in vals)
//            {
//                if (val.ToString().Equals(s))
//                {
//                    value = val;
//                    return;
//                }
//            }
//            foreach (var val in vals)
//            {
//                if (val.ToString().Equals(s, StringComparison.CurrentCultureIgnoreCase))
//                {
//                    value = val;
//                    return;
//                }
//            }
//            int cw = 0; //contains weight
//            int tcw = 0; //temp contains weight
//            T tv = default(T); //temp value
//            foreach (var val in vals)
//            {
//                if (s.Contains(val.ToString()))
//                {
//                    tcw = val.ToString().Length;
//                    if (tcw > cw)
//                    {
//                        //Debug.LogFormat("contains 1 {0}", val.ToString());
//                        cw = tcw;
//                        tv = val;
//                    }
//                }
//                else if (val.ToString().Contains(s))
//                {
//                    tcw = s.Length;
//                    if (tcw > cw)
//                    {
//                        //Debug.LogFormat("contains 2 {0}", s);
//                        cw = tcw;
//                        tv = val;
//                    }
//                }
//            }
//            foreach (var val in vals)
//            {
//                if (s.ToUpper().Contains(val.ToString().ToUpper()))
//                {
//                    tcw = val.ToString().Length;
//                    if (tcw > cw)
//                    {
//                        //Debug.LogFormat("contains 3 {0}", val.ToString());
//                        cw = tcw;
//                        tv = val;
//                    }
//                }
//                else if (val.ToString().ToUpper().Contains(s.ToUpper()))
//                {
//                    tcw = s.Length;
//                    if (tcw > cw)
//                    {
//                        //Debug.LogFormat("contains 4 {0}", s.ToUpper());
//                        cw = tcw;
//                        tv = val;
//                    }
//                }
//            }
//            foreach (var val in vals)
//            {
//                string s1 = val.ToString().Replace("_", "").Replace(" ", "").ToUpper();
//                string s2 = s.ToString().Replace("_", "").Replace(" ", "").ToUpper();
//                if (s1.Contains(s2))
//                {
//                    tcw = s2.Length;
//                    if (tcw > cw)
//                    {
//                        //Debug.LogFormat("contains 5 {0}", s2);
//                        cw = tcw;
//                        tv = val;
//                    }
//                }
//                else if (s2.Contains(s1))
//                {
//                    tcw = s1.Length;
//                    if (tcw > cw)
//                    {
//                        //Debug.LogFormat("contains 6 {0}", s1);
//                        cw = tcw;
//                        tv = val;
//                    }
//                }
//            }
//            if (cw > 0)
//            {
//                value = tv;
//                return;
//            }
//        }

//        value = vals.Last();
//    }
//}

public class HexColor
{
    Color fallbackColor = Color.white;
    Color parsedColor = Color.white;
    string code;
    bool parsed;
    public Color color
    {
        get
        {
            if (!parsed)
            {
                if (!code[0].Equals('#')) code = "#" + code;
                ColorUtility.TryParseHtmlString(code, out parsedColor);
            }
            return parsedColor;
        }
    }

    public HexColor(string _code)
    {
        code = _code;
    }

    public HexColor(string _code, Color _fallbackColor)
    {
        code = _code;
        fallbackColor = _fallbackColor;
    }


}

[Serializable]
public class FloatRange
{
    public float min, max;
    public float minLimit = 0, maxLimit = 1;
    public float randomValue
    {
        get
        {
            return UnityEngine.Random.Range(min, max);
        }
    }
    public bool IsInRange(float v)
    {
        return (v >= min && v <= max);
    }

    public override bool Equals(object obj)
    {
        if (obj != null && obj is FloatRange)
        {
            FloatRange o = (FloatRange)obj;
            return ((o.min == min) && (o.max == max));
        }
        return base.Equals(obj);
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }
}

[Serializable]
public class IntRange
{
    public int min, max;
    public int randomValue
    {
        get
        {
            return UnityEngine.Random.Range(min, max);
        }
    }
    public bool IsInRange(int v)
    {
        return (v >= min && v <= max);
    }

    public override bool Equals(object obj)
    {
        if (obj != null && obj is IntRange)
        {
            IntRange o = (IntRange)obj;
            return ((o.min == min) && (o.max == max));
        }
        return base.Equals(obj);
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }
}



[System.Serializable]
public class UnityEventInt : UnityEngine.Events.UnityEvent<int> { }

[System.Serializable]
public class UnityEventString : UnityEngine.Events.UnityEvent<string> { }