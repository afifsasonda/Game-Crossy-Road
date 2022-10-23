using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class GameManager : MonoBehaviour
{
    [SerializeField] Player player;
    [SerializeField] GameObject gameOverPanel;
    [SerializeField] GameObject grass;
    [SerializeField] GameObject road;
    [SerializeField] int extent = 7;
    [SerializeField] int frontDistance = 10;
    [SerializeField] int backDistance = -5;
    [SerializeField] int maxSameTerrainRepeat = 3;

    Dictionary<int, TerrainBlock> map = new Dictionary<int, TerrainBlock>(50);
    TMP_Text gameOverText;

    private void Start() 
    {
        //setup gameover panel
        gameOverPanel.SetActive(false);
        gameOverText = gameOverPanel.GetComponentInChildren<TMP_Text>();

        //view belakang
        for (int z = backDistance; z <= 0; z++)
        {
            CreateTerrain(grass,z);
        }

        //depan
        for (int z = 1; z <= frontDistance; z++)
        {
            
            var prefab = GetNextRandomTerrainPrefab(z);

            // instantiate blocknya
            CreateTerrain(prefab,z);
        }

        player.SetUp(backDistance,extent);


    }

    private int playerLastMaxTravel;

    private void Update() {
        // cek player masih hidup ga?
        if(player.IsDie && gameOverPanel.activeInHierarchy==false)
        {
            StartCoroutine(ShowGameOverPanel());
        }

        //infinite terrain system
        if(player.MaxTravel == playerLastMaxTravel)
        {
            return;
        }
        playerLastMaxTravel = player.MaxTravel;

        //membuat ke depan
        var randTbPrefab = GetNextRandomTerrainPrefab(player.MaxTravel + frontDistance );
        CreateTerrain(randTbPrefab, player.MaxTravel+frontDistance);

        //hapus yang belakang
        var lastTB = map[player.MaxTravel-1 + backDistance];

        //TerrainBlock lastTB = map[player.MaxTravel + frontDistance];
        //int lastPos = player.MaxTravel;
        //foreach (var (pos,tb) in map)
        //{
        //    if(pos<lastPos)
        //    {
        //        lastPos = pos;
        //        lastTB = tb;
        //    }

        //hapus dari daftar
         map.Remove(player.MaxTravel-1+backDistance);
         //hilangkan dari scene
         Destroy(lastTB.gameObject);

        //setup lagi supaya player ga bisa gerak ke belakang
         player.SetUp(player.MaxTravel+backDistance,extent);
    }
    

    IEnumerator ShowGameOverPanel()
    {
        
        yield return new WaitForSeconds(2);
        gameOverText.text = "Score : " + player.MaxTravel;
        gameOverPanel.SetActive(true);
    }

    private void CreateTerrain(GameObject prefab, int zPos)
    {
        var go = Instantiate(prefab,new Vector3(0,0,zPos),Quaternion.identity);
        var tb = go.GetComponent<TerrainBlock>();
        tb.Build(extent);

        map.Add(zPos,tb);
        //Debug.Log(map[zPos] is Road);

    }

    private GameObject GetNextRandomTerrainPrefab(int nextPos)
    {
        bool isUniform = true;
        var tbRef = map[nextPos-1];
        for (int distance = 2; distance <= maxSameTerrainRepeat; distance++)
        {
            if(map[nextPos - distance].GetType() != tbRef.GetType())
            {
                isUniform = false;
                break;
            }
        }
        if(isUniform)
        {
            if(tbRef is Grass)
            {
                return road;
            }
            else
            {
                return grass;
            }
        }
        // penentuan terrain block dengan probabilitas 50%
        return Random.value > 0.5f ? road : grass;
    }

}
