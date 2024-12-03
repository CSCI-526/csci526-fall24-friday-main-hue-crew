using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    public float speed = 5.0f;
    public float jumpForce = 2.0f;
    public float dashDistance = 3.2f;
    public float dashCooldown = 1.0f;
    public float dashTime = 0.2f;

    private bool isGrounded;
    private bool canJumpFromObstacle;
    private bool isDashing = false;
    private float lastDashTime = -Mathf.Infinity;
    private Rigidbody2D rb;
    private Vector3 originalPosition;
    public BackgroundColorSwapper colorSwapScript;

    public GameObject levelPassedText;
    public Image fadeImage;
    private float fadeDuration = 1.0f;
    public Text trapHitText;
    private bool hitTrap = false;


    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        originalPosition = transform.position;

        if (colorSwapScript == null)
        {
            colorSwapScript = FindObjectOfType<BackgroundColorSwapper>();
        }

        if (levelPassedText != null) levelPassedText.SetActive(false);
        if (fadeImage != null) fadeImage.color = new Color(0, 0, 0, 0);
        if (trapHitText != null)
        {
            trapHitText.gameObject.SetActive(false);
        }
    }

    public void FixedUpdate()
    {
        


    }

    private void Update()
    {
        if (isDashing || hitTrap) return;

        float moveHorizontal = Input.GetAxis("Horizontal");
        
        Vector2 movement = new Vector2(moveHorizontal, 0);
        transform.Translate(movement * speed * Time.deltaTime);

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (isGrounded || canJumpFromObstacle)
            {
                rb.AddForce(new Vector2(0, jumpForce), ForceMode2D.Impulse);
                if (canJumpFromObstacle && !isGrounded)
                {
                    canJumpFromObstacle = false;
                }
                isGrounded = false;
            }
        }

        if ((Input.GetKeyDown(KeyCode.V) || (Input.GetKeyDown(KeyCode.K))) && Time.time >= lastDashTime + dashCooldown)
        {
            if (MetricManager.instance != null)
            {
                MetricManager.instance.AddToNumDashes(1);
                MetricManager.instance.AddToDashPos(this.gameObject.transform.position);
            }
            StartCoroutine(PerformDash(moveHorizontal));
        }
    }

    private IEnumerator PerformDash(float moveHorizontal)
    {
        isDashing = true;
        lastDashTime = Time.time;

        float dashDirection = moveHorizontal != 0 ? Mathf.Sign(moveHorizontal) : (transform.localScale.x > 0 ? 1 : -1);

        Vector3 startPosition = transform.position;
        Vector3 targetPosition = startPosition + new Vector3(dashDirection * dashDistance, 0, 0);
        // Debug.Log($"Dashing from {startPosition} to {targetPosition} with dashDistance {dashDistance}");

        float elapsedTime = 0;

        while (elapsedTime < dashTime)
        {
            elapsedTime += Time.deltaTime;
            float step = (dashDistance / dashTime) * Time.deltaTime;
            Vector3 nextPosition = transform.position + new Vector3(dashDirection * step, 0, 0);

            RaycastHit2D hit = Physics2D.BoxCast(transform.position, GetComponent<BoxCollider2D>().size, 0, new Vector2(dashDirection, 0), step);
            if (hit.collider != null && hit.collider.CompareTag("Obstacle"))
            {
                transform.position = new Vector3(hit.point.x - dashDirection * 0.05f, transform.position.y, transform.position.z);
                // Debug.Log("Dash interrupted by obstacle.");
                isDashing = false;
                yield break;
            }

            transform.position = nextPosition;
            yield return null;
        }

        isDashing = false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Ground"))
        {
            isGrounded = true;
        } else if (collision.collider.CompareTag("Obstacle"))
        {
           Bounds obstacleBounds = collision.collider.bounds;

            canJumpFromObstacle = transform.position.y >= obstacleBounds.max.y;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Obstacle"))
        {
            canJumpFromObstacle = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Whitetrap")
    {
        if (colorSwapScript != null && colorSwapScript.IsBackgroundBlack())
        {
            Debug.Log("Player touched Whitetrap while the background is black. Stopping player.");
            hitTrap = true;
            StopPlayer();
        }
    }

    if (collision.gameObject.tag == "Blacktrap")
    {
        if (colorSwapScript != null && !colorSwapScript.IsBackgroundBlack())
        {
            Debug.Log("Player touched Blacktrap while the background is white. Stopping player.");
            hitTrap = true;
            StopPlayer();
        }
    }

        if (collision.gameObject.tag == "RedFlag")
        {
            speed = 0f;
            rb.velocity = Vector2.zero;
            LoadNextLevel();
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Whitetrap")
        {
            if (colorSwapScript != null && colorSwapScript.IsBackgroundBlack())
            {
                Debug.Log("Player is staying on Whitetrap while the background is black. Restarting level.");
                ResetPosition();
            }
        }

        if (collision.gameObject.tag == "Blacktrap")
        {
            if (colorSwapScript != null && !colorSwapScript.IsBackgroundBlack())
            {
                Debug.Log("Player is staying on Blacktrap while the background is black. Restarting level.");
                ResetPosition();
            }
        }
    }

    private void ResetPosition()
    {

        if (trapHitText != null)
        {
            trapHitText.text = "You hit a trap! Restarting level...";
            trapHitText.gameObject.SetActive(true);
        }

        StartCoroutine(RestartLevelWithDelay());
    }

    private IEnumerator RestartLevelWithDelay()
    {
        yield return new WaitForSeconds(2f); // Wait for 2 seconds before restarting
        hitTrap = false; // Reset the hitTrap flag
        if (MetricManager.instance != null)
        {
            MetricManager.instance.AddToResets(1);
            MetricManager.instance.AddToTrapResets(1);
            MetricManager.instance.AddToMetric2(this.gameObject.transform.position);
        }
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void SetJumpAllowed(bool allowed)
    {
        canJumpFromObstacle = allowed;
    }

    /*
    private void LoadNextLevel()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        if (MetricManager.instance != null)
        {
            MetricManager.instance.NextLevel(currentSceneIndex - 1);
        }
        int nextSceneIndex = currentSceneIndex + 1;

        if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
        {
            StartCoroutine(LevelTransition(nextSceneIndex));
        }
        else
        {
            Debug.Log("No more levels to load. This is the last level.");
            if (MetricManager.instance != null)
            {
                MetricManager.instance.NextLevel(currentSceneIndex - 1);
                MetricManager.instance.PushUpload();
            }
        }
    }*/

    private void LoadNextLevel()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        if (MetricManager.instance != null)
        {
            MetricManager.instance.NextLevel(currentSceneIndex - 1);
        }
        int nextSceneIndex = currentSceneIndex + 1;

        if (levelPassedText != null) levelPassedText.SetActive(true); // Always activate levelPassedText

        if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
        {
            StartCoroutine(LevelTransition(nextSceneIndex));
        }
        else
        {
            Debug.Log("No more levels to load. This is the last level.");
            if (MetricManager.instance != null)
            {
                //MetricManager.instance.NextLevel(currentSceneIndex - 1);
                MetricManager.instance.PushUpload();
            }
            
        }
    }


    private IEnumerator LevelTransition(int nextSceneIndex)
    {
        if (levelPassedText != null) levelPassedText.SetActive(true);

        yield return new WaitForSeconds(1.5f);

        if (fadeImage != null)
        {
            for (float t = 0; t < fadeDuration; t += Time.deltaTime)
            {
                fadeImage.color = new Color(0, 0, 0, t / fadeDuration);
                yield return null;
            }
            fadeImage.color = new Color(0, 0, 0, 1);
        }

        SceneManager.LoadScene(nextSceneIndex);
    }

    // Method to restart the level when the Restart button is clicked
    public void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    private void StopPlayer()
    {
        rb.velocity = Vector2.zero;
        speed = 0f;
        if (trapHitText != null)
        {
            trapHitText.text = "You hit a trap! Game Over";
            trapHitText.gameObject.SetActive(true);
        }
        StartCoroutine(RestartLevelWithDelay());
}
}
