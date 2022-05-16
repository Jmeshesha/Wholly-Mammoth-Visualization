using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
public class AudioManager : MonoBehaviour
{
    // Start is called before the first frame update
    public AudioSource source;
    public Camera cam;
    public CamManager camManager;
    public CinemachineBrain brain;
    public float ambientPitch;
    public float velocityScaler;
    bool isTransitioning = false;
    public float maxPitch;
    float prevTime = 0;
    void Start()
    {
        
    }
    float parabola(float maxX, float maxOutput, float x)
    {
        float a = 4 * maxOutput / (maxX * maxX);
        float k = maxX / 2;
        return -a * (x-k)*(x-k) + maxOutput;
    }

    // Update is called once per frame
    void Update()
    {
        float speed = 0;
        if (brain.ActiveBlend != null)
        {
            //Debug.Log(brain.CurrentCameraState.FinalPosition - camManager.getPrevCam().gameObject.transform.position);
            float currTime = brain.ActiveBlend.TimeInBlend/brain.ActiveBlend.Duration;
            float p = parabola(1, velocityScaler, currTime) ;
            speed = p*Vector3.Magnitude(cam.velocity);
            speed = Mathf.SmoothStep(0, maxPitch, speed);
            prevTime = currTime;
        }
        else
        {
            prevTime = 0;
        }
        
        float pitch = ambientPitch +  speed;
        //source.pitch = Mathf.SmoothStep(ambientPitch, maxPitch, pitch);
        source.pitch = pitch;

    }
    public void addTransitionSound()
    {

    }
}
