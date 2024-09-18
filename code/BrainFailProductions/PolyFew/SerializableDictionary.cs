using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;

namespace BrainFailProductions.PolyFew
{
	// Token: 0x020008E5 RID: 2277
	[DebuggerDisplay("Count = {Count}")]
	[Serializable]
	public class SerializableDictionary<TKey, TValue> : IDictionary<TKey, TValue>, ICollection<KeyValuePair<TKey, TValue>>, IEnumerable<KeyValuePair<TKey, TValue>>, IEnumerable
	{
		// Token: 0x17000510 RID: 1296
		// (get) Token: 0x0600309A RID: 12442 RVA: 0x002170CB File Offset: 0x002152CB
		public Dictionary<TKey, TValue> AsDictionary
		{
			get
			{
				return new Dictionary<TKey, TValue>(this);
			}
		}

		// Token: 0x17000511 RID: 1297
		// (get) Token: 0x0600309B RID: 12443 RVA: 0x002170D3 File Offset: 0x002152D3
		public int Count
		{
			get
			{
				return this._Count - this._FreeCount;
			}
		}

		// Token: 0x17000512 RID: 1298
		public TValue this[TKey key, TValue defaultValue]
		{
			get
			{
				int num = this.FindIndex(key);
				if (num >= 0)
				{
					return this._Values[num];
				}
				return defaultValue;
			}
		}

		// Token: 0x17000513 RID: 1299
		public TValue this[TKey key]
		{
			get
			{
				int num = this.FindIndex(key);
				if (num >= 0)
				{
					return this._Values[num];
				}
				throw new KeyNotFoundException(key.ToString());
			}
			set
			{
				this.Insert(key, value, false);
			}
		}

		// Token: 0x0600309F RID: 12447 RVA: 0x0021714F File Offset: 0x0021534F
		public SerializableDictionary() : this(0, null)
		{
		}

		// Token: 0x060030A0 RID: 12448 RVA: 0x00217159 File Offset: 0x00215359
		public SerializableDictionary(int capacity) : this(capacity, null)
		{
		}

		// Token: 0x060030A1 RID: 12449 RVA: 0x00217163 File Offset: 0x00215363
		public SerializableDictionary(IEqualityComparer<TKey> comparer) : this(0, comparer)
		{
		}

		// Token: 0x060030A2 RID: 12450 RVA: 0x0021716D File Offset: 0x0021536D
		public SerializableDictionary(int capacity, IEqualityComparer<TKey> comparer)
		{
			if (capacity < 0)
			{
				throw new ArgumentOutOfRangeException("capacity");
			}
			this.Initialize(capacity);
			this._Comparer = (comparer ?? EqualityComparer<TKey>.Default);
		}

		// Token: 0x060030A3 RID: 12451 RVA: 0x0021719B File Offset: 0x0021539B
		public SerializableDictionary(IDictionary<TKey, TValue> dictionary) : this(dictionary, null)
		{
		}

		// Token: 0x060030A4 RID: 12452 RVA: 0x002171A8 File Offset: 0x002153A8
		public SerializableDictionary(IDictionary<TKey, TValue> dictionary, IEqualityComparer<TKey> comparer) : this((dictionary != null) ? dictionary.Count : 0, comparer)
		{
			if (dictionary == null)
			{
				throw new ArgumentNullException("dictionary");
			}
			foreach (KeyValuePair<TKey, TValue> keyValuePair in dictionary)
			{
				this.Add(keyValuePair.Key, keyValuePair.Value);
			}
		}

		// Token: 0x060030A5 RID: 12453 RVA: 0x00217220 File Offset: 0x00215420
		public bool ContainsValue(TValue value)
		{
			if (value == null)
			{
				for (int i = 0; i < this._Count; i++)
				{
					if (this._HashCodes[i] >= 0 && this._Values[i] == null)
					{
						return true;
					}
				}
			}
			else
			{
				EqualityComparer<TValue> @default = EqualityComparer<TValue>.Default;
				for (int j = 0; j < this._Count; j++)
				{
					if (this._HashCodes[j] >= 0 && @default.Equals(this._Values[j], value))
					{
						return true;
					}
				}
			}
			return false;
		}

