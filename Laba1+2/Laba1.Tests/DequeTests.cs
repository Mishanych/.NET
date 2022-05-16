﻿using System;
using Xunit;
using System.Collections.Generic;
using System.Collections;
using OwnCollections;

namespace UnitTests
{
    public class Deque
    {

        [Fact]
        public void Constructor_FromSequence_InitializesFromSequence()
        {
            var deque = new Deque<int>(new int[] { 1, 2, 3 });
            Assert.Equal(3, deque.Capacity);
            Assert.Equal(3, deque.Count);
            Assert.Equal(new int[] { 1, 2, 3 }, deque);
        }

        [Fact]
        public void Add_IsAddToBack()
        {
            var deque1 = new Deque<int>(new[] { 1, 2 });
            var deque2 = new Deque<int>(new[] { 1, 2 });
            ((ICollection<int>)deque1).Add(3);
            deque2.AddToBack(3);
            Assert.Equal(deque1, deque2);
        }

        [Fact]
        public void IsReadOnly_ReturnsFalse()
        {
            var deque = new Deque<int>();
            Assert.False(((ICollection<int>)deque).IsReadOnly);
        }

        [Fact]
        public void Clear_EmptiesAllItems()
        {
            var deque = new Deque<int>(new[] { 1, 2, 3 });
            deque.Clear();
            Assert.Empty(deque);
            Assert.Equal(new int[] { }, deque);
        }

        #region Capacity Tests

        [Fact]
        public void Capacity_SetTo0_ActsLikeList()
        {
            var list = new List<int>();
            list.Capacity = 0;
            Assert.Equal(0, list.Capacity);

            var deque = new Deque<int>();
            deque.Capacity = 0;
            Assert.Equal(0, deque.Capacity);
        }

        [Fact]
        public void Capacity_SetLarger_UsesSpecifiedCapacity()
        {
            var deque = new Deque<int>(1);
            Assert.Equal(1, deque.Capacity);
            deque.Capacity = 17;
            Assert.Equal(17, deque.Capacity);
        }

        [Fact]
        public void Capacity_SetSmaller_UsesSpecifiedCapacity()
        {
            var deque = new Deque<int>(13);
            Assert.Equal(13, deque.Capacity);
            deque.Capacity = 7;
            Assert.Equal(7, deque.Capacity);
        }

        [Fact]
        public void Capacity_Set_PreservesData()
        {
            var deque = new Deque<int>(new int[] { 1, 2, 3 });
            Assert.Equal(3, deque.Capacity);
            deque.Capacity = 7;
            Assert.Equal(7, deque.Capacity);
            Assert.Equal(new[] { 1, 2, 3 }, deque);
        }

        [Fact]
        public void Capacity_Set_WhenSplit_PreservesData()
        {
            var deque = new Deque<int>(new int[] { 1, 2, 3 });
            deque.RemoveFromFront();
            deque.AddToBack(4);
            Assert.Equal(3, deque.Capacity);
            deque.Capacity = 7;
            Assert.Equal(7, deque.Capacity);
            Assert.Equal(new[] { 2, 3, 4 }, deque);
        }

        [Fact]
        public void Capacity_Set_SmallerThanCount_ActsLikeList()
        {
            var list = new List<int>(new int[] { 1, 2, 3 });
            Assert.Equal(3, list.Capacity);
            Assert.Throws<ArgumentOutOfRangeException>("value", () => { list.Capacity = 2; });

            var deque = new Deque<int>(new int[] { 1, 2, 3 });
            Assert.Equal(3, deque.Capacity);
            Assert.Throws<ArgumentOutOfRangeException>("value", () => { deque.Capacity = 2; });
        }

        const int DefaultCapacity = 8;

        [Fact]
        public void Constructor_CapacityOf0_ActsLikeList()
        {
            var list = new List<int>(0);
            Assert.Equal(0, list.Capacity);

            var deque = new Deque<int>(0);
            Assert.Equal(0, deque.Capacity);
        }

