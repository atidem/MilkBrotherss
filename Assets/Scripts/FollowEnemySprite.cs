using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowEnemySprite : MonoBehaviour
{
    public GameObject enemy;

    void Update() {
        transform.position = enemy.transform.position;
 }
}
