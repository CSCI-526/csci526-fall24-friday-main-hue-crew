using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UIElements;

// public class BackgroundColorSwapper : MonoBehaviour
// {
//     public GameObject background;
//     public GameObject[] blackObstacles;
//     public GameObject[] whiteObstacles;

//     public int maxSwaps = 3;
//     private int swapCount = 0;

//     public TextMeshProUGUI levelText;
//     public TextMeshProUGUI[] whiteBackgroundTexts;  // Array for texts visible on white background
//     public TextMeshProUGUI[] blackBackgroundTexts;  // Array for texts visible on black background

//     private SpriteRenderer spriteRenderer1;
//     private SpriteRenderer spriteRenderer2;
//     public TextMeshProUGUI flipsLeftText;

//     void Start()
//     {
//         if (background != null)
//         {
//             spriteRenderer1 = background.GetComponent<SpriteRenderer>();
//             spriteRenderer1.color = Color.white;
//         }
//         else
//         {
//             Debug.LogError("There is error here");
//         }

//         UpdateObstacleColliders();
//         UpdateTextColor();
//         UpdateFlipsLeftUI();
//     }

//     void Update()
//     {
//         if (Input.GetKeyDown(KeyCode.C))
//         {
//             if (swapCount < maxSwaps)
//             {
//                 if (MetricManager.instance != null)
//                 {
//                     MetricManager.instance.AddToMetric1(1);
//                 }
//                 SwapColors();
//                 swapCount++;
//                 UpdateFlipsLeftUI();
//             }
//             else
//             {
//                 Debug.Log("Swap limit reached for this level!");
//             }
//         }
//     }

//     void SwapColors()
//     {
//         if (spriteRenderer1 != null)
//         {
//             if (spriteRenderer1.color == Color.white)
//             {
//                 spriteRenderer1.color = Color.black;
//             }
//             else
//             {
//                 spriteRenderer1.color = Color.white;
//             }

//             UpdateObstacleColliders();
//             UpdateTextColor();
//         }
//     }

//     void UpdateObstacleColliders()
//     {
//         bool isBackgroundBlack = IsBackgroundBlack();  // Check the current background color

//         foreach (GameObject obstacle in blackObstacles)
//         {
//             if (obstacle != null)
//             {
//                 SpriteRenderer sr = obstacle.GetComponent<SpriteRenderer>();
//                 Collider2D collider = obstacle.GetComponent<Collider2D>();

//                 if (sr != null)
//                 {
//                     sr.enabled = !isBackgroundBlack;
//                 }
//                 if (collider != null)
//                 {
//                     collider.enabled = !isBackgroundBlack;
//                 }
//             }
//         }

//         foreach (GameObject obstacle in whiteObstacles)
//         {
//             if (obstacle != null)
//             {
//                 SpriteRenderer sr = obstacle.GetComponent<SpriteRenderer>();
//                 Collider2D collider = obstacle.GetComponent<Collider2D>();

//                 if (sr != null)
//                 {
//                     sr.enabled = isBackgroundBlack;
//                 }
//                 if (collider != null)
//                 {
//                     collider.enabled = isBackgroundBlack;
//                 }
//             }
//         }
//     }

//     void UpdateFlipsLeftUI()
//     {
//         if (flipsLeftText != null)
//         {
//             int flipsLeft = maxSwaps - swapCount;
//             flipsLeftText.text = $"Flips: {swapCount} / {maxSwaps}";
//         }
//     }

//     public bool IsBackgroundBlack()
//     {
//         if (spriteRenderer1 != null)
//         {
//             return spriteRenderer1.color == Color.black;
//         }
//         return false;
//     }

//     void UpdateTextColor()
//     {
//         /*Color textColor = IsBackgroundBlack() ? Color.white : Color.black;

//         // Update the level text color
//         if (levelText != null)
//         {
//             levelText.color = textColor;
//         }

//         // Set visibility for texts depending on the background color
//         if (whiteBackgroundTexts != null)
//         {
//             foreach (TextMeshProUGUI text in whiteBackgroundTexts)
//             {
//                 text.gameObject.SetActive(!IsBackgroundBlack());  // Show only if background is white
//             }
//         }

//         if (blackBackgroundTexts != null)
//         {
//             foreach (TextMeshProUGUI text in blackBackgroundTexts)
//             {
//                 text.gameObject.SetActive(IsBackgroundBlack());  // Show only if background is black
//             }
//         }*/
//     }
// }


public class BackgroundColorSwapper : MonoBehaviour
{
    public GameObject background;
    public GameObject[] blackObstacles;
    public GameObject[] whiteObstacles;

    public TextMeshProUGUI[] whiteBackgroundTexts;  // Array for texts visible on white background
    public TextMeshProUGUI[] blackBackgroundTexts;  // Array for texts visible on black background

    private SpriteRenderer spriteRenderer1;

    void Start()
    {
        if (background != null)
        {
            spriteRenderer1 = background.GetComponent<SpriteRenderer>();
            spriteRenderer1.color = Color.white;
        }
        else
        {
            Debug.LogError("Background GameObject is missing.");
        }

        UpdateObstacleColliders();
        UpdateTextColor();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C) || Input.GetKeyDown(KeyCode.J))
        {
            if (MetricManager.instance != null)
            {
                MetricManager.instance.AddToMetric1(1);
                MetricManager.instance.AddToSwapPos(GameObject.FindGameObjectWithTag("Player").transform.position);
            }
            SwapColors();
        }
    }

    void SwapColors()
    {
        if (spriteRenderer1 != null)
        {
            spriteRenderer1.color = spriteRenderer1.color == Color.white ? Color.black : Color.white;

            UpdateObstacleColliders();
            UpdateTextColor();
        }
    }

    void UpdateObstacleColliders()
    {
        bool isBackgroundBlack = IsBackgroundBlack();

        foreach (GameObject obstacle in blackObstacles)
        {
            if (obstacle != null)
            {
                SpriteRenderer sr = obstacle.GetComponent<SpriteRenderer>();
                Collider2D collider = obstacle.GetComponent<Collider2D>();

                if (sr != null)
                {
                    sr.enabled = !isBackgroundBlack;
                }
                if (collider != null)
                {
                    collider.enabled = !isBackgroundBlack;
                }
            }
        }

        foreach (GameObject obstacle in whiteObstacles)
        {
            if (obstacle != null)
            {
                SpriteRenderer sr = obstacle.GetComponent<SpriteRenderer>();
                Collider2D collider = obstacle.GetComponent<Collider2D>();

                if (sr != null)
                {
                    sr.enabled = isBackgroundBlack;
                }
                if (collider != null)
                {
                    collider.enabled = isBackgroundBlack;
                }
            }
        }
    }

    public bool IsBackgroundBlack()
    {
        return spriteRenderer1 != null && spriteRenderer1.color == Color.black;
    }

    void UpdateTextColor()
    {
        /*// Set visibility for texts depending on the background color
        if (whiteBackgroundTexts != null)
        {
            foreach (TextMeshProUGUI text in whiteBackgroundTexts)
            {
                text.gameObject.SetActive(!IsBackgroundBlack());  // Show only if background is white
            }
        }

        if (blackBackgroundTexts != null)
        {
            foreach (TextMeshProUGUI text in blackBackgroundTexts)
            {
                text.gameObject.SetActive(IsBackgroundBlack());  // Show only if background is black
            }
        }*/
    }
}