        [Fact]
        public void Constructor_CapacityOf0_PermitsAdd()
        {
            var deque = new Deque<int>(0);
            deque.AddToBack(13);
            Assert.Equal(new[] { 13 }, deque);
        }

        [Fact]
        public void Constructor_NegativeCapacity_ActsLikeList()
        {
            Assert.Throws<ArgumentOutOfRangeException>("capacity", () => new List<int>(-1));

            Assert.Throws<ArgumentOutOfRangeException>("capacity", () => new Deque<int>(-1));
        }

        [Fact]
        public void Constructor_FromEmptySequence_UsesDefaultCapacity()
        {
            var deque = new Deque<int>(new int[] { });
            Assert.Equal(DefaultCapacity, deque.Capacity);
        }

        #endregion

        #region IndexOf Tests

        [Fact]
        public void IndexOf_ItemPresent_ReturnsItemIndex()
        {
            var deque = new Deque<int>(new[] { 1, 2 });
            var result = deque.IndexOf(2);
            Assert.Equal(1, result);
        }

        [Fact]
        public void IndexOf_ItemNotPresent_ReturnsNegativeOne()
        {
            var deque = new Deque<int>(new[] { 1, 2 });
            var result = deque.IndexOf(3);
            Assert.Equal(-1, result);
        }

        [Fact]
        public void IndexOf_ItemPresentAndSplit_ReturnsItemIndex()
        {
            var deque = new Deque<int>(new[] { 1, 2, 3 });
            deque.RemoveFromBack();
            deque.AddToFront(0);
            Assert.Equal(0, deque.IndexOf(0));
            Assert.Equal(1, deque.IndexOf(1));
            Assert.Equal(2, deque.IndexOf(2));
        }

        #endregion

        #region Contains Tests

        [Fact]
        public void Contains_ItemPresent_ReturnsTrue()
        {
            var deque = new Deque<int>(new[] { 1, 2 }) as ICollection<int>;
            Assert.True(deque.Contains(2));
        }

        [Fact]
        public void Contains_ItemNotPresent_ReturnsFalse()
        {
            var deque = new Deque<int>(new[] { 1, 2 }) as ICollection<int>;
            Assert.False(deque.Contains(3));
        }

        [Fact]
        public void Contains_ItemPresentAndSplit_ReturnsTrue()
        {
            var deque = new Deque<int>(new[] { 1, 2, 3 });
            deque.RemoveFromBack();
            deque.AddToFront(0);
            var deq = deque as ICollection<int>;
            Assert.True(deq.Contains(0));
            Assert.True(deq.Contains(1));
            Assert.True(deq.Contains(2));
            Assert.False(deq.Contains(3));
        }

        #endregion

        #region CopyTo Tests

        [Fact]
        public void CopyTo_CopiesItems()
        {
            var deque = new Deque<int>(new[] { 1, 2, 3 });
            var results = new int[3];
            ((ICollection<int>)deque).CopyTo(results, 0);
        }

        [Fact]
        public void CopyTo_NullArray_ActsLikeList()
        {
            var list = new List<int>(new[] { 1, 2, 3 });
            Assert.Throws<ArgumentNullException>(() => ((ICollection<int>)list).CopyTo(null, 0));

            var deque = new Deque<int>(new[] { 1, 2, 3 });
            Assert.Throws<ArgumentNullException>(() => ((ICollection<int>)deque).CopyTo(null, 0));
        }

        [Fact]
        public void CopyTo_NegativeOffset_ActsLikeList()
        {
            var destination = new int[3];
            var list = new List<int>(new[] { 1, 2, 3 });
            Assert.Throws<ArgumentOutOfRangeException>(() => ((ICollection<int>)list).CopyTo(destination, -1));

            var deque = new Deque<int>(new[] { 1, 2, 3 });
            Assert.Throws<ArgumentOutOfRangeException>(() => ((ICollection<int>)deque).CopyTo(destination, -1));
        }

