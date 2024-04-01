using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class StoreData : MonoBehaviour
{
    public int clickNumber = 1;
    private string filename = "";
    private float gameStartTime;

    [System.Serializable]
    public class Block
    {
        public GameObject gameBlock;
        public float minProbability;
        public float maxProbability;
        public int value;
    }

    [System.Serializable]
    public class BlockList
    {
        public Block[] block;
    }

    public BlockList myBlockList = new BlockList();

    void Start()
    {
        gameStartTime = Time.time;
        string timestamp = System.DateTime.Now.ToString("HHmmss_ddMMyyyy");
        filename = Application.dataPath + "/Data/ftest" + timestamp + ".csv";

        // Write CSV header once at the beginning
        WriteCSVHeader();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            CastRay();
        }
    }

    void CastRay()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity))
        {
            // Printing which block is being clicked
            Debug.Log($"Block clicked: {hit.collider.gameObject.name}");

            // Store data in CSV
            WriteCSV(clickNumber, hit.collider.gameObject.name, Time.time - gameStartTime);
        }
    }

    private float CalculateReward(float minProbability, float maxProbability)
    {
        float randomProbability = Random.Range(minProbability, maxProbability);
        int total = Random.Range(75, 100);
        return randomProbability * total;
    }

    public void WriteCSVHeader()
    {
        using (TextWriter tw = new StreamWriter(filename, false))
        {
            tw.Write("Click,Time,ClickedBlock");
            for (int i = 0; i < myBlockList.block.Length; i++)
            {
                tw.Write($",{myBlockList.block[i].gameBlock.name}");
            }
            tw.WriteLine(); // Move to the next line after writing block names
        }
    }

    public void WriteCSV(int x, string blockName, float elapsedTime)
    {
        using (TextWriter tw = new StreamWriter(filename, true))
        {
            string formattedTime = elapsedTime.ToString("F2");
            tw.Write($"{x},{formattedTime}s,{blockName}");

            for (int i = 0; i < myBlockList.block.Length; i++)
            {
                float reward = CalculateReward(myBlockList.block[i].minProbability, myBlockList.block[i].maxProbability);
                tw.Write($",{Mathf.RoundToInt(reward)}");
            }
            tw.WriteLine(); // Move to the next line after writing rewards
        }
        clickNumber++;
    }

}
