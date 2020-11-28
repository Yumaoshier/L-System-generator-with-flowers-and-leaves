using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;




public class Turtle : MonoBehaviour
{

    private class LSystemState
    {
        public float size = 0.5f;
        public float angle = 0;
        public float x = 0;
        public float y = 0;

        public LSystemState Clone()
        {
            return (LSystemState) this.MemberwiseClone();
        }
    }

    private LSystem _lSystem = new LSystem();

    private List<char> _drawSymbols = new List<char>();

    private List<char> _leafSymbols = new List<char>();
    private List<char> _flowerSymbols = new List<char>();
    private string _leafPath;
    private string _flowerPath;
    private Vector3[] _leafRLRotations;
    private Vector3[] _flowerRLRotations;
   
    private float _angle;

    private Stack<LSystemState> stateStack = new Stack<LSystemState>();

    private List<GameObject> linesPool = new List<GameObject>();
    private List<GameObject> linesReturnPool = new List<GameObject>();
    private Dictionary<string, List<GameObject>> leavesPool = new Dictionary<string, List<GameObject>>();
    private Dictionary<string, List<GameObject>> leavesReturnPool = new Dictionary<string, List<GameObject>>();
    private Dictionary<string, List<GameObject>> flowersPool = new Dictionary<string, List<GameObject>>();
    private Dictionary<string, List<GameObject>> flowersReturnPool = new Dictionary<string, List<GameObject>>();

    private GameObject lineGo;
    private Dictionary<string, GameObject> leafGo = new Dictionary<string, GameObject>();
   // private GameObject leafGo;
    private Dictionary<string, GameObject> flowerGo = new Dictionary<string, GameObject>();
    //private GameObject flowerGo;

    private LSystemState curState;

    private Vector3[] defaultLineRenderPos = new Vector3[2];

    


    public int getCurInteration()
    {
        return _lSystem.curIteration;
    }

    public bool checkIfFinished()
    {
        return _lSystem.checkIfFinished();
    }

    public int getRestInteration()
    {
        return _lSystem.getRestInteration();
    }

    public void setDrawSymbols(char[] drawString, char[] leafS, char[] flowerS)
    {
        _drawSymbols.Clear();
        _drawSymbols.AddRange(drawString);
        _leafSymbols.Clear();
        _leafSymbols.AddRange(leafS);
        _flowerSymbols.Clear();
        _flowerSymbols.AddRange(flowerS);
    }

    public void setRules(string[] rules)
    {
      
        _lSystem.setRules(rules);
    }

    public void setData(string ax, float an, int inter, string ls, string fs)
    {
        
        _angle = an;
        _lSystem.setIteration(inter);
        _lSystem.setAxiom(ax);
        _leafPath = ls;
        _flowerPath = fs;
        TextAsset file = Resources.Load<TextAsset>("Files/RLRotations");
        FileParser.ParseRLRotations(file.text, _leafPath, out _leafRLRotations);
     
        FileParser.ParseRLRotations(file.text, _flowerPath, out _flowerRLRotations);
        reset(true);
    }

    public void generate(bool once = false)
    {
        reset();
       
        string sentence;
        if (once)
        {
            sentence = _lSystem.getNewSentence();
            
        }
        else
        {
            sentence = _lSystem.getNewSentencebyStep();
        }
      
        if (sentence == null)
        {
            return;
        }
        curState = new LSystemState();

        for(int i = 0; i < sentence.Length; i++)
        {
            char c = sentence[i];
            if (_drawSymbols.Contains(c))
            {
                drawLine();               
            }
            else if(_leafSymbols.Contains(c)){
                drawLeaf(_leafPath);
            }
            else if (_flowerSymbols.Contains(c))
            {
                drawFlower(_flowerPath);
            }
            else
            {
                switch (c)
                {
                    case '+':
                        curState.angle += _angle;
                        break;
                    case '-':
                        curState.angle -= _angle;
                        break;
                    case '[':
                        stateStack.Push(curState.Clone());
                        break;
                    case ']':
                        curState = stateStack.Pop();
                        break;
                    default:
                        break;
                }
            }
            
        }
    }

