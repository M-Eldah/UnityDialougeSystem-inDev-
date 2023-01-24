using DSystem;
using TMPro;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Collections;
/// <summary>
/// The Main class we use to interact with dialouge, you can use it or make your own
/// </summary>
public class DialougeController : MonoBehaviour
{
    // the source we use to play the audio
    public AudioSource source;
    //the main text we use to diaplay text
    public TextMeshProUGUI text;
    // in this code i used a prefab to show choices instead, you can just a array with 
    // with a set number of choices
    //the list i sue to keep track of the generated choices
    public List<GameObject> Choices;
    //The prefab i use so that i can generate as many choices as i want
    //but if have a set number of questions that you know you won't exceed 
    //you can use an array of textmeshpro instead and set the text to those
    public GameObject buttonPrefab;
    //the q_string1ect used to hold all the choices, not necessary but keeps everything neet 
    public GameObject Choiceholder;
    //The q_string1ect that hold the single Choice Components 
    public GameObject single;
    //You understand
    public GameObject multi;
    //the handeler that holds the dialouge we need
    public DialougeHandeler dialouge;

    #region DialougeController

    private void Start()
    {
        single.SetActive(false);
        multi.SetActive(false);
    }
    private void OnEnable()
    {
        //Subscribing our methods to the dialouge system events
        DialogSystem.dialougeNext += NextNode;
        DialogSystem.dialougeEnd += End;
    }
    private void OnDisable()
    {
        DialogSystem.dialougeNext -= NextNode;
        DialogSystem.dialougeEnd -= End;
    }
    void End()
    {
        //Ending the Dialouge
        single.SetActive(false);
        multi.SetActive(false);
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (DialogSystem.InDialouge)
            {
                DialogSystem.Next();
            }
            else
            {
                DsData Node = DialogSystem.DStart(dialouge.Data());
                if (Node != null)
                { UpdateDialougeUi(Node); }
            }
        }
    }

    public void UpdateDialougeUi(DsData Node)
    {
        ClearChoices();
        switch (Node.type)
        {
            case TextType.SingleNode:
                multi.SetActive(false);
                single.SetActive(true);
                if (Node.Dialouge.DialougeLocked)
                {
                    DsData NewNode = DialogSystem.DNext();
                    UpdateDialougeUi(NewNode);
                }
                else
                { text.text = Node.Dialouge.Text; }
                break;

            case TextType.MultiNode:
                single.SetActive(false);
                multi.SetActive(true);
                Choiceholder.GetComponent<RectTransform>().sizeDelta = new Vector2(0, Node.Choices.Count * 35 + (Node.Choices.Count - 1) * 8);
                for (int i = 0; i < Node.Choices.Count; i++)
                {
                    if (Node.Choices[i].DialougeLocked)
                    {
                        continue;
                    }
                    GameObject Button = Instantiate(buttonPrefab, Choiceholder.transform);
                    Choices.Add(Button);
                    Button.GetComponent<Choice>().assign(Node.Choices[i].Text, i, this);
                }
                break;
            case TextType.EmptyNode:
                if(Node.clip!=null)
                {
                    source.PlayOneShot(Node.clip);
                }
                break;
        }
    }

    private void ClearChoices()
    {
        if (Choices.Count != 0)
        {
            foreach (GameObject gameObject in Choices)
            {
                gameObject.transform.SetParent(null);
                Destroy(gameObject);
            }
        }
        Choices.Clear();
    }
    public void NextNode()
    {
        DsData Node = DialogSystem.DNext(0);
        if (Node != null)
        { 
            UpdateDialougeUi(Node); 
        }
    }
    public void SetPlayerChoice(int choice=0)
    {
        DsData Node = DialogSystem.DNext(choice);
        if (Node != null)
        { UpdateDialougeUi(Node); }
    }

    #endregion DialougeControllerSimulation
}