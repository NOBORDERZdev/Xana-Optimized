using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    private IRoomDataStrategy dataStrategy;

    // Method to set the data strategy (dependency injection)
    public void SetDataStrategy(IRoomDataStrategy strategy)
    {
        dataStrategy = strategy;
    }

    // Method to load room data
    public void LoadRoomData()
    {
        if (dataStrategy != null)
        {
            dataStrategy.LoadRoomData();
        }
    }

}
