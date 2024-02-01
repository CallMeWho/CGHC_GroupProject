using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ShopUIUpdateProcess : MonoBehaviour
{
    [SerializeField] public GameObject[] Positions;
    [SerializeField] public GameObject[] Books;

    public static GameObject ShopCanvasObj;

    [Header("Data Keeper")]
    [SerializeField] public GameInfo GameInfo;
    private bool bookGenerated = false;
    private List<GameObject> generatedBooks = new List<GameObject>();

    private void Start()
    {
        // hide dead screen
        gameObject.SetActive(false);

        ShopCanvasObj = gameObject;
        Canvas canvas = gameObject.GetComponent<Canvas>();
        canvas.sortingLayerName = "Map";
        canvas.sortingOrder = 10;
    }

    private void OnEnable()
    {
        if (!bookGenerated)
        {
            bookGenerated = true;
            RandomGenerateBook();
        }
    }

    private void OnDisable()
    {
        bookGenerated = false;
        ClearGeneratedBooks();
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
            generatedBooks.Add(instantiatedBook);
            //Positions[i].transform.position = selectedBook.transform.position; 
            //instantiatedBook.transform.SetParent(transform.parent);

            availableBooks.RemoveAt(randomIndex);
        }
    }

    private void ClearGeneratedBooks()
    {
        foreach (GameObject book in generatedBooks)
        {
            Destroy(book);
        }
        generatedBooks.Clear();
    }
}
