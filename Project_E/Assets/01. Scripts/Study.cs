using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Rendering.LookDev;

public class Study : MonoBehaviour
{
    public string say = "";
    public char text = 'a';
    public int var01 = 1;
    public float var02 = 0.4f;
    public double var03 = 0.5;
    public bool var04 = true;
    



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
        Debug.Log(this.say);
        Debug.Log(this.text);

        int a = 10;
        
        a = a + 1;


        Debug.Log(a++); 

        Debug.Log(++a); 

        Debug.Log(a);


    }

    // Update is called once per frame
    void Update()
    {
        
    }
}