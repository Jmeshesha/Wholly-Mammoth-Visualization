using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using System;
public class CamManager : MonoBehaviour
{

    private List<GameObject> virtualCameras = new List<GameObject>();
    public int currentMode = 0;
    public SlideManager slideManager;
    private CinemachineVirtualCamera currCam;
    private CinemachineVirtualCamera prevCam;

    public List<bool> camHasSkin;
    public GameObject skin;
    public GameObject skeleton;


    public float[] speeds;
    public float scrollSpeed = 0.5f;
    bool cursorToggle = true;
     // Start is called before the first frame update
    void Start()
    {
        foreach(Transform child in transform)
        {
            virtualCameras.Add(child.gameObject);
        }
        speeds = new float[virtualCameras.Count];
        Cursor.lockState = CursorLockMode.Locked;
        for(int i = 0; i < virtualCameras.Count; i++)
        {
            speeds[i] = virtualCameras[i].GetComponent<CinemachineVirtualCamera>().GetCinemachineComponent<CinemachinePOV>().m_HorizontalAxis.m_MaxSpeed;
            if (i != currentMode)
            {
                virtualCameras[i].SetActive(false);
            }
            else
            {
                virtualCameras[i].SetActive(true);
                currCam = virtualCameras[i].GetComponent<CinemachineVirtualCamera>();
            }
            
        }
        switchSkeleton();
        
        virtualCameras[0].SetActive(true);

        if(slideManager.slidesPerCam.Count != virtualCameras.Count)
        {
            throw new Exception("Error Slides Per Cam must have the same number of elements as the number of cameras");
        }

        if(virtualCameras.Count != camHasSkin.Count)
        {
            throw new Exception("Error: Cam Has Skin must have the same number of elements as the number of cameras");
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            currCam.GetCinemachineComponent<CinemachinePOV>().m_HorizontalAxis.m_InputAxisName = "Mouse X";
            currCam.GetCinemachineComponent<CinemachinePOV>().m_HorizontalAxis.m_MaxSpeed = 600;
        }
        if(Input.GetMouseButtonUp(0))
        {

            currCam.GetCinemachineComponent<CinemachinePOV>().m_HorizontalAxis.m_MaxSpeed = speeds[currentMode];
            currCam.GetCinemachineComponent<CinemachinePOV>().m_HorizontalAxis.m_InputAxisName = "";
            currCam.GetCinemachineComponent<CinemachinePOV>().m_HorizontalAxis.m_InputAxisValue = 1;
        }
       if((Input.GetMouseButtonDown(4) || Input.GetKeyDown(KeyCode.R)) && !slideManager.isTransitioning())
        {
            if (slideManager.isNextSlideCamChange())
            {
                switchToNewCam();
                switchSkeleton();
            }
            Debug.Log("New Slide: " + currentMode);
            slideManager.nextSlide();
            
            
        }
       if((Input.GetMouseButtonDown(3) || Input.GetKeyDown(KeyCode.E)) && !slideManager.isTransitioning())
        {
            if (slideManager.isPrevSlideCamChange())
            {
                switchToPrevCam();
                switchSkeleton();
            }
            slideManager.prevSlide();
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (cursorToggle)
            {
                Cursor.lockState = CursorLockMode.None;
                cursorToggle = false;
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
                cursorToggle = true;
            }
        }
        if(currCam != null)
        {
            currCam.GetCinemachineComponent<CinemachineFramingTransposer>().m_CameraDistance -= Input.mouseScrollDelta.y * scrollSpeed;
        }
        
        
    }
    float nfmod(float a, float b)
    {
        return a - b * Mathf.Floor(a / b);
    }
    private void switchToNewCam()
    {

        virtualCameras[currentMode].SetActive(false);
        prevCam = currCam;

        int newMode = (currentMode + 1) % virtualCameras.Count;
        virtualCameras[newMode].GetComponent<CinemachineVirtualCamera>().GetCinemachineComponent<CinemachinePOV>().m_HorizontalAxis.m_MaxSpeed = speeds[newMode];
        virtualCameras[newMode].GetComponent<CinemachineVirtualCamera>().GetCinemachineComponent<CinemachinePOV>().m_HorizontalAxis.Value = 0;
        currentMode = newMode;
        currCam = virtualCameras[currentMode].GetComponent<CinemachineVirtualCamera>();
        virtualCameras[currentMode].SetActive(true);
        
    }
    private void switchToPrevCam()
    {

        virtualCameras[currentMode].SetActive(false);
        prevCam = currCam;

        int newMode = (int)nfmod((currentMode - 1), virtualCameras.Count);
        virtualCameras[newMode].GetComponent<CinemachineVirtualCamera>().GetCinemachineComponent<CinemachinePOV>().m_HorizontalAxis.m_MaxSpeed = speeds[newMode];
        virtualCameras[newMode].GetComponent<CinemachineVirtualCamera>().GetCinemachineComponent<CinemachinePOV>().m_HorizontalAxis.Value = 0;
        currentMode = newMode;
        currCam = virtualCameras[currentMode].GetComponent<CinemachineVirtualCamera>();
        virtualCameras[currentMode].SetActive(true);

    }
    public CinemachineVirtualCamera getCurrCam()
    {
        return currCam;
    }
    public CinemachineVirtualCamera getPrevCam()
    {
        return currCam;
    }
    public void switchSkeleton()
    {
        if (camHasSkin[currentMode])
        {
            skin.SetActive(true);
            skeleton.SetActive(false);
        }
        else
        {
            skin.SetActive(false);
            skeleton.SetActive(true);
        }
    }
}
