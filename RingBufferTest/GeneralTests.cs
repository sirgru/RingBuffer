using NUnit.Framework;

namespace ES.RingBuffer
{
	[TestFixture]
	public class GeneralTests
	{
		void Add5Elemenets(RingBuffer<int> rb)
		{
			rb.Add(1);
			rb.Add(2);
			rb.Add(3);
			rb.Add(4);
			rb.Add(5);
		}

		void Add6ELements(RingBuffer<int> rb)
		{
			Add5Elemenets(rb);
			rb.Add(6);
		}

		[Test]
		public void RingBuffer_AutomaticExpansion_DoesExpand()
		{
			var rb = new RingBuffer<int>(5, RingOverflowBehavior.AutomaticExpansion);
			Add6ELements(rb);
			string memoryLayout = rb.ToString();
			Assert.AreEqual("1,2,3,4,5,6,0,0,0,0", memoryLayout);
		}

		[Test]
		public void RingBuffer_DropElement_DoesDrop()
		{
			var rb = new RingBuffer<int>(5, RingOverflowBehavior.DropElement);
			Add6ELements(rb);
			string memoryLayout = rb.ToString();
			Assert.AreEqual("1,2,3,4,5", memoryLayout);
		}

		[Test]
		public void RingBuffer_OverrideHead_DoesOverride()
		{
			var rb = new RingBuffer<int>(5, RingOverflowBehavior.OverrideHead);
			Add6ELements(rb);
			rb.Add(7);
			string memoryLayout = rb.ToString();
			Assert.AreEqual("7,2,3,4,5", memoryLayout);
		}

		[Test]
		public void RingBuffer_OverrideHeadAndContinue_DoesOverride()
		{
			var rb = new RingBuffer<int>(5, RingOverflowBehavior.OverrideHeadAndContinue);
			Add6ELements(rb);
			rb.Add(7);
			string memoryLayout = rb.ToString();
			Assert.AreEqual("6,7,3,4,5", memoryLayout);
		}

		[Test]
		public void RingBuffer_ThrowException_Throws()
		{
			var rb = new RingBuffer<int>(5, RingOverflowBehavior.ThrowException);
			Add5Elemenets(rb);
			Assert.Catch<System.OverflowException>(() => rb.Add(6));
		}

		[Test]
		public void RingBuffer_PeekHead_Peeks()
		{
			var rb = new RingBuffer<int>(5, RingOverflowBehavior.ThrowException);
			rb.Add(1);
			rb.Add(2);
			int head = rb.PeekHead;
			Assert.AreEqual(head, 1);
			string memoryLayout = rb.ToString();
			Assert.AreEqual("1,2,0,0,0", memoryLayout);
		}

		[Test]
		public void RingBuffer_UseHead_Uses()
		{
			var rb = new RingBuffer<int>(5, RingOverflowBehavior.ThrowException);
			rb.Add(1);
			rb.Add(2);
			int head = rb.UseHead();
			Assert.AreEqual(head, 1);
			string memoryLayout = rb.ToString();
			Assert.AreEqual("1,2,0,0,0", memoryLayout);
			head = rb.PeekHead;
			Assert.AreEqual(head, 2);
		}

		[Test]
		public void RingBuffer_Print_ProperFormatting()
		{
			var rb = new RingBuffer<int>(5, RingOverflowBehavior.ThrowException);
			Add5Elemenets(rb);
			string print = rb.Print(":", ";");
			Assert.AreEqual(":1;:2;:3;:4;:5", print);
		}

		[Test]
		public void RingBuffer_GetElementAtReverseIndex_Works()
		{
			var rb = new RingBuffer<int>(5, RingOverflowBehavior.ThrowException);
			Add5Elemenets(rb);
			int element = rb.GetElementAtReverseIndex(1);
			Assert.AreEqual(element, 4);
		}

		[Test]
		public void RingBuffer_Count()
		{
			var rb = new RingBuffer<int>(5, RingOverflowBehavior.ThrowException);
			rb.Add(1);
			rb.Add(2);
			Assert.AreEqual(rb.Count, 2);
		}

		[Test]
		public void RingBuffer_Capacity()
		{
			var rb = new RingBuffer<int>(5, RingOverflowBehavior.ThrowException);
			Assert.AreEqual(rb.Capacity, 5);
		}
	}
}
