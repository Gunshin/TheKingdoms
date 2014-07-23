//Author: Michael Stephens
//LastEdit: 2014/05/18
//Email: michaelstephens1993@gmail.com


using UnityEngine;
using System.Collections;

public class PerlinNoise {
	
	int[] primes = new int[3];
   System.Random rand;

	public PerlinNoise()
	{
      rand = new System.Random();
		generatePrimes();
	}

	public PerlinNoise(int seed_)
	{
      rand = new System.Random(seed_);
		generatePrimes ();
	}
	
	float generateNoise( float x_ )
	{
		int n = ((int)x_ << 13) ^ (int)x_;
		return (float)( 1.0 - ( (n * (n * n * primes[1] + primes[2]) + primes[3]) & 0x7fffffff ) / 1073741824.0);
	}
	
	float generateNoise( float x_ , float y_ )
	{
		int n = (int)x_ + (int)y_ * 57;
		n = (n << 13) ^ n;
		return (float)( 1.0 - ( (n * (n * n * primes[0] + primes[1]) + primes[2]) & 0x7fffffff ) / 1073741824.0);
	}
	
	float smoothedNoise( float x_ )
	{
		return (generateNoise(x_) / 2) + (generateNoise(x_ - 1) / 4) + (generateNoise(x_ + 1) / 4);
	}
	
	float smoothedNoise( float x_ , float y_ )
	{
		float corners = (( generateNoise(x_ - 1, y_ - 1) + generateNoise(x_ + 1, y_ - 1) + generateNoise(x_ - 1, y_ + 1) + generateNoise(x_ + 1, y_ + 1)) / 16);
		float sides = ( (generateNoise(x_ - 1, y_) + generateNoise(x_ + 1, y_) + generateNoise(x_, y_ + 1) + generateNoise(x_, y_ + 1) )/ 8);
		float center = generateNoise(x_, y_) / 4;
		return (corners + sides + center);
	}
	
	float interpolatedNoise( float x_ )
	{
	
		int integer_X = (int) x_;
		float fractional_X = x_ - integer_X;
		
		float v1 = smoothedNoise(integer_X);
		float v2 = smoothedNoise(integer_X + 1);
		
		return cosineInterpolate(v1, v2, fractional_X);
	}
	
	float interpolatedNoise( float x_ , float y_ )
	{
	
		float integer_X = (int) x_;
		float fractional_X = x_ - integer_X;
		
		float integer_Y = (int) y_;
		float fractional_Y = y_ - integer_Y;
		
		float v1 = smoothedNoise(integer_X, integer_Y);
		float v2 = smoothedNoise(integer_X + 1, integer_Y);
		float v3 = smoothedNoise(integer_X, integer_Y + 1);
		float v4 = smoothedNoise(integer_X + 1, integer_Y + 1);
		
		float i1 = cosineInterpolate(v1, v2, fractional_X);
		float i2 = cosineInterpolate(v3, v4, fractional_X);
		return cosineInterpolate(i1, i2, fractional_Y);
	
	}
	
   /// <summary>
   /// Converts x coordinate to float value
   /// </summary>
   /// <param name="x_"></param>
   /// <param name="persistence_"></param>
   /// <param name="octaves_"></param>
   /// <returns>float between -1.0f and 1.0f</returns>
	public float getPerlinNoise( int x_, int persistence_, int octaves_)
	{
		float total = 0;
		for( int i = 0; i < octaves_; ++i)
		{
			float frequency = Mathf.Pow(2.0f, (float)i);
			float amplitude = Mathf.Pow(persistence_, (float)i);
			
			total += interpolatedNoise(x_* frequency) * amplitude;
		}
		
		return total;
	}
	
   /// <summary>
   /// Converts x and y coordinate to float value
   /// </summary>
   /// <param name="x_"></param>
   /// <param name="y_"></param>
   /// <param name="persistence_"></param>
   /// <param name="octaves_"></param>
   /// <param name="zoom_"></param>
   /// <returns>float between -1.0f and 1.0f</returns>
	public float getPerlinNoise( int x_ , int y_, float persistence_, int octaves_, int zoom_)
	{
		float total = 0;
		for( int i = 0; i < octaves_; ++i)
		{
			float frequency = Mathf.Pow(2.0f, (float)i);
			float amplitude = Mathf.Pow(persistence_, (float)i);
			total += interpolatedNoise(x_ * frequency / zoom_, y_ * frequency / zoom_) * amplitude;
		}
		
		return total;
	}
	
	float cosineInterpolate(float a_, float b_, float x_)
	{
		float ft = x_ * Mathf.PI;
		float f = (1 - Mathf.Cos(ft)) * 0.5f;
		return a_ * (1 - f) + b_ * f;
	}
	
	void generatePrimes()
	{
		primes[0] = getNextPrime(15731 + rand.Next(-250,250));
      primes[1] = getNextPrime(789221 + rand.Next(-30000, 30000));
      primes[2] = getNextPrime(1376312589 + rand.Next(-30000, 30000));
	}
	
	int getNextPrime(int lowestBound_)
	{
		if(lowestBound_ % 2 == 0)
		{
			if(lowestBound_ == 2)
			{
				return lowestBound_;
			}
			
			lowestBound_++;
		}
		
		for(int i = lowestBound_; ; i += 2)
		{
			if(isPrime(i))
			{
				return i;
			}
		}
	}
	
	bool isPrime(int number_)
	{
		bool flag = true;
		for(int j = 2; j < Mathf.Sqrt((float)number_) + 1 && flag == true; ++j)
		{
			if(number_ % j == 0)
			{
				flag = false;
			}
		}
		return flag;
	}
}
