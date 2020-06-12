using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public GameController gameController;
    public Transform startPos;


    private void Update() {
        if(gameController.gameState == false)   // oyun bittiği anda başlangıç pozisyonuna geç.
        {
            transform.position = startPos.position;
        }
    }

     void OnCollisionEnter2D(Collision2D col) 
    {
            if(col.gameObject.tag == "PlayerTag")
            {
                gameController.finishGame(false);            
            }
    }


}
