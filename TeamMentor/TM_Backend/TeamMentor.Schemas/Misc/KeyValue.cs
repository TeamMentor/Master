/*using System;
using System.Collections.Generic;
using System.Linq;
using FluentSharp.CoreLib;

namespace TeamMentor.CoreLib
{
	public class KeyValue<TKey, TValue>
	{
		public TKey Key { get; set; }
		public TValue Value { get; set; }
		
	}
	public static class KeyValue_extensionMethods
	{
		public static List<KeyValue<TKey, TValue>> ConvertDictionary<TKey, TValue>(this Dictionary<TKey, TValue> dictionary)
		{
			try
			{
			    return dictionary.Keys.Select(key => new KeyValue<TKey, TValue> {Key = key, Value = dictionary[key]}).ToList();
			}
			catch (Exception ex)
			{
				ex.logWithStackTrace("in ConvertDictionary");
				return new List<KeyValue<TKey, TValue>>(); 
			}
		}
	}
}*/