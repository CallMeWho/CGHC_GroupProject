using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ShopUIUpdateProcess : MonoBehaviour
{
    public static GameObject ShopCanvasObj;

    [SerializeField] private GameObject[] bookPositions;
    [SerializeField] private GameObject[] availableBooks;

    private bool bookGenerated = false;
    private Queue<GameObject> generatedBooks = new Queue<GameObject>();

    private void Start()
    {
        // Set the game object to be inactive at the start
        gameObject.SetActive(false);

        // Assign the game object to the static variable for easy access
        ShopCanvasObj = gameObject;

        // Get the canvas component and set the sorting layer and order
        var canvas = gameObject.GetComponent<Canvas>();
        canvas.sortingLayerName = "Map";
        canvas.sortingOrder = 10;
    }

    private void OnEnable()
    {
        if (!bookGenerated)
        {
            bookGenerated = true;
            RandomGenerateBooks();
        }
    }

    private void OnDisable()
    {
        bookGenerated = false;
        ClearGeneratedBooks();
    }

    private void RandomGenerateBooks()
    {
        var availableBooksList = new List<GameObject>(availableBooks);

        for (int i = 0; i < bookPositions.Length; i++)
        {
            if (availableBooksList.Count == 0)
            {
                // Log an error if there are no available books
                Debug.LogError("Not enough available books!");
                return;
            }

            int randomIndex = Random.Range(0, availableBooksList.Count);
            GameObject selectedBook = availableBooksList[randomIndex];

            // Instantiate the selected book at the position of the book position object
            GameObject instantiatedBook = Instantiate(selectedBook,
                bookPositions[i].transform.position, Quaternion.identity, transform);
            generatedBooks.Enqueue(instantiatedBook);

            // Remove the selected book from the available books list
            availableBooksList.RemoveAt(randomIndex);
        }
    }

    private void ClearGeneratedBooks()
    {
        while (generatedBooks.Count > 0)
        {
            // Destroy the generated books
            GameObject book = generatedBooks.Dequeue();
            Destroy(book);
        }
    }
}