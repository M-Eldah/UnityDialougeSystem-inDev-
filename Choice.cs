using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Choice : MonoBehaviour
{
    public TMPro.TextMeshProUGUI text;
    public int id;
    public DialougeController DialougeController;
    public void assign(string _text,int _id,DialougeController controller)
    {
        text.text = _text;
        id = _id;
        DialougeController = controller;
    }
    public void makechoice()
    {
        DialougeController.SetPlayerChoice(id);
    }
}