		// Token: 0x060030A6 RID: 12454 RVA: 0x002172A2 File Offset: 0x002154A2
		public bool ContainsKey(TKey key)
		{
			return this.FindIndex(key) >= 0;
		}

		// Token: 0x060030A7 RID: 12455 RVA: 0x002172B4 File Offset: 0x002154B4
		public void Clear()
		{
			if (this._Count <= 0)
			{
				return;
			}
			for (int i = 0; i < this._Buckets.Length; i++)
			{
				this._Buckets[i] = -1;
			}
			Array.Clear(this._Keys, 0, this._Count);
			Array.Clear(this._Values, 0, this._Count);
			Array.Clear(this._HashCodes, 0, this._Count);
			Array.Clear(this._Next, 0, this._Count);
			this._FreeList = -1;
			this._Count = 0;
			this._FreeCount = 0;
			this._Version++;
		}

		// Token: 0x060030A8 RID: 12456 RVA: 0x00217352 File Offset: 0x00215552
		public void Add(TKey key, TValue value)
		{
			this.Insert(key, value, true);
		}

		// Token: 0x060030A9 RID: 12457 RVA: 0x00217360 File Offset: 0x00215560
		private void Resize(int newSize, bool forceNewHashCodes)
		{
			int[] array = new int[newSize];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = -1;
			}
			TKey[] array2 = new TKey[newSize];
			TValue[] array3 = new TValue[newSize];
			int[] array4 = new int[newSize];
			int[] array5 = new int[newSize];
			Array.Copy(this._Values, 0, array3, 0, this._Count);
			Array.Copy(this._Keys, 0, array2, 0, this._Count);
			Array.Copy(this._HashCodes, 0, array4, 0, this._Count);
			Array.Copy(this._Next, 0, array5, 0, this._Count);
			if (forceNewHashCodes)
			{
				for (int j = 0; j < this._Count; j++)
				{
					if (array4[j] != -1)
					{
						array4[j] = (this._Comparer.GetHashCode(array2[j]) & int.MaxValue);
					}
				}
			}
			for (int k = 0; k < this._Count; k++)
			{
				int num = array4[k] % newSize;
				array5[k] = array[num];
				array[num] = k;
			}
			this._Buckets = array;
			this._Keys = array2;
			this._Values = array3;
			this._HashCodes = array4;
			this._Next = array5;
		}

		// Token: 0x060030AA RID: 12458 RVA: 0x00217485 File Offset: 0x00215685
		private void Resize()
		{
			this.Resize(SerializableDictionary<TKey, TValue>.PrimeHelper.ExpandPrime(this._Count), false);
		}

		// Token: 0x060030AB RID: 12459 RVA: 0x0021749C File Offset: 0x0021569C
		public bool Remove(TKey key)
		{
			if (key == null)
			{
				throw new ArgumentNullException("key");
			}
			int num = this._Comparer.GetHashCode(key) & int.MaxValue;
			int num2 = num % this._Buckets.Length;
			int num3 = -1;
			for (int i = this._Buckets[num2]; i >= 0; i = this._Next[i])
			{
				if (this._HashCodes[i] == num && this._Comparer.Equals(this._Keys[i], key))
				{
					if (num3 < 0)
					{
						this._Buckets[num2] = this._Next[i];
					}
					else
					{
						this._Next[num3] = this._Next[i];
					}
					this._HashCodes[i] = -1;
					this._Next[i] = this._FreeList;
					this._Keys[i] = default(TKey);
					this._Values[i] = default(TValue);
					this._FreeList = i;
					this._FreeCount++;
					this._Version++;
					return true;
				}
				num3 = i;
			}
			return false;
		}

