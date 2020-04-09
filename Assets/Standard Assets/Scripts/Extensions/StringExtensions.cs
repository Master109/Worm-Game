﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Extensions
{
	public static class StringExtensions
	{
		public static string SubstringStartEnd (this string str, int startIndex, int endIndex)
		{
			return str.Substring(startIndex, endIndex - startIndex);
		}
		
		public static string RemoveEach (this string str, string remove)
		{
			return str.Replace(remove, "");
		}
		
		public static string StartAfter (this string str, string startAfter)
		{
			return str.Substring(str.IndexOf(startAfter) + startAfter.Length);
		}

		public static string RemoveStartEnd (this string str, int startIndex, int endIndex)
		{
			return str.Remove(startIndex, endIndex - startIndex);
		}

		public static string RemoveBetween (this string str, string startString, string endString)
		{
			string output = str;
			int indexOfStartString = str.IndexOf(startString);
			if (indexOfStartString != -1)
			{
				string startOfStr = str.Substring(0, indexOfStartString);
				str = str.Substring(indexOfStartString + startString.Length);
				output = startOfStr + str.RemoveStartEnd(0, str.IndexOf(endString) + endString.Length);
			}
			return output;
		}

		public static string Random (int iterations, params string[] choices)
		{
			string output = "";
			for (int i = 0; i < iterations; i ++)
				output += choices[UnityEngine.Random.Range(0, choices.Length)];
			return output;
		}

		public static string Random (int iterations, string choices)
		{
			string output = "";
			for (int i = 0; i < iterations; i ++)
				output += choices[UnityEngine.Random.Range(0, choices.Length)];
			return output;
		}

		public static int GetCount (this string str, string findStr)
		{
			int output = -1;
			int indexOfFindStr = 0;
			do
			{
				output ++;
				indexOfFindStr = str.IndexOf(findStr, indexOfFindStr + 1);
			} while (indexOfFindStr != -1);
			return output;
		}
	}
}