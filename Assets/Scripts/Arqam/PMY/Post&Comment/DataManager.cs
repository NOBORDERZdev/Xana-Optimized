using System.IO;
using UnityEngine;
using System.Collections.Generic;
using System;

public class DataManager : MonoBehaviour
{
    private string filePath;

    private void Awake()
    {
        filePath = Path.Combine(Application.persistentDataPath, "galleryData.json");
    }

    // Method to save all frames' data
    public void SaveGalleryData(GalleryData galleryData)
    {
        string jsonData = JsonUtility.ToJson(galleryData, true);
        File.WriteAllText(filePath, jsonData);
    }

    // Method to load all frames' data
    public GalleryData LoadGalleryData()
    {
        if (File.Exists(filePath))
        {
            string jsonData = File.ReadAllText(filePath);
            return JsonUtility.FromJson<GalleryData>(jsonData);
        }
        else
        {
            return new GalleryData { Frames = new List<FrameData>() };
        }
    }
}

[System.Serializable]
public class Comment
{
    public string UserID;
    public string CommentText;
    public DateTime Timestamp;
}

[System.Serializable]
public class Post
{
    public string PostID;
    public string FrameID;  // To associate the post with the frame
    public string Content;  // Post content, e.g., image URL, video link, etc.
    public List<Comment> Comments;  // List of comments on this post
}

[System.Serializable]
public class FrameData
{
    public string FrameID;
    public List<Post> Posts;  // List of posts in this frame
}

[System.Serializable]
public class GalleryData
{
    public List<FrameData> Frames;
}