		// Token: 0x060030AC RID: 12460 RVA: 0x002175B8 File Offset: 0x002157B8
		private void Insert(TKey key, TValue value, bool add)
		{
			if (key == null)
			{
				throw new ArgumentNullException("key");
			}
			if (this._Buckets == null)
			{
				this.Initialize(0);
			}
			int num = this._Comparer.GetHashCode(key) & int.MaxValue;
			int num2 = num % this._Buckets.Length;
			int num3 = 0;
			int i = this._Buckets[num2];
			while (i >= 0)
			{
				if (this._HashCodes[i] == num && this._Comparer.Equals(this._Keys[i], key))
				{
					if (add)
					{
						string text = "Key already exists: ";
						TKey tkey = key;
						throw new ArgumentException(text + ((tkey != null) ? tkey.ToString() : null));
					}
					this._Values[i] = value;
					this._Version++;
					return;
				}
				else
				{
					num3++;
					i = this._Next[i];
				}
			}
			int num4;
			if (this._FreeCount > 0)
			{
				num4 = this._FreeList;
				this._FreeList = this._Next[num4];
				this._FreeCount--;
			}
			else
			{
				if (this._Count == this._Keys.Length)
				{
					this.Resize();
					num2 = num % this._Buckets.Length;
				}
				num4 = this._Count;
				this._Count++;
			}
			this._HashCodes[num4] = num;
			this._Next[num4] = this._Buckets[num2];
			this._Keys[num4] = key;
			this._Values[num4] = value;
			this._Buckets[num2] = num4;
			this._Version++;
		}

		// Token: 0x060030AD RID: 12461 RVA: 0x00217750 File Offset: 0x00215950
		private void Initialize(int capacity)
		{
			int prime = SerializableDictionary<TKey, TValue>.PrimeHelper.GetPrime(capacity);
			this._Buckets = new int[prime];
			for (int i = 0; i < this._Buckets.Length; i++)
			{
				this._Buckets[i] = -1;
			}
			this._Keys = new TKey[prime];
			this._Values = new TValue[prime];
			this._HashCodes = new int[prime];
			this._Next = new int[prime];
			this._FreeList = -1;
		}

		// Token: 0x060030AE RID: 12462 RVA: 0x002177C4 File Offset: 0x002159C4
		private int FindIndex(TKey key)
		{
			if (key == null)
			{
				throw new ArgumentNullException("key");
			}
			if (this._Buckets != null)
			{
				int num = this._Comparer.GetHashCode(key) & int.MaxValue;
				for (int i = this._Buckets[num % this._Buckets.Length]; i >= 0; i = this._Next[i])
				{
					if (this._HashCodes[i] == num && this._Comparer.Equals(this._Keys[i], key))
					{
						return i;
					}
				}
			}
			return -1;
		}

		// Token: 0x060030AF RID: 12463 RVA: 0x00217848 File Offset: 0x00215A48
		public bool TryGetValue(TKey key, out TValue value)
		{
			int num = this.FindIndex(key);
			if (num >= 0)
			{
				value = this._Values[num];
				return true;
			}
			value = default(TValue);
			return false;
		}

		// Token: 0x17000514 RID: 1300
		// (get) Token: 0x060030B0 RID: 12464 RVA: 0x0021787D File Offset: 0x00215A7D
		public ICollection<TKey> Keys
		{
			get
			{
				return Enumerable.ToArray<TKey>(Enumerable.Take<TKey>(this._Keys, this.Count));
			}
		}

		// Token: 0x17000515 RID: 1301
		// (get) Token: 0x060030B1 RID: 12465 RVA: 0x00217895 File Offset: 0x00215A95
		public ICollection<TValue> Values
		{
			get
			{
				return Enumerable.ToArray<TValue>(Enumerable.Take<TValue>(this._Values, this.Count));
			}
		}

		// Token: 0x060030B2 RID: 12466 RVA: 0x002178AD File Offset: 0x00215AAD
		public void Add(KeyValuePair<TKey, TValue> item)
		{
			this.Add(item.Key, item.Value);
		}

		// Token: 0x060030B3 RID: 12467 RVA: 0x002178C4 File Offset: 0x00215AC4
		public bool Contains(KeyValuePair<TKey, TValue> item)
		{
			int num = this.FindIndex(item.Key);
			return num >= 0 && EqualityComparer<TValue>.Default.Equals(this._Values[num], item.Value);
		}

