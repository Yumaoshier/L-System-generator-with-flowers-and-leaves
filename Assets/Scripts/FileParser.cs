using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public static class FileParser 
{
    public static void ParseRules(string content, out Dictionary<string, turtleData> ruleDic)
    {

        ruleDic = new Dictionary<string, turtleData>();

        var datas = content.Split('}');
        foreach (string rawData in datas)
        {
            string key = "";
            turtleData td = new turtleData();

            var lines = rawData.Split('\n');           
            foreach(string rawLine in lines)
            {
                string line = rawLine.Trim();
                if (line == "{")
                    continue;
                if (line.Length == 0)
                    continue;
                else if (line.Length == 1 && line[0] == '\r')
                    continue;
                else if (line[0] == '/' && line[1] == '/')
                    continue;
                string value;
                
                
                if (line.IndexOf("name") != -1)
                {
                    value = line.Substring(line.IndexOf("=") + 1);
                    value = value.Trim();
                    key = value;

                }
                else if (line.IndexOf("drawSymbols") != -1)
                {
                    value = line.Substring(line.IndexOf("=") + 1);
                    value = value.Trim();
                    td.drawSymbols = value.ToCharArray();
                }
                else if (line.IndexOf("leafSymbol") != -1)
                {
                    value = line.Substring(line.IndexOf("=") + 1);
                    value = value.Trim();
                    td.leafSymbol = value.ToCharArray();
                }
                else if (line.IndexOf("leafImgPath") != -1)
                {
                    value = line.Substring(line.IndexOf("=") + 1);
                    value = value.Trim();
                    td.leafImgPath = value;
                }
                else if (line.IndexOf("flowerSymbol") != -1)
                {
                    value = line.Substring(line.IndexOf("=") + 1);
                    value = value.Trim();
                    td.flowerSymbol = value.ToCharArray();
                }
                else if (line.IndexOf("flowerImgPath") != -1)
                {
                    value = line.Substring(line.IndexOf("=") + 1);
                    value = value.Trim();
                    td.flowerImgPath = value;
                }
                else if (line.IndexOf("axiom") != -1)
                {
                    value = line.Substring(line.IndexOf("=") + 1);
                    value = value.Trim();
                    td.axiom = value;

                }
                else if (line.IndexOf("angle") != -1)
                {
                    value = line.Substring(line.IndexOf("=") + 1);
                    value = value.Trim();
                    td.angle = float.Parse(value);
                }
                else if (line.IndexOf("rules") != -1)
                {
                    value = line.Substring(line.IndexOf(":") + 1);
                    value = value.Trim();
                    td.rules = value.Split(';');
                }
                else if (line.IndexOf("interation") != -1)
                {
                    value = line.Substring(line.IndexOf("=") + 1);
                    value = value.Trim();
                    td.interation = Convert.ToInt32(value);
                }
            }
            

            if (key != "" && !ruleDic.ContainsKey(key))
            {
                ruleDic.Add(key, td);

            }
        }
    }


    public static void ParsePaths(string content, out Dictionary<string, string> pathDic)
    {

        pathDic = new Dictionary<string, string>();

        var lines = content.Split('\n');
        foreach (string rawLine in lines)
        {
            string line = rawLine.Trim();
            if (line.Length == 0)
                continue;
            else if (line.Length == 1 && line[0] == '\r')
                continue;
            else if (line[0] == '/' && line[1] == '/')
                continue;


            string[] str = line.Split('=');
            if (str.Length > 1)
            {
                pathDic.Add(str[0], str[1]);
            }

        }
    }

    public static void ParseRLRotations(string content, in string path, out Vector3[] rotationArray)
    {

        // rotationDic = new Dictionary<string, Vector3[]>();
        rotationArray = new Vector3[2];
        string curKey = "";
        var lines = content.Split('\n');
        foreach (string rawLine in lines)
        {
            string line = rawLine.Trim();
            if (line.Length == 0)
                continue;
            else if (line.Length == 1 && line[0] == '\r')
                continue;
            else if (line[0] == '/' && line[1] == '/')
                continue;

            if (line.Contains(":"))
            {
                curKey = line.Split(':')[0];
                if(curKey != path)
                {
                    curKey = "";
                }
               
            }
            else if (curKey != "" && line.Contains("="))
            {
                string[] str = line.Split('=');
                if (str.Length > 1)
                {
                    Vector3 v = Vector3.zero;
                    string[] c = str[1].Split(',');
                    if (c.Length >= 3)
                    {
                        v = new Vector3(float.Parse(c[0]), float.Parse(c[1]), float.Parse(c[2]));
                    }

                    if (str[0] == "R")
                    {
                        rotationArray[0] = v;
                        
                    }
                    else if (str[0] == "L")
                    {
                        rotationArray[1] = v;
                    }
                 
                }
            }
     

        }
    }

}
