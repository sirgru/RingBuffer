using System;
using System.Text;
using System.Collections.Generic;
using System.Collections;

namespace ES.RingBuffer
{
	public enum RingOverflowBehavior
	{
		/// <summary>
		/// The size of the Ring buffer will double and elements will be copied over to the new buffer.
		/// </summary>
		AutomaticExpansion,
		/// <summary>
		/// Throws OverflowException.
		/// </summary>
		ThrowException,
		/// <summary>
		/// The new element is ignored.
		/// </summary>
		DropElement,
		/// <summary>
		/// Overrides the element considered Head, but does not advance further. Additional overflows will override the same element, as long as it's not removed with UseHead().
		/// </summary>
		OverrideHead,
		/// <summary>
		/// Overrides the head and advances (the element that was 2nd now becomes the Head). Further overflows will keep overriding the element cosidered the current head.
		/// </summary>
		OverrideHeadAndContinue
	}

	public class RingBuffer<T> : IEnumerable<T>
	{
		private int _headIndex;
		private int _tailIndex;
		private int _usedElements;
		private RingOverflowBehavior _overflowBehavior;

		private List<T> _elements;

		public RingBuffer() : this(32, RingOverflowBehavior.AutomaticExpansion) { }

		public RingBuffer(int initialSize, RingOverflowBehavior overflowBehavior)
		{
			if (initialSize <= 0) throw new ArgumentException("One or more elements is required.");
			_overflowBehavior = overflowBehavior;
			_elements = new List<T>(initialSize);
			for (int i = 0; i < initialSize; i++) {
				_elements.Add(default(T));
			}
			_headIndex = 0;
			_tailIndex = 0;
			_usedElements = 0;
		}

		/// <summary>
		/// Adds the specified element as the Tail.
		/// </summary>
		/// <param name="element">The element to add.</param>
		/// <exception cref="System.OverflowException"></exception>
		public void Add(T element)
		{
			if (_usedElements == _elements.Count) {
				switch (_overflowBehavior) {
				case RingOverflowBehavior.AutomaticExpansion:
					Expand();
					break;
				case RingOverflowBehavior.ThrowException:
					throw new OverflowException("Ring buffer is full and expansion is not allowed.");
				case RingOverflowBehavior.DropElement:
					return;
				case RingOverflowBehavior.OverrideHead:
					_elements[_headIndex] = element;
					return;
				case RingOverflowBehavior.OverrideHeadAndContinue:
					_headIndex = (_headIndex + 1) % _elements.Count;
					_elements[_tailIndex] = element;
					_tailIndex = (_tailIndex + 1) % _elements.Count;
					return;
				}
			}

			_elements[_tailIndex] = element;
			_tailIndex = (_tailIndex + 1) % _elements.Count;
			_usedElements++;
		}

		/// <summary>
		/// Expands the backing store to allow more elements. Involves a Copy operation of previous elements.
		/// </summary>
		public void Expand()
		{
			int newCount = _elements.Count * 2;
			var newElemenets = new List<T>(newCount);
			for (int i = 0; i < newCount; i++) {
				newElemenets.Add(default(T));
			}

			int index = 0;
			while (_usedElements > 0) newElemenets[index++] = UseHead();

			_headIndex = 0;
			_tailIndex = _elements.Count;
			_usedElements = _elements.Count;
			_elements = newElemenets;
		}

		/// <summary>
		/// Gets the head element without removing it.
		/// </summary>
		public T PeekHead {
			get { return _elements[_headIndex]; }
		}

		/// <summary>
		/// Returns the head and removes it, thus advancing the buffer.
		/// </summary>
		/// <exception cref="System.InvalidOperationException">No elemenets but trying to get one.</exception>
		public T UseHead()
		{
			if (_usedElements == 0) throw new InvalidOperationException("No elemenets but trying to get one.");

			T head = _elements[_headIndex];
			if (_headIndex == _elements.Count - 1) _headIndex = 0;
			else _headIndex++;
			_usedElements--;
			return head;
		}

		/// <summary>
		/// Gets the number of currently used elements.
		/// </summary>
		public int Count {
			get { return _usedElements; }
		}

		/// <summary>
		/// Gets the capacity of the ring buffer.
		/// </summary>
		public int Capacity {
			get { return _elements.Count; }
		}

		/// <summary>
		/// Returns a simple string representation of the Ring, elements separated by commas.
		/// Values returned represent values in memory, including the layout, unused and previous elements.
		/// Useful for debugging.
		/// </summary>
		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();
			foreach (var element in _elements) {
				sb.Append(element.ToString()).Append(',');
			}
			sb.Length -= 1;
			return sb.ToString();
		}

		/// <summary>
		///  Returns the string representation of values in the ring, elements in order (from head to tail).
		///  When appending the string representation, elements are added between a prefix and a postfix string.
		///  Last postfix string is removed at the end.
		/// </summary>
		public string Print(string prefix = "", string postfix = ",")
		{
			StringBuilder sb = new StringBuilder();
			foreach (var item in this) {
				sb.Append(prefix).Append(item.ToString()).Append(postfix);
			}
			if (sb.Length > postfix.Length) sb.Length -= postfix.Length;
			return sb.ToString();
		}

		/// <summary>
		/// Returns the value of an element at specified index, counted in the reverse order (from tail to head), starting at 0.
		/// </summary>
		/// <exception cref="System.ArgumentOutOfRangeException"></exception>
		public T GetElementAtReverseIndex(int index)
		{
			if (index >= Count || index < 0) throw new ArgumentOutOfRangeException();

			int i = (_tailIndex - 1 - index) % _elements.Count;
			if (i < 0) i = i + _elements.Count;                      // Fixes negative modulo result for negative numbers.
			return _elements[i];
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		public IEnumerator<T> GetEnumerator()
		{
			for (int i = _headIndex, j = 0; j < Count; i = (i + 1) % _elements.Count, j++) yield return _elements[i];
		}
	}
}