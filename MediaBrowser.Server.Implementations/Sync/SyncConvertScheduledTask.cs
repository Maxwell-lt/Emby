﻿using MediaBrowser.Common.Configuration;
using MediaBrowser.Common.ScheduledTasks;
using MediaBrowser.Controller.Library;
using MediaBrowser.Controller.MediaEncoding;
using MediaBrowser.Controller.Sync;
using MediaBrowser.Controller.TV;
using MediaBrowser.Model.Logging;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CommonIO;
using MediaBrowser.Model.Tasks;

namespace MediaBrowser.Server.Implementations.Sync
{
    public class SyncConvertScheduledTask : IScheduledTask
    {
        private readonly ILibraryManager _libraryManager;
        private readonly ISyncRepository _syncRepo;
        private readonly ISyncManager _syncManager;
        private readonly ILogger _logger;
        private readonly IUserManager _userManager;
        private readonly ITVSeriesManager _tvSeriesManager;
        private readonly IMediaEncoder _mediaEncoder;
        private readonly ISubtitleEncoder _subtitleEncoder;
        private readonly IConfigurationManager _config;
        private readonly IFileSystem _fileSystem;
        private readonly IMediaSourceManager _mediaSourceManager;

        public SyncConvertScheduledTask(ILibraryManager libraryManager, ISyncRepository syncRepo, ISyncManager syncManager, ILogger logger, IUserManager userManager, ITVSeriesManager tvSeriesManager, IMediaEncoder mediaEncoder, ISubtitleEncoder subtitleEncoder, IConfigurationManager config, IFileSystem fileSystem, IMediaSourceManager mediaSourceManager)
        {
            _libraryManager = libraryManager;
            _syncRepo = syncRepo;
            _syncManager = syncManager;
            _logger = logger;
            _userManager = userManager;
            _tvSeriesManager = tvSeriesManager;
            _mediaEncoder = mediaEncoder;
            _subtitleEncoder = subtitleEncoder;
            _config = config;
            _fileSystem = fileSystem;
            _mediaSourceManager = mediaSourceManager;
        }

        public string Name
        {
            get { return "Convert media"; }
        }

        public string Description
        {
            get { return "Runs scheduled sync jobs"; }
        }

        public string Category
        {
            get
            {
                return "Sync";
            }
        }

        public Task Execute(CancellationToken cancellationToken, IProgress<double> progress)
        {
            return new SyncJobProcessor(_libraryManager, _syncRepo, (SyncManager)_syncManager, _logger, _userManager, _tvSeriesManager, _mediaEncoder, _subtitleEncoder, _config, _fileSystem, _mediaSourceManager)
                .Sync(progress, cancellationToken);
        }

        /// <summary>
        /// Creates the triggers that define when the task will run
        /// </summary>
        /// <returns>IEnumerable{BaseTaskTrigger}.</returns>
        public IEnumerable<TaskTriggerInfo> GetDefaultTriggers()
        {
            return new[] { 
            
                // Every so often
                new TaskTriggerInfo { Type = TaskTriggerInfo.TriggerInterval, IntervalTicks = TimeSpan.FromHours(3).Ticks}
            };
        }

        public string Key
        {
            get { return "SyncPrepare"; }
        }
    }
}
