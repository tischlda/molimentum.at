using System.Collections.Generic;
using Molimentum.Models;
using Raven.Client;

namespace Molimentum.Areas.Api.Models.Mails
{
    public class PositionReportProcessor : ProcessorBase<PositionReport>
    {
        protected override void Save(IDocumentSession session, IDictionary<string, string> data)
        {
            PositionReport positionReport;

            positionReport = data.ContainsKey("ID") ? session.Load<PositionReport>(data["ID"]) : new PositionReport();

            positionReport.Comment = data["COMMENT"];
            positionReport.DateTime = ParseDateTime(data["TIME"]);

            Position position;
            if (Position.TryParse(data["LATITUDE"], data["LONGITUDE"], out position))
            {
                positionReport.Position = position;
            }

            session.Store(positionReport);

            session.SaveChanges();
        }
    }
}