    private void drawLine()
    {
        GameObject newLine = getLineFromPool();
        LineRenderer newLineRender = null;
        if (!newLine)
        {
            if (!lineGo)
            {
                lineGo = Resources.Load("Prefabs/Line") as GameObject;
            }
            newLine = Instantiate(lineGo, transform);
            newLine.transform.localPosition = Vector3.zero;
            if (defaultLineRenderPos[1] == Vector3.zero)
            {
                newLineRender = newLine.GetComponent<LineRenderer>();
                defaultLineRenderPos[0] = newLineRender.GetPosition(0);
                defaultLineRenderPos[1] = newLineRender.GetPosition(1);
            }        
        }

        if (!newLine.activeSelf)
        {
            newLine.SetActive(true);
        }
        linesPool.Add(newLine);
        if (!newLineRender)
        {
            newLineRender = newLine.GetComponent<LineRenderer>();
        }
          
        newLineRender.SetPosition(0, new Vector3(curState.x + defaultLineRenderPos[0].x, curState.y + defaultLineRenderPos[0].y, defaultLineRenderPos[0].z));
        setRotate();
        newLineRender.SetPosition(1, new Vector3(curState.x + defaultLineRenderPos[1].x, curState.y + defaultLineRenderPos[1].y, defaultLineRenderPos[1].z));
    }

    private void drawLeaf(string path)
    {
        GameObject go = getLeafFromPool(path);
        if (!go)
        {
            if (!leafGo.ContainsKey(path))
            {
                GameObject tempGo = Resources.Load(path) as GameObject;
                if (tempGo)
                {
                    leafGo.Add(path, tempGo);
                }
                
            }
            go = Instantiate(leafGo[path], transform);
           // go.transform.localPosition = Vector3.zero;
        }
        if (!go.activeSelf)
        {
            go.SetActive(true);
        }
        if (!leavesPool.ContainsKey(path))
        {
            leavesPool[path] = new List<GameObject>();
        }
        
        leavesPool[path].Add(go);
     
        Vector3 pos = go.transform.localPosition;
        pos.x = curState.x + defaultLineRenderPos[0].x;
        pos.y = curState.y + defaultLineRenderPos[0].y;
       
        if (!checkRLRotation())  //L
        {
           
            go.transform.eulerAngles = _leafRLRotations[1];
           // pos.x = -curState.x + transform.position.x;
        }
        else
        {
            go.transform.eulerAngles = _leafRLRotations[0];
           
        }

        go.transform.localPosition = pos;


    }

    private void drawFlower(string path)
    {
        GameObject go = getFlowerFromPool(path);
        if (!go)
        {
            if (!flowerGo.ContainsKey(path))
            {
                GameObject tempGo = Resources.Load(path) as GameObject;
                if (tempGo)
                {
                    flowerGo.Add(path, tempGo);
                }

            }
            go = Instantiate(flowerGo[path], transform);
       
        }
        if (!go.activeSelf)
        {
            go.SetActive(true);
        }
         if (!flowersPool.ContainsKey(path))
        {
            flowersPool[path] = new List<GameObject>();
        }
        
        flowersPool[path].Add(go);
        Vector3 pos = go.transform.localPosition;
        pos.x = curState.x + defaultLineRenderPos[0].x;
        pos.y = curState.y + defaultLineRenderPos[0].y;

        if (!checkRLRotation())
        {
           
            go.transform.eulerAngles = _flowerRLRotations[1];
          //  pos.x = -curState.x + transform.position.x;
        }
        else
        {
            go.transform.eulerAngles = _flowerRLRotations[0];
            
        }

        go.transform.localPosition = pos;


    }

    private void translate()
    {

    }

    //return true: R, return flase: L
    private bool checkRLRotation()
    {
        return Mathf.Sin(curState.angle * Mathf.PI / 180) >= 0;
      
    }

    private void setRotate()
    {
        if(curState.angle != 0)
        {
            curState.x += Mathf.Sin(curState.angle * Mathf.PI / 180) * curState.size;
            curState.y += Mathf.Cos(curState.angle * Mathf.PI / 180) * curState.size;
        }
        else
        {
            curState.y += curState.size;
        }
    }