        [Fact]
        public void CopyTo_InsufficientSpace_ActsLikeList()
        {
            var destination = new int[3];
            var list = new List<int>(new[] { 1, 2, 3 });
            Assert.Throws<ArgumentException>(() => ((ICollection<int>)list).CopyTo(destination, 1));

            var deque = new Deque<int>(new[] { 1, 2, 3 });
            Assert.Throws<ArgumentException>(() => ((ICollection<int>)deque).CopyTo(destination, 1));
        }

        #endregion

        #region Remove Tests

        [Fact]
        public void RemoveFromFront_Empty_ActsLikeStack()
        {
            var stack = new Stack<int>();
            Assert.Throws<InvalidOperationException>(() => stack.Pop());

            var deque = new Deque<int>();
            Assert.Throws<InvalidOperationException>(() => deque.RemoveFromFront());
        }

        [Fact]
        public void RemoveFromBack_Empty_ActsLikeQueue()
        {
            var queue = new Queue<int>();
            Assert.Throws<InvalidOperationException>(() => queue.Dequeue());

            var deque = new Deque<int>();
            Assert.Throws<InvalidOperationException>(() => deque.RemoveFromBack());
        }

        [Fact]
        public void Remove_ItemPresent_RemovesItemAndReturnsTrue()
        {
            var deque = new Deque<int>(new[] { 1, 2, 3, 4 });
            var result = deque.Remove(3);
            Assert.True(result);
            Assert.Equal(new[] { 1, 2, 4 }, deque);
        }

        [Fact]
        public void Remove_ItemNotPresent_KeepsItemsReturnsFalse()
        {
            var deque = new Deque<int>(new[] { 1, 2, 3, 4 });
            var result = deque.Remove(5);
            Assert.False(result);
            Assert.Equal(new[] { 1, 2, 3, 4 }, deque);
        }

        [Fact]
        public void RemoveMultiple()
        {
            RemoveTest(new[] { 1, 2, 3 });
            RemoveTest(new[] { 1, 2, 3, 4 });
        }

        private void RemoveTest(IReadOnlyCollection<int> initial)
        {
            for (int count = 0; count <= initial.Count; ++count)
            {
                for (int rotated = 0; rotated <= initial.Count; ++rotated)
                {
                    for (int index = 0; index <= initial.Count - count; ++index)
                    {
                        var result = new List<int>(initial);
                        for (int i = 0; i != rotated; ++i)
                        {
                            var item = result[0];
                            result.RemoveAt(0);
                            result.Add(item);
                        }
                        result.RemoveRange(index, count);

                        var deque = new Deque<int>(initial);

                        for (int i = 0; i != rotated; ++i)
                        {
                            var item = deque[0];
                            deque.RemoveFromFront();
                            deque.AddToBack(item);
                        }

                        deque.RemoveRange(index, count);

                        Assert.Equal(result, deque);
                    }
                }
            }
        }

        [Fact]
        public void Remove_RangeOfZeroElements_HasNoEffect()
        {
            var deque = new Deque<int>(new[] { 1, 2, 3 });
            deque.RemoveRange(1, 0);
            Assert.Equal(new[] { 1, 2, 3 }, deque);
        }

        [Fact]
        public void Remove_NegativeCount_ActsLikeList()
        {
            var list = new List<int>(new[] { 1, 2, 3 });
            Assert.Throws<ArgumentOutOfRangeException>("count", () => list.RemoveRange(1, -1));

            var deque = new Deque<int>(new[] { 1, 2, 3 });
            Assert.Throws<ArgumentOutOfRangeException>("count", () => deque.RemoveRange(1, -1));
        }

        #endregion

        #region Insert Tests

        [Fact]
        public void Insert_InsertsElementAtIndex()
        {
            var deque = new Deque<int>(new[] { 1, 2 });
            deque.Insert(1, 13);
            Assert.Equal(new[] { 1, 13, 2 }, deque);
        }

