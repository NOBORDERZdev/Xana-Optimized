using UnityEngine;

public class RFMEconomyManager : MonoBehaviour
{
    public static int money = 150;

    public static void RemoveMoney(int amount)
    {
        money -= amount;
    }


    public static bool CanAfford(int amount)
    {
        return money >= amount;
    }

    public static bool PayToPlayRFM(int amount)
    {
        if (CanAfford(amount))
        {
            RemoveMoney(amount);
            return true;
        }

        return false;
    }
}
