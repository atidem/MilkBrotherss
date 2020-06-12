using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRightPos : MonoBehaviour
{
    public Transform player;
    public int dir;             // (dir=yon) 0 => sağda / 1 => solda / 2 => altta / 3 => üstte
    
    void Update() 
    {   
        if(dir == 0)
            this.transform.position = new Vector3(player.position.x+4 , player.position.y , player.position.z);
        if(dir == 1)
            this.transform.position = new Vector3(player.position.x-4 , player.position.y , player.position.z);
        if(dir == 2)
            this.transform.position = new Vector3(player.position.x , player.position.y-4 , player.position.z);
        if(dir == 3)
            this.transform.position = new Vector3(player.position.x , player.position.y+4 , player.position.z);
        
    }
}
    // Bu scriptin amacı; pacman'in çecresine yerleştirilen 4 görünmez pacman'e asıl pacman'i takip etmesini söylemek.
    // Bu şekilde farklı enemylere bu pacman'ler target olarak gösterilip, farklı takip yolları oluşturmaları sağlanır.
