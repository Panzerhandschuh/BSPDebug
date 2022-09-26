using LibBSP;
using UnityEngine;

public class VisDataDebug : MonoBehaviour
{
    public int numVecs;
    public int vecSize;
    public byte[] data;

	public void Init(Visibility visibility)
	{
		numVecs = visibility.NumClusters;
		vecSize = visibility.ClusterSize;
		data = visibility.Data;
	}
}
