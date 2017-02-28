using System;
using System.IO;
using System.Configuration;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Web;

using LossRunsNavServer.Models;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace LossRunsNavServer.App_Code
{
    public sealed class BatchManager
    {
        private static volatile BatchManager instance;
        private static object syncRoot = new Object();
        private BlockingCollection<BatchData> incomplete;
        private BlockingCollection<IncBatchData> incQueue;

        private string mainDirPath;
        private string mainPath;
        private string mainIncPath;
        private string mainListPath;
        private ThreadSafeList<BatchData> mainList;
        private readonly BlockingCollection<BatchData> master = new BlockingCollection<BatchData>();
        private int? totalBatches;

        Logging logger;
        Logging exLogger;

        private BatchManager()
        {
            logger = new Logging("BatchManager");
            exLogger = new Logging(Logging.LogType.Exceptions);
            mainDirPath = HttpContext.Current.Server.MapPath(".") + "\\Master\\";
            mainPath = mainDirPath + "BatchManager.json";
            mainIncPath = mainDirPath + "IncompleteManager.json";
            mainListPath = mainDirPath + "BatchList.json";

            if (!Directory.Exists(mainDirPath))
            {
                Directory.CreateDirectory(mainDirPath);
            }

            master = GetBatchManager(mainPath).Result;
            incomplete = GetBatchManager(mainIncPath).Result;

            mainList = GetBatchList(mainListPath).Result;
            incQueue = new BlockingCollection<IncBatchData>();
        }

        public static void StartManager()
        {
            BatchManager nothing;
            if (instance == null)
            {
                nothing = Instance;
                Instance.logger.Log("Batch Manager Started");
            }
            else
            {
                Instance.logger.Log("Batch Manager Already Started");
            }
        }

        async Task<BlockingCollection<BatchData>> GetBatchManager(string path)
        {
            BlockingCollection<BatchData> block;
            if (File.Exists(path))
            {
                using (StreamReader reader = new StreamReader(path))
                {
                    try
                    {
                        block = JsonConvert.DeserializeObject<BlockingCollection<BatchData>>(reader.ReadToEnd());
                        SetTotal(mainList.Count);
                        logger.Log("Batch Manager Loaded from Disk");
                    }
                    catch (Exception ex)
                    {
                        block = null;
                        exT(ex);
                    }
                    finally
                    {
                        reader.Close();
                    }
                }
            }
            else
            {
                block = new BlockingCollection<BatchData>();
                logger.Log("New Batch Manager created");
            }

            return block;
        }

        async Task<ThreadSafeList<BatchData>> GetBatchList(string path)
        {
            ThreadSafeList<BatchData> list;
            if (File.Exists(path))
            {
                using (StreamReader reader = new StreamReader(path))
                {
                    try
                    {
                        list = JsonConvert.DeserializeObject<ThreadSafeList<BatchData>>(reader.ReadToEnd());
                        logger.Log("Batch List loaded");
                    }
                    catch (Exception ex)
                    {
                        list = null;
                        exT(ex);
                    }
                    finally
                    {
                        reader.Close();
                    }
                }
            }
            else
            {
                list = new ThreadSafeList<BatchData>();
                logger.Log("New Batch List created");
            }

            return list;
        }

        async Task Save(string path)
        {
            using (StreamWriter writer = new StreamWriter(path, false))
            {
                try
                {
                    if (path == mainPath)
                        writer.Write(JsonConvert.SerializeObject(master));
                    if (path == mainIncPath)
                        writer.Write(JsonConvert.SerializeObject(incomplete));
                    if (path == mainListPath)
                        writer.Write(JsonConvert.SerializeObject(mainList));
                    logger.Log("Save of document at " + path + "Sucessful");
                }
                catch
                {
                    var ex = new Exception("Could not save");
                    exF(ex);
                    throw ex;
                }
                finally
                {
                    writer.Close();
                }
            }
        }

        public void SetTotal(int t)
        {
            if (totalBatches == null)
            {
                totalBatches = t;
                logger.Log("Total Batches Set: " + t.ToString());
            }
            else
            {
                logger.Log("Total Batches Not Set. Total still " + totalBatches.ToString());
            }
        }

        public async Task<int?> GetTotal()
        {
            return totalBatches;
        }

        public async Task<string> GetPath()
        {
            return mainPath;
        }

        public static BatchManager Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                            instance = new BatchManager();
                    }
                }

                return instance;
            }
        }

        public async Task<int> GetCount()
        {
            return (master != null) ? master.Count() : 0;
        }

        public async Task<List<BatchResult>> Add(BatchData b)
        {
            bool success = master.TryAdd(b);
            List<BatchResult> results = new List<BatchResult>();
            if (success)
            {
                if (!b.isListed)
                {
                    mainList.Add(b);
                    b.isListed = true;
                    try
                    {
                        await Save(mainListPath);
                        results.Add(BatchResult.ListSuccess);
                    }
                    catch
                    { results.Add(BatchResult.CouldNotSaveList); }
                }
                try
                {
                    await Save(mainPath);
                    results.Add(BatchResult.Success);
                }
                catch
                { results.Add(BatchResult.CouldNotSave); }

            }
            else
                results.Add(BatchResult.Failure);

            var resLogger = new Logging(Logging.LogType.Results);
            foreach (var res in results)
            {
                await resLogger.Log(typeof(BatchManager), b, null, res);
            }
            resLogger.Write();

            return results;

        }

        public async Task<BatchResult> AddInc(BatchData b)
        {
            var resLogger = new Logging(Logging.LogType.Results);
            bool success = incomplete.TryAdd(b);
            if (success)
            {
                try
                {
                    await Save(mainPath);
                    resLogger.Log(typeof(BatchManager), b, null, BatchResult.Success, true);
                }
                catch
                {
                    resLogger.Log(typeof(BatchManager), b, null, BatchResult.CouldNotSave, true);
                    return BatchResult.CouldNotSave;
                }
                return BatchResult.Success;
            }
            else
            {
                resLogger.Log(typeof(BatchManager), b, null, BatchResult.Failure, true);
                return BatchResult.Failure;
            }

        }

        public async Task<BatchData> Take()
        {
            var resLogger = new Logging(Logging.LogType.Results);
            BatchData value;
            bool success = master.TryTake(out value);
            if (success)
            {
                try
                {
                    await Save(mainPath);
                    resLogger.Log(typeof(BatchManager), value, "Batch Take Result: {0}", BatchResult.Success, true);
                }
                catch
                {
                    resLogger.Log(typeof(BatchManager), value, "Batch Take Result: {0}", BatchResult.CouldNotSave, true);
                    throw new Exception(BatchResult.CouldNotSave.GetType().ToString());
                }

                return value;
            }
            else
            {
                resLogger.Log(typeof(BatchManager), value, "Batch Take Result: {0}", BatchResult.Failure, true);
                return null;
            }
        }

        public async Task<BatchData> TakeInc()
        {
            var resLogger = new Logging(Logging.LogType.Results);
            BatchData value;
            bool success = incomplete.TryTake(out value);
            if (success)
            {
                try
                { await Save(mainPath); }
                catch
                {
                    resLogger.Log(typeof(BatchManager), value, "IncBatch Take Result: {0}", BatchResult.CouldNotSave, true);
                    throw new Exception(BatchResult.CouldNotSave.GetType().ToString());
                }
                resLogger.Log(typeof(BatchManager), value, "IncBatch Take Result: {0}", BatchResult.Success, true);
                return value;
            }
            else
            {
                resLogger.Log(typeof(BatchManager), value, "IncBatch Take Result: {0}", BatchResult.Failure, true);
                return null;
            }
        }

        public async Task<List<BatchResult>> UpdateInc(int id, bool isInc, int lastIndex)
        {
            BatchData b = null;
            List<BatchResult> results = new List<BatchResult>();
            var list = mainList.Clone();
            if (await listHasItem(id, b))
            {
                b.isAvail = isInc;
                b.LastComplete = lastIndex;
                try
                {
                    if (b.isAvail)
                    {
                        bool success = incomplete.TryAdd(b);
                        if (success)
                        {
                            try
                            { await Save(mainPath); }
                            catch
                            { results.Add(BatchResult.CouldNotSave); }

                             results.Add(BatchResult.Updated);
                        }
                        else
                            results.Add(BatchResult.Failure);
                    }
                    else
                        results.Add(BatchResult.Completed);
                }
                catch
                {
                    results.Add(BatchResult.NotAvail);
                }
            }
            return results;
        }

        private async Task<bool> listHasItem(int id, BatchData b)
        {
            foreach (var item in mainList.Clone())
            {
                if (item.Id == id)
                {
                    b = item;
                    return true;
                }
            }
            return false;
        } 

        public async Task<bool> AddToQueue(IncBatchData incBatch)
        {
            logger.Log(string.Format("IncBatch {0} Added to IncQueue", incBatch.Key));
            return incQueue.TryAdd(incBatch);
        }

        public async Task LoadQueue()
        {
            while(incQueue.Count > 0)
            {
                var item = incQueue.Take();
                try
                {
                    if (!incomplete.TryAdd(item.ConvertToParent()))
                        AddToQueue(item);
                }
                catch (Exception ex)
                {
                    // Log Exception
                    AddToQueue(item);
                }
            }
        }

        public enum BatchResult
        {
            Success,
            Failure,
            Updated,
            NotAvail,
            Completed,
            CouldNotSave,
            NoBatches,
            ListSuccess,
            CouldNotSaveList,
            NotListed,
            NullResult
        }

        private async Task exT(Exception ex)
        {
            exLogger.Log(typeof(BatchManager), ex, true, true);
        }

        private async Task exF(Exception ex)
        {
            exLogger.Log(typeof(BatchManager), ex, true, false);
        }
    }

    public class ThreadSafeList<T> : IList<T>
    {
        protected List<T> _interalList = new List<T>();

        // Other Elements of IList implementation

        public IEnumerator<T> GetEnumerator()
        {
            return Clone().GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return Clone().GetEnumerator();
        }

        protected static object _lock = new object();

        public int Count
        {
            get
            {
                return ((IList<T>)_interalList).Count;
            }
        }

        public bool IsReadOnly
        {
            get
            {
                return ((IList<T>)_interalList).IsReadOnly;
            }
        }

        public T this[int index]
        {
            get
            {
                return ((IList<T>)_interalList)[index];
            }

            set
            {
                ((IList<T>)_interalList)[index] = value;
            }
        }

        public List<T> Clone()
        {
            List<T> newList = new List<T>();

            lock (_lock)
            {
                _interalList.ForEach(x => newList.Add(x));
            }

            return newList;
        }

        public int IndexOf(T item)
        {
            return ((IList<T>)_interalList).IndexOf(item);
        }

        public void Insert(int index, T item)
        {
            ((IList<T>)_interalList).Insert(index, item);
        }

        public void RemoveAt(int index)
        {
            ((IList<T>)_interalList).RemoveAt(index);
        }

        public void Add(T item)
        {
            ((IList<T>)_interalList).Add(item);
        }

        public void Clear()
        {
            ((IList<T>)_interalList).Clear();
        }

        public bool Contains(T item)
        {
            return ((IList<T>)_interalList).Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            ((IList<T>)_interalList).CopyTo(array, arrayIndex);
        }

        public bool Remove(T item)
        {
            return ((IList<T>)_interalList).Remove(item);
        }

    }
}