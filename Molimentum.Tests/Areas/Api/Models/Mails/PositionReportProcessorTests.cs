using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Molimentum.Areas.Api.Models.Mails;
using Molimentum.Models;
using Raven.Client;
using Xunit;

namespace Molimentum.Tests.Areas.Api.Models.Mails
{
    public class PositionReportProcessorTests : IDisposable
    {
        private readonly IDocumentStore _documentStore;

        public PositionReportProcessorTests()
        {
            _documentStore = RavenDocumentStoreFactory.CreateInMemoryDocumentStore();
        }

        public void Dispose()
        {
            if (_documentStore != null) _documentStore.Dispose();
        }

        [Fact]
        public void NewPositionReportGetsStored()
        {
            using (var session = _documentStore.OpenSession())
            {
                var data = new Dictionary<string, string>
                {
                    { "KEY", "TestKey" },
                    { "TIME", "18.12.2011 19:06" },
                    { "LATITUDE", "42°00.00'N" },
                    { "LONGITUDE", "17°00.00'E" },
                    { "COMMENT", "TestComment" }
                };


                var positionReportProcessor = new PositionReportProcessor();
                positionReportProcessor.Process(session, data);


                var positionReports = session.Query<PositionReport>().Customize(c => c.WaitForNonStaleResults());

                positionReports.AsEnumerable().Should().HaveCount(1);
                var positionReport = positionReports.First();

                var expectedPositionReport = new PositionReport
                {
                    DateTime = new DateTimeOffset(2011, 12, 18, 19, 6, 0, new TimeSpan(0)),
                    Position = new Position(42, 17),
                    Comment = "TestComment"
                };

                positionReport.ShouldHave().AllPropertiesBut(p => p.Id).EqualTo(expectedPositionReport);
            }
        }

        [Fact]
        public void PositionReportUpdateGetStored()
        {
            var originalPositionReport = new PositionReport
            {
                DateTime = new DateTimeOffset(2011, 12, 18, 19, 5, 0, new TimeSpan(0)),
                Position = new Position(41, 16),
                Comment = "TestComment0"
            };

            using (var session = _documentStore.OpenSession())
            {
                session.Store(originalPositionReport);
                session.SaveChanges();
            }

            using (var session = _documentStore.OpenSession())
            {
                var data = new Dictionary<string, string>
                {
                    { "KEY", "TestKey" },
                    { "ID", originalPositionReport.Id },
                    { "TIME", "18.12.2011 19:06" },
                    { "LATITUDE", "42°00.00'N" },
                    { "LONGITUDE", "17°00.00'E" },
                    { "COMMENT", "TestComment" }
                };


                var positionReportProcessor = new PositionReportProcessor();
                positionReportProcessor.Process(session, data);


                var positionReports = session.Query<PositionReport>().Customize(c => c.WaitForNonStaleResults());

                positionReports.AsEnumerable().Should().HaveCount(1);
                var positionReport = positionReports.First();

                var expectedPositionReport = new PositionReport
                {
                    DateTime = new DateTimeOffset(2011, 12, 18, 19, 6, 0, new TimeSpan(0)),
                    Position = new Position(42, 17),
                    Comment = "TestComment"
                };

                positionReport.ShouldHave().AllPropertiesBut(p => p.Id).EqualTo(expectedPositionReport);
            }
        }

        [Fact]
        public void PositionReportGetsDeleted()
        {
            var originalPositionReport = new PositionReport
            {
                DateTime = new DateTimeOffset(2011, 12, 18, 19, 5, 0, new TimeSpan(0)),
                Position = new Position(41, 16),
                Comment = "TestComment0"
            };

            using (var session = _documentStore.OpenSession())
            {
                session.Store(originalPositionReport);
                session.SaveChanges();
            }

            using (var session = _documentStore.OpenSession())
            {
                var data = new Dictionary<string, string>
                {
                    { "KEY", "TestKey" },
                    { "ID", originalPositionReport.Id },
                    { "COMMAND", "DELETE" }
                };


                var positionReportProcessor = new PositionReportProcessor();
                positionReportProcessor.Process(session, data);


                var positionReports = session.Query<PositionReport>().Customize(c => c.WaitForNonStaleResults());

                positionReports.AsEnumerable().Should().HaveCount(0);
            }
        }
    }
}