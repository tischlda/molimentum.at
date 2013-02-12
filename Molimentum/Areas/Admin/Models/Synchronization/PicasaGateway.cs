using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using Google.GData.Photos;

namespace Molimentum.Areas.Admin.Models.Synchronization
{
    public class PicasaGateway
    {
        private readonly string _albumThumbSizes;
        private readonly string _imageSizes;
        
        private readonly string _applicationName;
        private readonly string _userName;
        private readonly string _password;

        private readonly PicasaService _service;

        public PicasaGateway()
        {
            _albumThumbSizes = "160u,320,400";
            _imageSizes = "160u,320,640,800,1024,d";
            
            _applicationName = ConfigurationManager.AppSettings["Google.ApplicationName"];
            _userName = ConfigurationManager.AppSettings["Google.UserName"];
            _password = ConfigurationManager.AppSettings["Google.Password"];

            _service = new PicasaService(_applicationName);
            _service.setUserCredentials(_userName, _password);

        }

        public IEnumerable<PicasaEntry> ListAlbums()
        {
            var albumQuery = new AlbumQuery(PicasaQuery.CreatePicasaUri(_userName))
            {
                Thumbsize = _albumThumbSizes,
                NumberToRetrieve = Int32.MaxValue
            };
            var albumFeed = _service.Query(albumQuery);

            return albumFeed.Entries.Cast<PicasaEntry>();
        }

        public IEnumerable<PicasaEntry> ListPhotos(string albumId)
        {
            var photoQuery = new PhotoQuery(PicasaQuery.CreatePicasaUri(_userName, albumId))
            {
                Thumbsize = _imageSizes,
                NumberToRetrieve = Int32.MaxValue
            };
            var photoFeed = _service.Query(photoQuery);

            return photoFeed.Entries.Cast<PicasaEntry>();
        }
    }
}