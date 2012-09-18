
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

public class ZigDepthmapToParticles : MonoBehaviour
{
    public Vector3 gridScale = Vector3.one;
    public Vector2 DesiredResolution = new Vector2(160, 120); // should be a divisor of 640x480
    public bool onlyUsers = true; //only emit particles for users.
    public bool worldSpace = true; //emit in worldspace.
    //the particle emission coordinates will be based on the scale of this behavior's transform 
    //if you are not in world space, then particles are placed in a grid according to your image spacce resolution (ie 160x120)

    public Vector3 velocity = new Vector3(0f,1f,0f);
    public GameObject particlePrefab;
    private ParticleEmitter[] particleEmitters;
    static int MAX_PARTICLES_PER_PE = 16250;
    int factorX;
    int factorY;
    private int YScaled;
    private int XScaled;
    
    int XRes;
    int YRes;
    int emitterCount;
    public Color color;
    public float size = .1f;
    public float energy = 1f;
    
    public int cycles = 10;
    // Use this for initialization
    void Start()
    {
        // init stuff
        
        YRes = ZigInput.Depth.yres;
        XRes = ZigInput.Depth.xres;
        factorX = (int)(XRes / DesiredResolution.x);
        factorY = (int)(YRes / DesiredResolution.y);
        YScaled = YRes / factorY;
        XScaled = XRes / factorX;
        
     
        emitterCount = 1 + ((XScaled * YScaled) / MAX_PARTICLES_PER_PE);
        
        particleEmitters = new ParticleEmitter[emitterCount*cycles];
        for (int i = 0; i < (emitterCount * cycles); i++)
        {
            particleEmitters[i] = ((GameObject)Instantiate(particlePrefab, Vector3.zero, Quaternion.identity)).GetComponent<ParticleEmitter>();
            //particleEmitters[i].particles = new Particle[MAX_PARTICLES_PER_PE];
        }
        ZigInput.Instance.AddListener(gameObject);        
    }
    
    private int cycle = 0;
    void LateUpdate()
    {
        int x = 0;
     int y = 0;
        short[] rawDepthhMap = ZigInput.Depth.data;
        short[] rawLabelMap = ZigInput.LabelMap.data;
        for (int i = cycle*emitterCount; i < (cycle+1)*emitterCount; i++)
        {
            particleEmitters[i].ClearParticles();
            for (int particleIndex = 0; particleIndex < MAX_PARTICLES_PER_PE; particleIndex++)
            {
                if (y >= YScaled)
                {
                    break;                   
                }
                Vector3 scale = transform.localScale;
                int index = x * factorX + XRes * factorY * y;
                Vector3 vec = new Vector3 (x * factorX, y*factorY,rawDepthhMap[index]);
                vec = worldSpace ? ZigInput.ConvertImageToWorldSpace(vec) : vec;                
                vec = Vector3.Scale(vec,scale);
                
                if (onlyUsers)
                {
                    if (rawLabelMap[index] != 0)
                    {
                        
                        particleEmitters[i].Emit(transform.rotation * vec + transform.position, velocity, size, energy, color);
                    }
                }
                else
                {
                    particleEmitters[i].Emit(transform.rotation * vec + transform.position, velocity, size, energy, color);
                }
                x = (x + 1) % XScaled;
                y = (x == 0) ? y+1 : y;
            }

            if (y >= YScaled)
            {
                break;
            }
        }
        cycle = (cycle + 1) % cycles;
    }
}