		// Token: 0x060030B4 RID: 12468 RVA: 0x00217904 File Offset: 0x00215B04
		public void CopyTo(KeyValuePair<TKey, TValue>[] array, int index)
		{
			if (array == null)
			{
				throw new ArgumentNullException("array");
			}
			if (index < 0 || index > array.Length)
			{
				throw new ArgumentOutOfRangeException(string.Format("index = {0} array.Length = {1}", index, array.Length));
			}
			if (array.Length - index < this.Count)
			{
				throw new ArgumentException(string.Format("The number of elements in the dictionary ({0}) is greater than the available space from index to the end of the destination array {1}.", this.Count, array.Length));
			}
			for (int i = 0; i < this._Count; i++)
			{
				if (this._HashCodes[i] >= 0)
				{
					array[index++] = new KeyValuePair<TKey, TValue>(this._Keys[i], this._Values[i]);
				}
			}
		}

		// Token: 0x17000516 RID: 1302
		// (get) Token: 0x060030B5 RID: 12469 RVA: 0x00085A40 File Offset: 0x00083C40
		public bool IsReadOnly
		{
			get
			{
				return false;
			}
		}

		// Token: 0x060030B6 RID: 12470 RVA: 0x002179BC File Offset: 0x00215BBC
		public bool Remove(KeyValuePair<TKey, TValue> item)
		{
			return this.Remove(item.Key);
		}

		// Token: 0x060030B7 RID: 12471 RVA: 0x002179CB File Offset: 0x00215BCB
		public SerializableDictionary<TKey, TValue>.Enumerator GetEnumerator()
		{
			return new SerializableDictionary<TKey, TValue>.Enumerator(this);
		}

		// Token: 0x060030B8 RID: 12472 RVA: 0x002179D3 File Offset: 0x00215BD3
		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		// Token: 0x060030B9 RID: 12473 RVA: 0x002179D3 File Offset: 0x00215BD3
		IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<!0, !1>>.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		// Token: 0x04004BA0 RID: 19360
		[SerializeField]
		[HideInInspector]
		private int[] _Buckets;

		// Token: 0x04004BA1 RID: 19361
		[SerializeField]
		[HideInInspector]
		private int[] _HashCodes;

		// Token: 0x04004BA2 RID: 19362
		[HideInInspector]
		[SerializeField]
		private int[] _Next;

		// Token: 0x04004BA3 RID: 19363
		[SerializeField]
		[HideInInspector]
		private int _Count;

		// Token: 0x04004BA4 RID: 19364
		[SerializeField]
		[HideInInspector]
		private int _Version;

		// Token: 0x04004BA5 RID: 19365
		[SerializeField]
		[HideInInspector]
		private int _FreeList;

		// Token: 0x04004BA6 RID: 19366
		[HideInInspector]
		[SerializeField]
		private int _FreeCount;

		// Token: 0x04004BA7 RID: 19367
		[SerializeField]
		[HideInInspector]
		private TKey[] _Keys;

		// Token: 0x04004BA8 RID: 19368
		[SerializeField]
		[HideInInspector]
		private TValue[] _Values;

		// Token: 0x04004BA9 RID: 19369
		private readonly IEqualityComparer<TKey> _Comparer;

		// Token: 0x020008E6 RID: 2278
		private static class PrimeHelper
		{
			// Token: 0x060030BA RID: 12474 RVA: 0x002179E0 File Offset: 0x00215BE0
			public static bool IsPrime(int candidate)
			{
				if ((candidate & 1) != 0)
				{
					int num = (int)Math.Sqrt((double)candidate);
					for (int i = 3; i <= num; i += 2)
					{
						if (candidate % i == 0)
						{
							return false;
						}
					}
					return true;
				}
				return candidate == 2;
			}

			// Token: 0x060030BB RID: 12475 RVA: 0x00217A14 File Offset: 0x00215C14
			public static int GetPrime(int min)
			{
				if (min < 0)
				{
					throw new ArgumentException("min < 0");
				}
				for (int i = 0; i < SerializableDictionary<TKey, TValue>.PrimeHelper.Primes.Length; i++)
				{
					int num = SerializableDictionary<TKey, TValue>.PrimeHelper.Primes[i];
					if (num >= min)
					{
						return num;
					}
				}
				for (int j = min | 1; j < 2147483647; j += 2)
				{
					if (SerializableDictionary<TKey, TValue>.PrimeHelper.IsPrime(j) && (j - 1) % 101 != 0)
					{
						return j;
					}
				}
				return min;
			}

