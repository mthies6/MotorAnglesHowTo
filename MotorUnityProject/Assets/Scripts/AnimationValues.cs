using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

[RequireComponent(typeof(Animator))]
public class AnimationValues : MonoBehaviour
{
    //the animation clip that you want to be turned into an angle text file
    public AnimationClip animClip;
    //the GameObject representation of the motor, which can be viewed in the Unity animation sheet (only object with curves)
    public GameObject[] objs;
    //array of rotatingObject instances 
    private Motor[] rotatingObjects;

    //global index for keyframes/events
    private int currKeyframe;
    //global length for number of keyframes/events
    private int numOfKeyframes;
    //array of times (time of each keyframe)
    private float[] keyframeTimes;
    //number of frames (50 fps * seconds) in the animation (used for interpolation; 50 fps value from Peter)
    private int count;

    //output of interpolated values
    private (float[] times, float[] angles) interpodArray;

    private void Start()
    {
        numOfKeyframes = animClip.events.Length;
        currKeyframe = 0;

        //initialize array of RotatingObjects to be the same length as # of important GameObjects passed on
        rotatingObjects = new Motor[objs.Length];

        //initialize each RotatingObject instance (associate the proper gameObjects passed in and initialize other values)
        for (int i = 0; i < objs.Length; i++)
        {
            objs[i].AddComponent<Motor>();
            rotatingObjects[i] = objs[i].GetComponent<Motor>(); 
            rotatingObjects[i].inGame = objs[i];
            rotatingObjects[i].importantAngles = new float[numOfKeyframes];
        }

        //assign the time array to be of size (# of events)
        keyframeTimes = new float[numOfKeyframes];
    }

    void Update()
    {
        //press space bar to write all logged angle values from rotatingObjects to a file
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SaveToFile();
            InterpoFile();
        }
    }

    public void LogFrame() //for each event (keyframe)
    {
        //add the time it happened 
        keyframeTimes[currKeyframe] = Time.time;

        //for each rotatingObject, log all values
        for (int i = 0; i < rotatingObjects.Length; i++) {
            rotatingObjects[i].LogValue(currKeyframe);
        }

        currKeyframe++;
    }

    public void SaveToFile()
    {
        Debug.Log("saving to file");
        Debug.Log("0 axis: " + rotatingObjects[0].importantAxis);
        Debug.Log("1 axis: " + rotatingObjects[1].importantAxis);
        Debug.Log("2 axis: " + rotatingObjects[2].importantAxis);

        //array of lines to be written, each line represents a keyframe 
        string[] lines = new string[numOfKeyframes];

        for (int i = 0; i < numOfKeyframes; i++) //for each keyframe...
        {
            //start line off with time
            lines[i] = keyframeTimes[i] + ": ";

            //for each object, we concatenate only the important weight
            for (int obj = 0; obj < rotatingObjects.Length; obj++)
            {
                if (obj == rotatingObjects.Length - 1)//if last object
                {
                    lines[i] += rotatingObjects[obj].importantAngles[i];
                } else
                {
                    lines[i] += (rotatingObjects[obj].importantAngles[i] + ", ");
                }
            }
        }

        File.WriteAllLines(Application.dataPath + "/TextFiles/test.txt", lines);
    }

    public void InterpoFile() //interpolate values from save to file 
    {
        int valuesPerSecond = 50; //change based on desired read rate
        //set interpolation count based on total time of animation (last logged frame time)
        count = (int)Mathf.Ceil(keyframeTimes[^1] * valuesPerSecond);

        //interpolated times 
        float[] interpoTimes = new float[count];

        //run original values through interpolation 
        for (int i = 0; i < rotatingObjects.Length; i++)
        {
            //interpolate the important angles
            interpodArray = Interpolation.Interpolate1D(keyframeTimes, rotatingObjects[i].importantAngles, count);

            if (i == 0) //if this is the 0th object, we also need to store the times 
            {
                interpoTimes = (float[])interpodArray.times.Clone();
            }

            //every run through we want to store the interpolated angles to their instance
            rotatingObjects[i].interpolated = (float[])interpodArray.angles.Clone();
        }


        //array of lines to be written, each line represents an interpolated sample 
        string[] lines = new string[interpoTimes.Length];

        for (int i = 0; i < interpoTimes.Length; i++) //for each keyframe...
        {
            //start line off with time
            lines[i] = interpoTimes[i] + ", ";

            for (int obj = 0; obj < rotatingObjects.Length; obj++)
            {
                //if this is the last object, end with no comma
                if (obj == rotatingObjects.Length - 1)
                {
                    lines[i] += rotatingObjects[obj].interpolated[i] + "";
                }
                else //otherwise, comma separate 
                {
                    lines[i] += rotatingObjects[obj].interpolated[i] + ", ";
                }
            }
        }

        File.WriteAllLines(Application.dataPath + "/TextFiles/interpolationTest.txt", lines);

    }
}