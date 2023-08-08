/// <summary>
/// This Script will be used to give punch power etc wrt NFT gloves etc bought by the player
/// </summary>
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomParameters : MonoBehaviour
{
    public float playerSpeed = 2f, range = 0.2f, playerHealth = 200f,
        punchPower = 5f, kickPower = 8f, grabDamage = 20f, specialBarFill = 0.2f,
        damageIncTime = 0f;

    public static CustomParameters instance;

    private void Awake()
    {
        if (instance == null) { instance = this; }
        playerSpeed = 1f;
    }
    void SetGloves(int id)
    {
        switch (id)
        {
            case 1:
                punchPower = punchPower + id;
                break;
            case 2:
                punchPower = punchPower + id;
                break;
            case 3:
                punchPower = punchPower + id;
                break;
            case 4:
                punchPower = punchPower + id;
                break;
            case 5:
                punchPower = punchPower + id;
                break;
            case 6:
                punchPower = punchPower + id;
                break;
            case 7:
                punchPower = punchPower + id;
                break;
            case 8:
                punchPower = punchPower + id;
                break;
            case 9:
                punchPower = punchPower + id;
                break;
            case 10:
                punchPower = punchPower + id;
                break;
            default:
                punchPower = 5f;
                break;
        }
    }

    void SetPant(int id)
    {
        switch (id)
        {
            case 1:
                playerSpeed = playerSpeed + (playerSpeed * 0.05f);
                break;
            case 2:
                break;
            case 3:
                range = 2 * range;
                break;
            case 4:
                playerHealth = playerHealth + (playerHealth * 0.02f);
                break;
            case 5:
                kickPower = kickPower + 3;
                break;
            case 6:
                grabDamage = grabDamage + 5;
                break;
            case 7:
                playerHealth = playerHealth + (playerHealth * 0.05f);
                break;
            case 8:
                punchPower = punchPower + (punchPower * 0.02f);
                kickPower = kickPower + (kickPower * 0.02f);
                break;
            case 9:
                playerSpeed = playerSpeed + (playerSpeed * 0.08f);
                break;
            case 10:
                kickPower = kickPower + 8;
                break;
            case 11:
                punchPower = punchPower + (punchPower * 0.05f);
                kickPower = kickPower + (kickPower * 0.05f);
                break;
            case 12:
                punchPower = punchPower + (punchPower * 0.03f);
                kickPower = kickPower + (kickPower * 0.03f);
                break;
            case 13:
                break;
            case 14:
                playerHealth = playerHealth + (playerHealth * 0.1f);
                break;
            case 15:
                punchPower = punchPower + (punchPower * 0.05f);
                kickPower = kickPower + (kickPower * 0.05f);
                grabDamage = grabDamage + (grabDamage * 0.05f);
                break;
        }
    }

    void SetTattoo(int id)
    {
        switch (id)
        {
            case 1:
                specialBarFill = specialBarFill + (specialBarFill * 0.05f);
                break;
            case 2:
                punchPower = punchPower + (punchPower * 0.05f);
                kickPower = kickPower + (kickPower * 0.05f);
                break;
            case 3:
                punchPower = punchPower + (punchPower * 0.02f);
                kickPower = kickPower + (kickPower * 0.02f);
                break;
            case 4:
                break;
            case 5:
                break;
        }
    }

    void SetNeckChain(int id)
    {
        switch (id)
        {
            case 1:
                punchPower = punchPower + (punchPower * 0.02f);
                kickPower = kickPower + (kickPower * 0.02f);
                break;
            case 2:
                break;
            case 3:
                damageIncTime = 10f;
                break;
            case 4:
                break;
            case 5:
                grabDamage = grabDamage + (grabDamage * 0.02f);
                break;
            case 6:
                break;
            case 7:
                break;
            case 8:
                break;
            case 9:
                break;
            case 10:
                punchPower = punchPower + (punchPower * 0.05f);
                kickPower = kickPower + (kickPower * 0.05f);
                break;
            case 11:
                grabDamage = grabDamage + (grabDamage * 0.05f);
                break;
            case 12:
                break;
        }
    }

    void SetMuscles(int id)
    {
        switch (id)
        {
            case 1:
                punchPower = punchPower + (punchPower * 0.05f);
                kickPower = kickPower + (kickPower * 0.05f);
                grabDamage = grabDamage + (grabDamage * 0.05f);
                break;
            case 2:
                punchPower = punchPower - (punchPower * 0.02f);
                kickPower = kickPower - (kickPower * 0.02f);
                grabDamage = grabDamage - (grabDamage * 0.02f);
                break;
            case 3:
                punchPower = 5f;
                kickPower = 8f;
                grabDamage = 20f;
                break;
        }
    }
}
