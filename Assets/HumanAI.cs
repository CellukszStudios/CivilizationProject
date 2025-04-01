using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Profiling.Memory.Experimental;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

public class HumanAI : MonoBehaviour
{
    [Header("Human Attributes")]
    public float Money = 10;
    public float Corruption;
    public float GasPrice;
    public float tax;
    public string name;
    public GameObject Country;
    public GameObject WorkPlace;
    public List<GameObject> properties = new List<GameObject>();
    public bool isMunkanelkuli = true;
    public bool isHajlektalan = true;

    [Header("ScriptAttributes")]
    private string[] abc_magan = { "a","á", "e", "é", "i","í", "o","ó", "ö", "õ", "u", "ú", "ü", "û"};
    private string[] abc_massal = { "b","c","cs","dz","dzs", "d","f", "g","h","j","k","p","s","t", "l", "m", "n", "r", "v", "z",};
    private NavMeshAgent agent;
    public Animator anim;
    public GameObject HumanPrefab;
    public GameObject Children;
    public CountryList country_list;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        GameObject Clist = GameObject.FindWithTag("list");
        country_list = Clist.GetComponent<CountryList>();
        SetName();
        SetAttributes();
        StartCoroutine(Behaviour());
        StartCoroutine(BehaviourEconomy());
        StartCoroutine(eat());
        StartCoroutine(Reproduction());
        StartCoroutine(Death());
    }

    private void SetAttributes()
    {
        Corruption = UnityEngine.Random.Range(0, 100);
        tax = UnityEngine.Random.Range(0, 100);
        GasPrice = UnityEngine.Random.Range(0f, 5f);
    }

    private void SetAnimations()
    {
        if (agent.hasPath)
            anim.SetFloat("Blend", 1);
        else
            anim.SetFloat("Blend", 0);
    }

    private void Update()
    {
        SetAnimations();
    }

    string RandName()
    {
        string r_name = "";

        for (int i = 0; i < UnityEngine.Random.Range(2, 4); i++)
        {
            r_name += abc_magan[UnityEngine.Random.Range(0, abc_magan.Length)];
            r_name += abc_massal[UnityEngine.Random.Range(0, abc_massal.Length)];
        }
        return r_name;
    }

    private void SetName()
    {
        for (int i = 0; i < UnityEngine.Random.Range(2, 4); i++)
        {
            name += abc_magan[UnityEngine.Random.Range(0, abc_magan.Length)];
            name += abc_massal[UnityEngine.Random.Range(0, abc_massal.Length)];
        }

        name += " ";

        for (int i = 0; i < UnityEngine.Random.Range(2, 4); i++)
        {
            name += abc_magan[UnityEngine.Random.Range(0, abc_magan.Length)];
            name += abc_massal[UnityEngine.Random.Range(0, abc_massal.Length)];
        }

        gameObject.name = name;
    }

    private void BuyProperty()
    {
        if (Money > 500)
        {
            CountryAI cAI = Country.GetComponent<CountryAI>();
            if (cAI.Houses.Count < 0) return;

            GameObject house = cAI.Houses[UnityEngine.Random.Range(0, cAI.Houses.Count - 1)];
            if (house && house.GetComponent<HouseObject>().owner == null)
            {
                properties.Add(house);
                house.GetComponent<HouseObject>().owner = gameObject;
                isHajlektalan = false;
                Money -= 500;
            }
        }
    }

    private void GetWork()
    {
        CountryAI cAI = Country.GetComponent<CountryAI>();
        if (cAI.Factories.Count <= 0) return;
        foreach (GameObject factory in cAI.Factories) 
        { 
            FactoryObject fObj = factory.GetComponent<FactoryObject>();
            if (fObj.workers.Count < fObj.MaxWorkers)
            {
                WorkPlace = factory;
                isMunkanelkuli = false;
                fObj.workers.Add(gameObject);
            }
        }
    }

    private void GoWork()
    {
        if (!WorkPlace) return;

        agent.SetDestination(WorkPlace.transform.position);
    }

    private void GoHome()
    {
        if (properties.Count <= 0) return;
        int randIndex = UnityEngine.Random.Range(0, properties.Count);
        if (agent)
            agent.SetDestination(properties[randIndex].transform.position);
    }

    private void GetPaymet()
    {
        if (!WorkPlace) return;
        CountryAI cAI = Country.GetComponent<CountryAI>();
        Money += 10*((100-cAI.tax)/100);
    }

    private void SetPolitic()
    {
        CountryAI cAI = Country.GetComponent<CountryAI>();

        if (Money > 3500 && cAI.politicans.Count < cAI.MaxPoliticans && !cAI.politicans.Contains(gameObject))
        {
            cAI.politicans.Add(gameObject);
        }
    }

    private void Expenses()
    {
        CountryAI cAI = Country.GetComponent<CountryAI>();
        Money -= cAI.GasPrice*properties.Count;
    }

    private void ReProduce()
    {
        CountryAI cAI = Country.GetComponent<CountryAI>();
        GameObject child = Instantiate(HumanPrefab,transform.position,Quaternion.identity);
        HumanAI cHumanAI = child.GetComponent<HumanAI>();

        cHumanAI.name = "";
        child.name = "";

        string[] splitted_name = name.Split(' ');

        cHumanAI.name = splitted_name[0] + " " + RandName();
        child.name = "";
        child.name = cHumanAI.name;
        cHumanAI.WorkPlace = null;
        cHumanAI.properties.Clear();
        cHumanAI.Money = 400;

        Money = Money/2;

        cAI.people.Add(child);
        Children = child;
    }

    IEnumerator Death()
    {
        yield return new WaitForSeconds(6000);
        Destroy(gameObject);
    }

    IEnumerator Reproduction()
    {
        yield return new WaitForSeconds(500);
        if (Money > 50 && WorkPlace && !Children)
        {
            ReProduce();
        } 
        StartCoroutine(Reproduction());
    }

    IEnumerator eat()
    {
        yield return new WaitForSeconds(50);
        if (Money >= 2)
        {
            Money -= 2;
        }else
        {
            country_list.worldDeaths++;
            Destroy(gameObject);
        }
        StartCoroutine(eat());
    }

    IEnumerator Behaviour()
    {
        yield return new WaitForSeconds(5);
        if (!WorkPlace) GetWork();
        if (WorkPlace) 
            isMunkanelkuli = false;
        else
            isMunkanelkuli= true;
        SetPolitic();
        GoWork();
        yield return new WaitForSeconds(5);
        GetPaymet();
        Expenses();
        GoHome();

        StartCoroutine(Behaviour());
    }

    IEnumerator BehaviourEconomy()
    {
        yield return new WaitForSeconds(2);
        BuyProperty();

        StartCoroutine(BehaviourEconomy());
    }
}
