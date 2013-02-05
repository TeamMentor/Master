using System;
using System.Collections.Generic;
using O2.DotNetWrappers.ExtensionMethods;

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
				var keyValueList = new List<KeyValue<TKey, TValue>>();
				foreach (TKey key in dictionary.Keys)
				{
					keyValueList.Add(new KeyValue<TKey, TValue> { Key = key, Value = dictionary[key] });
				}
				return keyValueList;
			}
			catch (Exception ex)
			{
				ex.logWithStackTrace("in ConvertDictionary");
				return new List<KeyValue<TKey, TValue>>(); 
			}
		}
	}
}