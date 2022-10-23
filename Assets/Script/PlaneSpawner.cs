using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaneSpawner : MonoBehaviour
{
    [SerializeField] GameObject planePrefab;
    [SerializeField] int spawnZPos = 7;
    [SerializeField] Player player;
    [SerializeField] float timeOut = 5;
    [SerializeField] float timer;
    int playerLastMaxTravel=0;
    private void Start() 
    {
        
    }

    private void SpawnPlane() 
    {
        player.enabled = false;
        var position = new Vector3(player.transform.position.x,1,player.CurrentTravel + spawnZPos);
        var rotation = Quaternion.Euler(0,180,0);
        var planeObject = Instantiate(planePrefab, position, rotation);
        var plane = planeObject.GetComponent<Plane>();
        plane.SetUpTarget(player);
    }

    private void Update() {
        //jika player ada kemajuan
        if(player.MaxTravel != playerLastMaxTravel)
        {
            //maka reset timer
            timer=0;
            playerLastMaxTravel=player.MaxTravel;
            return;
        }

        //kalo ga maju2 timer
        if(timer < timeOut)
        {
            timer += Time.deltaTime;
            return;
        }

        //kalo sudah timeout
        if(player.IsJumping()==false && player.IsDie==false)
        {
            SpawnPlane();
        }
    }
}
