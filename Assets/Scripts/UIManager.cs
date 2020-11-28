using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;


public struct turtleData
{
    public char[] drawSymbols;
    public char[] leafSymbol;
    public char[] flowerSymbol;
    public string leafImgPath;
    public string flowerImgPath;
    public string axiom;
    public float angle;
    public string[] rules;
    public int interation;

    public turtleData(string ds = "F", string ls = "", string fs = "", string lip = "", string fip = "", string ax = "F", float an = 0f, string rs = "", int it = 0)
    {
        drawSymbols = ds.ToCharArray();
        leafSymbol = ls.ToCharArray();
        flowerSymbol = fs.ToCharArray();
        leafImgPath = lip;
        flowerImgPath = fip;
        axiom = ax;
        angle = an;
        rules = rs.Split(';');
        interation = it;
    }

    public bool checkValue(turtleData td)
    {
        bool ifSame = axiom == td.axiom && angle == td.angle && interation == td.interation && leafImgPath == td.leafImgPath && flowerImgPath == td.flowerImgPath;
        if (!ifSame)
            return false;


        ifSame = Enumerable.SequenceEqual(drawSymbols, td.drawSymbols);
        if (!ifSame)
            return false;

        ifSame = Enumerable.SequenceEqual(leafSymbol, td.leafSymbol);
        if (!ifSame)
            return false;

        ifSame = Enumerable.SequenceEqual(flowerSymbol, td.flowerSymbol);
        if (!ifSame)
            return false;

        ifSame = Enumerable.SequenceEqual(rules, td.rules);
        return ifSame;

    }
}

public class UIManager : MonoBehaviour
{

    public InputField inputDrawSymbols;
    public InputField inputLeafSymbol;
    public Dropdown dropLeafImgs;
    public InputField inputFlowerSymbol;
    public Dropdown dropFlowerImgs;
    public InputField inputAxiom;

    public List<InputField> inputRules = new List<InputField>(3);
    
    public InputField inputAngle;
    public InputField inputIteration;
    public Dropdown dropGivenExamples;
    public GameObject warnPanel;
    public Text warnText;
    public GameObject bannedInput;
    public Text curIterationText;
    public Slider scaleSlider;
    public Text scaleText;
    public Slider rotateSlider;
    public Text rotateText;
    public Slider ySlider;
    public Text yText;
    public Slider xSlider;
    public Text xText;




    public Turtle turtle;
    private int ruleSize = 3;
    
    private turtleData curTurtleData;
    private turtleData preTurtleData;
    private float warnTextShowTime;
    private bool isWarnTextShowed = false;
    private List<turtleData> exampleTurtleDatas = new List<turtleData>();
    private List<string> leafPaths = new List<string>();
    private List<string> flowerPaths = new List<string>();




    private void InitializeExampleRules()
    {
        TextAsset file = Resources.Load<TextAsset>("Files/ExampleRules");
        Dictionary<string, turtleData> ruleDic;
        FileParser.ParseRules(file.text, out ruleDic);
        List<string> dropOptions = new List<string>();
       
        foreach(var d in ruleDic)
        {
            dropOptions.Add(d.Key);
            exampleTurtleDatas.Add(d.Value);
        }
        dropGivenExamples.ClearOptions();
        dropGivenExamples.AddOptions(dropOptions);
    }
    
    private void InitializePaths()
    {
        TextAsset file = Resources.Load<TextAsset>("Files/LeafImgPaths");
        Dictionary<string, string> pathDic;
        FileParser.ParsePaths(file.text, out pathDic);
        List<string> dropOptions = new List<string>();
        foreach (var d in pathDic)
        {
            dropOptions.Add(d.Key);
            leafPaths.Add(d.Value);
        }
        dropLeafImgs.ClearOptions();
        dropLeafImgs.AddOptions(dropOptions);

        file = Resources.Load<TextAsset>("Files/FlowerImgPaths");
        pathDic.Clear();
        FileParser.ParsePaths(file.text, out pathDic);
        dropOptions.Clear();
        foreach (var d in pathDic)
        {
            dropOptions.Add(d.Key);
            flowerPaths.Add(d.Value);
        }
        dropFlowerImgs.ClearOptions();
        dropFlowerImgs.AddOptions(dropOptions);
    }


    // Start is called before the first frame update
    void Start()
    {
        warnTextShowTime = 0;
        InitializeExampleRules();
        InitializePaths();
        dropGivenExamples.onValueChanged.AddListener(delegate
        {
            dropExamplesValueChanged();
        });
        
        
    }

    // Update is called once per frame
    void Update()
    {
      
        if (isWarnTextShowed)
        {
            warnTextShowTime += Time.deltaTime;
            //Debug.Log(dialogShowTime);
            Color color = new Color();
            color.a = Mathf.Lerp(0, 1, warnTextShowTime / 4);           
            warnPanel.GetComponent<Image>().color = color;
        }
        else
        {
            if(warnTextShowTime == 0)
            {
                return;
            }
            else if(warnTextShowTime > 0)
            {
                warnTextShowTime -= Time.deltaTime;
                // Debug.Log(dialogShowTime);
                Color color = new Color();
                color.a = Mathf.Lerp(0, 1, warnTextShowTime / 4);
                warnPanel.GetComponent<Image>().color = color;
            }           
            else
            {
                warnTextShowTime = 0;
                warnPanel.SetActive(false);
            }
        }

        if (warnTextShowTime >= 1f)
        {
          
            isWarnTextShowed = false;

        }
    }

