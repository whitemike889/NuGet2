﻿using System.Diagnostics;
namespace NuGet.Client
{
    public enum MessageLevel
    {
        Info,
        Warning,
        Debug,
        Error
    }

    public interface IExecutionLogger
    {
        void Log(MessageLevel level, string message, params object[] args);

        FileConflictAction ResolveFileConflict(string message);
    }

    public class NullExecutionLogger : IExecutionLogger
    {
        private static readonly IExecutionLogger _instance = new NullExecutionLogger();

        public static IExecutionLogger Instance
        {
            get
            {
                return _instance;
            }
        }

        public void Log(MessageLevel level, string message, params object[] args)
        {
        }

        public FileConflictAction ResolveFileConflict(string message)
        {
            return FileConflictAction.IgnoreAll;
        }
    }

    public class ShimLogger : ILogger
    {
        private IExecutionLogger _logger;

        public ShimLogger(IExecutionLogger logger)
        {
            _logger = logger;
        }

        public void Log(NuGet.MessageLevel level, string message, params object[] args)
        {
            _logger.Log((MessageLevel)(int)level, message, args);
        }

        public NuGet.FileConflictResolution ResolveFileConflict(string message)
        {
            var action = _logger.ResolveFileConflict(message);
            Debug.Assert(action != FileConflictAction.PromptUser);

            if (action == FileConflictAction.Overwrite ||
                action == FileConflictAction.OverwriteAll)
            {
                return FileConflictResolution.Overwrite;
            }
            else
            {
                return FileConflictResolution.Ignore;
            }
        }
    }
}
