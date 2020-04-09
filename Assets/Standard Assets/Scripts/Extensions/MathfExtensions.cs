using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Extensions
{
	public static class MathfExtensions
	{
		public const int NULL_INT = 1234567890;
		public const float NULL_FLOAT = NULL_INT;
		public const float INCHES_TO_CENTIMETERS = 2.54f;
		public const float NATURAL_NUMBER = 2.7182818284590452353602874713526624977572470936999595749669676277240766303535475945713821785251664274274663919320030599218174135966290435729003342952605956307381323286279434907632338298807531952510190115738341879307021540891499348841675092447614606680822648001684774118537423454424371075390777449920695517027618386062613313845830007520449338265602976067371132007093287091274437470472306969772093101416928368190255151086574637721112523897844250569536967707854499699679468644549059879316368892300987931277361782154249992295763514822082698951936680331825288693984964651058209392398294887933203625094431173012381970684161403970198376793206832823764648042953118023287825098194558153017567173613320698112509961818815930416903515988885193458072738667385894228792284998920868058257492796104841984443634632449684875602336248270419786232090021609902353043699418491463140934317381436405462531520961836908887070167683964243781405927145635490613031072085103837505101157477041718986106873969655212671546889570350354f;
		
		public static float SnapToInterval (float f, float interval)
		{
			if (interval == 0)
				return f;
			else
				return Mathf.Round(f / interval) * interval;
		}
		
		public static int Sign (float f)
		{
			if (f == 0)
				return 0;
			else
				return (int) Mathf.Sign(f);
		}
		
		public static bool AreOppositeSigns (float f1, float f2)
		{
			return Mathf.Abs(Sign(f1) - Sign(f2)) == 2;
		}
		
		public enum RoundingMethod
		{
			HalfOrMoreRoundsUp,
			HalfOrLessRoundsDown,
			RoundUpIfNotWhole,
			RoundDownIfNotWhole
		}

		public static float GetClosestNumber (float f, params float[] numbers)
		{
			float closestNumber = numbers[0];
			float number;
			for (int i = 1; i < numbers.Length; i ++)
			{
				number = numbers[i];
				if (Mathf.Abs(f - number) < Mathf.Abs(f - closestNumber))
					closestNumber = number;
			}
			return closestNumber;
		}

		public static int GetIndexOfClosestNumber (float f, params float[] numbers)
		{
			int indexOfClosestNumber = 0;
			float closestNumber = numbers[0];
			float number;
			for (int i = 1; i < numbers.Length; i ++)
			{
				number = numbers[i];
				if (Mathf.Abs(f - number) < Mathf.Abs(f - closestNumber))
				{
					closestNumber = number;
					indexOfClosestNumber = i;
				}
			}
			return indexOfClosestNumber;
		}

		public static float RegularizeAngle (float angle)
		{
			while (angle >= 360 || angle < 0)
				angle += Mathf.Sign(360 - angle) * 360;
			return angle;
		}

		public static float ClampAngle (float ang, float min, float max)
		{
			ang = WrapAngle(ang);
			min = WrapAngle(min);
			max = WrapAngle(max);
			float minDist = Mathf.Min(Mathf.DeltaAngle(ang, min), Mathf.DeltaAngle(ang, max));
			if (WrapAngle(ang + Mathf.DeltaAngle(ang, minDist)) == min)
				return min;
			else if (WrapAngle(ang + Mathf.DeltaAngle(ang, minDist)) == max)
				return max;
			else
				return ang;
		}

		public static float WrapAngle (float ang)
		{
			if (ang < 0)
				ang += 360;
			else if (ang > 360)
				ang = 360 - ang;
			return ang;
		}
	}
}