    public void onClickGenAll()
    {
        if (turtle.checkIfFinished())
        {
            if (checkValue())
                return;
        }

        if (bannedInput.activeSelf)  //during the step, do not need to set value
        {
            bannedInput.SetActive(false);
        }
        else
        {
            setVariables();
        }
       
        setCurIterationText(inputIteration.text);
        turtle.generate(true);
    }

    public void onClickGenStep()
    {
        if (bannedInput.activeSelf)   //during the step
        {
            if (turtle.getRestInteration() == 1)
            {
                bannedInput.SetActive(false);
            }
       
        }
        else 
        {
            if (checkValue())
                return;

            bannedInput.SetActive(true);
            setVariables();
        }

       
        turtle.generate();
        setCurIterationText(turtle.getCurInteration().ToString());
    }

    public void onClickBannedPanel()
    {
        showWarnText("Please do not change the value during the step!");
    }

    public void onScaleSlidValueChanged()
    {
       
        turtle.transform.localScale = Vector3.one * scaleSlider.value;
        scaleText.text = scaleSlider.value.ToString();
    }

    public void onRotateSlidValueChanged()
    {
        Vector3 ea = turtle.transform.localEulerAngles;
        ea.y = rotateSlider.value;
        turtle.transform.localEulerAngles = ea;
        rotateText.text = rotateSlider.value.ToString();
    }

    public void onYSlidValueChanged()
    {
        Vector3 pos = turtle.transform.localPosition;
        pos.y = ySlider.value;
        turtle.transform.localPosition = pos;
        yText.text = ySlider.value.ToString();
    }

    public void onXSlidValueChanged()
    {
        Vector3 pos = turtle.transform.localPosition;
        pos.x = xSlider.value;
        turtle.transform.localPosition = pos;
        xText.text = xSlider.value.ToString();
    }

    private void setCurIterationText(string it)
    {
        curIterationText.text = it;
    }

    private bool checkValue()
    {
        if(inputDrawSymbols.text == "" || inputAxiom.text == "")
        {
            return true;
        }
        float angle = 0;
        int interation = 0;
        if(inputAngle.text == "")
        {
            inputAngle.text = "0";
        }
        else
        {
            angle = float.Parse(inputAngle.text);
        }
        if(inputIteration.text == "")
        {
            inputIteration.text = "0";
        }
        else
        {
            interation = Convert.ToInt32(inputIteration.text);
        }

        string ruleS = "";
        for (int i = 0; i < inputRules.Count; i++)
        {
            if(inputRules[i].text == "" || inputRules[i].text == null)
            {
                continue;
            }
           
            ruleS += ";" + inputRules[i].text;
        }
        if(ruleS.Length > 0)
        {
            ruleS.Remove(0, 1);
        }
        
        curTurtleData = new turtleData(inputDrawSymbols.text, inputLeafSymbol.text, inputFlowerSymbol.text, leafPaths[dropLeafImgs.value], flowerPaths[dropFlowerImgs.value],  inputAxiom.text, angle, ruleS, interation);
        bool same = preTurtleData.checkValue(curTurtleData);
        if (!same)
            preTurtleData = curTurtleData;
        return same;
            
    }

    private void setVariables()
    {
        turtle.setDrawSymbols(curTurtleData.drawSymbols, curTurtleData.leafSymbol, curTurtleData.flowerSymbol);
      
        turtle.setRules(curTurtleData.rules);
        turtle.setData(curTurtleData.axiom, curTurtleData.angle, curTurtleData.interation, curTurtleData.leafImgPath, curTurtleData.flowerImgPath);
        
    }

   private void dropExamplesValueChanged()
    {
        turtleData tData = exampleTurtleDatas[dropGivenExamples.value];
        if (tData.drawSymbols != null)
        {
            inputDrawSymbols.text = new string(tData.drawSymbols);
        }
        else
        {
            inputDrawSymbols.text = "";
        }
        if (tData.leafSymbol != null)
        {
            inputLeafSymbol.text = new string(tData.leafSymbol);
        }
        else
        {
            inputLeafSymbol.text = "";
        }
        if (tData.flowerSymbol != null)
        {
            inputFlowerSymbol.text = new string(tData.flowerSymbol);
        }
        else
        {
            inputFlowerSymbol.text = "";
        }

        if (tData.leafImgPath != null)
        {
            dropLeafImgs.value = leafPaths.IndexOf(tData.leafImgPath);
        }
        else
        {
            dropLeafImgs.value = 0;
        }
        if (tData.leafImgPath != null)
        {
            dropFlowerImgs.value = flowerPaths.IndexOf(tData.flowerImgPath);
        }
        else
        {
            dropFlowerImgs.value = 0;
        }
     
        inputAxiom.text = tData.axiom;
        inputAngle.text = tData.angle.ToString();
        for(int i=0; i < ruleSize; i++)
        {
            if(i < tData.rules.Length)
            {
                inputRules[i].text = tData.rules[i];
            }
            else
            {
                inputRules[i].text = "";
            }
           
        }
        inputIteration.text = tData.interation.ToString();
    }


    private void showWarnText(string text)
    {
        warnText.text = text;
        warnPanel.SetActive(true);
        isWarnTextShowed = true;
    }

}
