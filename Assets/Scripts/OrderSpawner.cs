using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrderSpawner : MonoBehaviour
{
    public string bread = "SlicedBun";
    public string lettuce = "SlicedLettuce";
    public string ketchup = "Ketchup";
    public string mustard = "Mustard";
    public string plate = "Plate";
    public List <string> hotdogType = new List <string> ();
    public List <string> order = new List <string> ();

    private void Start()
    {
        hotdogType.Add("NormalDog");
        hotdogType.Add("FootlongDog");
        hotdogType.Add("SausageDog");
    }


    private void createOrder()
    {
        order.Clear ();

        order.Add(plate);
        order.Add(bread);

        string selectedHotdog = hotdogType[Random.Range(0, hotdogType.Count)];
        order.Add(selectedHotdog);

        if (Random.Range(0, 2) == 1) //50% chance to have ketchup or not
        {
            string selectedCondiment1 = ketchup;
            order.Add(selectedCondiment1); // Add first condiment to the order
        }

        if (Random.Range(0, 2) == 1) //50% chance to have mustard or not
        {
            string selectedCondiment2 = mustard;
            order.Add(selectedCondiment2); // Add second condiment to the order
        }

        // Print out the order for debugging purposes
        Debug.Log("Order Created: ");
        foreach (string item in order)
        {
            Debug.Log(item); // Log each item in the order
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Customer"))
        {
            createOrder();
        }
    }
}