    private GameObject getLineFromPool()
    {
        int count = linesReturnPool.Count;
        if (count > 0)
        {
            GameObject go = linesReturnPool[count - 1];
            linesReturnPool.RemoveAt(count - 1);
            return go;
        }
        return null;
    }
    private GameObject getLeafFromPool(string key)
    {
        if (!leavesReturnPool.ContainsKey(key))
            return null;

        int count = leavesReturnPool[key].Count;
        if (count > 0)
        {
            GameObject go = leavesReturnPool[key][count - 1];
            leavesReturnPool[key].RemoveAt(count - 1);
            return go;
        }
        return null;
    }

    private GameObject getFlowerFromPool(string key)
    {
        if (!flowersReturnPool.ContainsKey(key))
            return null;

        int count = flowersReturnPool[key].Count;
        if (count > 0)
        {
            GameObject go = flowersReturnPool[key][count - 1];
            flowersReturnPool[key].RemoveAt(count - 1);
            return go;
        }
        return null;
    }

    public void reset(bool destroy = false)
    {
        if (destroy)
        {
            for (int i = linesPool.Count - 1; i >= 0; i--)
            {
                Destroy(linesPool[i]);

                linesPool.RemoveAt(i);
            }
            for (int i = linesReturnPool.Count - 1; i >= 0; i--)
            {
                Destroy(linesReturnPool[i]);
                linesReturnPool.RemoveAt(i);
            }


            foreach (var key in leavesPool.Keys)
            {

                for (int i = leavesPool[key].Count - 1; i >= 0; i--)
                {
                    Destroy(leavesPool[key][i]);
                    leavesPool[key].RemoveAt(i);
                }

            }

            foreach (var key in leavesReturnPool.Keys)
            {
                for (int i = leavesReturnPool[key].Count - 1; i >= 0; i--)
                {
                    Destroy(leavesReturnPool[key][i]);

                    leavesReturnPool[key].RemoveAt(i);
                }
            }

            foreach (var key in flowersPool.Keys)
            {

                for (int i = flowersPool[key].Count - 1; i >= 0; i--)
                {
                    Destroy(flowersPool[key][i]);
                    flowersPool[key].RemoveAt(i);
                }
            }

            foreach (var key in flowersReturnPool.Keys)
            {

                for (int i = flowersReturnPool[key].Count - 1; i >= 0; i--)
                {
                    Destroy(flowersReturnPool[key][i]);
                    flowersReturnPool[key].RemoveAt(i);
                }
            }
        }
        else
        {
            for (int i = linesPool.Count - 1; i >= 0; i--)
            {

                linesPool[i].GetComponent<LineRenderer>().SetPositions(defaultLineRenderPos);
                linesPool[i].SetActive(false);
                linesReturnPool.Add(linesPool[i]);
                linesPool.RemoveAt(i);
            }

            foreach (var key in leavesPool.Keys)
            {

                for (int i = leavesPool[key].Count - 1; i >= 0; i--)
                {

                    leavesPool[key][i].SetActive(false);
                    if(leavesPool[key][i].transform.position.x < 0)
                    {
                        Vector3 pos = leavesPool[key][i].transform.position;
                        pos.x = -pos.x;
                        leavesPool[key][i].transform.position = pos;
                    }
                    if (!leavesReturnPool.ContainsKey(key))
                    {
                        leavesReturnPool[key] = new List<GameObject>();
                    }
                    leavesReturnPool[key].Add(leavesPool[key][i]);
                    leavesPool[key].RemoveAt(i);
                }
            }

            foreach (var key in flowersPool.Keys)
            {

                for (int i = flowersPool[key].Count - 1; i >= 0; i--)
                {

                    flowersPool[key][i].SetActive(false);
                    if (flowersPool[key][i].transform.position.x < 0)
                    {
                        Vector3 pos = flowersPool[key][i].transform.position;
                        pos.x = -pos.x;
                        flowersPool[key][i].transform.position = pos;
                    }
                    if (!flowersReturnPool.ContainsKey(key))
                    {
                        flowersReturnPool[key] = new List<GameObject>();
                    }
                    flowersReturnPool[key].Add(flowersPool[key][i]);
                    flowersPool[key].RemoveAt(i);
                }
            }

        }


        /*
        if (checkIfFinished())
        {
            
            _lSystem.reset();
            
        }*/


    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


}
