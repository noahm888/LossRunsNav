using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DebugUtils;
using System.Threading.Tasks;
using LossRunsNavServer.Models;

namespace LossRunsNavServer.App_Code
{
    public class Logging
    {
        ErrorLogger logger;
        LogType type;

        public Logging()
        {
            logger = new ErrorLogger();
        }

        public Logging(string logName)
        {
            logger = new ErrorLogger(logName);
        }

        public Logging(LogType setType)
        {
            type = setType;

            switch (type)
            {
                case LogType.Update:
                    logger = new ErrorLogger("IncBatchUpdate");
                    break;
                case LogType.Add:
                    logger = new ErrorLogger("BatchAdditions");
                    break;
                case LogType.Exceptions:
                    logger = new ErrorLogger("Exceptions");
                    break;
                case LogType.Request:
                    logger = new ErrorLogger("Request");
                    break;
                case LogType.Results:
                    logger = new ErrorLogger("Results");
                    break;
                default:
                    logger = new ErrorLogger();
                    break;
            }
        }

        private async Task LogBatchResult(Type instance, BatchData batch, string message, BatchManager.BatchResult result)
        {
            if (type == LogType.Results)
            {
                if (batch == null)
                {
                    batch = new BatchData();
                    batch.Id = -1;
                    batch.Key = "Null Batch";
                    batch.isAvail = false;
                    batch.isListed = true;
                    batch.LastComplete = 500;
                }

                if (message == null)
                {
                    logger.AddLine(string.Format("Class {0}:", instance.ReflectedType.ToString()));
                    logger.AddLine(string.Format("Batch {0} Result: {1}", batch.Key, result));
                }
                else
                {
                    logger.AddLine(string.Format("Class {0}:", instance.ReflectedType.ToString()));
                    logger.AddLine(string.Format(message + "Result: {1}", batch.Key, result));
                }
            }
            else
            {
                var exLogger = new Logging(LogType.Exceptions);
                Exception ex = new Exception(exceptionMessage(LogType.Request));
                exLogger.LogException(typeof(Logging), ex, false, true);
                throw ex;
            }
        }

        private async Task LogBatchResult(Type instance, BatchData batch, string message, BatchManager.BatchResult result, bool autoWrite)
        {
            await LogBatchResult(instance, batch, message, result);

            if (autoWrite)
                Write();
        }

        private async Task LogBatchRequest(BatchData batch, string message, BatchManager.BatchResult result)
        {
            if (type == LogType.Request)
            {
                if (message == null)
                    logger.AddLine(string.Format("Batch {0} Requested. Result: {1}", batch.Key, result));
                else
                    logger.AddLine(string.Format(message + "Result: {1}", batch.Key, result));
            }
            else
            {
                var exLogger = new Logging(LogType.Exceptions);
                Exception ex = new Exception(exceptionMessage(LogType.Request));
                exLogger.LogException(typeof(Logging), ex, false, true);
                throw ex;
            }
        }

        private async Task LogBatchRequest(BatchData batch, string message, BatchManager.BatchResult result, bool autoWrite)
        {
            await LogBatchRequest(batch, message, result);

            if (autoWrite)
                Write();
        }

        private async Task LogBatchAdd(BatchData batch, string message)
        {
            if (type == LogType.Add)
            {
                string status;
                if (batch.isAvail)
                    status = "Availablle";
                else
                    status = "Not Availablle";

                if (message == null)
                        logger.AddLine(string.Format("Batch {0} Added. Status: {1}", batch.Key, status));
                    else
                        logger.AddLine(string.Format(message + "Status: {1}", batch.Key, status));
            }
            else
            { 
                var exLogger = new Logging(LogType.Exceptions);
                Exception ex = new Exception(exceptionMessage(LogType.Add));
                exLogger.LogException(typeof(Logging), ex, false, true);
                throw ex;
            }
        }

        private async Task LogBatchAdd(BatchData batch, string message, bool autoWrite)
        {
            await LogBatchAdd(batch, message);

            if (autoWrite)
                Write();
        }

