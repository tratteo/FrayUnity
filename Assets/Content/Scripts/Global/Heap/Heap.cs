namespace Fray
{
    public class Heap<T> where T : IHeapItem<T>
    {
        private readonly T[] items;
        private readonly bool sortDownOnUpdate;
        private int currentItemCount;

        public int Count => currentItemCount;

        public Heap(int maxSize, bool sortDownOnUpdate = false)
        {
            items = new T[maxSize];
            this.sortDownOnUpdate = sortDownOnUpdate;
            currentItemCount = 0;
        }

        public void Update(T item)
        {
            SortUp(item);
            if (sortDownOnUpdate) SortDown(item);
        }

        public void Add(T item)
        {
            item.HeapIndex = currentItemCount;
            items[currentItemCount] = item;
            SortUp(item);
            currentItemCount++;
        }

        public T Peek()
        {
            T first = items[0];
            currentItemCount--;
            items[0] = items[currentItemCount];
            items[0].HeapIndex = 0;
            SortDown(items[0]);
            return first;
        }

        public bool Contains(T item) => Equals(items[item.HeapIndex], item);

        private void SortDown(T item)
        {
            while (true)
            {
                int leftChild = item.HeapIndex * 2 + 1;
                int rightChild = item.HeapIndex * 2 + 2;
                if (leftChild < currentItemCount)
                {
                    var swapIndex = leftChild;
                    if (rightChild < currentItemCount)
                    {
                        if (items[leftChild].CompareTo(items[rightChild]) < 0)
                        {
                            swapIndex = rightChild;
                        }
                    }
                    if (item.CompareTo(items[swapIndex]) < 0)
                    {
                        Swap(item, items[swapIndex]);
                    }
                    else return;
                }
                else return;
            }
        }

        private void Swap(T a, T b)
        {
            items[a.HeapIndex] = b;
            items[b.HeapIndex] = a;
            (b.HeapIndex, a.HeapIndex) = (a.HeapIndex, b.HeapIndex);
        }

        private void SortUp(T item)
        {
            int parentIndex = (item.HeapIndex - 1) / 2;
            while (true)
            {
                T parent = items[parentIndex];
                if (item.CompareTo(parent) > 0)
                {
                    Swap(item, parent);
                }
                else break;
                parentIndex = (item.HeapIndex - 1) / 2;
            }
        }
    }
}