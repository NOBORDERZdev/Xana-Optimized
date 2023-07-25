namespace XanaNFT
{
    [System.Serializable]
    public class TokenDetails
    {
        public string _id;
        public string transactionHash;
        public ReturnValues returnValues;
        public string @event;
        public MetaData metaData;
        public string tokenId;
        public int copies;
        public string chain;
        public bool award;
        public string en_nft_description;
        public string en_nft_name;
        public string ja_nft_description;
        public string ja_nft_name;
        public string ko_nft_description;
        public string ko_nft_name;
        public string zh_ch_nft_description;
        public string zh_ch_nft_name;
        public string zh_nft_description;
        public string zh_nft_name;
        public int timestamp;
        public string mainTokenId;
        public bool tokenCollectionFlag;
        public bool isAddedToMuseum;
        public string imagekit;
    }

}