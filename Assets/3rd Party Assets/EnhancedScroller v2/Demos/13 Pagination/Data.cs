namespace EnhancedScrollerDemos.Pagination
{
    /// <summary>
    /// Super simple data class to hold information for each cell.
    /// </summary>
    public class Data
    {
        public string someText;
    }


    public class WorldItemDetail
    {
        public string IdOfWorld;
        public string EnvironmentName;
        public string WorldDescription;
        public string ThumbnailDownloadURL;
        public string ThumbnailDownloadURLHigh;
        //public string CreatorName;
        public string CreatedAt;
        public string UserLimit;
        public string UserAvatarURL;
        public string UpdatedAt = "00";
        public string EntityType = "None";
        public string BannerLink;
        public int PressedIndex;
        public string[] WorldTags;
        public string Creator_Name;
        public string CreatorAvatarURL;
        public string CreatorDescription;
        public string WorldVisitCount;
        public bool isFavourite;
    }

}