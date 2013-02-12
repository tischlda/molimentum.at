using System;
using System.Threading;
using Raven.Client;

namespace Molimentum.Tests
{
    public static class Utils
    {
        private const int _defaultPollingInterval = 10;
        private const int _defaultTimeout = 10000;

        public static void WaitForNonStaleIndexes(this IDocumentSession session)
        {
            WaitForNonStaleIndexes(session, _defaultPollingInterval, _defaultTimeout);
        }

        public static void WaitForNonStaleIndexes(this IDocumentSession session, int millisecondsPollingInterval, int millisecondsTimeout)
        {
            var maxTime = DateTime.Now.AddMilliseconds(millisecondsTimeout);
            
            while (session.Advanced.DocumentStore.DatabaseCommands.GetStatistics().StaleIndexes.Length > 0)
            {
                if (DateTime.Now > maxTime) throw new TimeoutException();

                Thread.Sleep(millisecondsPollingInterval);
            }
        }
    }
}
