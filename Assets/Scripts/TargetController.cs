using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TargetController : MonoBehaviour
{
    public GameController gameController;
    public PlayerController playerController;

    public void degis()
    {
        int rand = Random.Range(0 , playerController.spawnPoints.Count);

        if(playerController.spawnPoints.Count > 0)
        {
            transform.position = playerController.spawnPoints[rand];
        }
    }
}
