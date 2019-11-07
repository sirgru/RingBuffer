# Readme

This is a very simple implementation of a `RingBuffer<T>`, wit a slim API and without thread safety.

`Add(T element)` - Adds the specified element as the Tail.

`Expand()` - Expands the backing store to allow more elements. Involves a Copy operation of previous elements.

`PeekHead` - Gets the head element without removing it.

`UseHead()` - Returns the head and removes it, advancing the buffer.

`Count` - Gets the number of currently used elements.

`Capacity` -  Gets the capacity of the ring buffer.

`ToString()` - Returns a simple string representation of the Ring, elements separated by commas. Values returned represent values in memory, including the layout, unused and previous elements. Useful for debugging.

`Print(string prefix = "", string postfix = ",")` - Returns the string representation of values in the ring, elements in order (from head to tail), separated by commas. When appending the string representation, elements are added between a prefix and a postfix string. Last postfix string is removed at the end.

`GetElementAtReverseIndex(int index)` - Returns the value of an element at specified index, counted in the reverse order (from tail to head), starting at 0.

It allows the selection of following behaviours when the ring is full:

* AutomaticExpansion: The size of the Ring buffer will double and elements will be copied over to the new buffer.
* ThrowException: Throws OverflowException.
* DropElement: The new element is ignored.
* OverrideHead: Overrides the element considered Head, but does not advance further. Additional overflows will override the same element, as long as it's not removed with UseHead().
* OverrideHeadAndContinue: Overrides the head and advances (the element that was 2nd now becomes the Head). Further overflows will keep overriding the element considered the current head.

Implements IEnumerable interface, can be iterated over with foreach (from Head to Tail without modifying the Ring).
