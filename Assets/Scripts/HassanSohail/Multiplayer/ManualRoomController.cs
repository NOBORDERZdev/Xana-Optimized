using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManualRoomController : MonoBehaviour
{
    [SerializeField] GameObject RoomsView;
    [SerializeField] GameObject RoomBtn;
    [SerializeField] List<RoomBtn> roomBtnList;
    private void OnEnable()
    {
        HideRoomList();
    }

    public void InitiateRoomBtn(string room, string count)
    {
        if (!RoomsView.activeInHierarchy)
        {
            RoomsView.SetActive(true);
        }
        GameObject btn = Instantiate(RoomBtn, RoomsView.transform);
        btn.GetComponent<ManualRoomJoin>().Int(room, count);
        roomBtnList.Add(new RoomBtn ( room,btn) );
    }

    public void HideRoomList()
    {
        if (RoomsView.transform.childCount > 0)
        {
            for (int i = RoomsView.transform.childCount-1; i >= 0; i--)
            {
                UnityEngine.Object.Destroy(RoomsView.transform.GetChild(i).gameObject);
            }
        }
        RoomsView.SetActive(false);
    }

    public void UpdateRoomBtn(string name, string count) {

        GameObject tempBtn = roomBtnList.Find(p => p.RoomName.Equals(name)).BtnObj;
        tempBtn.GetComponent<ManualRoomJoin>().Int(name, count);
    }

    public void DeleteRoomBtn(string name) {
        RoomBtn tempBtn = roomBtnList.Find(p => p.RoomName.Equals(name));
        Destroy(tempBtn.BtnObj);
        roomBtnList.Remove(tempBtn);
    }
}

[Serializable]
class RoomBtn {
    public string RoomName;
    public GameObject BtnObj;

    public RoomBtn(string name , GameObject obj) {
        RoomName = name;
        BtnObj = obj;
    }
}
