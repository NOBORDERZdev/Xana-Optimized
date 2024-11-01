using Photon.Pun;
using System.Collections.Generic;
using System;

public class CommentSystem : MonoBehaviourPunCallbacks
{
    private GalleryData galleryData;
    private DataManager dataManager;

    private void Start()
    {
        dataManager = GetComponent<DataManager>(); // FindObjectOfType<DataManager>();
        galleryData = dataManager.LoadGalleryData();

        // Load existing data on each player's start
       // SyncExistingDataWithNewPlayers();
    }

    //private void SyncExistingDataWithNewPlayers()
    //{
    //    // Send all frames, posts, and comments to new players
    //    foreach (var frame in galleryData.Frames)
    //    {
    //        foreach (var post in frame.Posts)
    //        {
    //            //photonView.RPC(nameof(ReceivePost), RpcTarget.Others, frame.FrameID, post.PostID, post.Content);
    //            ReceivePost(frame.FrameID, post.PostID, post.Content);
    //            foreach (var comment in post.Comments)
    //            {
    //                //photonView.RPC(nameof(ReceiveComment), RpcTarget.Others, frame.FrameID, post.PostID, comment.UserID, comment.CommentText, comment.Timestamp.ToString());
    //                ReceiveComment(frame.FrameID, post.PostID, comment.UserID, comment.CommentText, comment.Timestamp.ToString());
    //            }
    //        }
    //    }
    //}

    // Adding a new post, sync across players
    public void AddPost(string frameID, string postContent, string postID)
    {
        Post newPost = new Post { PostID = postID, FrameID = frameID, Content = postContent, Comments = new List<Comment>() };

        FrameData frame = galleryData.Frames.Find(f => f.FrameID == frameID);
        if (frame == null)
        {
            frame = new FrameData { FrameID = frameID, Posts = new List<Post>() };
            galleryData.Frames.Add(frame);
        }
        
        frame.Posts.Add(newPost);
        //dataManager.SaveGalleryData(galleryData);

        //photonView.RPC(nameof(ReceivePost), RpcTarget.All, frameID, postID, postContent);
        //ReceivePost(frameID, postID, postContent);
    }

    //[PunRPC]
    //private void ReceivePost(string frameID, string postID, string postContent)
    //{
    //    Post newPost = new Post { PostID = postID, FrameID = frameID, Content = postContent, Comments = new List<Comment>() };
    //    FrameData frame = galleryData.Frames.Find(f => f.FrameID == frameID);
    //    if (frame == null)
    //    {
    //        frame = new FrameData { FrameID = frameID, Posts = new List<Post>() };
    //        galleryData.Frames.Add(frame);
    //    }
    //    frame.Posts.Add(newPost);
    //}

    // Adding a new comment, sync across players
    public void AddComment(string frameID, string postID, string userID, string commentText)
    {
        Comment newComment = new Comment { UserID = userID, CommentText = commentText, Timestamp = DateTime.Now };

        FrameData frame = galleryData.Frames.Find(f => f.FrameID == frameID);
        Post post = frame?.Posts.Find(p => p.PostID == postID);
        post?.Comments.Add(newComment);

        //dataManager.SaveGalleryData(galleryData);

        //photonView.RPC(nameof(ReceiveComment), RpcTarget.All, frameID, postID, userID, commentText, newComment.Timestamp.ToString());
        //ReceiveComment(frameID, postID, userID, commentText, newComment.Timestamp.ToString());
    }

    //[PunRPC]
    //private void ReceiveComment(string frameID, string postID, string userID, string commentText, string timestamp)
    //{
    //    Comment newComment = new Comment { UserID = userID, CommentText = commentText, Timestamp = DateTime.Parse(timestamp) };

    //    FrameData frame = galleryData.Frames.Find(f => f.FrameID == frameID);
    //    Post post = frame?.Posts.Find(p => p.PostID == postID);
    //    post?.Comments.Add(newComment);
    //}

}
