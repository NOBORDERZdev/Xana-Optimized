using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;

public class AnimatedTextures : MonoBehaviour
{
    public string Filename;

    private List<Texture2D> mFrames = new List<Texture2D>();
    private List<float> mFrameDelay = new List<float>();

    private int mCurFrame = 0;
    private float mTime = 0.0f;

    public RawImage image;
    IEnumerator Start()
    {
        //if (string.IsNullOrWhiteSpace(Filename))
        //{
        //    return;
        //}

        var path = Path.Combine("jar:file://" + Application.dataPath + "!/assets/", Filename);
        //"jar:file://" + Application.dataPath + "!/assets/"
        //https://xanalia.s3.amazonaws.com/award/1635592727516.gif

        UnityWebRequest www = UnityWebRequest.Get("https://xanalia.s3.amazonaws.com/award/1635592727516.gif");
        yield return www.SendWebRequest();

        byte[] imageData = www.downloadHandler.data;

        using (var decoder = new MG.GIF.Decoder(imageData))
        {
            var img = decoder.NextImage();

            while (img != null)
            {
                mFrames.Add(img.CreateTexture());
                mFrameDelay.Add(img.Delay / 1000.0f);
                img = decoder.NextImage();
            }
        }
        image.texture = mFrames[0];
    }

    void Update()
    {
        if (mFrames == null)
        {
            return;
        }

        mTime += Time.deltaTime;

        if (mTime >= mFrameDelay[mCurFrame])
        {
            mCurFrame = (mCurFrame + 1) % mFrames.Count;
            mTime = 0.0f;

            image.texture = mFrames[mCurFrame];

        }
    }
}

