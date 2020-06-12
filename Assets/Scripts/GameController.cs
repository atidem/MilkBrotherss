using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    public bool gameState;                  // true:oyun başladı/devam ediyor, false:oyun bitti
    public int gameCount = 0;               // oyunun kaçıncı kez oynandığını tutar.
    public float gameScore = 0;             // her oynanış sonunda hesaplanan puan. playerController scriptinde her saniye -0.5 olarak güncellenir. Oyun sonunda kazanmak veya kayıp bağlı olarak +50 veya -50 olarak güncellenir
    public int gameSecond = 0;

    public float penaltyScore = -0.5f;
    public float TargetScore = 50;          // target'a ulaşırsa alacağı (+) puan. -EDITORDEN DEGISTIRILEBILIR-
    public float EnemyScore = -50;          // enemy tarafından yakalanırsa alacağı (-) puan. -EDITORDEN DEGISTIRILEBILIR-
    
    float sec;                              // her oynanış içi nsaniye sayacı

    public PlayerController player;


    float enmDis=1000.0f;     // yakınlaşma kontrolü için
    float tarDis=10000.0f;    // yakınlaşma kontrolü için
    Vector3 playerLocation = new Vector3(0,0,0);
    void Update() 
    {
        GameObject playerV = GameObject.FindWithTag("PlayerTag");
        GameObject target = GameObject.FindWithTag("PlayerTargetTag");
        GameObject enmR = GameObject.Find("Enemy RIGHT");
        GameObject enmL = GameObject.Find("Enemy LEFT");
        GameObject enmU = GameObject.Find("Enemy UP");
        GameObject enmD = GameObject.Find("Enemy DOWN");
        GameObject enmC = GameObject.Find("Enemy (1)");
        GameObject[] d = { enmR, enmL, enmU, enmD,enmC};

        float minDist = 100.0f;                                                                             // using for find closest ghost's distance .
        float disTarget = Vector3.Distance(playerV.transform.position, target.transform.position);          // distance from target

        int i = 0;
        for(;i<d.Length;i++)
        {
            float dis = Vector3.Distance(playerV.transform.position,d[i].transform.position);
            if (minDist > dis) minDist = dis;
        }

        if (gameState == true)       // eğer oyun devam ediyorsa
        {
        
            gameScore += Time.deltaTime * penaltyScore;             // her saniyede puandan penaltyScore kadar (yani şimdilik 0.5) çıkar
            if (minDist <= enmDis)                                   // şuanki en yakın hayalet bir önceki en yakın hayaletten daha uzaksa ++ degilse --
                gameScore -= 0.5f;
            else
                gameScore += 0.5f;
            enmDis = minDist;

            if (tarDis <= disTarget)                                // şuanki hedef uzaklığı bir önceki hedef uzaklığından büyükse -- degilse ++
                gameScore -= 0.5f;
            else
                gameScore += 0.5f;
            tarDis = disTarget;

            if (Vector3.Distance(playerLocation, playerV.transform.position)==0)     //konum değiştiremediği her hamleyi cezalandırmak için
            {
                gameScore -= 0.7f;
            }

            playerLocation = playerV.transform.position;

            sec = sec + Time.deltaTime;                               // oyun süresini hesapla
            gameSecond = (int)sec;                                    // oyun süresini int'e çevir (çok gerekli değil daha temiz gözükmesi için)


        }
        else { player.getScreen(player.a); }

    }

    public void finishGame(bool isWin)                  // pacman hedefe ulaştığında veya canavar tarafındam yakalandığında çağırılan fonksiyon
    {
        player.getScreen(player.a);

        if (isWin == true)                               // pacman hedefe ulaştıysa fonksiyona iswin=true gelir ve puan ona göre belirlenir
            gameScore = gameScore + TargetScore;  
        else if(isWin == false)                         // pacman yakalandıysa fonksiyona isWin=false gelir ve puan ona göre belirlenir
            gameScore = gameScore + EnemyScore;

        gameCount += 1;                 // oyun sayacını 1 arttır. (kaçıncı oyun olduğunu görmek için)    
        sec = 0;                        // oyun süresi 0 yapılır.

        gameState = false;              // oyun durdurulur

        player.degis();

        player._helloRequester.info = player.a+ " " + gameScore + " " + gameState;
        player._helloRequester.fonk();
        Invoke("ResetGame", 0.1f);

    }

    public void ResetGame()
    {
        File.Delete(player.a + ".png");
        player.resetImg();
        player.getScreen(player.a);
        gameScore = 0;
        gameState = true;
        gameSecond = 0;
        Resources.UnloadUnusedAssets();
    }
}
