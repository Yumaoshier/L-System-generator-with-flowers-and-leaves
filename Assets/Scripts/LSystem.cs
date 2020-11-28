using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Text;
using System;

public class LSystem
{
   // private string sentence;
    private Dictionary<char, string> rules = new Dictionary<char, string>();
    private int _iteration = 0;
    public int curIteration { get { return _curIteration; } }
    private int _curIteration = 0;

    private string _axiom;
    public string curSentence { get { return _curSentence; } }
    private string _curSentence;


    public bool checkIfFinished()
    {
        return _curIteration == _iteration;
    }

    public int getRestInteration()
    {
        return _iteration - _curIteration;
    }


    public void setRules(string[] ruleStr)
    {
        rules.Clear();
        for (int i = 0; i < ruleStr.Length; i++)
        {
            string[] rule = ruleStr[i].Split('=');
            if(rule.Length >= 2)
            {
                char key = Convert.ToChar(rule[0]);
                if (rules.ContainsKey(key))
                {
                    rules[key] = rule[1];
                }
                else
                {
                    rules.Add(key, rule[1]);
                }
                
            }
            
        }
       
    }

    public void setIteration(int i)
    {
        reset();
        _iteration = i;
        
    }

    public void setAxiom(string axiom)
    {
        _axiom = axiom;
        _curSentence = axiom;
    }

    public void reset()
    {
        _curIteration = 0;
      
    }

   public string getNewSentence()
    {
        if (_curIteration >= _iteration)
        {
            return null;
        }


        while (_curIteration < _iteration)
        {
            getNewSentencebyStep();
            
        }
     
        return _curSentence;
    }

    public string getNewSentencebyStep()
    {
        if(_curIteration >= _iteration)
        {
            return null;
        }

        StringBuilder nextGen = new StringBuilder();
        for (int i = 0; i < _curSentence.Length; i++)
        {
            if (rules.ContainsKey(_curSentence[i]))
            {
                string v = rules[_curSentence[i]];
                if (v != null)
                {
                    nextGen.Append(v);
                }
            }
            else
            {
                nextGen.Append(_curSentence[i]);
            }
        }
        _curIteration++;
        _curSentence = nextGen.ToString();
        return _curSentence;
    }


}
