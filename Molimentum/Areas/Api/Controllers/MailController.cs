using System.Configuration;
using System.Net;
using System.Web.Mvc;
using Molimentum.Areas.Api.Models.Mails;
using Raven.Client;

namespace Molimentum.Areas.Api.Controllers
{
    public class MailController : Controller
    {
        private readonly IDocumentSession _session;
        
        public MailController(IDocumentSession session)
        {
            _session = session;
        }

        [HttpGet]
        public ActionResult Receive()
        {
            return View(new Mail());
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Receive(Mail mail)
        {
            var commandMail = CommandMail.Parse(mail);

            if (!commandMail.Fields.ContainsKey("KEY") ||
                commandMail.Fields["KEY"] != ConfigurationManager.AppSettings["MailKey"])
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);

            var commandMailProcessor = new CommandMailProcessor();
            commandMailProcessor.Process(_session, commandMail);

            return new HttpStatusCodeResult(HttpStatusCode.OK);
        }
    }
}
