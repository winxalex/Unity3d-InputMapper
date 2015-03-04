/*
 * 
 * Created by Dimitris Tavlikos
 * Website: http://software.tavlikos.com
 * Contact: dimitris@tavlikos.com
 * 
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace ws.winx.csharp.data
{
	/// <summary>
	/// FlexibleDictionary is a generic Dictionary but you can add items to it during a foreach iteration
	/// </summary>
	/// <typeparam name="TKey">The type of the keys</typeparam>
	/// <typeparam name="TValue">The type of the values</typeparam>
	[Serializable()]
	public class FlexibleDictionary<TKey, TValue> : Dictionary<TKey, TValue>
	{

		#region Constructors

		/// <summary>
		/// Creates a new instance of the FlexibleDictionary class
		/// </summary>
		public FlexibleDictionary()
			: base(0, null)
		{

		}//end ctor

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="dictionary"></param>
		public FlexibleDictionary(IDictionary<TKey, TValue> dictionary)
			: base(dictionary, null)
		{

		}//end ctor

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="dictionary"></param>
		/// <param name="comparer"></param>
		public FlexibleDictionary(IDictionary<TKey, TValue> dictionary, IEqualityComparer<TKey> comparer)
			: base((dictionary != null) ? dictionary.Count : 0, comparer)
		{

			if (dictionary == null)
			{

				throw new ArgumentNullException("dictionary", "The IDictionary parameter must not be null!");

			}//end if

			foreach (KeyValuePair<TKey, TValue> eachItem in dictionary)
			{

				this.Add(eachItem.Key, eachItem.Value);

			}//end foreach

		}//end ctor

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="comparer"></param>
		public FlexibleDictionary(IEqualityComparer<TKey> comparer)
			: base(0, comparer)
		{

		}//end ctor

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="capacity"></param>
		public FlexibleDictionary(int capacity)
			: base(capacity, null)
		{

		}//end ctor

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="capacity"></param>
		/// <param name="comparer"></param>
		public FlexibleDictionary(int capacity, IEqualityComparer<TKey> comparer)
			: base(capacity, comparer)
		{

		}//end ctor

		/// <summary>
		/// Deserialization constructor
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
		protected FlexibleDictionary(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{

		}//end ctor

		#endregion

		/// <summary>
		/// Returns a FlexibleDictionaryEnumerator
		/// </summary>
		/// <returns></returns>
		public new IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
		{

			this.pEnumerationRunning = true;
			return new FlexibleDictionaryEnumerator<TKey, TValue>(this, this.ToList<KeyValuePair<TKey, TValue>>());

		}//end public IEnumerator<KeyValuePair<Tkey, TValue>> GetEnumerator

		private int pNewCount = -1;
		/// <summary>
		/// Returns the items that have been added during a foreach iteration
		/// Returns -1 if it is invoked outside a foreach iteration
		/// </summary>
		public int NewCount
		{

			get
			{

				return this.pNewCount;

			}
			protected set
			{

				this.pNewCount = value;

			}//end get set

		}//end public int AddedCount

		[NonSerialized()]
		private bool pEnumerationRunning;
		/// <summary>
		/// Determines if the dictionary is being enumerated or not
		/// </summary>
		public bool EnumerationRunning
		{

			get
			{
				
				return this.pEnumerationRunning;

			}//end get set

		}//end public bool EnumerationStarted

		[NonSerialized()]
		private List<TKey> itemsToRemove = new List<TKey>();

		/// <summary>
		/// Removes an item from the FlexibleDictionary
		/// Can be used inside foreach enumeration also with no risk at all
		/// </summary>
		/// <param name="key">The key of the item to remove</param>
		/// <returns>True if the item has been successfully removed, otherwise false</returns>
		public new bool Remove(TKey key)
		{

			if (this.pEnumerationRunning)
			{

				if (this.itemsToRemove.Count == this.Count)
				{

					return false;

				}//end if

				
				if (this.ContainsKey(key))
				{

					this.itemsToRemove.Add(key);
					return true;

				}
				else
				{

					return false;

				}//end if else

			}
			else
			{

				return base.Remove(key);

			}//end if else

		}//end void Remove

		/// <summary>
		/// The specific enumerator for the FlexibleDictionary
		/// </summary>
		/// <typeparam name="T1">The type of the keys</typeparam>
		/// <typeparam name="T2">The type of the values</typeparam>
		private class FlexibleDictionaryEnumerator<T1, T2> : IEnumerator<KeyValuePair<T1, T2>>
		{
			
			private int index;
			private int itemCount;
			private FlexibleDictionary<T1, T2> dictionary;
			private List<KeyValuePair<T1, T2>> pairListCache;
			private KeyValuePair<T1, T2> pCurrent;

			/// <summary>
			/// Creates a new instance of the FlexibleDictionaryEnumerator
			/// </summary>
			/// <param name="dictionary">The dictionary which will be enumerated</param>
			/// <param name="pairListCache">The list of KeyValuePairs for indexing purposes</param>
			public FlexibleDictionaryEnumerator(FlexibleDictionary<T1, T2> dictionary, List<KeyValuePair<T1, T2>> pairListCache)
			{

				this.dictionary = dictionary;
				this.itemCount = this.dictionary.Count;
				this.pCurrent = new KeyValuePair<T1, T2>();
				this.index = 0;
				this.pairListCache = pairListCache;

			}//end ctor
			

			#region IEnumerator<KeyValuePair<T1,T2>> Members

			/// <summary>
			/// Returns the current item in the list
			/// </summary>
			public KeyValuePair<T1, T2> Current
			{

				get
				{
				
					return this.pCurrent;

				}//end get

			}//end KeyValuePair<TKey, Tvalue> Current

			#endregion

			#region IEnumerator Members

			object IEnumerator.Current
			{
				get
				{

					return null;

				}//end get
			}

			/// <summary>
			/// Moves the enumerator to the next item in the list
			/// </summary>
			/// <returns>False if the last item has been enumerated</returns>
			public bool MoveNext()
			{
				
				if (this.index < this.itemCount)
				{

					this.dictionary.NewCount = this.dictionary.Count - this.itemCount;
					this.pCurrent = this.pairListCache[this.index++];
                    
					return true;

				}//end if

				this.index = this.dictionary.Count + 1;
				this.pCurrent = new KeyValuePair<T1, T2>();
				this.dictionary.pEnumerationRunning = false;

				if (this.dictionary.itemsToRemove.Count > 0)
				{

					foreach (T1 eachItem in this.dictionary.itemsToRemove)
					{

						this.dictionary.Remove(eachItem);
						
					}//end foreach
					this.dictionary.itemsToRemove.Clear();

				}//end if

				return false;
				
			}//end bool MoveNext

			/// <summary>
			/// Resets all indexes
			/// </summary>
			public void Reset()
			{

				this.index = 0;
				this.itemCount = 0;
				this.pCurrent = new KeyValuePair<T1, T2>();

			}//end void Reset

			#endregion

			#region IDisposable Members

			public void Dispose()
			{

			}

			#endregion

		}//end class FlexibleDictionaryEnumerator

	}//end class FlexibleDictionary<TKey, TValue>
	
	
	
	
	/// <summary>
	/// Class containing method extensions
	/// </summary>
	public static class FlexibleDictionaryExtensions
	{
		
		/// <summary>
		/// Converts an IEnumerable collection to Dictionary
		/// </summary>
		/// <typeparam name="TSource">The type of the collection</typeparam>
		/// <typeparam name="TKey">The type of the key of the FlexibleDictionary</typeparam>
		/// <typeparam name="TValue">The type of the value of the FlexibleDictionary</typeparam>
		/// <param name="source">The type of the IEnumerable collection</param>
		/// <param name="keySelector">Function for determining the key of the FlexibleDictionary</param>
		/// <param name="valueSelector">Function for determining the value of the collection</param>
		/// <returns></returns>
		public static FlexibleDictionary<TKey, TValue> ToFlexibleDictionary<TSource, TKey, TValue>(this IEnumerable<TSource> source, 
			Func<TSource, TKey> keySelector, 
			Func<TSource, TValue> valueSelector)
		{

			FlexibleDictionary<TKey, TValue> toReturn = new FlexibleDictionary<TKey, TValue>();

			foreach (TSource eachItem in source)
			{

				toReturn.Add(keySelector(eachItem), valueSelector(eachItem));

			}//end foreach

			return toReturn;

		}//end static FlexibleDictionary<TKey, TValue> ToFlexibleDictionary
		
	}//end public static class FlexibledictionaryExtensions
	
}

