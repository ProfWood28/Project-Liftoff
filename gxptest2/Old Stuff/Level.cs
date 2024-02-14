using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GXPEngine;

public class Level
{
    private List<Sprite> level = new List<Sprite>();

    public void Add(Sprite levelObject)
    {
        level.Add(levelObject);
        Console.WriteLine("Added: {0}", levelObject);
    }

    public void Remove(Sprite levelObject)
    {
        level.Remove(levelObject);
        Console.WriteLine("Removed: {0}", levelObject);
    }

    public void RemoveAndDestroy(Sprite levelObject)
    {
        level.Remove(levelObject);
        levelObject.Destroy();
        Console.WriteLine("Removed and Destroyed: {0}", levelObject);
    }

    public Sprite GetFromID(int index)
    {
        return level[index];
    }

    public Sprite FindFirst(Sprite levelObject)
    {
        int index = level.IndexOf(levelObject);
        return level[index];
    }
    public Sprite FindLast(Sprite levelObject)
    {
        int index = level.LastIndexOf(levelObject);
        return level[index];
    }

    public List<Sprite> GetAll()
    {
        return level;
    }
}
