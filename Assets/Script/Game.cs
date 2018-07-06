using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game
{
    public Data[,] data = new Data[3,3];
    public bool started;
    public void Initialise()
    {
        for (int i = 0; i < 3; i++)
        {
            for (int x = 0; x < 3; x++)
            {
                data[x,i] = Data.Empty;
            }
        }
        started = true;
    }
}
public enum Data
{
    Empty,My,Op
}