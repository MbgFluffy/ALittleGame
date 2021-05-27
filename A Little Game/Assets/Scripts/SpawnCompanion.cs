using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnCompanion : MonoBehaviour
{
    public GameObject companionPrefab;
    private GameObject companion;
    private bool isSpawned = false;
    public Camera cam;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            if (!isSpawned)
            {
                companion = Instantiate(companionPrefab) as GameObject;
                companion.transform.position = transform.position;
                companion.GetComponent<FollowTransform>().cam = cam;
                isSpawned = true;
            }

            else if (isSpawned)
            {
                Destroy(companion);
                isSpawned = false;
            }

        }
    }
}
