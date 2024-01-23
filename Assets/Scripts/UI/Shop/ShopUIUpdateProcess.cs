using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ShopUIUpdateProcess : MonoBehaviour
{
    [SerializeField] public GameObject[] Positions;
    [SerializeField] public GameObject[] Books;

    [Header("Data Keeper")]
    [SerializeField] public GameInfo GameInfo;

    private void Start()
    {
        RandomGenerateBook();
    }

    private void RandomGenerateBook()
    {
        List<GameObject> availableBooks = new List<GameObject>(Books); // Create a list to store available books

        for (int i = 0; i < Positions.Length; i++)
        {
            if (availableBooks.Count == 0)
            {
                Debug.Log("Not enough available books!");
                return;
            }

            int randomIndex = Random.Range(0, availableBooks.Count); 
            GameObject selectedBook = availableBooks[randomIndex];

            GameObject instantiatedBook = Instantiate(selectedBook, Positions[i].transform.position, Quaternion.identity, transform);
            //Positions[i].transform.position = selectedBook.transform.position; 
            //instantiatedBook.transform.SetParent(transform.parent);

            availableBooks.RemoveAt(randomIndex);
        }
    }
}
