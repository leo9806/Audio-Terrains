using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Linq;
using UnityEngine;

public class Testing : MonoBehaviour {

    // FMOD system variables
    private FMOD.System system;
    private FMOD.ChannelGroup channelGroup;
    private FMOD.Channel channel;
    private FMOD.Sound sound;

    private FMOD.DSP dsp;
    private const int windowSize = 1024;
    private const int sampleRate = 44100;
    private float sampleFrequency;
    private uint songLength;
    private int length;

    private int fftHistoryMaxSize;
    private List<int> beatDetector_bandLimits;

    // To display the dominant frequency and 
    // show if channel is playing
    //float dominantDsp = 0.0f;
    bool isPlaying;
    public bool beginHeight;

    // contains the amplitude values for each frequency band
    // obtained by fft
    private List<List<float>> allFrequencyAmpl = new List<List<float>>();
    private List<float[]> amplitudeValues = new List<float[]>();
    private List<float> dominantAmpl = new List<float>(); // contains the amplitude values of the dominant frequencies
    private List<float> historyBuffer = new List<float>(); 

    /// <summary>
    /// Initializes the window size and runs the necessary
    /// methods.
    /// </summary>
    void Start () {
        length = 0;
        Initialize();
        LoadSoundFromPath(Application.dataPath + "/Sounds/rockA.wav");
        PlaySound();
    }

    /// <summary>
    /// Initializes the FMOD system variables.
    /// </summary>
    /// <returns></returns>
    public int Initialize()
    {
        FMOD.RESULT result = FMOD.Factory.System_Create(out system); // creates the system
        if (result != FMOD.RESULT.OK)
        {
            return (int)result;
        }
        else
        {
            system.init(1, FMOD.INITFLAGS.NORMAL, IntPtr.Zero); // initializes the system
            system.createDSPByType(FMOD.DSP_TYPE.FFT, out dsp); // creates the dsp 
            dsp.setParameterInt((int)FMOD.DSP_FFT.WINDOWTYPE, (int)FMOD.DSP_FFT_WINDOW.HANNING); // sets the dsp window type to hann
            dsp.setParameterInt((int)FMOD.DSP_FFT.WINDOWSIZE, windowSize); // sets the window size 
        }

        return 0;
    }

    /// <summary>
    /// Loads the sound from path
    /// </summary>
    /// <param name="pathToFile"></param>
    /// <returns></returns>
    public int LoadSoundFromPath(string pathToFile)
    {
        // creates the sound with loop mode off
        FMOD.RESULT result = system.createSound(pathToFile, FMOD.MODE.LOOP_OFF | FMOD.MODE.CREATESAMPLE, out sound);
        return 0;
    }

    /// <summary>
    /// Plays the sound and adds the dsp on the channel.
    /// </summary>
    public void PlaySound()
    {
        system.playSound(sound, channelGroup, false, out channel);
        sound.getLength(out songLength, FMOD.TIMEUNIT.MS);
        
        channel.getFrequency(out sampleFrequency);
        channel.addDSP(0, dsp);
        dsp.setActive(true);
    }

    public void InitializeBeatDetector()
    {
        int bandSize = (int)sampleFrequency / windowSize;

        beatDetector_bandLimits.Clear();
    }
	
	// Update is called once per frame
	void Update ()
    {
        system.update();
        channel.isPlaying(out isPlaying);

        if (isPlaying)
        {
            GetSpectrum();
        }
        else
        {   
            FindMaxAmplitude();
            GameObject.Find("Mesh").GetComponent<MeshHeightHelper>().enabled = true;
            GameObject.Find("Mesh").GetComponent<MeshHeightHelper>().timer = (int)(songLength / 1000);
            Debug.Log(amplitudeValues.Count);
            Release();
        } 
	}

    /// <summary>
    /// Performs the fft and stores the values inside the list in logarithmic scale.
    /// </summary>
    /// <param name="spectrum"></param>
    public void GetSpectrum()
    {
        // the IntPtr is to handle the unmanaged spectrum data
        IntPtr unmanagedData;
        uint length; // the length of the fft spectrum
        int nyquistLength, numChannels = 0;
        float domFreq;

        // getting the fft spectrum data and sending it to unmanagedData along with the length
        dsp.getParameterData((int)FMOD.DSP_FFT.SPECTRUMDATA, out unmanagedData, out length);
        dsp.getParameterFloat((int)FMOD.DSP_FFT.DOMINANT_FREQ, out domFreq);
        // the spectrum data is then saved in a fft parameter and is constructed into a structure
        FMOD.DSP_PARAMETER_FFT dspFFT = (FMOD.DSP_PARAMETER_FFT)Marshal.PtrToStructure(unmanagedData, typeof(FMOD.DSP_PARAMETER_FFT));

        float[] tempSpectrum = new float[windowSize/2];

        nyquistLength = dspFFT.length / 2;
        numChannels = dspFFT.numchannels;

        if (nyquistLength > 0)
        {
            for (int len = 0; len < nyquistLength; ++len)
            {
                for (int channel = 0; channel < numChannels; ++channel)
                {
                    tempSpectrum[len] += dspFFT.spectrum[channel][len];
                }
            }

        }
        Debug.Log(domFreq);
        amplitudeValues.Add(tempSpectrum);
    }

    public void GetBeat()
    {
        //
    }

    /// <summary>
    /// Finds the max amplitude from each window
    /// </summary>
    private void FindMaxAmplitude()
    {
        float max = 0f;

        for (int i = 0; i < amplitudeValues.Count; ++i)
        {
            max = amplitudeValues[i].Max();
            dominantAmpl.Add(max);
        }
    }

    /// <summary>
    /// Returns the list of the dominant values
    /// </summary>
    /// <returns></returns>
    public List<float> GetDominantValues()
    {
        return dominantAmpl;
    }

    /// <summary>
    /// Releases the FMOD system resources.
    /// </summary>
    private void Release()
    {
        channel.removeDSP(dsp);
        sound.release();
        dsp.release();
        system.close();
        system.release();
        //Debug.Log("System Released and SurfaceCreator is enabled");
        this.enabled = false;
    }

    /// <summary>
    /// Releases the FMOD system resources upon application exit.
    /// </summary>
    void OnApplicationQuit()
    {
        channel.removeDSP(dsp);
        sound.release();
        dsp.release();
        system.close();
        system.release();
    }
}
