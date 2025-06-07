using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;
using System.Text;
using MongoDB.Driver;
using System;
using MongoDB.Bson;
using System.Collections.Generic;
using Newtonsoft.Json;
public class TextReader : MonoBehaviour
{
    public GameObject questiontypestring;
    public GameObject questiontypebool;
    public GameObject questiontypeint;
    public GameObject questiontypemulti;

    public TMP_InputField inputField;
    public Button submitButton;
    public Button Upload;

    private static BsonDocument document = new BsonDocument();

    private int qn;

    private string connectionString;

    public TextAsset textFile;

    public TextAsset jsonFile;

    public TextMeshProUGUI question;

    private Question currentq;

    public Toggle myToggle;
    public TMP_Dropdown myTMPDropdown1;
    public TMP_Dropdown myTMPDropdown2;

    private Dictionary<string, Question> questions;

    public GameObject UploadButtom;
    public GameObject NextButtom;

    [System.Serializable]
    public class Question
    {
        public string text;
        public string type;
        public bool mult_choice;
    }

    void Start()
    {
        questiontypestring.SetActive(false);
        questiontypebool.SetActive(false);
        questiontypeint.SetActive(false);
        questiontypemulti.SetActive(false);
        inputField.text = "";
        myToggle.isOn = false;
        myTMPDropdown1.value = 0;
        myTMPDropdown2.value = 0;
        questions = JsonConvert.DeserializeObject<Dictionary<string, Question>>(jsonFile.text);
        connectionString = textFile.text;
        submitButton.onClick.AddListener(OnSubmit);
        Upload.onClick.AddListener(MongoDB);
        UploadButtom.SetActive(false);
    }

    void Update()
    {
        currentq = questions[qn.ToString()];
        string q = currentq.text;
        question.text = q;
        if (currentq.type == "str" && !currentq.mult_choice)
        {
            questiontypestring.SetActive(true);
            questiontypebool.SetActive(false);
            questiontypeint.SetActive(false);
            questiontypemulti.SetActive(false);
        }
        else if (currentq.type == "str" && currentq.mult_choice)
        {
            questiontypestring.SetActive(false);
            questiontypebool.SetActive(false);
            questiontypeint.SetActive(false);
            questiontypemulti.SetActive(true);
        }
        else if (currentq.type == "int")
        {
            questiontypestring.SetActive(false);
            questiontypebool.SetActive(false);
            questiontypeint.SetActive(true);
            questiontypemulti.SetActive(false);
        }
        else if (currentq.type == "bool")
        {
            questiontypestring.SetActive(false);
            questiontypebool.SetActive(true);
            questiontypeint.SetActive(false);
            questiontypemulti.SetActive(false);
        }
            

    }

    void OnSubmit()
    {
        if (questiontypestring.activeInHierarchy)
        {
            string userInput = inputField.text;
            Debug.Log("User input: " + userInput);
            document[qn.ToString()] = userInput;
        }
        else if (questiontypebool.activeInHierarchy)
        {
            bool userInput = false;
            if (myToggle.isOn)
            {
                userInput = true;
            }
            Debug.Log("User input: " + userInput);
            document[qn.ToString()] = userInput;
        }
        else if (questiontypeint.activeInHierarchy)
        {
            int value = myTMPDropdown1.value + 1;
            int userInput = value;
            Debug.Log("User input: " + userInput);
            document[qn.ToString()] = userInput;
        }
        else if (questiontypemulti.activeInHierarchy)
        {
            int value = myTMPDropdown2.value;
            string userInput = "null";
            if (value == 0)
            {
                userInput = "A";
            }
            else if (value == 1)
            {
                userInput = "B";
            }
            else if (value == 2)
            {
                userInput = "C";
            }
            else if (value == 3)
            {
                userInput = "D";
            }
            Debug.Log("User input: " + userInput);
            document[qn.ToString()] = userInput;
        }
        qn += 1;
        inputField.text = "";
        myToggle.isOn = false;
        myTMPDropdown1.value = 0;
        myTMPDropdown2.value = 0;
        if (qn == 36)
        {
            UploadButtom.SetActive(true);
            NextButtom.SetActive(false);
        }

    }

    async void MongoDB()
    {
        var client = new MongoClient(connectionString);
        var database = client.GetDatabase("Data");
        var collection = database.GetCollection<BsonDocument>("Current Answers");

        document["timestamp"] = BsonDateTime.Create(System.DateTime.UtcNow);

        await collection.InsertOneAsync(document);

        Debug.Log("Uploaded to MongoDB!");
    }
    
}
