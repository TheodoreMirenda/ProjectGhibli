using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Linq;
using Unity.Entities;
using TJ.DOTS;

namespace Gobillions
{
public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public GameObject player;
    public int goblinCount;
    [SerializeField] private Text goblinCountText;
    public Vector3[] towerSpots = new Vector3[0];
    public List<Tower> towers = new List<Tower>();

    private void Awake() {
        if(instance==null)
            instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
    }

    private void Start() {
        towers = FindObjectsOfType<Tower>().ToList();
        towerSpots = new Vector3[towers.Count];
        for (int i = 0; i < towers.Count; i++) {
            towerSpots[i] = towers[i].transform.position;
        }
    }
        private void Update() {
        goblinCountText.text = goblinCount.ToString();
    }
}
}