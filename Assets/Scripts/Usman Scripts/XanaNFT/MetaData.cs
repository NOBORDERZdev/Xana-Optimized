namespace XanaNFT
{
    [System.Serializable]
    public class MetaData
    {
        public string name;
        public string description;
        public string image;
        public string thumbnft;
        public Properties properties;
        public int totalSupply;
        public string externalLink;
        
    }
    [System.Serializable]
    public class CreatorDetails {
        public string _id;
        public string username;
        public string email;
        public string role;
        public int following;
        public int followers;
        public object createdAt;
        public object emailVerificationToken;
        public bool emailVerified;
        public string about;
        public string en_about;
        public string ja_about;
        public string ko_about;
        public string zh_about;
        public string zh_ch_about;
        public string referralCode;
        public bool dataShared;
        public string address;
        public string firstName;
        public string lastName;
        public Links links;
        public string name;
        public string phoneNumber;
        public string title;
        public int transalte_again;
        public bool resetPasswordStart;
        public string signUpRef;
        public string profile_image;
        public string oldusername;
    }

    [System.Serializable]
    public class Links
    {
        public string facebook;
        public string website;
        public string discord;
        public string twitter;
        public string instagram;
        public string youtube;
    }

}