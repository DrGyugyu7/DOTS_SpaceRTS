using System.Collections;
using TMPro;
using UnityEngine;

public class TextEffect : MonoBehaviour
{
    [SerializeField] TMP_Text tmpProText;
    string writer;
    [SerializeField] private Coroutine coroutine;

    [SerializeField] float delayBeforeStart = 0f;
    [SerializeField] float timeBtwChars = 0.1f;
    [SerializeField] string leadingChar = "";
    [SerializeField] bool leadingCharBeforeDelay = false;

    enum options { clear, complete }

    // Use this for initialization
    void Awake()
    {
        if (tmpProText != null)
        {
            writer = tmpProText.text;
        }
    }
    public void SetNewText(string newText)
    {
        writer = newText;
        StartTypewriter();
    }
    void Start()
    {
        if (tmpProText != null)
        {
            tmpProText.text = "";
        }
    }
    private void OnEnable()
    {
        StartTypewriter();
    }
    private void StartTypewriter()
    {
        StopAllCoroutines();
        if (tmpProText != null)
        {
            tmpProText.text = "";

            StartCoroutine("TypeWriterTMP");
        }
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    IEnumerator TypeWriterTMP()
    {
        tmpProText.text = leadingCharBeforeDelay ? leadingChar : "";

        yield return new WaitForSeconds(delayBeforeStart);

        foreach (char c in writer)
        {
            if (tmpProText.text.Length > 0)
            {
                tmpProText.text = tmpProText.text.Substring(0, tmpProText.text.Length - leadingChar.Length);
            }
            tmpProText.text += c;
            tmpProText.text += leadingChar;
            yield return new WaitForSeconds(timeBtwChars);
        }

        if (leadingChar != "")
        {
            tmpProText.text = tmpProText.text.Substring(0, tmpProText.text.Length - leadingChar.Length);
        }
    }
}
