using System;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using Molimentum.Areas.Admin.Models.Synchronization;

namespace Molimentum.Areas.Admin.Controllers
{
    public class PicasaImportController : AsyncController
    {
        private readonly IPicasaImport _picasaImport;

        public PicasaImportController(IPicasaImport picasaImport)
        {
            _picasaImport = picasaImport;
        }

        public ActionResult Index()
        {
            return View();
        }

        private static readonly object _importLock = new object();
        private static bool _importIsRunning = false;

        public Task<ActionResult> Import()
        {
            return Task.Factory.StartNew<ActionResult>(() =>
            {
                lock (_importLock)
                {
                    if (_importIsRunning) throw new InvalidOperationException("Import already running.");
                    _importIsRunning = true;
                }

                try
                {
                    _picasaImport.ImportPicasa();
                    return new HttpStatusCodeResult(HttpStatusCode.OK);
                }
                finally
                {
                    _importIsRunning = false;
                }
            });
        }
    }
}