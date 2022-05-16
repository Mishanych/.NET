using System;

namespace OwnCollections
{
    class Program
    {
        static void Main(string[] args)
        {
            Deque<int> deque = new Deque<int>();
            Console.WriteLine(deque.IsEmpty);
            deque.AddToBack(32);
            deque.PrintDeque();

            deque.AddToBack(17);
            deque.PrintDeque();

            Console.WriteLine(deque.Count);
            Console.WriteLine(deque.IsEmpty);
            deque.AddToFront(99);
            deque.PrintDeque();

            deque.AddToFront(57);
            deque.PrintDeque();

            deque.RemoveFromBack();
            deque.PrintDeque();

            deque.RemoveFromFront();
            deque.PrintDeque();
        }
    }
}
