using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GlobalConstants;

namespace PMY {
    public class PMYLobby_FbEvents : MonoBehaviour
    {
        public int portalIndex = 1;

        // Start is called before the first frame update
        void Start()
        {

        }

        public void CorporateRoomEvent()
        {
            string eventName = FirebaseTrigger.Corporate_Room.ToString();
            SendFirebaseEvent(eventName + portalIndex);
        }

        public void GalleryEvent()
        {
            string eventName = FirebaseTrigger.Gallery.ToString();
            SendFirebaseEvent(eventName);
        }

    }
}
