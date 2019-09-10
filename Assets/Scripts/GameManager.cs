using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;


    public List<CuttableObject> Woods = new List<CuttableObject>();
    public List<CuttableObject> InactiveWoods = new List<CuttableObject>();
    public int Score = 0;

    private List<GameObject> WoodPrefabs = new List<GameObject>();

    [SerializeField] private float _tableWidth = 4;
    [SerializeField] private float _yScaleMax = 1.5f;
    [SerializeField] private float _yScaleMin = 0.4f;
    [SerializeField] private float _girthScaleMax = 1f;
    [SerializeField] private float _girthScaleMin = 0.2f;
    [SerializeField] private float _cutRegionScaleMax = 0.4f;       //Absolute Scale
    [SerializeField] private float _cutRegionScaleMin = 0.3f;       //Absolute Scale
    [SerializeField] private float _spawnPeriod = 2f;
    [SerializeField] private Canvas screenCanvas;

    private float _spawnCooldown = 0f;
    private bool _isPaused = false;
    private bool _isGameOver = false;

    void Start()
    {
        if (instance == null)
            instance = this;

        Score = 0;
        Object[] logs = Resources.LoadAll("Prefabs/Woods/");
        foreach (Object o in logs)
        {
            WoodPrefabs.Add( (GameObject) o );
        }

        PauseGame(false);
        _isGameOver = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (_isPaused || _isGameOver)
            return;

        _spawnCooldown -= Time.deltaTime;

        if (_spawnCooldown <= 0)
        {
            _spawnCooldown = _spawnPeriod;
            SpawnRandomWood();
        }
    }

    public void GameOver()
    {
        Debug.Log("Game Over");
        _isGameOver = true;
        screenCanvas.transform.GetChild(0).gameObject.SetActive(false);
        screenCanvas.transform.GetChild(1).gameObject.SetActive(false);
        screenCanvas.transform.GetChild(3).gameObject.SetActive(true);
        screenCanvas.transform.GetChild(3).GetChild(0).GetComponent<Text>().text = "Score: " + Score;
    }

    public void RestartLevel()
    {
        SceneManager.LoadScene(0);
    }

    public void IncrementScore()
    {
        Debug.Log("Good Cut");
        Score++;
        screenCanvas.transform.GetChild(0).GetComponent<Text>().text = "Score: " + Score;
    }

    public void PauseGame(bool b)
    {
        _isPaused = b;

        if (_isPaused)
        {
            Time.timeScale = 0f;
        }
        else
        {
            Time.timeScale = 1f;
        }
    }

    private void SpawnRandomWood()
    {
        int woodType = Random.Range(0, WoodPrefabs.Count);
        float yScale = Random.Range(_yScaleMin, _yScaleMax);
        float centralOffset = Random.Range(yScale - _tableWidth / 2, _tableWidth / 2 - yScale);
        float girthScale = Random.Range(_girthScaleMin, _girthScaleMax);

        float cutRegionScale = Random.Range(_cutRegionScaleMin, _cutRegionScaleMax);
        float tolerance = yScale * (1 - cutRegionScale) * 0.8f;
        float cutRegionOffset = Random.Range(-tolerance, tolerance);

        SpawnWood(woodType, centralOffset, yScale, girthScale, cutRegionOffset, cutRegionScale);
    }

    private void SpawnWood(int woodType, float centralOffset, float yScale, float girthScale, float cutRegionOffset, float cutRegionScale)
    {
        if (InactiveWoods.Count > 0)
        {
            CuttableObject wood = InactiveWoods[Random.Range(0, InactiveWoods.Count)];
            wood.transform.position = new Vector3(centralOffset, 3f, 3f);
            wood.transform.localScale = new Vector3(girthScale, yScale, girthScale);

            wood.cutRegion.transform.localPosition += Vector3.up * cutRegionOffset;
            wood.cutRegion.transform.localScale = new Vector3(1.01f, cutRegionScale, 1.01f);

            wood.gameObject.SetActive(true);
            InactiveWoods.Remove(wood);
        }
        else
        {
            GenerateWood(woodType, centralOffset, yScale, girthScale, cutRegionOffset, cutRegionScale);
        }
    }

    private void GenerateWood(int woodType, float centralOffset, float yScale, float girthScale, float cutRegionOffset, float cutRegionScale)
    {
        CuttableObject wood = GameObject.Instantiate(WoodPrefabs[woodType]).GetComponent<CuttableObject>();
        wood.transform.position = new Vector3(centralOffset, 3f, 3f);
        wood.transform.localScale = new Vector3(girthScale , yScale , girthScale);

        wood.cutRegion.transform.localPosition += Vector3.up * cutRegionOffset;
        wood.cutRegion.transform.localScale = new Vector3(1.01f, cutRegionScale, 1.01f);

        Woods.Add(wood);
    }

    private void GenerateRandomWood()
    {
        int woodType = Random.Range(0, WoodPrefabs.Count);
        float yScale = Random.Range(_yScaleMin, _yScaleMax);
        float centralOffset = Random.Range(yScale -_tableWidth/2, _tableWidth / 2 - yScale);
        float girthScale = Random.Range(_girthScaleMin, _girthScaleMax);

        float cutRegionScale = Random.Range(_cutRegionScaleMin, _cutRegionScaleMax);
        float tolerance = yScale * (1 - cutRegionScale) * 0.8f;
        float cutRegionOffset = Random.Range(-tolerance, tolerance);

        GenerateWood(woodType, centralOffset, yScale, girthScale, cutRegionOffset, cutRegionScale);
    }

    public bool IsPaused()
    {
        return _isPaused;
    }

    public bool IsGameOver()
    {
        return _isGameOver;
    }

    public float GetTableWidth()
    {
        return _tableWidth;
    }

}
