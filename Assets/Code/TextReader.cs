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
public class TextReader : MonoBehaviour
{
    public TMP_InputField inputField;
    public Button submitButton;
    public Button Upload;

    private static BsonDocument document = new BsonDocument();

    private int qn;

    private string connectionString;

    public TextAsset textFile;

    void Start()
    {
        connectionString = textFile.text;
        submitButton.onClick.AddListener(OnSubmit);
        Upload.onClick.AddListener(MongoDB);
    }

    void OnSubmit()
    {
        string userInput = inputField.text;
        Debug.Log("User input: " + userInput);
        document[qn.ToString()] = userInput;
        qn += 1;
        inputField.text = "";

    }

    async void MongoDB()
    {
        var client = new MongoClient(connectionString);
        var database = client.GetDatabase("Data");
        var collection = database.GetCollection<BsonDocument>("Answers");

        document["timestamp"] = BsonDateTime.Create(System.DateTime.UtcNow);

        await collection.InsertOneAsync(document);

        Debug.Log("Uploaded to MongoDB!");
    }
    
}
