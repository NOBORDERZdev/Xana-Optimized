using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChinaConstantGods
{
    public static string REMMBERED = "remmbered";
    public static string REMMBEREDLOGIN = "remmberedLogin";
    public static string AUTH = "token";
    public static string USERNOTFOUND = "user not found";
    public static string KYCVALUE = "Kycvalue";
    public static string KYCSTATUS="KycStatus";
    public static string USERID="userid" ;
    public static string MOBILENUMBER= "mobile";

    #region API'S

    public static string API_BASEURL = "https://api.xanalia.cn/";
    public static string APPREGISTERPOST = "u/api/user/appRegister";
    public static string LOGINPOST = "auth/login";
    public static string VERIFICATIONPOST ="u/api/user/appVerifyKYC";
    public static string GETVERIFICATIONCODE = "u/api/message/send";
    public static string USERINFO = "u/api/user/info";
    public static string ALPHAPASS = "goods/api/scene/user/scenesName";
    

    #endregion
}
