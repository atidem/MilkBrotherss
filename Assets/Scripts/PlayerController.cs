using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using UnityEngine.Tilemaps;
using System;

public class PlayerController : MonoBehaviour
{
    public float speed;                     // player'ın hızı
    private Vector2 direction;              // player'ın yönü
    public int a;

    Rigidbody2D rb2d;
    int turn = 0;                           // dönme annındaki eşik degiskeni
    public HelloRequester _helloRequester;

    public Camera cam;


    public Tilemap tp;
    public List<Vector3> spawnPoints;

    public GameObject target;

    void Start ()
    {
        //cam = gameObject.GetComponent<Camera>();
        cam = GameObject.FindGameObjectWithTag("getScreen").GetComponent<Camera>();

        // "train.bat" for training
        // "play.bat" for play 
        System.Diagnostics.Process.Start("play.bat");

        //StartCoroutine(wait());
        System.Threading.Thread.Sleep(10000);

        resetImg();
        Time.captureFramerate = 10;
        rb2d = GetComponent<Rigidbody2D>();     

        _helloRequester = new HelloRequester();
        _helloRequester.Start();

        spawnPoints = new List<Vector3>();

        for(int x = -19 ; x < tp.size.x ; x++)        // tilemap i sonuna kadar dolaş spawnlanabileceği tüm noktaları spawnpoints'e ata
        {
            for(int y = -15 ; y < tp.size.y ; y++)
            {
                Vector3Int virtualtilepos = new Vector3Int(x,y,0);
                Vector3 realtilepos = tp.CellToWorld(virtualtilepos);
                Tile tile = (Tile)tp.GetTile(virtualtilepos);
                if(tile != null)
                {
                    spawnPoints.Add( new Vector3(realtilepos.x + .5f,realtilepos.y+.5f,realtilepos.z));
                }               
            }
        }
    }

    public GameController gameController;
    public Transform startPos ;

    void OnCollisionEnter2D(Collision2D col) 
    {
       if(col.gameObject.tag == "PlayerTargetTag")
        {
            gameController.finishGame(true);
        }
    }

    public bool randomTargetMode = false;

    Texture2D RTImage(Camera camera)
    {
        // The Render Texture in RenderTexture.active is the one
        // that will be read by ReadPixels.
        camera.enabled = true;
        var rt = RenderTexture.GetTemporary(Screen.width, Screen.height);
        camera.targetTexture = rt;

        var currentRT = RenderTexture.active;
        RenderTexture.active = camera.targetTexture;

        // Render the camera's view.
        camera.Render();

        // Make a new texture and read the active Render Texture into it.
        Texture2D image = new Texture2D(camera.targetTexture.width, camera.targetTexture.height);
        image.ReadPixels(new Rect(0, 0, camera.targetTexture.width, camera.targetTexture.height), 0, 0);
        image.Apply();

        // Replace the original active Render Texture.
        RenderTexture.active = currentRT;

        //Destroy(rt);
        //Destroy(currentRT);
        return image;
    }

    public void getScreen(int b)
    {
        Texture2D texture = RTImage(cam);
        byte[] _bytes = texture.EncodeToJPG();
        System.IO.File.WriteAllBytes(b + ".png", _bytes);
        Texture2D.DestroyImmediate(texture);
    }

    public void resetImg()
    { a = 0; }

    //IEnumerator wait()
    //{ yield return new WaitForSecondsRealtime(20); }

    public void degis()             // gamestate = flase olduğunda yani oyun bittiğiginde player'ın rastgele bir bölgede doğmasını sağlar ve oyunu başlatır
    {
        int rand = UnityEngine.Random.Range(0, spawnPoints.Count);

        if (spawnPoints.Count > 0)
        {
            transform.position = spawnPoints[rand];
        }

    }
    private void Update() 
    {
        getScreen(a);
        if (gameController.gameState == true)
        {
             
             // get input images
            _helloRequester.info = a+" " + gameController.gameScore + " " + gameController.gameState;
            _helloRequester.fonk();
            
            // acting bounded for bug 
            if (a > 210)
            {
                gameController.finishGame(false);
            }
                
                 
            switch (_helloRequester.message)
            {
                case "r":
                    direction = Vector2.right;
                    break;
                case "l":
                    direction = Vector2.left;
                    break;
                case "u":
                    direction = Vector2.up;
                    break;
                case "d":
                    direction = Vector2.down;
                    break;
                case "end":
                    UnityEditor.EditorApplication.isPlaying = false;
                    break;
                default:
                    Debug.Log("cant access action");
                    break;
            }

            File.Delete(a - 1 + ".png");// images remover
            a++;
        }

        rb2d.velocity = direction * speed;          // player'ın rigidbody'sine yöne göre speed kadar güç uygula
        if (rb2d.velocity.x == 0)                   // dönüşlerde daha rahat olması için x ve y eşik değeri
        {
            turn = 1;
        }
        if (rb2d.velocity.y == 0)
        {
            turn = 2;
        }
        
    }
}

