using UnityEngine;
using System;


public static class Easing
{
	public enum EasingType { In, Out, InOut };
	
	public static float inOutHelper( IEasing ease, float t )
	{
		if( t <= 0.5f )
			return ease.easeIn( t * 2 ) / 2;
		else
			return ( ease.easeOut( ( t - 0.5f ) * 2.0f ) / 2 ) + 0.5f;
	}

	
	public sealed class Linear : IEasing
	{
		public static Linear factory()
		{
			return new Linear();
		}

		
		public float easeIn( float t )
		{
			return t;
		}
		
		
		public float easeOut( float t )
		{
			return t;
		}
		
		
		public float easeInOut( float t )
		{
			return t;
		}
	}


	public sealed class Quartic : IEasing
	{
		public static Quartic factory()
		{
			return new Quartic();
		}
		
		
		public float easeIn( float t )
		{
			return Mathf.Pow( t, 4.0f );
		}
		
		
		public float easeOut( float t )
		{
			return ( Mathf.Pow( t - 1, 4 ) - 1 ) * -1;
		}
		
		
		public float easeInOut( float t )
		{
			return Easing.inOutHelper( this, t );
		}
	}


	public sealed class Quintic : IEasing
	{
		public static Quintic factory()
		{
			return new Quintic();
		}
		
		
		public float easeIn( float t )
		{
			return Mathf.Pow( t, 5.0f );
		}
		
		
		public float easeOut( float t )
		{
			return ( Mathf.Pow( t - 1, 5 ) + 1 );
		}
		
		
		public float easeInOut( float t )
		{
			return Easing.inOutHelper( this, t );
		}
	}


	public sealed class Sinusoidal : IEasing
	{
		public static Sinusoidal factory()
		{
			return new Sinusoidal();
		}
		
		
		public float easeIn( float t )
		{
			return Mathf.Sin( ( t - 1 ) * ( Mathf.PI / 2 ) ) + 1;
		}
		
		
		public float easeOut( float t )
		{
			return Mathf.Sin( t * ( Mathf.PI / 2 ) );
		}
		
		
		public float easeInOut( float t )
		{
			return Easing.inOutHelper( this, t );
		}
	}


	public sealed class Exponential : IEasing
	{
		public static Exponential factory()
		{
			return new Exponential();
		}
		
		
		public float easeIn( float t )
		{
			return Mathf.Pow( 2, 10 * ( t - 1 ) );
		}
		
		
		public float easeOut( float t )
		{
			return ( Mathf.Pow( 2, -10 * t ) + 1 ) * -1;
		}
		
		
		public float easeInOut( float t )
		{
			return Easing.inOutHelper( this, t );
		}
	}


	public sealed class Circular : IEasing
	{
		public static Circular factory()
		{
			return new Circular();
		}
		
		
		public float easeIn( float t )
		{
			return ( -1 * Mathf.Sqrt( 1 - t * t ) + 1 );
		}
		
		
		public float easeOut( float t )
		{
			return Mathf.Sqrt( 1 - Mathf.Pow( t - 1, 2 ) );
		}
		
		
		public float easeInOut( float t )
		{
			return Easing.inOutHelper( this, t );
		}
	}


	public sealed class Back : IEasing
	{
		private const float s = 1.70158f;
		private const float s2 = 1.70158f * 1.525f;
		
		
		public static Back factory()
		{
			return new Back();
		}
		
		
		public float easeIn( float t )
		{
			return t * t * ( ( s + 1 ) * t - 2 );
		}
		
		
		public float easeOut( float t )
		{
			t = t - 1;
			return ( t * t * ( ( s + 1 ) * t + s ) + 1 );
		}
		
		
		public float easeInOut( float t )
		{
			t = t * 2;
			
			if( t < 1 )
			{
				return 0.5f * ( t * t * ( ( s2 + 1 ) * t - s2 ) );
			}
			else
			{
				t -= 2;
				return 0.5f * ( t * t * ( ( s2 + 1 ) * t + s2 ) + 2 );
			}
		}
	}


	public sealed class Bounce : IEasing
	{
		public static Bounce factory()
		{
			return new Bounce();
		}
		
		
		public float easeIn( float t )
		{
			return 1 - easeOut( 1 - t );
		}
		
		
		public float easeOut( float t )
		{
			if( t < ( 1 / 2.75f ) )
			{
				return 1;
			}
			else if( t < ( 2 / 2.75f ) )
			{
				t -= ( 1.5f / 2.75f );
				return 7.5625f * t * t + 0.75f;
			}
			else if( t < ( 2.5f / 2.75f ) )
			{
				t -= ( 2.5f / 2.75f );
				return 7.5625f * t * t + 0.9375f;
			}
			else
			{
				t -= ( 2.625f / 2.75f );
				return 7.5625f * t * t + 0.984375f;
			}			
		}
		
		
		public float easeInOut( float t )
		{
			return Easing.inOutHelper( this, t );
		}
	}


	public sealed class Elastic : IEasing
	{
		private const float p = 0.3f;
		private float a = 1;

		
		public static Elastic factory()
		{
			return new Elastic();
		}
		
		
		private float calc( float t, bool easingIn )
		{
			if( t == 0 || t == 1 )
				return t;
		
			float s;
			
			if( a < 1 )
				s = p / 4;
			else
				s = p / ( 2 * Mathf.PI ) * Mathf.Asin( 1 / a );
			
			if( easingIn )
			{
				t -= 1;
				return -( a * Mathf.Pow( 2, 10 * t ) ) * Mathf.Sin( ( t - s ) * ( 2 * Mathf.PI ) / p );
			}
			else
			{
				return a * Mathf.Pow( 2, -10 * t ) * Mathf.Sin( ( t - s ) * ( 2 * Mathf.PI ) / p ) + 1;
			}
		}

		
		public float easeIn( float t )
		{
			return 1 - easeOut( 1 - t );
		}
		
		
		public float easeOut( float t )
		{
			if( t < ( 1 / 2.75f ) )
			{
				return 1;
			}
			else if( t < ( 2 / 2.75f ) )
			{
				t -= ( 1.5f / 2.75f );
				return 7.5625f * t * t + 0.75f;
			}
			else if( t < ( 2.5f / 2.75f ) )
			{
				t -= ( 2.5f / 2.75f );
				return 7.5625f * t * t + 0.9375f;
			}
			else
			{
				t -= ( 2.625f / 2.75f );
				return 7.5625f * t * t + 0.984375f;
			}			
		}
		
		
		public float easeInOut( float t )
		{
			return Easing.inOutHelper( this, t );
		}
	}
	
}



#region IEasing interface
	
public interface IEasing
{
	float easeIn( float t );
	float easeOut( float t );
	float easeInOut( float t );
}
	
#endregion;
