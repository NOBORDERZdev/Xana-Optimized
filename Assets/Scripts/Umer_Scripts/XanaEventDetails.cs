using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[System.Serializable]
public class XanaEventDetails
{
    public static XanaEventDetails eventDetails;
    public bool DataIsInitialized = false;
    public int id;
    public string name;
    public string eventType;
    public string description;
    public string thumbnail;
    public string mainImg;
    public string overview;
    public string youtubeUrl;
    public string eventLink;
    public string startTime;
    public string endTime;
    public int museumId;
    public int environmentId;
    public int createdBy;
    public bool isPublic;
    public bool messages;
    public bool voiceChat;
    public bool emotes;
    public bool selfie;
    public int approvalStatus;
    public int allowedNFTs;
    public bool isCreated;
    public string unityDeepLink;
    public string repeat;
    public string rpt_freq;
    public string occurrences;
    public string occurence_dates;
    public bool recurrence;
    public string[] recurrence_dates;
    public int recurrence_count;
    public string recurrence_until;
    public string recurrence_frequency;
    public int recurrence_interval;
    public double duration;
    public string xana_world_id;
    public string env_class;
    public bool youtubeUrl_isActive;
    public string createdAt;
    public string updatedAt;
    public List<EventUserRoles> eventsUserRoles;
    public string museumName;
    public string environmentName;
    public string userLimit;
}


[System.Serializable]
public class EventUserRoles
{
    public int id;
    public int customEventId;
    public string key;
    public bool value;
    public string createdAt;
    public string updatedAt;
}

[System.Serializable]
public class Environment2
{
    public int id;
    public string name;
}
[System.Serializable]
public class EventDataDetails
{
    public bool success;
    public XanaEventDetails data;
    public string msg;
}
[System.Serializable]
public class EnvironmentDetails
{
    public bool success;
    public XanaEnvironmentDetails data;
    public string msg;
}
[System.Serializable]
public class XanaEnvironmentDetails
{
    public string name;
    public int user_limit;
    public string entityType;
}