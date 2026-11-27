using UnityEngine;
using System.Collections;
using NUnit.Framework;
using System.Collections.Generic;

[System.Serializable]
public class GameData
{
    public Vector3 playerPos;
    public dayState day;
    public int pills;
    public int sanity;
    public List<string> inventory;
    public List<StrokesData> strokes;
    public string sceneName;
    public List<string> scannedItems;

    //this is the constructor of this class
    //we will call this from our dataManager singleton, it has a Vector3(0,0,0) default value
    public GameData()
    {
        this.playerPos = Vector3.zero;
        this.day = dayState.day1;
        this.sanity = 5;
        this.pills = 5;
        this.inventory = new List<string>();
        this.strokes = new List<StrokesData>();
        this.sceneName = "RefHouse A"; // default scene 
        this.scannedItems = new List<string>();
    }
}

[System.Serializable]
public class StrokesData
{
    public List<Vector3> points = new List<Vector3>();
}