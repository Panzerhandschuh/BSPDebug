using System;
using System.Collections.Generic;
using LibBSP;
using UnityEngine;

public class VisDataDebug : MonoBehaviour
{
    public int numVecs;
    public int vecSize;
	public byte[] byteData;
    public int[] intData;

	public int[] clusterPVSOffsets;
	public int[] clusterPASOffsets;

	public List<int> pvsData = new List<int>();
	public List<int> pasData = new List<int>();

	public void Init(Visibility visibility)
	{
		numVecs = visibility.NumClusters;
		vecSize = visibility.ClusterSize;

		byteData = visibility.Data;

		var startOffset = visibility.Bsp.MapType.IsSubtypeOf(MapType.Source) ? 4 : 8;
		intData = new int[numVecs * 2];
		for (var i = 0; i < numVecs * 2; i++)
			intData[i] = BitConverter.ToInt32(visibility.Data, startOffset + i * sizeof(int));

		clusterPVSOffsets = new int[numVecs];
		clusterPASOffsets = new int[numVecs];
		for (var i = 0; i < numVecs; i++)
		{
			clusterPVSOffsets[i] = visibility.GetOffsetForClusterPVS(i);
			clusterPASOffsets[i] = visibility.GetOffsetForClusterPAS(i);

			if (visibility.Bsp.MapType.IsSubtypeOf(MapType.Source))
			{
				var pvs = byteData[clusterPVSOffsets[i]];
				pvsData.Add(pvs);
				if (pvs == 0) // # of clusters to skip that are not visible
					pvsData.Add(byteData[clusterPVSOffsets[i] + 1] * 8);

				var pas = byteData[clusterPASOffsets[i]];
				pasData.Add(pas);
				if (pas == 0) // # of clusters to skip that are not visible
					pasData.Add(byteData[clusterPASOffsets[i] + 1] * 8);
			}
		}
	}
}