        [Fact]
        public void Insert_NegativeIndex_ActsLikeList()
        {
            var list = new List<int>(new[] { 1, 2, 3 });
            Assert.Throws<ArgumentOutOfRangeException>("index", () => list.Insert(-1, 0));

            var deque = new Deque<int>(new[] { 1, 2, 3 });
            Assert.Throws<ArgumentOutOfRangeException>("index", () => deque.Insert(-1, 0));
        }

        [Fact]
        public void Insert_IndexTooLarge_ActsLikeList()
        {
            var list = new List<int>(new[] { 1, 2, 3 });
            Assert.Throws<ArgumentOutOfRangeException>("index", () => list.Insert(list.Count + 1, 0));

            var deque = new Deque<int>(new[] { 1, 2, 3 });
            Assert.Throws<ArgumentOutOfRangeException>("index", () => deque.Insert(deque.Count + 1, 0));
        }

        [Fact]
        public void InsertMultiple()
        {
            InsertTest(new[] { 1, 2, 3 }, new[] { 7, 13 });
            InsertTest(new[] { 1, 2, 3, 4 }, new[] { 7, 13 });
        }

        private void InsertTest(IReadOnlyCollection<int> initial, IReadOnlyCollection<int> items)
        {
            var totalCapacity = initial.Count + items.Count;
            for (int rotated = 0; rotated <= totalCapacity; ++rotated)
            {
                for (int index = 0; index <= initial.Count; ++index)
                {
                    var result = new List<int>(initial);
                    for (int i = 0; i != rotated; ++i)
                    {
                        var item = result[0];
                        result.RemoveAt(0);
                        result.Add(item);
                    }
                    result.InsertRange(index, items);

                    var deque = new Deque<int>(initial);
                    deque.Capacity += items.Count;

                    for (int i = 0; i != rotated; ++i)
                    {
                        var item = deque[0];
                        deque.RemoveFromFront();
                        deque.AddToBack(item);
                    }

                    deque.InsertRange(index, items);

                    Assert.Equal(result, deque);
                }
            }
        }

        [Fact]
        public void Insert_RangeOfZeroElements_HasNoEffect()
        {
            var deque = new Deque<int>(new[] { 1, 2, 3 });
            deque.InsertRange(1, new int[] { });
            Assert.Equal(new[] { 1, 2, 3 }, deque);
        }

        [Fact]
        public void InsertMultiple_MakesRoomForNewElements()
        {
            var deque = new Deque<int>(new[] { 1, 2, 3 });
            deque.InsertRange(1, new[] { 7, 13 });
            Assert.Equal(new[] { 1, 7, 13, 2, 3 }, deque);
            Assert.Equal(5, deque.Capacity);
        }

        #endregion

        #region RemoveAt Tests

        [Fact]
        public void RemoveAt_RemovesElementAtIndex()
        {
            var deque = new Deque<int>(new[] { 1, 2, 3 });
            deque.RemoveFromBack();
            deque.AddToFront(0);
            deque.RemoveAt(1);
            Assert.Equal(new[] { 0, 2 }, deque);
        }

        [Fact]
        public void RemoveAt_Index0_IsSameAsRemoveFromFront()
        {
            var deque1 = new Deque<int>(new[] { 1, 2 });
            var deque2 = new Deque<int>(new[] { 1, 2 });
            deque1.RemoveAt(0);
            deque2.RemoveFromFront();
            Assert.Equal(deque1, deque2);
        }

        [Fact]
        public void RemoveAt_LastIndex_IsSameAsRemoveFromBack()
        {
            var deque1 = new Deque<int>(new[] { 1, 2 });
            var deque2 = new Deque<int>(new[] { 1, 2 });
            deque1.RemoveAt(1);
            deque2.RemoveFromBack();
            Assert.Equal(deque1, deque2);
        }

        [Fact]
        public void RemoveAt_NegativeIndex_ActsLikeList()
        {
            var list = new List<int>(new[] { 1, 2, 3 });
            Assert.Throws<ArgumentOutOfRangeException>("index", () => list.RemoveAt(-1));

            var deque = new Deque<int>(new[] { 1, 2, 3 });
            Assert.Throws<ArgumentOutOfRangeException>("index", () => deque.RemoveAt(-1));
        }

