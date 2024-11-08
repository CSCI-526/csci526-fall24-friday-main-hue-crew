using UnityEngine;
using TMPro;

public class HelpTextTrigger : MonoBehaviour
{
    public GameObject helpText;
    private TextMeshProUGUI textComponent;
    private BackgroundColorSwapper backgroundColorSwapper;

    public enum VisibleOn
    {
        BlackBackground,
        WhiteBackground,
        BothBackgrounds
    }

    public VisibleOn visibleOn = VisibleOn.BothBackgrounds;
    public KeyCode dismissKey = KeyCode.Space; 

    private bool playerInTrigger = false;

    private void Start()
    {
        helpText.SetActive(false);
        textComponent = helpText.GetComponent<TextMeshProUGUI>();
        backgroundColorSwapper = FindObjectOfType<BackgroundColorSwapper>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInTrigger = true;
            ShowHelpText();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInTrigger = false;
            HideHelpText();
        }
    }

    private void ShowHelpText()
    {
        if (ShouldShowText())
        {
            UpdateTextColor();
            helpText.SetActive(true);
        }
    }

    private void HideHelpText()
    {
        helpText.SetActive(false);
    }

    private bool ShouldShowText()
    {
        if (backgroundColorSwapper != null)
        {
            bool isBackgroundBlack = backgroundColorSwapper.IsBackgroundBlack();
            return visibleOn == VisibleOn.BothBackgrounds ||
                   (visibleOn == VisibleOn.BlackBackground && isBackgroundBlack) ||
                   (visibleOn == VisibleOn.WhiteBackground && !isBackgroundBlack);
        }
        return false;
    }

    private void UpdateTextColor()
    {
        if (backgroundColorSwapper != null && textComponent != null)
        {
            textComponent.color = backgroundColorSwapper.IsBackgroundBlack() ? Color.white : Color.black;
        }
    }

    private void Update()
    {
        if (playerInTrigger)
        {
            if (ShouldShowText())
            {
                UpdateTextColor();
                helpText.SetActive(true);

                if (Input.GetKeyDown(dismissKey))
                {
                    HideHelpText();
                }
            }
            else
            {
                HideHelpText();
            }
        }
    }
}