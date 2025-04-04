using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CountryAI : MonoBehaviour
{
    [Header("Country Attributes")]
    public float Money = 0;
    public float tax = 0;
    public float corruption = 0;
    public float GasPrice = 0;
    public int GovernMentCycle = 0;
    public int MaxPoliticans = 5;
    public float Depth = 0;
    public GameObject TownHall;
    public GameObject President;
    public List<GameObject> people = new List<GameObject>();
    public List<GameObject> politicans = new List<GameObject>();
    public List<GameObject> army_people = new List<GameObject>();
    public List<GameObject> LoaningCountries = new List<GameObject>();

    [Header("Population")]
    public int Hajlektalans = 0;
    public int Munkanelkulis = 0;
    [Header("Richest Man")]
    public GameObject RichestMan;
    public int RichestManProperties;
    public float RichestManMoney;

    [Header("Economy")]
    public List<GameObject> Houses = new List<GameObject>();
    public List<GameObject> Factories = new List<GameObject>();

    [Header("Politics")]
    public float LeftWing = 0;
    public float RightWing = 0;
    public GameObject PreviousMinister = null;

    [Header("Script Attributes")]
    public GameObject TownHallPrefab;
    public GameObject HumanPrefab;
    public GameObject HousePrefab;
    public GameObject FactoryPrefab;
    public CountryList country_list;
    public GridManagerScript GridSystem;
    public int StartNumber = 10;

    private void Start()
    {
        GameObject Clist = GameObject.FindWithTag("list");
        GameObject gridManager = GameObject.FindWithTag("Grid");
        GridSystem = gridManager.GetComponent<GridManagerScript>();

        country_list = Clist.GetComponent<CountryList>();

        if (!TownHall) TownHall = Build(1000, TownHallPrefab);

        SettleCountry();
        StartCoroutine(StartCor());
    }

    private void Update()
    {
        for (int i = 0; i < people.Count; i++)
        {
            if (people[i] == null) people.Remove(people[i]);
        }
    }

    private void SetRichestMan()
    {
        if (people.Count <= 0) return;

        GameObject RichMan = people[0];

        for (int i = 0; i < people.Count-1; i++)
        {
            if (people[i] && RichMan)
                if (people[i].GetComponent<HumanAI>().Money > RichMan.GetComponent<HumanAI>().Money) RichMan = people[i];
            RichestMan = RichMan;

            if (!RichestMan) return;

            RichestManMoney = RichestMan.GetComponent<HumanAI>().Money;
            RichestManProperties = RichestMan.GetComponent<HumanAI>().properties.Count;
        }
    }

    private void GetPopulation()
    {
        int HajlektalanSum = 0;
        int MunkanelkulisSum = 0;

        for (int i = 0; i < people.Count; i++)
        {
            if (people[i])
            {
                HumanAI hAI = people[i].GetComponent<HumanAI>();
                if (!hAI) return;
                if (hAI.isHajlektalan) HajlektalanSum++;
            }else
            {
                people.Remove(people[i]);
            }
        }

        for (int i = 0; i < people.Count; i++)
        {
            if (!people[i])
            {
                people.Remove(people[i]);
                return;
            }
            HumanAI hAI = people[i].GetComponent<HumanAI>();
            if (hAI.isMunkanelkuli) MunkanelkulisSum++;
        }

        Munkanelkulis = MunkanelkulisSum;
        Hajlektalans = HajlektalanSum;
    }

    private void Politics()
    {
        LeftWing += UnityEngine.Random.Range(-3,10);
        RightWing += UnityEngine.Random.Range(-3,10);

        if (LeftWing < 0) LeftWing = 0; 
        if (RightWing < 0) RightWing = 0; 
    }

    private void SettleCountry()
    {
        for (int i = 0; i < StartNumber; i++)
        {
            GameObject human = Instantiate(HumanPrefab, RandPos(10, 10), Quaternion.identity);
            human.GetComponent<HumanAI>().Country = gameObject;
            people.Add(human);
        }

        for (int i = 0; i < 3; i++)
        {
            GameObject human = people[UnityEngine.Random.Range(0, people.Count-1)];
            if (politicans.Contains(human)) return;
            politicans.Add(human);
        }
    }

    private void FormGovernment()
    {
        if (politicans.Count == 0 || politicans.Count == 1)
        {
            for (int i = 0; i < 2; i++)
            {
                GameObject human = people[UnityEngine.Random.Range(0, people.Count)];
                if (human)
                {
                    if (politicans.Contains(human)) return;
                    politicans.Add(human);
                }
  
            }
        }

        if (politicans.Count <= 1) return;

        President = politicans[UnityEngine.Random.Range(0, politicans.Count)];
        if (!President)
        {
            politicans.Remove(President);
            if (politicans.Count > 0)
            {
                FormGovernment();
            }
        }

        if (PreviousMinister)
            PreviousMinister.GetComponent<HumanAI>().WorkPlace = null;

        if (!President) return;

        if (President && President == PreviousMinister) FormGovernment();
        PreviousMinister = President;
        if (President)
        {
            corruption = President.GetComponent<HumanAI>().Corruption;
            tax = President.GetComponent<HumanAI>().tax;
            GasPrice = President.GetComponent<HumanAI>().GasPrice;
            President.GetComponent<HumanAI>().WorkPlace = TownHall;
        }

        GovernMentCycle = 0;
        GovernMentCycle++;
    }

    private void SelectProject()
    {
        int index = UnityEngine.Random.Range(0, 2);
        int buyableHouses = 0;

        foreach (GameObject house in Houses)
        {
            if (house.GetComponent<HouseObject>().owner == null) buyableHouses++;
        }

        switch (index)
        {
            case 0:
                if (Hajlektalans < Munkanelkulis || Hajlektalans == 0 || buyableHouses > people.Count) break; 
                GameObject house = Build(2500, HousePrefab);
                if (house) Houses.Add(house);
                break;
            case 1:
                if (Hajlektalans > Munkanelkulis || Munkanelkulis == 0) break;
                GameObject factory = Build(5000, FactoryPrefab);
                if (factory) Factories.Add(factory);
                break;
        }
    }

    Vector3 RandPos(float xRange, float zRange)
    {
        int RandomIndex = UnityEngine.Random.Range(0, GridSystem.Grids.Count);
        GameObject RandomGrid = GridSystem.Grids[RandomIndex];
        TileScript RandomTile = RandomGrid.GetComponent<TileScript>();

        if (!RandomTile.isBuildable || Vector3.Distance(transform.position, RandomGrid.transform.position) > 400) 
            return RandPos(xRange, zRange);

        float x = RandomGrid.transform.position.x;
        float z = RandomGrid.transform.position.z;
        Vector3 pos = new Vector3(x, 0, z);

        RandomTile.isBuildable = false;

        return pos;
    }

    GameObject Build(float cost, GameObject prefab)
    {
        if (cost > Money)
            return null;
        else if (UnityEngine.Random.Range(0, 100) < 30)
            GetLoan();

        GameObject building = Instantiate(prefab, RandPos(100,100), Quaternion.identity);

        Money -= cost;

        return building;
    }

    float CollectTax()
    {
        return ((people.Count*4)*(tax/100))*(corruption/100);
    }

    float Corruption()
    {
        float cost = ((100 - corruption) / 100);
        return cost;
    }

    private void Expenses()
    {
        Money -= Factories.Count*2f;
    }

    private void GetLoan()
    {
        if (country_list.Countries.Count == 0) return; 
        GameObject RandomCountry = country_list.Countries[UnityEngine.Random.Range(0, country_list.Countries.Count)];
        CountryAI cAI = RandomCountry.GetComponent<CountryAI>();
        float amount = UnityEngine.Random.Range(0, cAI.Money);
        if (cAI.Money >= amount)
        {
            cAI.Money -= amount;
            Money += amount;
            Depth += amount;
            if (LoaningCountries.Contains(RandomCountry)) return;
            LoaningCountries.Add(RandomCountry);
        }
    }

    private void PayLoan()
    {
        if (LoaningCountries.Count <= 0) return;

        int RandomIndex = UnityEngine.Random.Range(0, LoaningCountries.Count);
        GameObject LoanerCountry = LoaningCountries[RandomIndex];
        CountryAI cAI = LoanerCountry.GetComponent<CountryAI>();

        float amount = Depth * 0.25f;
        if (Money - amount >= 0)
        {
            cAI.Money += amount;
            Money -= amount;
            Depth -= amount;
        }

        if (Depth < 2) LoaningCountries.Remove(LoanerCountry);
    }

    IEnumerator StartCor()
    {
        yield return new WaitForSeconds(5);
        country_list.Countries.Add(gameObject);
        StartCoroutine(Behaviour());
        StartCoroutine(Election());
        FormGovernment();
    }

    IEnumerator Election()
    {
        yield return new WaitForSeconds(120);
        if (LeftWing > RightWing)
            FormGovernment();
        else
            GovernMentCycle++;
        LeftWing = 0;
        RightWing = 0;
        StartCoroutine(Election());
    }

    IEnumerator Behaviour()
    {
        yield return new WaitForSeconds(1);
        GetPopulation();
        Money += CollectTax();
        if (President)
            President.GetComponent<HumanAI>().Money += CollectTax() * Corruption();
        else
            FormGovernment();

        Expenses();
        Politics();
        SelectProject();
        SetRichestMan();
        PayLoan();

        StartCoroutine(Behaviour());
    }
}