// -----------------------------------------------------------------------
// <copyright file="Instance.cs" company="-">
// Copyright (c) 2013 larukedi (eser@sent.com). All rights reserved.
// </copyright>
// <author>larukedi (http://github.com/larukedi/)</author>
// -----------------------------------------------------------------------

//// This program is free software: you can redistribute it and/or modify
//// it under the terms of the GNU General Public License as published by
//// the Free Software Foundation, either version 3 of the License, or
//// (at your option) any later version.
//// 
//// This program is distributed in the hope that it will be useful,
//// but WITHOUT ANY WARRANTY; without even the implied warranty of
//// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//// GNU General Public License for more details.
////
//// You should have received a copy of the GNU General Public License
//// along with this program.  If not, see <http://www.gnu.org/licenses/>.

namespace Tasslehoff.Runner
{
    using System;
    using System.Globalization;
    using System.Threading;
    using Tasslehoff.Globals;
    using Tasslehoff.Library.Cron;
    using Tasslehoff.Library.DataAccess;
    using Tasslehoff.Library.Services;
    using Tasslehoff.Library.Utils;
    using Tasslehoff.Runner.Tasks;
    using Tasslehoff.Runner.Utils;

    /// <summary>
    /// Instance class.
    /// </summary>
    public class Instance : ServiceControllable
    {
        // fields

        /// <summary>
        /// The context
        /// </summary>
        private static Instance context = null;

        /// <summary>
        /// The configuration.
        /// </summary>
        private readonly InstanceConfig configuration;

        /// <summary>
        /// The database
        /// </summary>
        private readonly Database database;

        /// <summary>
        /// The cron manager
        /// </summary>
        private readonly CronManager cronManager;

        /// <summary>
        /// The message queue
        /// </summary>
        private RabbitMQConnection messageQueue = null;

        /// <summary>
        /// The cache
        /// </summary>
        private MemcachedConnection cache = null;

        // constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Instance"/> class.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        public Instance(InstanceConfig configuration) : base()
        {
            // singleton pattern
            if (Instance.context == null)
            {
                Instance.context = this;
            }

            this.configuration = configuration;
            Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo(configuration.Culture);

            this.database = new Database(this.configuration.DatabaseDriver, this.configuration.DatabaseConnectionString);

            RabbitMQConnection.Address = configuration.RabbitMQAddress;

            this.cronManager = new CronManager();
        }

        // properties

        /// <summary>
        /// Gets the context.
        /// </summary>
        /// <value>
        /// The context.
        /// </value>
        public static Instance Context
        {
            get
            {
                return Instance.context;
            }
        }

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public override string Name
        {
            get
            {
                return "Tasslehoff Runner";
            }
        }

        /// <summary>
        /// Gets the description.
        /// </summary>
        /// <value>
        /// The description.
        /// </value>
        public override string Description
        {
            get
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// Gets the configuration.
        /// </summary>
        /// <value>
        /// The configuration.
        /// </value>
        public InstanceConfig Configuration
        {
            get
            {
                return this.configuration;
            }
        }

        /// <summary>
        /// Gets the database.
        /// </summary>
        /// <value>
        /// The database.
        /// </value>
        public Database Database
        {
            get
            {
                return this.database;
            }
        }

        /// <summary>
        /// Gets the cron manager.
        /// </summary>
        /// <value>
        /// The cron manager.
        /// </value>
        public CronManager CronManager
        {
            get
            {
                return this.cronManager;
            }
        }

        /// <summary>
        /// Gets the message queue.
        /// </summary>
        /// <value>
        /// The message queue.
        /// </value>
        internal RabbitMQConnection MessageQueue
        {
            get
            {
                return this.messageQueue;
            }
        }

        /// <summary>
        /// Gets the cache.
        /// </summary>
        /// <value>
        /// The cache.
        /// </value>
        internal MemcachedConnection Cache
        {
            get
            {
                return this.cache;
            }
        }

        // methods

        /// <summary>
        /// Services the start.
        /// </summary>
        protected override void ServiceStart()
        {
            this.messageQueue = new RabbitMQConnection();

            string[] memcachedAddresses = !string.IsNullOrWhiteSpace(this.configuration.MemcachedAddresses) ? this.configuration.MemcachedAddresses.Split(',') : new string[0];
            this.cache = new MemcachedConnection(memcachedAddresses);

            CheckSourcesTask checkSourcesTask = new CheckSourcesTask();
            CronItem checkSourcesCronItem = new CronItem(Recurrence.Periodically(TimeSpan.FromSeconds(25)), checkSourcesTask.Do);
            this.cronManager.Add("checkSources", checkSourcesCronItem);

            FetchStoriesTask fetchStoriesTask = new FetchStoriesTask();
            CronItem fetchStoriesCronItem = new CronItem(Recurrence.Periodically(TimeSpan.FromSeconds(5)), fetchStoriesTask.Do, TimeSpan.FromSeconds(4));
            this.cronManager.Add("fetchStories", fetchStoriesCronItem);

            //TestTask testTask = new TestTask();
            //CronItem testCronItem = new CronItem(Recurrence.Periodically(TimeSpan.FromSeconds(10)), testTask.Do, TimeSpan.FromSeconds(5));
            //this.cronManager.Add("test", testCronItem);

            this.cronManager.Start();
        }

        /// <summary>
        /// Services the stop.
        /// </summary>
        protected override void ServiceStop()
        {
            this.cronManager.Stop();
            this.cronManager.Clear();

            VariableUtils.CheckAndDispose(this.cache);
            this.cache = null;

            VariableUtils.CheckAndDispose(this.messageQueue);
            this.messageQueue = null;
        }

        /// <summary>
        /// Called when [dispose].
        /// </summary>
        protected override void OnDispose()
        {
            base.OnDispose();

            VariableUtils.CheckAndDispose(this.cronManager);

            VariableUtils.CheckAndDispose(this.cache);
            this.cache = null;

            VariableUtils.CheckAndDispose(this.messageQueue);
            this.messageQueue = null;
        }
    }
}
