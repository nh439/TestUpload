using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TestUpload.Models.Entity;
using TestUpload.Models.View;

namespace TestUpload.Repository.SQL
{
    public class FileStorageRepository
    {
        private readonly DataContext _context;
        public FileStorageRepository(DataContext context)
        {
            _context = context;
        }
        public async Task<int> Create(List<FileStorage> items)
        {
            _context.fileStorage.AddRange(items);
            return await _context.SaveChangesAsync();
        }
        public async Task<List<FilestorageView>> Getall()
        {
            List<FilestorageView> views = new List<FilestorageView>();
            var data = await (from storage in _context.fileStorage
                        select new
                        {
                            storage.Id,
                            storage.Filename,
                            storage.FileExtension,
                            storage.FileSize,
                            storage.AddDate,
                            storage.pass,
                            storage.Comment,
                            storage.UserId,
                            storage.FileType
                        }
                      ).ToListAsync();
            foreach(var i in data)
            {
                views.Add(new FilestorageView
                {
                    AddDate = i.AddDate,
                    FileExtension = i.FileExtension,
                    Comment = i.Comment,
                    Filename = i.Filename,
                    FileSize = i.FileSize,
                    HasPassword = !string.IsNullOrEmpty(i.pass) ? true : false,
                    Id=i.Id,
                    UserId=i.UserId,
                    FileType=i.FileType

                });
            }
            return views;
        }
        public async Task<FilestorageView> GetById(string id)
        {
            var i = await (from storage in _context.fileStorage where storage.Id==id
                              select new
                              {
                                  storage.Id,
                                  storage.Filename,
                                  storage.FileExtension,
                                  storage.FileSize,
                                  storage.AddDate,
                                  storage.pass,
                                  storage.Comment,
                                  storage.UserId,
                                  storage.FileType
                              }
                     ).FirstOrDefaultAsync();
           var views = new FilestorageView
            {
                AddDate = i.AddDate,
                FileExtension = i.FileExtension,
                Comment = i.Comment,
                Filename = i.Filename,
                FileSize = i.FileSize,
                HasPassword = !string.IsNullOrEmpty(i.pass) ? true : false,
                Id = i.Id,
                UserId = i.UserId,
                FileType=i.FileType

            };
            return views;
        }
        public async Task<List<FilestorageView>> GetByUser(long user)
        {
            List<FilestorageView> views = new List<FilestorageView>();
            var data = await (from storage in _context.fileStorage where storage.UserId==user
                              select new
                              {
                                  storage.Id,
                                  storage.Filename,
                                  storage.FileExtension,
                                  storage.FileSize,
                                  storage.AddDate,
                                  storage.pass,
                                  storage.Comment,
                                  storage.UserId,
                                  storage.FileType
                              }
                      ).ToListAsync();
            foreach (var i in data)
            {
                views.Add(new FilestorageView
                {
                    AddDate = i.AddDate,
                    FileExtension = i.FileExtension,
                    Comment = i.Comment,
                    Filename = i.Filename,
                    FileSize = i.FileSize,
                    HasPassword = !string.IsNullOrEmpty(i.pass) ? true : false,
                    Id = i.Id,
                    UserId = i.UserId,
                    FileType = i.FileType

                });
            }
            return views;
        }
        public FileStorage GetFile(string reference)
        {
            var data = _context.fileStorage.Where(x => x.Id == reference && string.IsNullOrEmpty(x.pass)).FirstOrDefault();
            return data != null ? data : null;
        }
        public FileStorage GetFile(string reference, string password)
        {
            var data = _context.fileStorage.Where(x => x.Id == reference && x.pass == password).FirstOrDefault();
            return data != null ? data : null;
        }
        public int Remove(List<string> references)
        {
            foreach (var reference in references)
            {
                var item = _context.fileStorage.FirstOrDefault();
                if (string.IsNullOrEmpty(item.pass))
                {
                    _context.fileStorage.Attach(item);
                    _context.fileStorage.Remove(item);
                }
            }
            return _context.SaveChanges();
        }
        public bool RemoveOne(string reference)
        {
            var item = _context.fileStorage.FirstOrDefault();
            if (string.IsNullOrEmpty(item.pass))
            {
                _context.fileStorage.Attach(item);
                _context.fileStorage.Remove(item);
            }
            return _context.SaveChanges() > 0 ? true : false;

        }
        public bool VerifyRemoved(string reference, string password)
        {
            var item = _context.fileStorage.Where(x => x.Id == reference && x.pass == password).FirstOrDefault();
            if (item != null)
            {
                _context.fileStorage.Attach(item);
                _context.fileStorage.Remove(item);
                _context.SaveChanges();
                return true;
            }
            return false;
        }
        public bool Update(FileStorage item)
        {
            if (string.IsNullOrEmpty(item.pass))
            {

                _context.Update(item);
                return _context.SaveChanges() > 0 ? true : false;
            }
            return false;
        }
        public bool VerifyUpdate(FileStorage item, string password)
        {
            if (item.pass == password)
            {
                _context.Update(item);
                return _context.SaveChanges() > 0 ? true : false;
            }
            return false;
        }
    }
}
