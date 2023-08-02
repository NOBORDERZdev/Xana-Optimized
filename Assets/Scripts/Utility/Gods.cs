using UnityEngine;

using UnityEngine.Networking;
using System.Text;
using System.Threading.Tasks;
using BestHTTP.JSON.LitJson;

public enum ConditionOperator { And, Or }
public enum ActionOnConditionFail { DontDraw, JustDisable }

public static class Gods
{
    public static int GetAspectRatio()
    {
        float aspect = Camera.main.aspect;

        //BELOW iPHONE X...9:16
        if (aspect >= 0.55f && aspect <= 0.57f)
        {
            return 0;
        }
        //iPHONE X AND ABOVE...18:37
        else if (aspect >= 0.45f && aspect <= 0.47f)
        {
            return 1;
        }
        //TAB...3:4
        else//if (aspect >= 0.74f && aspect <= 0.76f)
        {
            return 2;
        }
    }

    public static T DeserializeJSON<T>(string jsonString) where T : class
    {
        object data = JsonMapper.ToObject<T>(jsonString);
        return (T)data;
    }

    public static string SerializeJSON<T>(T modelData) where T : class
    {
        return JsonMapper.ToJson(modelData);
    }

}
