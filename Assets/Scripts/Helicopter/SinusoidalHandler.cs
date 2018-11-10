using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * What up game dev decal crew
 * I made this bc dealing with all the sine wave bullshit in IdleState and PursuitState was getting too annoying
 * This is called overengineering but it's fun soooooooo
 */
public static class SinusoidalHandler
{
	/**
 	* A Sine wave is defined as:
 	* 		f(t) = A * sin(B * t + C)
 	* Where:
 	* 		A is the amplitude of the function
 	* 		period = 2pi/B is the period of the function
 	* 		PS = C/B is the phase shift of the function
 	* 
 	*/


	public static Sinusoidal create(float A, float B, float C) {
		return new SineWave (A, B, C);
	}

	public static Sinusoidal compose(Sinusoidal first, Sinusoidal second) {
		List<SineWave> together = new List<SineWave> ();
		together.AddRange (first.getElements ());
		together.AddRange (second.getElements());
		return new SineWaveComposition (together);
	}

	public interface Sinusoidal {
		float value (float input, bool updateIndexedInput = false);
		float nextValue (float deltaInput);
		float indexedValue();
		List<SineWave> getElements ();
	}


	public class SineWave : Sinusoidal {
		private float A;
		private float B;
		private float C;
		private float period;
		private float PS;

		private float lastReferencedInput;
		private float lastReferencedValue;

		public SineWave(float A, float B, float C) {
			this.A = A;
			this.B = B;
			this.C = C;
			this.period = 2 * Mathf.PI * (1 / B);
			this.PS = C / B;
		}

		public float value(float input, bool updateIndexedInput = false) {
			if (updateIndexedInput) {
				lastReferencedInput = input;
				lastReferencedValue = A * Mathf.Sin (B * input + C);
				return lastReferencedValue;
			}
			return A * Mathf.Sin (B * input + C);
		}

		public float nextValue(float deltaInput) {
			return value ((lastReferencedInput + deltaInput) % period, true);
		}

		public List<SineWave> getElements() {
			List<SineWave> justThis = new List<SineWave> ();
			justThis.Add (this);
			return justThis;
		}

		public float indexedValue() {
			return lastReferencedValue;
		}
	}

	/**
 	* An IMMUTABLE representaiton of a composition of SineWaves
 	*/
	private class SineWaveComposition : Sinusoidal {
		private List<SineWave> elements;
		private float lastReferencedValue;

		public SineWaveComposition(List<SineWave> toCompose) {
			elements = toCompose;
		}

		public List<SineWave> getElements() {
			return elements;
		}

		public float value(float input, bool updateIndexedInput) {
			float total = 0f;
			foreach (SineWave wave in elements) {
				total += wave.value (input, updateIndexedInput);
			}
			if (updateIndexedInput) {
				lastReferencedValue = total;
			}
			return total;
		}

		public float nextValue(float deltaInput) {
			float total = 0f;
			foreach (SineWave wave in elements) {
				total += wave.nextValue (deltaInput);
			}
			lastReferencedValue = total;
			return total;
		}

		public float indexedValue() {
			return lastReferencedValue;
		}
	}
}