			// Token: 0x060030BC RID: 12476 RVA: 0x00217A78 File Offset: 0x00215C78
			public static int ExpandPrime(int oldSize)
			{
				int num = 2 * oldSize;
				if (num > 2146435069 && 2146435069 > oldSize)
				{
					return 2146435069;
				}
				return SerializableDictionary<TKey, TValue>.PrimeHelper.GetPrime(num);
			}

			// Token: 0x04004BAA RID: 19370
			public static readonly int[] Primes = new int[]
			{
				3,
				7,
				11,
				17,
				23,
				29,
				37,
				47,
				59,
				71,
				89,
				107,
				131,
				163,
				197,
				239,
				293,
				353,
				431,
				521,
				631,
				761,
				919,
				1103,
				1327,
				1597,
				1931,
				2333,
				2801,
				3371,
				4049,
				4861,
				5839,
				7013,
				8419,
				10103,
				12143,
				14591,
				17519,
				21023,
				25229,
				30293,
				36353,
				43627,
				52361,
				62851,
				75431,
				90523,
				108631,
				130363,
				156437,
				187751,
				225307,
				270371,
				324449,
				389357,
				467237,
				560689,
				672827,
				807403,
				968897,
				1162687,
				1395263,
				1674319,
				2009191,
				2411033,
				2893249,
				3471899,
				4166287,
				4999559,
				5999471,
				7199369
			};
		}

		// Token: 0x020008E7 RID: 2279
		public struct Enumerator : IEnumerator<KeyValuePair<TKey, TValue>>, IEnumerator, IDisposable
		{
			// Token: 0x17000517 RID: 1303
			// (get) Token: 0x060030BE RID: 12478 RVA: 0x00217ABE File Offset: 0x00215CBE
			public KeyValuePair<TKey, TValue> Current
			{
				get
				{
					return this._Current;
				}
			}

			// Token: 0x060030BF RID: 12479 RVA: 0x00217AC6 File Offset: 0x00215CC6
			internal Enumerator(SerializableDictionary<TKey, TValue> dictionary)
			{
				this._Dictionary = dictionary;
				this._Version = dictionary._Version;
				this._Current = default(KeyValuePair<TKey, TValue>);
				this._Index = 0;
			}

			// Token: 0x060030C0 RID: 12480 RVA: 0x00217AF0 File Offset: 0x00215CF0
			public bool MoveNext()
			{
				if (this._Version != this._Dictionary._Version)
				{
					throw new InvalidOperationException(string.Format("Enumerator version {0} != Dictionary version {1}", this._Version, this._Dictionary._Version));
				}
				while (this._Index < this._Dictionary._Count)
				{
					if (this._Dictionary._HashCodes[this._Index] >= 0)
					{
						this._Current = new KeyValuePair<TKey, TValue>(this._Dictionary._Keys[this._Index], this._Dictionary._Values[this._Index]);
						this._Index++;
						return true;
					}
					this._Index++;
				}
				this._Index = this._Dictionary._Count + 1;
				this._Current = default(KeyValuePair<TKey, TValue>);
				return false;
			}

			// Token: 0x060030C1 RID: 12481 RVA: 0x00217BDC File Offset: 0x00215DDC
			void IEnumerator.Reset()
			{
				if (this._Version != this._Dictionary._Version)
				{
					throw new InvalidOperationException(string.Format("Enumerator version {0} != Dictionary version {1}", this._Version, this._Dictionary._Version));
				}
				this._Index = 0;
				this._Current = default(KeyValuePair<TKey, TValue>);
			}

			// Token: 0x17000518 RID: 1304
			// (get) Token: 0x060030C2 RID: 12482 RVA: 0x00217C3A File Offset: 0x00215E3A
			object IEnumerator.Current
			{
				get
				{
					return this.Current;
				}
			}

			// Token: 0x060030C3 RID: 12483 RVA: 0x00002265 File Offset: 0x00000465
			public void Dispose()
			{
			}

			// Token: 0x04004BAB RID: 19371
			private readonly SerializableDictionary<TKey, TValue> _Dictionary;

			// Token: 0x04004BAC RID: 19372
			private int _Version;

			// Token: 0x04004BAD RID: 19373
			private int _Index;

			// Token: 0x04004BAE RID: 19374
			private KeyValuePair<TKey, TValue> _Current;
		}
	}
}