        [Fact]
        public void RemoveAt_IndexTooLarge_ActsLikeList()
        {
            var list = new List<int>(new[] { 1, 2, 3 });
            Assert.Throws<ArgumentOutOfRangeException>("index", () => list.RemoveAt(list.Count));

            var deque = new Deque<int>(new[] { 1, 2, 3 });
            Assert.Throws<ArgumentOutOfRangeException>("index", () => deque.RemoveAt(deque.Count));
        }

        #endregion

        #region GetItem Tests

        [Fact]
        public void GetItem_ReadsElements()
        {
            var deque = new Deque<int>(new[] { 1, 2, 3 });
            Assert.Equal(1, deque[0]);
            Assert.Equal(2, deque[1]);
            Assert.Equal(3, deque[2]);
        }

        [Fact]
        public void GetItem_Split_ReadsElements()
        {
            var deque = new Deque<int>(new[] { 1, 2, 3 });
            deque.RemoveFromBack();
            deque.AddToFront(0);
            Assert.Equal(0, deque[0]);
            Assert.Equal(1, deque[1]);
            Assert.Equal(2, deque[2]);
        }

        [Fact]
        public void GetItem_IndexTooLarge_ActsLikeList()
        {
            var list = new List<int>(new[] { 1, 2, 3 });
            Assert.Throws<ArgumentOutOfRangeException>("index", () => list[3]);

            var deque = new Deque<int>(new[] { 1, 2, 3 });
            Assert.Throws<ArgumentOutOfRangeException>("index", () => deque[3]);
        }

        [Fact]
        public void GetItem_NegativeIndex_ActsLikeList()
        {
            var list = new List<int>(new[] { 1, 2, 3 });
            Assert.Throws<ArgumentOutOfRangeException>("index", () => list[-1]);

            var deque = new Deque<int>(new[] { 1, 2, 3 });
            Assert.Throws<ArgumentOutOfRangeException>("index", () => deque[-1]);
        }

        #endregion

        #region SetItem Tests

        [Fact]
        public void SetItem_WritesElements()
        {
            var deque = new Deque<int>(new[] { 1, 2, 3 });
            deque[0] = 7;
            deque[1] = 11;
            deque[2] = 13;
            Assert.Equal(new[] { 7, 11, 13 }, deque);
        }

        [Fact]
        public void SetItem_IndexTooLarge_ActsLikeList()
        {
            var list = new List<int>(new[] { 1, 2, 3 });
            Assert.Throws<ArgumentOutOfRangeException>("index", () => { list[3] = 13; });

            var deque = new Deque<int>(new[] { 1, 2, 3 });
            Assert.Throws<ArgumentOutOfRangeException>("index", () => { deque[3] = 13; });
        }

        [Fact]
        public void SetItem_NegativeIndex_ActsLikeList()
        {
            var list = new List<int>(new[] { 1, 2, 3 });
            Assert.Throws<ArgumentOutOfRangeException>("index", () => { list[-1] = 13; });

            var deque = new Deque<int>(new[] { 1, 2, 3 });
            Assert.Throws<ArgumentOutOfRangeException>("index", () => { deque[-1] = 13; });
        }

        #endregion

        #region Nongeneric Tests

        [Fact]
        public void NongenericIndexOf_ItemPresent_ReturnsItemIndex()
        {
            var deque = new Deque<int>(new[] { 1, 2 }) as IList;
            var result = deque.IndexOf(2);
            Assert.Equal(1, result);
        }

        [Fact]
        public void NongenericIndexOf_ItemNotPresent_ReturnsNegativeOne()
        {
            var deque = new Deque<int>(new[] { 1, 2 }) as IList;
            var result = deque.IndexOf(3);
            Assert.Equal(-1, result);
        }

