using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;

public class CreateWords : MonoBehaviour
{
    #region "By Devesh"
    PubNubManager pubNubMgr;
    #endregion

    WordData wordData = new WordData();

    public string[] allAnswers = new string[7000];
    public TextAsset wordsFile;
    string[] splitFile = new string[] { "\r\n", "\r", "\n" };
    string[] answers = new string[4];
    Dictionary<string, string> randColl = new Dictionary<string, string>();
    public string[] wordStorage = new string[20];
    public GameObject textParent;
    public TextMeshProUGUI textObject;

    private string path = "";//use persistent path when building
    private string persistentPath = "";

    private void Start()
    {
        #region "By Devesh"
        pubNubMgr = new PubNubManager();
        pubNubMgr.Init();
        #endregion

        SetPaths();
        //GetDataFromFile(); Used only once to get the contents of the words file
        Get20Words();
    }

    public void GetDataFromFile()
    {
        allAnswers = wordsFile.ToString().Split(splitFile, System.StringSplitOptions.None);
    }

    void Get4Words()
    {
        int ULimit = allAnswers.Length;
        int numCount = 4;
        randColl = new Dictionary<string, string>();
        do
        {
            try
            {
                int i = UnityEngine.Random.Range(0, ULimit);
                randColl.Add(allAnswers[i], allAnswers[i]);
            }
            catch
            {
                //
            }
        }
        while (randColl.Count < numCount);

        for (int i = 0; i < randColl.Count; i++)
        {
            answers[i] = randColl.ElementAt(i).Value;
        }
    }

    public void Get20Words()
    {
        foreach(Transform child in textParent.transform)
        {
            Destroy(child.gameObject);
        }
        for (int c = 0; c < 20; c++)
        {
            Get4Words();
            wordStorage[c] = answers[0] + "," + answers[1] + "," + answers[2] + "," + answers[3];
            TextMeshProUGUI text =  Instantiate(textObject, textParent.transform);
            text.text = wordStorage[c];
        }
    }

    void SetPaths()
    {
        path = Application.dataPath + Path.AltDirectorySeparatorChar + "SaveData.json";
        persistentPath = Application.persistentDataPath + Path.AltDirectorySeparatorChar + "SaveData.json";
    }

    public void SaveToFile()
    {
        wordData.wordStorage = this.wordStorage; 
        string savePath = persistentPath;

        string json = JsonUtility.ToJson(wordData);

        using StreamWriter writer = new StreamWriter(savePath);
        writer.Write(json);
    }

    public void LoadFromFile()
    {
        using StreamReader reader = new StreamReader(persistentPath);
        string json = reader.ReadToEnd();

        WordData data = JsonUtility.FromJson<WordData>(json);
        UpdateData(data);
    }

    void UpdateData(WordData data) //take the data from the loaded file and update the UI
    {
        foreach (Transform child in textParent.transform)
        {
            Destroy(child.gameObject);
        }

        for (int c = 0; c < 20; c++)
        {
            TextMeshProUGUI text = Instantiate(textObject, textParent.transform);
            text.text = data.wordStorage[c];
        }
    }

    #region  "By Devesh"

    public void OnSubmit_Click()
    {
        SaveToFile();
        PublishWordFile(persistentPath);
    }

    void PublishWordFile(string filePath)
    {
        if(System.IO.File.Exists(filePath))
        {
            string content = System.IO.File.ReadAllText(filePath).Trim();
            PublishWordMsg(content);
        }
    }

    void PublishWordMsg(object msg)
    {
        pubNubMgr.PublishMsg(msg);
    }

    #endregion


}
