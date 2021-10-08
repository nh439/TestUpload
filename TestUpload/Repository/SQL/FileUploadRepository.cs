using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TestUpload.Models.Entity;

namespace TestUpload.Repository.SQL
{
    public class FileUploadRepository
    {
        private readonly DataContext _context;
        public FileUploadRepository(DataContext context)
        {
            _context = context;
        }
        public async Task<int> Create(List<FileUpload> items)
        {
            _context.fileUploads.AddRange(items);
            return await _context.SaveChangesAsync();
        }
        public async Task<List<FileUpload>> Getall()
        {
            return await _context.fileUploads.OrderBy(x => x.AddDate).ToListAsync();
        }
        public async Task<List<FileUpload>> GetByUser(long user)
        {
            return await _context.fileUploads.Where(x=>x.UserId==user).ToListAsync();
        }
        public FileUpload GetFileByRef(string reference)
        {
            var data = _context.fileUploads.Where(x => x.Id == reference).FirstOrDefault();
            return data != null ? data : null;
        }
        public FileUpload GetFile(string reference)
        {
            var data = _context.fileUploads.Where(x => x.Id == reference && string.IsNullOrEmpty(x.pass)).FirstOrDefault();
            return data != null ? data : null;
        }
        public FileUpload GetFile(string reference,string password)
        {
            var data = _context.fileUploads.Where(x => x.Id == reference && x.pass==password).FirstOrDefault();
            return data != null ? data : null;
        }
        public int Remove(List<string> references)
        {
             foreach(var reference in references)
             {
              var  item =_context.fileUploads.FirstOrDefault();
                if (string.IsNullOrEmpty(item.pass))
                {
                    _context.fileUploads.Attach(item);
                    _context.fileUploads.Remove(item);
                }
             }
            return _context.SaveChanges();
        }
        public bool RemoveOne(string reference)
        {
            var item = _context.fileUploads.FirstOrDefault();
            
                _context.fileUploads.Attach(item);
                _context.fileUploads.Remove(item);
            
            return _context.SaveChanges() > 0 ? true : false;

        }
        public bool VerifyRemoved(string reference,string password)
        {
            var item = _context.fileUploads.Where(x => x.Id == reference && x.pass == password).FirstOrDefault();
            if(item != null)
            {
                _context.fileUploads.Attach(item);
                _context.fileUploads.Remove(item);
                _context.SaveChanges();
                return true;
            }
            return false;
        }
        public bool Update(FileUpload item)
        {
            if (string.IsNullOrEmpty(item.pass))
            {

                _context.Update(item);
                return _context.SaveChanges() > 0 ? true : false;
            }
            return false;
        }
        public bool VerifyUpdate(FileUpload item,string password)
        {
            if (item.pass == password)
            {
                _context.Update(item);
                return _context.SaveChanges() > 0 ? true : false;
            }
            return false;
        }
        public async Task<int> FormatAsync(long user)
        {           
            return await _context.Database.ExecuteSqlRawAsync(string.Format("delete from fileuploads where UserId={0}",user));
        }
    }
}