        private async Task LogBatchUpdate(BatchData batch, string message)
        {
            if (type == LogType.Update)
            {
                if (batch.isAvail)
                {
                    if (message == null)
                        logger.AddLine(string.Format("Batch {0} Incomplete. Last index {1}", batch.Key, batch.LastComplete));
                    else
                        logger.AddLine(string.Format(message, batch.Key, batch.LastComplete));
                }
                else
                    logger.AddLine(string.Format("Batch {0} complete", batch.Key));
            }
            else
            {
                var exLogger = new Logging(LogType.Exceptions);
                Exception ex = new Exception(exceptionMessage(LogType.Update));
                exLogger.LogException(typeof(Logging), ex, false, true);
                throw ex;
            }
        }

        private async Task LogBatchUpdate(BatchData batch, string message, bool autoWrite)
        {
            await LogBatchUpdate(batch, message);

            if (autoWrite)
                Write();
        }

        private async Task LogException(Type instance, Exception ex, bool isCatch)
        {
            if (type == LogType.Exceptions)
            {
                if (isCatch)
                {
                    logger.AddLine(string.Format("Exception Caught in class {0}:", instance.ReflectedType.ToString()));
                    logger.AddLine(string.Format("Message: {0}", ex.Message));
                }
                else
                {
                    logger.AddLine(string.Format("Exception in class {0}:", instance.ReflectedType.ToString()));
                    logger.AddLine(string.Format("Message: {0}", ex.Message));
                    logger.AddLine(string.Format("Passed to caller"));
                }
            }
            else
            {
                var exLogger = new Logging(LogType.Exceptions);
                exLogger.LogException(typeof(Logging), ex, false, true);
                throw ex;
            }
        }

        private async Task LogException(Type instance, Exception ex, bool isCatch, bool autoWrite)
        {
            await LogException(instance, ex, isCatch);

            if (autoWrite)
                Write();
        }

        /// <summary>
        /// Default Logger. Logs timestamp
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public async Task Log()
        {
            logger.Write("");
        }

        /// <summary>
        /// Default Logger. Logs typed message
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public async Task Log(string message)
        {
             logger.Write(message);
        }

        /// <summary>
        /// Default Logger. Logs typed message w/ autoWrite option
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public async Task Log(string message, bool autoWrite)
        {
            if (autoWrite)
                logger.Write(message);
            else
                logger.AddLine(message);
        }

        /// <summary>
        /// Result Log w/o autoWrite
        /// </summary>
        /// <param name="ins"></param>
        /// <param name="batch"></param>
        /// <param name="message"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public async Task Log(Type ins, BatchData batch, string message, BatchManager.BatchResult result)
        {
            // Result Log
            Log(ins, batch, message, null, result, false, false);
        }

        /// <summary>
        /// Request Log w/o autoWrite
        /// </summary>
        /// <param name="batch"></param>
        /// <param name="message"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public async Task Log(BatchData batch, string message, BatchManager.BatchResult result)
        {
            // Request Log 
            Log(null, batch, message, null, result, false, false);
        }

        /// <summary>
        /// Exception Log w/o autoWrite
        /// </summary>
        /// <param name="ins"></param>
        /// <param name="ex"></param>
        /// <param name="isTrue"></param>
        /// <returns></returns>
        public async Task Log(Type ins, Exception ex, bool isTrue)
        {
            // Exception Log w/o autoWrite
            Log(ins, null, null, ex, BatchManager.BatchResult.NullResult, isTrue, false);
        }

        /// <summary>
        ///  Add or Update Log w/o autoWrite. Auto handles Add vs update.
        /// </summary>
        /// <param name="batch"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public async Task Log(BatchData batch, string message)
        {
            // Add or Update Log
            switch (type)
            {
                case LogType.Update:
                    Log(null, batch, message, null, BatchManager.BatchResult.NullResult, false, false);
                    break;
                case LogType.Add:
                    Log(null, batch, message, null, BatchManager.BatchResult.NullResult, false, false);
                    break;
                default:
                    logger.Write(message);
                    break;
            }
        }