        [Fact]
        public void NongenericIndexOf_ItemPresentAndSplit_ReturnsItemIndex()
        {
            var deque_ = new Deque<int>(new[] { 1, 2, 3 });
            deque_.RemoveFromBack();
            deque_.AddToFront(0);
            var deque = deque_ as IList;
            Assert.Equal(0, deque.IndexOf(0));
            Assert.Equal(1, deque.IndexOf(1));
            Assert.Equal(2, deque.IndexOf(2));
        }

        [Fact]
        public void NongenericIndexOf_WrongItemType_ReturnsNegativeOne()
        {
            var list = new List<int>(new[] { 1, 2 }) as IList;
            Assert.Equal(-1, list.IndexOf(this));

            var deque = new Deque<int>(new[] { 1, 2 }) as IList;
            Assert.Equal(-1, deque.IndexOf(this));
        }

        [Fact]
        public void NongenericContains_WrongItemType_ReturnsFalse()
        {
            var list = new List<int>(new[] { 1, 2 }) as IList;
            Assert.False(list.Contains(this));

            var deque = new Deque<int>(new[] { 1, 2 }) as IList;
            Assert.False(deque.Contains(this));
        }

        [Fact]
        public void NongenericContains_ItemPresent_ReturnsTrue()
        {
            var deque = new Deque<int>(new[] { 1, 2 }) as IList;
            Assert.True(deque.Contains(2));
        }

        [Fact]
        public void NongenericContains_ItemNotPresent_ReturnsFalse()
        {
            var deque = new Deque<int>(new[] { 1, 2 }) as IList;
            Assert.False(deque.Contains(3));
        }

        [Fact]
        public void NongenericContains_ItemPresentAndSplit_ReturnsTrue()
        {
            var deque_ = new Deque<int>(new[] { 1, 2, 3 });
            deque_.RemoveFromBack();
            deque_.AddToFront(0);
            var deque = deque_ as IList;
            Assert.True(deque.Contains(0));
            Assert.True(deque.Contains(1));
            Assert.True(deque.Contains(2));
            Assert.False(deque.Contains(3));
        }

        [Fact]
        public void NongenericIsReadOnly_ReturnsFalse()
        {
            var deque = new Deque<int>() as IList;
            Assert.False(deque.IsReadOnly);
        }

        [Fact]
        public void NongenericCopyTo_CopiesItems()
        {
            var deque = new Deque<int>(new[] { 1, 2, 3 }) as IList;
            var results = new int[3];
            deque.CopyTo(results, 0);
        }

        [Fact]
        public void NongenericCopyTo_NullArray_ActsLikeList()
        {
            var list = new List<int>(new[] { 1, 2, 3 }) as IList;
            Assert.Throws<ArgumentNullException>(() => list.CopyTo(null, 0));

            var deque = new Deque<int>(new[] { 1, 2, 3 }) as IList;
            Assert.Throws<ArgumentNullException>(() => deque.CopyTo(null, 0));
        }

        [Fact]
        public void NongenericCopyTo_NegativeOffset_ActsLikeList()
        {
            var destination = new int[3];
            var list = new List<int>(new[] { 1, 2, 3 }) as IList;
            Assert.Throws<ArgumentOutOfRangeException>(() => list.CopyTo(destination, -1));

            var deque = new Deque<int>(new[] { 1, 2, 3 }) as IList;
            Assert.Throws<ArgumentOutOfRangeException>(() => deque.CopyTo(destination, -1));
        }

        [Fact]
        public void NongenericCopyTo_InsufficientSpace_ActsLikeList()
        {
            var destination = new int[3];
            var list = new List<int>(new[] { 1, 2, 3 }) as IList;
            Assert.Throws<ArgumentException>(() => list.CopyTo(destination, 1));

            var deque = new Deque<int>(new[] { 1, 2, 3 }) as IList;
            Assert.Throws<ArgumentException>(() => deque.CopyTo(destination, 1));
        }

