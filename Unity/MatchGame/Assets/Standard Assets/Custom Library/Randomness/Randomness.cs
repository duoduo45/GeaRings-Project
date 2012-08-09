using UnityEngine;
using System.Collections;

public class RandFuncs {

//	Return an array of integers containing the numbers from zero to num.
	public static int[] Indices(int num) {
		int[] result = new int[num];
		
		for (int i = 0; i < result.Length; i++) {
			result[i] = i;
		}
		
		return result;
	}
	

//	Uniform random number, 0..1 range.
	public static float Rnd() {
		return Random.value;
	}
	

//	Random integer in the range first..last-1.
	public static int RndRange(int first, int last) {
		return Random.Range(first, last);
	}

	
//	As above, but the start of the range is assumed to be zero.
	public static int RndRange(int last) {
		return Random.Range(0, last);
	}


//	Random shuffling of the supplied integer array.
	public static void Shuffle(int[] ints) {
		int temp;
		
		for (int i = 0; i < ints.Length; i++) {
			temp = ints[i];
			int swapIndex = RndRange(ints.Length);
			ints[i] = ints[swapIndex];
			ints[swapIndex] = temp;
		}
	}
	
//	Random shuffling of the supplied integer array.
	public static void Shuffle(GameObject[] objects) {
		GameObject temp;
		
		for (int i = 0; i < objects.Length; i++) {
			temp = objects[i];
			int swapIndex = RndRange(objects.Length);
			objects[i] = objects[swapIndex];
			objects[swapIndex] = temp;
		}
	}
	
//	Sum of the values in the supplied integer array.
	public static int Sum(int[] ints) {
		int result = 0;
		
		for (int i = 0; i < ints.Length; i++) {
			result += ints[i];
		}
		
		return result;
	}
	

//	Sum of the values in the supplied float array.	
	public static float Sum(float[] floats) {
		float result = 0f;
		
		for (int i = 0; i < floats.Length; i++) {
			result += floats[i];
		}
		
		return result;
	}
	

//	Choose an integer at random, according to the supplied distribution.
	public static int Sample(float[] distro, float total) {
		float randVal = total * Rnd();
		
		for (int i = 0; i < distro.Length; i++) {
			if (randVal < distro[i]) {
				return i;
			}
			
			randVal -= distro[i];
		}
		
		return distro.Length - 1;
	}
	

//	As above, but calculate the total too.
	public static int Sample(float[] distro) {
		float total = Sum(distro);
		return Sample(distro, total);
	}


//	Sample several items without replacement.
	public static int[] SampleRemove(int maxNum, int numSamples) {
		int[] result = new int[numSamples];
		int numNeeded = numSamples;
		
		for (int numLeft = maxNum; numLeft > 0; numLeft--) {
			float chance = (float) numNeeded / (float) numLeft;
			
			if (Rnd() < chance) {
				result[--numNeeded] = numLeft - 1;
			}
			
			if (numNeeded == 0) {
				break;
			}
		}
		
		if (numNeeded != 0) {
			result[0] = 0;
		}
		
		return result;
	}
	

//	Make a string representation of an integer array.	
	public static string Dump(int[] ints) {
		if (ints.Length == 0) {
			return "<Empty>";
		}
		
		string result = ints[0].ToString();
		
		for (int i = 1; i < ints.Length; i++) {
			result += ", " + ints[i].ToString();
		}
		
		return result;
	}
	

//	Make a string representation of a float array.	
	public static string Dump(float[] floats) {
		if (floats.Length == 0) {
			return "<Empty>";
		}
		
		string result = floats[0].ToString();
		
		for (int i = 1; i < floats.Length; i++) {
			result += ", " + floats[i].ToString();
		}
		
		return result;
	}
}


//	Class to perform fast sampling without replacement from a distribution. This
//	has a linear time preprocessing step that is performed before sampling
//	takes place.
public class FastSample {
	float[] distro;
	int[] aliases;
	float mean;
	
	
	public FastSample(params float[] initDistro) {
		distro = new float[initDistro.Length];
		float total = 0f;
		
		for (int i = 0; i < distro.Length; i++) {
			distro[i] = initDistro[i];
			total += distro[i];
		}
		
		Debug.Log(string.Format("Distro: {0}", RandFuncs.Dump(distro)));

		
		mean = total / (float) distro.Length;
		
		Debug.Log(string.Format("Mean: {0}", mean));
		
		int[] sep = new int[distro.Length];
		
		int shortMarker = 0;
		int tallMarker = sep.Length - 1;
		
		for (int i = 0; i < distro.Length; i++) {
			if (distro[i] < mean) {
				sep[shortMarker++] = i;
			} else {
				sep[tallMarker--] = i;
			}
		}
		
		Debug.Log(string.Format("Sep: {0}", RandFuncs.Dump(sep)));
		tallMarker++;
		
		Debug.Log(string.Format("Tall marker: {0}", tallMarker));

		
		aliases = new int[distro.Length];
		
		for (int i = 0; i < (sep.Length - 1); i++) {
			int curr = sep[i];
			float shortfall = mean - distro[curr];
			int firstTall = sep[tallMarker];
			distro[firstTall] -= shortfall;
			aliases[curr] = firstTall;
			
			if (distro[firstTall] < mean) {
				tallMarker++;
			}
		}
		
		aliases[sep[sep.Length - 1]] = sep[sep.Length - 1];
	}
	
	
	public int Next() {
		float rndVal = RandFuncs.Rnd() * (float) distro.Length;
		int slotNum = Mathf.Min(Mathf.FloorToInt(rndVal), distro.Length - 1);
		float fracPart = (rndVal - (float) slotNum) * mean;
		
		if (fracPart < distro[slotNum]) {
			return slotNum;
		} else {
			return aliases[slotNum];
		}
	}
}