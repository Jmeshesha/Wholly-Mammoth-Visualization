using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlideManager : MonoBehaviour
{
    
    [Tooltip("The number of slides every virtual camera will contain")]
    public List<int> slidesPerCam;
    public int currSlideNum = 0;
    public float transitionSpeed = 0.5f;
    float originalAlpha;
    int totalSlides;
    bool isEndCurrSlide = false;
    bool isStartNewSlide = false;
    bool isPrevSlideTransition = false;
    GameObject currentSlide;
    List<GameObject> slides;
    CanvasGroup canvasGroup;
    public bool isScenematicMode = false;
    // Start is called before the first frame update

    float nfmod(float a, float b)
    {
        return a - b * Mathf.Floor(a / b);
    }
    void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        originalAlpha = canvasGroup.alpha;
        slides = new List<GameObject>();
        foreach (Transform child in transform)
        {
            slides.Add(child.gameObject);
            child.gameObject.SetActive(false);
        }
        if(currSlideNum >= slides.Count || currSlideNum < 0)
        {
            currSlideNum = 0;
        }

        slides[currSlideNum].SetActive(true);
    }
    public bool isTransitioning()
    {
        return isEndCurrSlide || isStartNewSlide;
    }

    public bool isNextSlideCamChange()
    {
        if (isScenematicMode)
        {
            return true;
        }
        int i = 0;
        int slides = currSlideNum;
        while (i < slidesPerCam.Count &&  slides - slidesPerCam[i]  >= 0)
        {
            slides -= slidesPerCam[i];
            i++;
        }

        return slidesPerCam[i] - slides < 2;
    }
    public bool isPrevSlideCamChange()
    {
        if (isScenematicMode)
        {
            return true;
        }
        int i = 0;
        int slides = currSlideNum;
        while (i < slidesPerCam.Count && slides - slidesPerCam[i] >= 0)
        {
            slides -= slidesPerCam[i];
            i++;
        }
        Debug.Log(slides < 1);
        return slides < 1;
    }
    public void nextSlide()
    {
        isEndCurrSlide = true;

        isPrevSlideTransition = false;
    }
    public void prevSlide()
    {
        isEndCurrSlide = true;
        isPrevSlideTransition = true;

    }
    private void transitionSlides()
    {
        int nextSlideNum = (currSlideNum + 1) % slides.Count;
        if (isScenematicMode)
        {
            int i = 0;
            int slide = currSlideNum;
            while (i < slidesPerCam.Count && slide - slidesPerCam[i] >= 0)
            {
                slide -= slidesPerCam[i];
                i++;
            }
            if (isPrevSlideTransition)
            {
                nextSlideNum = (int)nfmod((currSlideNum - slidesPerCam[i] + slide), slides.Count);

            }
            else
            {
                nextSlideNum = (currSlideNum + slidesPerCam[i] - slide) % slides.Count;

            }
        }

        if (isPrevSlideTransition)
        {
            nextSlideNum = (int)nfmod((currSlideNum - 1), slides.Count);
        }
        slides[currSlideNum].SetActive(false);
        slides[nextSlideNum].SetActive(true);
        currSlideNum = nextSlideNum;

    }
    // Update is called once per frame
    void Update()
    {   if(isEndCurrSlide && isScenematicMode)
        {
            transitionSlides();
            isEndCurrSlide = false;
        }else if (isEndCurrSlide) {
            canvasGroup.alpha -= Time.deltaTime * transitionSpeed;
            if(canvasGroup.alpha <= 0) {
                canvasGroup.alpha = 0;
                isEndCurrSlide = false;
                isStartNewSlide = true;
                transitionSlides();
            }
        }else if (isStartNewSlide) {
            canvasGroup.alpha += Time.deltaTime * transitionSpeed;
            if(canvasGroup.alpha >= originalAlpha) {
                canvasGroup.alpha = originalAlpha;
                isStartNewSlide = false;
            }
        }
        if (Input.GetKeyUp(KeyCode.Q))
        {
            if (isScenematicMode)
            {
                isScenematicMode = false;
                canvasGroup.alpha = originalAlpha;
            }
            else
            {
                isScenematicMode = true;
                canvasGroup.alpha = 0;
            }
        }
    }

}