        [Fact]
        public void NongenericCopyTo_WrongType_ActsLikeList()
        {
            var destination = new IList[3];
            var list = new List<int>(new[] { 1, 2, 3 }) as IList;
            Assert.Throws<ArgumentException>(() => list.CopyTo(destination, 0));

            var deque = new Deque<int>(new[] { 1, 2, 3 }) as IList;
            Assert.Throws<ArgumentException>(() => deque.CopyTo(destination, 0));
        }

        [Fact]
        public void NongenericCopyTo_MultidimensionalArray_ActsLikeList()
        {
            var destination = new int[3, 3];
            var list = new List<int>(new[] { 1, 2, 3 }) as IList;
            Assert.Throws<ArgumentException>(() => list.CopyTo(destination, 0));

            var deque = new Deque<int>(new[] { 1, 2, 3 }) as IList;
            Assert.Throws<ArgumentException>(() => deque.CopyTo(destination, 0));
        }

        [Fact]
        public void NongenericAdd_WrongType_ActsLikeList()
        {
            var list = new List<int>() as IList;
            Assert.Throws<ArgumentException>("value", () => list.Add(this));

            var deque = new Deque<int>() as IList;
            Assert.Throws<ArgumentException>("value", () => deque.Add(this));
        }

        [Fact]
        public void NongenericNullableType_AllowsInsertingNull()
        {
            var deque = new Deque<int?>();
            var list = deque as IList;
            var result = list.Add(null);
            Assert.Equal(0, result);
            Assert.Equal(new int?[] { null }, deque);
        }

        [Fact]
        public void NongenericClassType_AllowsInsertingNull()
        {
            var deque = new Deque<object>();
            var list = deque as IList;
            var result = list.Add(null);
            Assert.Equal(0, result);
            Assert.Equal(new object[] { null }, deque);
        }

        [Fact]
        public void NongenericStruct_AddNull_ActsLikeList()
        {
            var list = new List<int>() as IList;
            Assert.Throws<ArgumentNullException>(() => list.Add(null));

            var deque = new Deque<int>() as IList;
            Assert.Throws<ArgumentNullException>(() => deque.Add(null));
        }

        [Fact]
        public void NongenericStruct_InsertNull_ActsMostlyLikeList()
        {
            var list = new List<int>() as IList;
            Assert.Throws<ArgumentNullException>("item", () => list.Insert(0, null)); 

            var deque = new Deque<int>() as IList;
            Assert.Throws<ArgumentNullException>("value", () => deque.Insert(0, null));
        }

        [Fact]
        public void NongenericRemove_RemovesItem()
        {
            var deque = new Deque<int>(new[] { 13 });
            var list = deque as IList;
            list.Remove(13);
            Assert.Equal(new int[] { }, deque);
        }

        [Fact]
        public void NongenericRemove_WrongType_DoesNothing()
        {
            var list = new List<int>(new[] { 13 }) as IList;
            list.Remove(this);
            list.Remove(null);
            Assert.Equal(1, list.Count);

            var deque = new Deque<int>(new[] { 13 }) as IList;
            deque.Remove(this);
            deque.Remove(null);
            Assert.Equal(1, deque.Count);
        }

        [Fact]
        public void NongenericGet_GetsItem()
        {
            var deque = new Deque<int>(new[] { 13 }) as IList;
            var value = (int)deque[0];
            Assert.Equal(13, value);
        }

        [Fact]
        public void NongenericSet_SetsItem()
        {
            var deque = new Deque<int>(new[] { 13 });
            var list = deque as IList;
            list[0] = 7;
            Assert.Equal(new[] { 7 }, deque);
        }

        [Fact]
        public void NongenericStruct_SetNull_ActsLikeList()
        {
            var list = new List<int>(new[] { 13 }) as IList;
            Assert.Throws<ArgumentNullException>("value", () => { list[0] = null; });

            var deque = new Deque<int>(new[] { 13 }) as IList;
            Assert.Throws<ArgumentNullException>("value", () => { deque[0] = null; });
        }

        #endregion
    }
}