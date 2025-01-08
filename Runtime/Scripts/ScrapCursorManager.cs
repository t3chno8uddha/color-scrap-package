using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ScrapCursorManager : MonoBehaviour
{
    [SerializeField] float zDistance;

    [SerializeField] float threshold = 2f;

    [SerializeField] float smoothness;

    bool tapped = false;

    [SerializeField] CustomRenderTexture finalTex;

    [SerializeField] GameObject puzzlePhase;

    [SerializeField] Camera scrapCam, maskCam;
    [SerializeField] GameObject scrapGame;

    ColorScrapEntryPoint _entryPoint;
    [SerializeField] Button homeButton;

    [SerializeField] GameObject penCursor;

    TrailRenderer trail;

    [SerializeField] Transform lineParent;

    private void Awake()
    {
        homeButton.onClick.AddListener(FinishOnButton);
    }

    void FinishOnButton()
    {
        _entryPoint.InvokeGameFinished();
    }

    void Start()
    {
        Shader.SetGlobalTexture("_ScrapTexture", scrapCam.targetTexture);

        Shader.SetGlobalTexture("_ScrapMaskTexture", maskCam.targetTexture);
    }

    //RenderTexture NewText() {  return new RenderTexture(256, 256, 16); }

    void Update()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                if (!trail)
                {
                    trail = Instantiate(penCursor, lineParent).GetComponent<TrailRenderer>();
                }

                Vector3 touchPosition = touch.position;
                touchPosition.z = zDistance; // Set an appropriate Z distance

                Vector3 newPosition = Camera.main.ScreenToWorldPoint(touchPosition);

                trail.transform.position = newPosition;
                tapped = true;
            }
            else if (touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary)
            {
                Vector3 touchPosition = touch.position;
                touchPosition.z = zDistance; // Set an appropriate Z distance

                Vector3 newPosition = Camera.main.ScreenToWorldPoint(touchPosition);

                trail.transform.position = Vector3.MoveTowards(trail.transform.position, newPosition, smoothness * Vector3.Distance(trail.transform.position, newPosition) * Time.deltaTime);
            }
            else if (touch.phase == TouchPhase.Ended)
            {
                CheckSimilarity();
            }
        }
        else if (Input.touchCount == 0 && tapped) // No touches but previously tapped
        {
            tapped = false;
        }
    }

    public void CheckSimilarity()
    {
        Texture2D tex = new Texture2D(finalTex.width, finalTex.height, TextureFormat.RGBA32, false);

        // Copy CRT content to the Texture2D
        RenderTexture.active = finalTex;
        tex.ReadPixels(new Rect(0, 0, finalTex.width, finalTex.height), 0, 0);
        tex.Apply();
        RenderTexture.active = null;

        // Access the raw texture data
        Color32[] pixels = tex.GetPixels32();
        int totalPixels = pixels.Length;

        int blackCount = 0;
            
        // Iterate through pixels
        for (int i = 0; i < totalPixels; i++)
        {
            Color32 pixel = pixels[i];

            if (pixel.r == 0 && pixel.g == 0 && pixel.b == 0)
            {
                blackCount++;
            }
        }

        float blackPercentage = (blackCount / (float)totalPixels) * 100f;

        if (blackPercentage < threshold)
        {
            puzzlePhase.SetActive(true);
            scrapGame.SetActive(false);
        }
    }

    public void WinGame()
    {
        print("Won Game");
        SetFinishForPackage();
    }

    public void SetEntryPoint(ColorScrapEntryPoint entryPoint)
    {
        _entryPoint = entryPoint;
    }

    void SetFinishForPackage()
    {
        StartCoroutine(FinishAfterFireworks());
    }

    IEnumerator FinishAfterFireworks()
    {
        yield return new WaitForSecondsRealtime(5f);
        _entryPoint.InvokeGameFinished();
    }
}