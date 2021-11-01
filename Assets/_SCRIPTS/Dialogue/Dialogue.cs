using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dialogue
{
    static TextAsset txtAssets;
    static string conversation;//parsed into
    public string[] sentences;

    public Dialogue(string SCENE, string filename)//constructor
    {
        txtAssets = (TextAsset)Resources.Load(filename);
        conversation = remLineBreak(txtAssets.text);//parsed into
        sentences = conversation.Split(';');
        sentences = this.Process(SCENE);
        //a processed scene is an array of sentences, each sentence starting with the speaker tag (//BRIGHTSQUARE//>)
    }

    private string[] Process(string SCENE)//sets "sentences" array to desired dialogue in script
    {
        bool found = false;

        for (int i = 0; i < sentences.Length; i++)
        {
            if (sentences[i] == SCENE)
            {
                sentences = sentences[i + 1].Split(']');//separates text bubbles
                found = true;
                break;
            }
            //Now we have the conversation split into seperate text bubbles from player or other       
        }
        if (!found)
        {
            Debug.Log("Scene not found");
            return null;
        }
        return this.sentences;
    }


    private static string remLineBreak(string a)
    {
        char[] script = a.ToCharArray();
        char[] finScript = new char[a.Length];
        int count = 0;
        for (int i = 0; i < a.Length; i++)
        {
            if (script[i] != '\n' && script[i] != '\r')
            {
                finScript[count] = script[i];
                count++;
            }
        }
        string s = new string(finScript);
        return s;
    }
}



