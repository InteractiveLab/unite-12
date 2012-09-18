using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TimestampedObject <T>
{
	public T obj { get; private set; }
	public float timeStamp { get; private set; }
	
	public TimestampedObject(T _obj, float _timeStamp) 
	{
		obj = _obj;
		timeStamp = _timeStamp;
	}
	
	public TimestampedObject(T _obj)
	{
		obj = _obj;
		timeStamp = Time.time;
	}
}

public class TimedBuffer<T>{
	
	protected float timeFrame;

	protected List<TimestampedObject<T>> buffer = new List<TimestampedObject<T>>();
	
	public TimedBuffer(float _timeFrame)
	{
		timeFrame = _timeFrame;
	}
	
	public void AddDataPoint(T obj, float timeStamp)
	{
		buffer.Add(new TimestampedObject<T>(obj,timeStamp));
	}
	
	public void AddDataPoint(T obj)
	{
		buffer.Add(new TimestampedObject<T>(obj));
	}
	
	public void Prune()
	{
        buffer.RemoveAll((TimestampedObject<T> t) => (Time.time > (t.timeStamp + timeFrame)));
	}

    public void Clear()
    {
        buffer.Clear();
    }

	public List<TimestampedObject<T>> Buffer	
	{
		get 
		{
			Prune();
			return buffer;
		}
	}

    public T GetPointNearTimeDifference(float deltaTime)
    {
        return GetObjectNearTimeDifference(deltaTime).obj;
    }

    public TimestampedObject<T> GetObjectNearTimeDifference(float deltaTime)
    {
        float time = Time.time;
        Prune();
        //switching the order would be faster, but it probably isn't necessary
        TimestampedObject<T> obj = buffer.FindLast((TimestampedObject<T> t) => (time > (t.timeStamp + deltaTime)));
        if (null == obj) {
            throw new System.InvalidOperationException();
        }
        return obj;
    }
}
