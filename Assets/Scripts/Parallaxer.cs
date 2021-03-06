﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallaxer : MonoBehaviour {

    class PoolObject {
        public Transform transform;
        public bool inUse;

        public PoolObject(Transform t) {
            transform = t;
        }
        public void Use() {
            inUse = true;
        }
        public void Dispose() {
            inUse = false;
        }
    }

    [System.Serializable]
    public struct YSpawnRange {
        public float minY;
        public float maxY;
    }

    public GameObject Prefab;
    public int poolSize;
    public float shiftSpeed;
    public float spawnRate;

    public YSpawnRange ySpawnRange;
    public Vector3 defaultSpawnPosition;
    public bool spawnImmediate; // like particle prewarm
    public Vector3 immediateSpawnPosition;
    public Vector2 targetAspectRatio;

    float spawnTimer;
    float targetAspect;

    PoolObject[] poolObjects;
    GameManager gameManager;

    void Awake() {
        Configure();
    }

    void Start() {
        gameManager = GameManager.Instance;
        
    }

    void OnEnable() {
        GameManager.OnGameOver += OnGameOver;
        
    }

    void OnDisable() {
        GameManager.OnGameOver -= OnGameOver;
        
    }

    void OnGameOver() {
        for (int i = 0; i < poolObjects.Length; i++) {
            poolObjects[i].Dispose();
            poolObjects[i].transform.position = Vector3.one * 1000;
        }
        Configure();
    }

    void Update() {
        if (gameManager.GameOver) return;

        Shift();
        spawnTimer += Time.deltaTime;
        if (spawnTimer > spawnRate) {
            Spawn();
            spawnTimer = 0;
        }
        
    }

    void Configure() {
        targetAspect = targetAspectRatio.x / targetAspectRatio.y;
        poolObjects = new PoolObject[poolSize];
        for (int i = 0; i < poolObjects.Length; i++) {
            GameObject go = Instantiate(Prefab) as GameObject;
            Transform t = go.transform;
            t.SetParent(transform);
            t.position = Vector3.one * 1000;
            poolObjects[i] = new PoolObject(t);
        }

        if (spawnImmediate) {
            SpawnImmediate();
        }

    }

    void Spawn() {
        Transform t = GetPoolObject();
        if (t == null) return; // if true, poolSize is too small
        Vector3 pos = Vector3.zero;
        pos.x = (defaultSpawnPosition.x * Camera.main.aspect) / targetAspect;
        pos.y = Random.Range(ySpawnRange.minY, ySpawnRange.maxY);
        t.position = pos;
    }

    void SpawnImmediate() {
        Transform t = GetPoolObject();
        if (t == null) return; // if true, poolSize is too small
        Vector3 pos = Vector3.zero;
        pos.x = (immediateSpawnPosition.x * Camera.main.aspect) / targetAspect;
        pos.y = Random.Range(ySpawnRange.minY, ySpawnRange.maxY);
        t.position = pos;
        Spawn();
    }

    void Shift() {
        // video at 1:45:00
        for (int i = 0; i < poolObjects.Length; i++) {
            poolObjects[i].transform.position += -Vector3.right * shiftSpeed * Time.deltaTime;
            CheckDisposeObject(poolObjects[i]);
        }

    }

    void CheckDisposeObject(PoolObject poolObject) {
        if (poolObject.transform.position.x < (-defaultSpawnPosition.x * Camera.main.aspect) / targetAspect) {
            poolObject.Dispose();
            poolObject.transform.position = Vector3.one * 1000; // to get the object out of sight
        }

    }

    Transform GetPoolObject() {
        for (int i = 0; i < poolObjects.Length; i++) {
            if (!poolObjects[i].inUse) {
                poolObjects[i].Use();
                return poolObjects[i].transform;
            }
        }
        return null;
    }

}
