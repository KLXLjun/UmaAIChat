using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public class AsyncThreadSafeQueue<T>
{
    private readonly Queue<T> queue = new Queue<T>();
    private readonly SemaphoreSlim queueLock = new SemaphoreSlim(1, 1);
    private readonly SemaphoreSlim itemAvailable = new SemaphoreSlim(0);
    private bool isClearing = false;

    // 异步读取（阻塞直到有数据）
    public async Task<T> DequeueAsync()
    {
        // 等待队列中有可用的项目
        await itemAvailable.WaitAsync();

        await queueLock.WaitAsync();
        try
        {
            // 如果正在清除，则等待
            while (isClearing)
            {
                await Task.Delay(10); // 等待短暂时间再检查
            }

            return queue.Dequeue();
        }
        finally
        {
            queueLock.Release();
        }
    }

    // 非阻塞读取：如果队列为空立即返回默认值
    public async Task<T> TryDequeueAsync()
    {
        await queueLock.WaitAsync();
        try
        {
            if (queue.Count == 0 || isClearing)
            {
                return default(T); // 如果队列为空或者正在清除，返回默认值（null 或默认值）
            }
            else
            {
                return queue.Dequeue();
            }
        }
        finally
        {
            queueLock.Release();
        }
    }

    // 异步入队操作
    public async Task EnqueueAsync(T item)
    {
        await queueLock.WaitAsync();
        try
        {
            queue.Enqueue(item);
        }
        finally
        {
            queueLock.Release();
            itemAvailable.Release(); // 通知有新项目可用
        }
    }

    // 异步清除并填入新数据
    public async Task ClearAndEnqueueRangeAsync(IEnumerable<T> newItems)
    {
        await queueLock.WaitAsync();
        try
        {
            isClearing = true;

            // 清除队列
            queue.Clear();

            // 填入新数据
            foreach (var item in newItems)
            {
                queue.Enqueue(item);
                itemAvailable.Release(); // 每插入一个新项目，通知等待的读取
            }

            isClearing = false;
        }
        finally
        {
            queueLock.Release();
        }
    }

    public async Task Clear()
    {
        await queueLock.WaitAsync();
        try
        {
            isClearing = true;

            // 清除队列
            queue.Clear();

            isClearing = false;
        }
        finally
        {
            queueLock.Release();
        }
    }

    // 获取队列大小
    public async Task<int> CountAsync()
    {
        await queueLock.WaitAsync();
        try
        {
            return queue.Count;
        }
        finally
        {
            queueLock.Release();
        }
    }
}
