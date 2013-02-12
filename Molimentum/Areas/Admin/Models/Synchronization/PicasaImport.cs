using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Google.GData.Photos;
using Molimentum.Models;
using Raven.Client;
using Raven.Client.Linq;

namespace Molimentum.Areas.Admin.Models.Synchronization
{
    public class PicasaImport : IPicasaImport
    {
        private readonly IDocumentStore _documentStore;
        private readonly PicasaGateway _picasa;
        private Action<string> _logger;
        
        public PicasaImport(IDocumentStore documentStore)
        {
            _documentStore = documentStore;
            _picasa = new PicasaGateway();
            _logger = message => { };
        }

        public Action<string> Logger
        {
            get
            {
                return _logger;
            }
            set
            {
                if (value == null) throw new NullReferenceException("value");

                _logger = value;
            }
        }

        public void ImportPicasa()
        {
            var start = DateTime.Now;

            Logger("Starting import...");

            Parallel.ForEach(
                _picasa.ListAlbums().Where(albumEntry => IsToImport(albumEntry)),
                albumEntry =>
                {
                    using (var session = _documentStore.OpenSession())
                    {
                        var albumAccessor = new AlbumAccessor(albumEntry);

                        Logger(albumEntry.Title.Text);

                        var syncStatus = GetItemSyncStatus<Album>(session, albumAccessor.Id);

                        if (syncStatus.SourceETag != albumEntry.GetEtag())
                        {
                            var album = GetAlbum(session, albumAccessor.Id);
                            MapAlbumData(albumEntry, album);

                            syncStatus.SourceETag = albumEntry.GetEtag();
                        }

                        session.SaveChanges();
                    }
                });

            Logger(String.Format("Finished import, duration: {0}s", (DateTime.Now - start).TotalSeconds));
        }

        private bool IsToImport(PicasaEntry albumEntry)
        {
            var albumAccessor = new AlbumAccessor(albumEntry);

            return !albumAccessor.AlbumTitle.StartsWith("Scrapbook")
                && !albumAccessor.AlbumTitle.StartsWith("Profile");
        }

        private void MapAlbumData(PicasaEntry albumEntry, Album album)
        {
            var albumAccessor = new AlbumAccessor(albumEntry);

            album.Title = albumAccessor.AlbumTitle;
            album.Body = albumAccessor.AlbumSummary;
            album.DateTime = Convert.FromDateTime(albumEntry.Published).Value;
            album.ThumbnailLinks = CreateAlbumThumbnailLinks(albumEntry);
            album.Pictures = CreatePictures(albumAccessor.Id);

            album.Slug = album.Title.GenerateSlug();

            var firstPicture = album.Pictures.OrderBy(item => item.DateTime).FirstOrDefault();
            var lastPicture = album.Pictures.OrderByDescending(item => item.DateTime).FirstOrDefault();

            album.DateFrom = firstPicture == null ? (DateTimeOffset?)null : firstPicture.DateTime;
            album.DateTo = lastPicture == null ? (DateTimeOffset?)null : lastPicture.DateTime;
        }

        private IEnumerable<PictureLink> CreateAlbumThumbnailLinks(PicasaEntry albumEntry)
        {
            foreach (var thumbnail in albumEntry.Media.Thumbnails)
            {
                // work around picasa bug - height is not returned correctly
                using (var thumbStream = WebRequest.Create(thumbnail.Url).GetResponse().GetResponseStream())
                using (var image = Image.FromStream(thumbStream))
                {
                    yield return new PictureLink
                    {
                        Url = thumbnail.Url,
                        Width = image.Size.Width,
                        Height = image.Size.Height
                    };
                }
            }
        }

        private IEnumerable<Picture> CreatePictures(string albumId)
        {
            foreach (var photoEntry in _picasa.ListPhotos(albumId))
            {
                var photo = new PhotoAccessor(photoEntry);
                Logger("   " + photo.PhotoTitle);

                yield return new Picture
                {
                    Id = photo.Id,
                    Title = photo.PhotoTitle,
                    Body = photo.PhotoSummary,
                    DateTime = Convert.FromTimestamp(photo.Timestamp).Value,
                    Links = CreatePictureLinks(photoEntry),
                    Position = (photoEntry.Location != null)
                        ? new Position(photoEntry.Location.Latitude, photoEntry.Location.Longitude)
                        : null
                };
            }
        }

        private IEnumerable<PictureLink> CreatePictureLinks(PicasaEntry photoEntry)
        {
            return photoEntry.Media.Thumbnails.Select(
                thumbnail => new PictureLink
                {
                    Url = thumbnail.Url,
                    Width = Int32.Parse(thumbnail.Width),
                    Height = Int32.Parse(thumbnail.Height)
                });
        }

        private ItemSyncStatus<T> GetItemSyncStatus<T>(IDocumentSession session, string itemId)
        {
            var syncStatus = session.Query<ItemSyncStatus<T>>().FirstOrDefault(item => item.ItemId == itemId);

            if (syncStatus == null)
            {
                syncStatus = new ItemSyncStatus<T> { ItemId = itemId };
                session.Store(syncStatus);
            }

            return syncStatus;
        }

        private Album GetAlbum(IDocumentSession session, string albumId)
        {
            var album = session.Load<Album>(albumId);

            if (album == null)
            {
                album = new Album { Id = albumId };
                session.Store(album);
            }

            return album;
        }
    }
}
