using UnityEngine;
using System.Collections.Generic;

public class Triple<A, B, C>
{
    public A valueA;
    public B valueB;
    public C valueC;
    public bool add;

    public Triple(A a_, B b_, C c_, bool add_)
    {
        valueA = a_;
        valueB = b_;
        valueC = c_;
        add = add_;
    }
}

public class HTNState
{
    /// <summary>
    /// Dictionary of all ids and objects so that we can grab the object if needed. Does not need/want a deep copy.
    /// </summary>
    Dictionary<string, object> objectDict = new Dictionary<string, object>();

    /// <summary>
    /// Dictionary containing all relations in this state.
    /// </summary>
    Dictionary<string, Dictionary<string, List<string>>> relations = new Dictionary<string, Dictionary<string, List<string>>>();

    public HTNState()
    {
        
    }



    public bool AddObject(string id_, object obj_)
    {
        if(!objectDict.ContainsKey(id_))
        {
            objectDict.Add(id_, obj_);
            return true;
        }

        return false;
    }

    public bool RemoveObject(string id_)
    {
        return objectDict.Remove(id_);
    }

    public object GetObject(string id_)
    {
        if(objectDict.ContainsKey(id_))
        {
            return objectDict[id_];
        }

        return null;
    }

    public bool Add(string variable_, string valueA_, string valueB_)
    {
        if(!relations.ContainsKey(variable_))
        {
            relations[variable_] = new Dictionary<string, List<string>>();
        }

        if(!relations[variable_].ContainsKey(valueA_))
        {
            relations[variable_].Add(valueA_, new List<string>(){valueB_});
            return true;
        }

        if(!relations[variable_][valueA_].Contains(valueB_))
        {
            relations[variable_][valueA_].Add(valueB_);
            return true;
        }

        return false;
    }

    public bool Remove(string variable_, string valueA_, string valueB_)
    {
        bool successful = false;
        if(relations.ContainsKey(variable_) && relations[variable_].ContainsKey(valueA_))
        {
            successful = relations[variable_][valueA_].Remove(valueB_);
            if(successful && relations[variable_][valueA_].Count == 0)
            {
                relations[variable_].Remove(valueA_);
                if(relations[variable_].Count == 0)
                {
                    relations.Remove(variable_);
                }
            }
        }

        return false;
    }

    public bool Remove(string variable_, string valueA_)
    {
        bool successful = false;
        if(relations.ContainsKey(variable_))
        {
            successful = relations.Remove(valueA_);
            if(successful && relations[variable_].Count == 0)
            {
                relations.Remove(variable_);
            }
        }

        return successful;
    }

    public void UpdateRelation(string variable_, string valueA_, string valueB_, bool flag_)
    {
        if (flag_)
        {
            Add(variable_, valueA_, valueB_);
        }
        else
        {
            Remove(variable_, valueA_, valueB_);
        }
    }

    public void UpdateRelation(Triple<string, string, string> relation_)
    {
        UpdateRelation(relation_.valueA, relation_.valueB, relation_.valueC, relation_.add);
    }

    public Dictionary<string, List<string>> GetRelations(string variable_)
    {
        if(relations.ContainsKey(variable_))
        {
            return relations[variable_];
        }

        return null;
    }

    public List<string> GetRelations(string variable_, string valueA_)
    {
        Dictionary<string, List<string>> rel = GetRelations(variable_);
        if(rel != null && rel.ContainsKey(valueA_))
        {
            return rel[valueA_];
        }

        return null;
    }

    public bool HasRelation(string variable_, string valueA_, string valueB_)
    {
        return relations.ContainsKey(variable_) && relations[variable_].ContainsKey(valueA_) && relations[variable_][valueA_].Contains(valueB_);
    }

    public bool HasRelation(string variable_, string valueA_)
    {
        return relations.ContainsKey(variable_) && relations[variable_].ContainsKey(valueA_);
    }

    public List<string> Unify(string variableA_, string valueA_, string variableB_, string valueB_)
    {

        List<string> listA = GetRelations(variableA_, valueA_);
        List<string> listB = GetRelations(variableB_, valueB_);

        List<string> matchingElements = new List<string>();

        foreach(string elemA in listA)
        {
            foreach(string elemB in listB)
            {
                if(elemA.Equals(elemB))
                {
                    matchingElements.Add(elemA);
                }
            }
        }

        return matchingElements;
    }

    //public HTNState Clone()
    //{
    //    Dictionary<string, List<string>> temp = new Dictionary<string, List<string>>();
    //}

    public void Print()
    {
        UnityEngine.Debug.Log("StateRelations");
        foreach (var variable in relations.Keys)
        {
            foreach (var valueA in relations[variable].Keys)
            {
                string valueB = "[";
                valueB += string.Join(", ", relations[variable][valueA].ToArray());
                valueB += "]";
                UnityEngine.Debug.Log("variable: " + variable + " valueA: " + valueA + " valueB: " + valueB);
            }
        }
    }

    List<string> DeepCopy(List<string> original_)
    {
        string[] copy = new string[original_.Count];
        original_.CopyTo(copy);
        return new List<string>(copy);
    }

    Dictionary<string, List<string>> DeepCopy(Dictionary<string, List<string>> dict_)
    {
        Dictionary<string, List<string>> copy = new Dictionary<string, List<string>>();
        foreach(KeyValuePair<string, List<string>> k in dict_)
        {
            copy.Add(k.Key, DeepCopy(k.Value));
        }
        return copy;
    }

    Dictionary<string, Dictionary<string, List<string>>> DeepCopy(Dictionary<string, Dictionary<string, List<string>>> dict_)
    {
        Dictionary<string, Dictionary<string, List<string>>> copy = new Dictionary<string, Dictionary<string, List<string>>>();
        foreach (KeyValuePair<string, Dictionary<string, List<string>>> k in dict_)
        {
            copy.Add(k.Key, DeepCopy(k.Value));
        }
        return copy;
    }

    public HTNState DeepCopy()
    {
        HTNState copy = new HTNState();
        copy.relations = DeepCopy(relations);
        copy.objectDict = objectDict; // straight reference since this will never change between states

        return copy;
    }
}
