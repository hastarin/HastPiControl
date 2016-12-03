// Code generated by Microsoft (R) AutoRest Code Generator 0.16.0.0
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.

namespace HastPiControl.Adafruit
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Rest;
    using Models;

    /// <summary>
    /// Extension methods for AdafruitIO.
    /// </summary>
    public static partial class AdafruitIOExtensions
    {
            /// <summary>
            /// All feeds for current user
            /// </summary>
            /// The Feeds endpoint returns information about the user's feeds. The
            /// response includes the latest value of each feed, and other metadata about
            /// each feed.
            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            public static IList<Feed> GetFeeds(this IAdafruitIO operations)
            {
                return Task.Factory.StartNew(s => ((IAdafruitIO)s).GetFeedsAsync(), operations, CancellationToken.None, TaskCreationOptions.None, TaskScheduler.Default).Unwrap().GetAwaiter().GetResult();
            }

            /// <summary>
            /// All feeds for current user
            /// </summary>
            /// The Feeds endpoint returns information about the user's feeds. The
            /// response includes the latest value of each feed, and other metadata about
            /// each feed.
            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            /// <param name='cancellationToken'>
            /// The cancellation token.
            /// </param>
            public static async Task<IList<Feed>> GetFeedsAsync(this IAdafruitIO operations, CancellationToken cancellationToken = default(CancellationToken))
            {
                using (var _result = await operations.GetFeedsWithHttpMessagesAsync(CustomHeaders, cancellationToken).ConfigureAwait(false))
                {
                    return _result.Body;
                }
            }

            /// <summary>
            /// Create a new Feed
            /// </summary>
            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            public static Feed CreateFeed(this IAdafruitIO operations)
            {
                return Task.Factory.StartNew(s => ((IAdafruitIO)s).CreateFeedAsync(), operations, CancellationToken.None, TaskCreationOptions.None, TaskScheduler.Default).Unwrap().GetAwaiter().GetResult();
            }

            /// <summary>
            /// Create a new Feed
            /// </summary>
            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            /// <param name='cancellationToken'>
            /// The cancellation token.
            /// </param>
            public static async Task<Feed> CreateFeedAsync(this IAdafruitIO operations, CancellationToken cancellationToken = default(CancellationToken))
            {
                using (var _result = await operations.CreateFeedWithHttpMessagesAsync(CustomHeaders, cancellationToken).ConfigureAwait(false))
                {
                    return _result.Body;
                }
            }

            /// <summary>
            /// Get feed by ID, Feed key, or Feed Name
            /// </summary>
            /// Returns feed based on ID
            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            /// <param name='id'>
            /// ID, key, or name of feed to use
            /// </param>
            public static Feed GetFeed(this IAdafruitIO operations, string id)
            {
                return Task.Factory.StartNew(s => ((IAdafruitIO)s).GetFeedAsync(id), operations, CancellationToken.None, TaskCreationOptions.None, TaskScheduler.Default).Unwrap().GetAwaiter().GetResult();
            }

            /// <summary>
            /// Get feed by ID, Feed key, or Feed Name
            /// </summary>
            /// Returns feed based on ID
            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            /// <param name='id'>
            /// ID, key, or name of feed to use
            /// </param>
            /// <param name='cancellationToken'>
            /// The cancellation token.
            /// </param>
            public static async Task<Feed> GetFeedAsync(this IAdafruitIO operations, string id, CancellationToken cancellationToken = default(CancellationToken))
            {
                using (var _result = await operations.GetFeedWithHttpMessagesAsync(id, CustomHeaders, cancellationToken).ConfigureAwait(false))
                {
                    return _result.Body;
                }
            }

            /// <summary>
            /// Replace an existing Feed
            /// </summary>
            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            /// <param name='id'>
            /// ID, key, or name of Feed
            /// </param>
            public static Feed ReplaceFeed(this IAdafruitIO operations, string id)
            {
                return Task.Factory.StartNew(s => ((IAdafruitIO)s).ReplaceFeedAsync(id), operations, CancellationToken.None, TaskCreationOptions.None, TaskScheduler.Default).Unwrap().GetAwaiter().GetResult();
            }

            /// <summary>
            /// Replace an existing Feed
            /// </summary>
            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            /// <param name='id'>
            /// ID, key, or name of Feed
            /// </param>
            /// <param name='cancellationToken'>
            /// The cancellation token.
            /// </param>
            public static async Task<Feed> ReplaceFeedAsync(this IAdafruitIO operations, string id, CancellationToken cancellationToken = default(CancellationToken))
            {
                using (var _result = await operations.ReplaceFeedWithHttpMessagesAsync(id, CustomHeaders, cancellationToken).ConfigureAwait(false))
                {
                    return _result.Body;
                }
            }

            /// <summary>
            /// Update properties of an existing Feed
            /// </summary>
            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            /// <param name='id'>
            /// ID, key, or name of feed to use
            /// </param>
            public static Feed UpdateFeed(this IAdafruitIO operations, string id)
            {
                return Task.Factory.StartNew(s => ((IAdafruitIO)s).UpdateFeedAsync(id), operations, CancellationToken.None, TaskCreationOptions.None, TaskScheduler.Default).Unwrap().GetAwaiter().GetResult();
            }

            /// <summary>
            /// Update properties of an existing Feed
            /// </summary>
            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            /// <param name='id'>
            /// ID, key, or name of feed to use
            /// </param>
            /// <param name='cancellationToken'>
            /// The cancellation token.
            /// </param>
            public static async Task<Feed> UpdateFeedAsync(this IAdafruitIO operations, string id, CancellationToken cancellationToken = default(CancellationToken))
            {
                using (var _result = await operations.UpdateFeedWithHttpMessagesAsync(id, CustomHeaders, cancellationToken).ConfigureAwait(false))
                {
                    return _result.Body;
                }
            }

            /// <summary>
            /// Delete an existing Feed
            /// </summary>
            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            /// <param name='id'>
            /// ID, key, or name of feed to use
            /// </param>
            public static string DestroyFeed(this IAdafruitIO operations, string id)
            {
                return Task.Factory.StartNew(s => ((IAdafruitIO)s).DestroyFeedAsync(id), operations, CancellationToken.None, TaskCreationOptions.None, TaskScheduler.Default).Unwrap().GetAwaiter().GetResult();
            }

            /// <summary>
            /// Delete an existing Feed
            /// </summary>
            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            /// <param name='id'>
            /// ID, key, or name of feed to use
            /// </param>
            /// <param name='cancellationToken'>
            /// The cancellation token.
            /// </param>
            public static async Task<string> DestroyFeedAsync(this IAdafruitIO operations, string id, CancellationToken cancellationToken = default(CancellationToken))
            {
                using (var _result = await operations.DestroyFeedWithHttpMessagesAsync(id, CustomHeaders, cancellationToken).ConfigureAwait(false))
                {
                    return _result.Body;
                }
            }

            /// <summary>
            /// All data for current feed
            /// </summary>
            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            /// <param name='feedId'>
            /// ID, key, or name of feed
            /// </param>
            /// <param name='startTime'>
            /// Start time for filtering data. Returns data created after given time.
            /// </param>
            /// <param name='endTime'>
            /// End time for filtering data. Returns data created before given time.
            /// </param>
            /// <param name='limit'>
            /// Limit the number of records returned.
            /// </param>
            public static IList<Data> AllFeedData(this IAdafruitIO operations, string feedId, string startTime = default(string), string endTime = default(string), int? limit = default(int?))
            {
                return Task.Factory.StartNew(s => ((IAdafruitIO)s).AllFeedDataAsync(feedId, startTime, endTime, limit), operations, CancellationToken.None, TaskCreationOptions.None, TaskScheduler.Default).Unwrap().GetAwaiter().GetResult();
            }

            /// <summary>
            /// All data for current feed
            /// </summary>
            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            /// <param name='feedId'>
            /// ID, key, or name of feed
            /// </param>
            /// <param name='startTime'>
            /// Start time for filtering data. Returns data created after given time.
            /// </param>
            /// <param name='endTime'>
            /// End time for filtering data. Returns data created before given time.
            /// </param>
            /// <param name='limit'>
            /// Limit the number of records returned.
            /// </param>
            /// <param name='cancellationToken'>
            /// The cancellation token.
            /// </param>
            public static async Task<IList<Data>> AllFeedDataAsync(this IAdafruitIO operations, string feedId, string startTime = default(string), string endTime = default(string), int? limit = default(int?), CancellationToken cancellationToken = default(CancellationToken))
            {
                using (var _result = await operations.AllFeedDataWithHttpMessagesAsync(feedId, startTime, endTime, limit, CustomHeaders, cancellationToken).ConfigureAwait(false))
                {
                    return _result.Body;
                }
            }

            /// <summary>
            /// Create new Data
            /// </summary>
            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            /// <param name='feedId'>
            /// ID, key, or name of feed
            /// </param>
            public static Data CreateData(this IAdafruitIO operations, string feedId)
            {
                return Task.Factory.StartNew(s => ((IAdafruitIO)s).CreateDataAsync(feedId), operations, CancellationToken.None, TaskCreationOptions.None, TaskScheduler.Default).Unwrap().GetAwaiter().GetResult();
            }

            /// <summary>
            /// Create new Data
            /// </summary>
            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            /// <param name='feedId'>
            /// ID, key, or name of feed
            /// </param>
            /// <param name='cancellationToken'>
            /// The cancellation token.
            /// </param>
            public static async Task<Data> CreateDataAsync(this IAdafruitIO operations, string feedId, CancellationToken cancellationToken = default(CancellationToken))
            {
                using (var _result = await operations.CreateDataWithHttpMessagesAsync(feedId, CustomHeaders, cancellationToken).ConfigureAwait(false))
                {
                    return _result.Body;
                }
            }

            /// <summary>
            /// Create new Data and Feed
            /// </summary>
            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            /// <param name='feedId'>
            /// ID, key, or name of feed
            /// </param>
            public static Data SendData(this IAdafruitIO operations, string feedId)
            {
                return Task.Factory.StartNew(s => ((IAdafruitIO)s).SendDataAsync(feedId), operations, CancellationToken.None, TaskCreationOptions.None, TaskScheduler.Default).Unwrap().GetAwaiter().GetResult();
            }

            /// <summary>
            /// Create new Data and Feed
            /// </summary>
            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            /// <param name='feedId'>
            /// ID, key, or name of feed
            /// </param>
            /// <param name='cancellationToken'>
            /// The cancellation token.
            /// </param>
            public static async Task<Data> SendDataAsync(this IAdafruitIO operations, string feedId, CancellationToken cancellationToken = default(CancellationToken))
            {
                using (var _result = await operations.SendDataWithHttpMessagesAsync(feedId, CustomHeaders, cancellationToken).ConfigureAwait(false))
                {
                    return _result.Body;
                }
            }

            /// <summary>
            /// Receive data?
            /// </summary>
            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            /// <param name='feedId'>
            /// ID, key, or name of feed
            /// </param>
            public static Data ReceiveData(this IAdafruitIO operations, string feedId)
            {
                return Task.Factory.StartNew(s => ((IAdafruitIO)s).ReceiveDataAsync(feedId), operations, CancellationToken.None, TaskCreationOptions.None, TaskScheduler.Default).Unwrap().GetAwaiter().GetResult();
            }

            /// <summary>
            /// Receive data?
            /// </summary>
            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            /// <param name='feedId'>
            /// ID, key, or name of feed
            /// </param>
            /// <param name='cancellationToken'>
            /// The cancellation token.
            /// </param>
            public static async Task<Data> ReceiveDataAsync(this IAdafruitIO operations, string feedId, CancellationToken cancellationToken = default(CancellationToken))
            {
                using (var _result = await operations.ReceiveDataWithHttpMessagesAsync(feedId, CustomHeaders, cancellationToken).ConfigureAwait(false))
                {
                    return _result.Body;
                }
            }

            /// <summary>
            /// Previous Data in Queue
            /// </summary>
            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            /// <param name='feedId'>
            /// ID, key, or name of feed
            /// </param>
            public static Data PreviousData(this IAdafruitIO operations, string feedId)
            {
                return Task.Factory.StartNew(s => ((IAdafruitIO)s).PreviousDataAsync(feedId), operations, CancellationToken.None, TaskCreationOptions.None, TaskScheduler.Default).Unwrap().GetAwaiter().GetResult();
            }

            /// <summary>
            /// Previous Data in Queue
            /// </summary>
            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            /// <param name='feedId'>
            /// ID, key, or name of feed
            /// </param>
            /// <param name='cancellationToken'>
            /// The cancellation token.
            /// </param>
            public static async Task<Data> PreviousDataAsync(this IAdafruitIO operations, string feedId, CancellationToken cancellationToken = default(CancellationToken))
            {
                using (var _result = await operations.PreviousDataWithHttpMessagesAsync(feedId, CustomHeaders, cancellationToken).ConfigureAwait(false))
                {
                    return _result.Body;
                }
            }

            /// <summary>
            /// Next Data in Queue
            /// </summary>
            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            /// <param name='feedId'>
            /// ID, key, or name of feed
            /// </param>
            public static Data NextData(this IAdafruitIO operations, string feedId)
            {
                return Task.Factory.StartNew(s => ((IAdafruitIO)s).NextDataAsync(feedId), operations, CancellationToken.None, TaskCreationOptions.None, TaskScheduler.Default).Unwrap().GetAwaiter().GetResult();
            }

            /// <summary>
            /// Next Data in Queue
            /// </summary>
            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            /// <param name='feedId'>
            /// ID, key, or name of feed
            /// </param>
            /// <param name='cancellationToken'>
            /// The cancellation token.
            /// </param>
            public static async Task<Data> NextDataAsync(this IAdafruitIO operations, string feedId, CancellationToken cancellationToken = default(CancellationToken))
            {
                using (var _result = await operations.NextDataWithHttpMessagesAsync(feedId, CustomHeaders, cancellationToken).ConfigureAwait(false))
                {
                    return _result.Body;
                }
            }

            /// <summary>
            /// Last Data in Queue
            /// </summary>
            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            /// <param name='feedId'>
            /// ID, key, or name of feed
            /// </param>
            public static Data LastData(this IAdafruitIO operations, string feedId)
            {
                return Task.Factory.StartNew(s => ((IAdafruitIO)s).LastDataAsync(feedId), operations, CancellationToken.None, TaskCreationOptions.None, TaskScheduler.Default).Unwrap().GetAwaiter().GetResult();
            }

            /// <summary>
            /// Last Data in Queue
            /// </summary>
            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            /// <param name='feedId'>
            /// ID, key, or name of feed
            /// </param>
            /// <param name='cancellationToken'>
            /// The cancellation token.
            /// </param>
            public static async Task<Data> LastDataAsync(this IAdafruitIO operations, string feedId, CancellationToken cancellationToken = default(CancellationToken))
            {
                using (var _result = await operations.LastDataWithHttpMessagesAsync(feedId, CustomHeaders, cancellationToken).ConfigureAwait(false))
                {
                    return _result.Body;
                }
            }

            /// <summary>
            /// Returns data based on ID
            /// </summary>
            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            /// <param name='feedId'>
            /// ID, key, or name of feed to use
            /// </param>
            /// <param name='id'>
            /// ID, key, or name of Data
            /// </param>
            public static Data GetData(this IAdafruitIO operations, string feedId, string id)
            {
                return Task.Factory.StartNew(s => ((IAdafruitIO)s).GetDataAsync(feedId, id), operations, CancellationToken.None, TaskCreationOptions.None, TaskScheduler.Default).Unwrap().GetAwaiter().GetResult();
            }

            /// <summary>
            /// Returns data based on ID
            /// </summary>
            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            /// <param name='feedId'>
            /// ID, key, or name of feed to use
            /// </param>
            /// <param name='id'>
            /// ID, key, or name of Data
            /// </param>
            /// <param name='cancellationToken'>
            /// The cancellation token.
            /// </param>
            public static async Task<Data> GetDataAsync(this IAdafruitIO operations, string feedId, string id, CancellationToken cancellationToken = default(CancellationToken))
            {
                using (var _result = await operations.GetDataWithHttpMessagesAsync(feedId, id, CustomHeaders, cancellationToken).ConfigureAwait(false))
                {
                    return _result.Body;
                }
            }

            /// <summary>
            /// Replace existing Data
            /// </summary>
            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            /// <param name='feedId'>
            /// ID, key, or name of feed
            /// </param>
            /// <param name='id'>
            /// ID, key, or name of Data
            /// </param>
            public static Data ReplaceData(this IAdafruitIO operations, string feedId, string id)
            {
                return Task.Factory.StartNew(s => ((IAdafruitIO)s).ReplaceDataAsync(feedId, id), operations, CancellationToken.None, TaskCreationOptions.None, TaskScheduler.Default).Unwrap().GetAwaiter().GetResult();
            }

            /// <summary>
            /// Replace existing Data
            /// </summary>
            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            /// <param name='feedId'>
            /// ID, key, or name of feed
            /// </param>
            /// <param name='id'>
            /// ID, key, or name of Data
            /// </param>
            /// <param name='cancellationToken'>
            /// The cancellation token.
            /// </param>
            public static async Task<Data> ReplaceDataAsync(this IAdafruitIO operations, string feedId, string id, CancellationToken cancellationToken = default(CancellationToken))
            {
                using (var _result = await operations.ReplaceDataWithHttpMessagesAsync(feedId, id, CustomHeaders, cancellationToken).ConfigureAwait(false))
                {
                    return _result.Body;
                }
            }

            /// <summary>
            /// Update properties of existing Data
            /// </summary>
            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            /// <param name='feedId'>
            /// ID, key, or name of feed
            /// </param>
            /// <param name='id'>
            /// ID, key, or name of Data
            /// </param>
            public static Data UpdateData(this IAdafruitIO operations, string feedId, string id)
            {
                return Task.Factory.StartNew(s => ((IAdafruitIO)s).UpdateDataAsync(feedId, id), operations, CancellationToken.None, TaskCreationOptions.None, TaskScheduler.Default).Unwrap().GetAwaiter().GetResult();
            }

            /// <summary>
            /// Update properties of existing Data
            /// </summary>
            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            /// <param name='feedId'>
            /// ID, key, or name of feed
            /// </param>
            /// <param name='id'>
            /// ID, key, or name of Data
            /// </param>
            /// <param name='cancellationToken'>
            /// The cancellation token.
            /// </param>
            public static async Task<Data> UpdateDataAsync(this IAdafruitIO operations, string feedId, string id, CancellationToken cancellationToken = default(CancellationToken))
            {
                using (var _result = await operations.UpdateDataWithHttpMessagesAsync(feedId, id, CustomHeaders, cancellationToken).ConfigureAwait(false))
                {
                    return _result.Body;
                }
            }

            /// <summary>
            /// Delete existing Data
            /// </summary>
            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            /// <param name='feedId'>
            /// ID, key, or name of feed
            /// </param>
            /// <param name='id'>
            /// ID, key, or name of Data
            /// </param>
            public static string DestroyData(this IAdafruitIO operations, string feedId, string id)
            {
                return Task.Factory.StartNew(s => ((IAdafruitIO)s).DestroyDataAsync(feedId, id), operations, CancellationToken.None, TaskCreationOptions.None, TaskScheduler.Default).Unwrap().GetAwaiter().GetResult();
            }

            /// <summary>
            /// Delete existing Data
            /// </summary>
            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            /// <param name='feedId'>
            /// ID, key, or name of feed
            /// </param>
            /// <param name='id'>
            /// ID, key, or name of Data
            /// </param>
            /// <param name='cancellationToken'>
            /// The cancellation token.
            /// </param>
            public static async Task<string> DestroyDataAsync(this IAdafruitIO operations, string feedId, string id, CancellationToken cancellationToken = default(CancellationToken))
            {
                using (var _result = await operations.DestroyDataWithHttpMessagesAsync(feedId, id, CustomHeaders, cancellationToken).ConfigureAwait(false))
                {
                    return _result.Body;
                }
            }

            /// <summary>
            /// All groups for current user
            /// </summary>
            /// The Groups endpoint returns information about the user's groups.
            /// The response includes the latest value of each feed in the group, and other
            /// metadata about the group.
            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            public static IList<Group> AllGroups(this IAdafruitIO operations)
            {
                return Task.Factory.StartNew(s => ((IAdafruitIO)s).AllGroupsAsync(), operations, CancellationToken.None, TaskCreationOptions.None, TaskScheduler.Default).Unwrap().GetAwaiter().GetResult();
            }

            /// <summary>
            /// All groups for current user
            /// </summary>
            /// The Groups endpoint returns information about the user's groups.
            /// The response includes the latest value of each feed in the group, and other
            /// metadata about the group.
            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            /// <param name='cancellationToken'>
            /// The cancellation token.
            /// </param>
            public static async Task<IList<Group>> AllGroupsAsync(this IAdafruitIO operations, CancellationToken cancellationToken = default(CancellationToken))
            {
                using (var _result = await operations.AllGroupsWithHttpMessagesAsync(CustomHeaders, cancellationToken).ConfigureAwait(false))
                {
                    return _result.Body;
                }
            }

            /// <summary>
            /// Create a new Group
            /// </summary>
            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            public static Group CreateGroup(this IAdafruitIO operations)
            {
                return Task.Factory.StartNew(s => ((IAdafruitIO)s).CreateGroupAsync(), operations, CancellationToken.None, TaskCreationOptions.None, TaskScheduler.Default).Unwrap().GetAwaiter().GetResult();
            }

            /// <summary>
            /// Create a new Group
            /// </summary>
            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            /// <param name='cancellationToken'>
            /// The cancellation token.
            /// </param>
            public static async Task<Group> CreateGroupAsync(this IAdafruitIO operations, CancellationToken cancellationToken = default(CancellationToken))
            {
                using (var _result = await operations.CreateGroupWithHttpMessagesAsync(CustomHeaders, cancellationToken).ConfigureAwait(false))
                {
                    return _result.Body;
                }
            }

            /// <summary>
            /// Returns Group based on ID
            /// </summary>
            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            /// <param name='id'>
            /// ID, key, or name of Group
            /// </param>
            public static Group GetGroup(this IAdafruitIO operations, string id)
            {
                return Task.Factory.StartNew(s => ((IAdafruitIO)s).GetGroupAsync(id), operations, CancellationToken.None, TaskCreationOptions.None, TaskScheduler.Default).Unwrap().GetAwaiter().GetResult();
            }

            /// <summary>
            /// Returns Group based on ID
            /// </summary>
            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            /// <param name='id'>
            /// ID, key, or name of Group
            /// </param>
            /// <param name='cancellationToken'>
            /// The cancellation token.
            /// </param>
            public static async Task<Group> GetGroupAsync(this IAdafruitIO operations, string id, CancellationToken cancellationToken = default(CancellationToken))
            {
                using (var _result = await operations.GetGroupWithHttpMessagesAsync(id, CustomHeaders, cancellationToken).ConfigureAwait(false))
                {
                    return _result.Body;
                }
            }

            /// <summary>
            /// Replace an existing Group
            /// </summary>
            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            /// <param name='id'>
            /// ID, key, or name of Group
            /// </param>
            public static Group ReplaceGroup(this IAdafruitIO operations, string id)
            {
                return Task.Factory.StartNew(s => ((IAdafruitIO)s).ReplaceGroupAsync(id), operations, CancellationToken.None, TaskCreationOptions.None, TaskScheduler.Default).Unwrap().GetAwaiter().GetResult();
            }

            /// <summary>
            /// Replace an existing Group
            /// </summary>
            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            /// <param name='id'>
            /// ID, key, or name of Group
            /// </param>
            /// <param name='cancellationToken'>
            /// The cancellation token.
            /// </param>
            public static async Task<Group> ReplaceGroupAsync(this IAdafruitIO operations, string id, CancellationToken cancellationToken = default(CancellationToken))
            {
                using (var _result = await operations.ReplaceGroupWithHttpMessagesAsync(id, CustomHeaders, cancellationToken).ConfigureAwait(false))
                {
                    return _result.Body;
                }
            }

            /// <summary>
            /// Update properties of an existing Group
            /// </summary>
            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            /// <param name='id'>
            /// ID, key, or name of Group
            /// </param>
            public static Group UpdateGroup(this IAdafruitIO operations, string id)
            {
                return Task.Factory.StartNew(s => ((IAdafruitIO)s).UpdateGroupAsync(id), operations, CancellationToken.None, TaskCreationOptions.None, TaskScheduler.Default).Unwrap().GetAwaiter().GetResult();
            }

            /// <summary>
            /// Update properties of an existing Group
            /// </summary>
            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            /// <param name='id'>
            /// ID, key, or name of Group
            /// </param>
            /// <param name='cancellationToken'>
            /// The cancellation token.
            /// </param>
            public static async Task<Group> UpdateGroupAsync(this IAdafruitIO operations, string id, CancellationToken cancellationToken = default(CancellationToken))
            {
                using (var _result = await operations.UpdateGroupWithHttpMessagesAsync(id, CustomHeaders, cancellationToken).ConfigureAwait(false))
                {
                    return _result.Body;
                }
            }

            /// <summary>
            /// Delete an existing Group
            /// </summary>
            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            /// <param name='id'>
            /// ID, key, or name of Group
            /// </param>
            public static string DestroyGroup(this IAdafruitIO operations, string id)
            {
                return Task.Factory.StartNew(s => ((IAdafruitIO)s).DestroyGroupAsync(id), operations, CancellationToken.None, TaskCreationOptions.None, TaskScheduler.Default).Unwrap().GetAwaiter().GetResult();
            }

            /// <summary>
            /// Delete an existing Group
            /// </summary>
            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            /// <param name='id'>
            /// ID, key, or name of Group
            /// </param>
            /// <param name='cancellationToken'>
            /// The cancellation token.
            /// </param>
            public static async Task<string> DestroyGroupAsync(this IAdafruitIO operations, string id, CancellationToken cancellationToken = default(CancellationToken))
            {
                using (var _result = await operations.DestroyGroupWithHttpMessagesAsync(id, CustomHeaders, cancellationToken).ConfigureAwait(false))
                {
                    return _result.Body;
                }
            }

    }
}