        /// <summary>
        /// Result Log w/ Autowrite
        /// </summary>
        /// <param name="ins"></param>
        /// <param name="batch"></param>
        /// <param name="message"></param>
        /// <param name="result"></param>
        /// <param name="autoWrite"></param>
        /// <returns></returns>
        public async Task Log(Type ins, BatchData batch, string message, BatchManager.BatchResult result, bool autoWrite)
        {
            // Result Log w/ Autowrite
            Log(ins, batch, message,null, result, false, autoWrite);
        }

        /// <summary>
        /// Request Log w/ Autowrite
        /// </summary>
        /// <param name="batch"></param>
        /// <param name="message"></param>
        /// <param name="result"></param>
        /// <param name="autoWrite"></param>
        /// <returns></returns>
        public async Task Log(BatchData batch, string message, BatchManager.BatchResult result, bool autoWrite)
        {
            // Request Log w/ Autowrite
            Log(null, batch, message, null, result, false, autoWrite);
        }

        /// <summary>
        /// Exception Log w/ Autowrite
        /// </summary>
        /// <param name="ins"></param>
        /// <param name="ex"></param>
        /// <param name="isTrue"></param>
        /// <param name="autoWrite"></param>
        /// <returns></returns>
        public async Task Log(Type ins, Exception ex, bool isTrue, bool autoWrite)
        {
            // Exception Log w/ Autowrite
            Log(ins, null, null, ex, BatchManager.BatchResult.NullResult, isTrue, autoWrite);
        }

        /// <summary>
        /// Add or Update Log w/ Autowrite. Auto handles Add vs update.
        /// </summary>
        /// <param name="batch"></param>
        /// <param name="message"></param>
        /// <param name="autoWrite"></param>
        /// <returns></returns>
        public async Task Log(BatchData batch, string message, bool autoWrite)
        {
            // Add or Update Log w/ Autowrite
            switch (type)
            {
                case LogType.Update:
                    Log(null, batch, message, null, BatchManager.BatchResult.NullResult, false, autoWrite);
                    break;
                case LogType.Add:
                    Log(null, batch, message, null, BatchManager.BatchResult.NullResult, false, autoWrite);
                    break;
                default:
                    logger.Write(message);
                    break;
            }
        }

        /// <summary>
        /// Full Logger. Separates based on log type. Only use if neccessary
        /// </summary>
        /// <param name="ins"></param>
        /// <param name="batch"></param>
        /// <param name="message"></param>
        /// <param name="ex"></param>
        /// <param name="result"></param>
        /// <param name="isTrue"></param>
        /// <param name="autoWrite"></param>
        /// <returns></returns>
        public async Task Log(Type ins, BatchData batch, string message, Exception ex, BatchManager.BatchResult result, bool isTrue, bool autoWrite)
        {
            // Main Logger
            switch (type)
            {
                case LogType.Update:
                    LogBatchUpdate(batch, message, autoWrite);
                    break;
                case LogType.Add:
                    LogBatchAdd(batch, message, autoWrite);
                    break;
                case LogType.Exceptions:
                    LogException(ins, ex, isTrue, autoWrite);
                    break;
                case LogType.Request:
                    LogBatchRequest(batch, message, result, autoWrite);
                    break;
                case LogType.Results:
                    LogBatchResult(ins, batch, message, result, autoWrite);
                    break;
                default:
                    logger.Write(message);
                    break;
            }
        }

        public void Write()
        {
            logger.Write(true);
        }

        public string exceptionMessage(LogType corectType)
        {
            return string.Format("Wrong Method for type {0}. Use {1}", type.ToString(), corectType.ToString());
        }

        public enum LogType
        {
            Exceptions,
            Update,
            Add,
            Request,
            Results,
            OutLog
        }
    }
}