using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using lazyimgservice.core.Models;
using Microsoft.AspNetCore.Http.Extensions;
using System.IO;
using Microsoft.AspNetCore.Http;
using System.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;

namespace lazyimgservice.core.Controllers
{
    [Route("api/[controller]")]
    public class FileController : Controller
    {
        public FileController(EFContext db, IHostingEnvironment env) { this.db = db; this.env = env; }
        private readonly EFContext db;
        private readonly IHostingEnvironment env;

        /// <summary>
        /// Get a list of available file records containing information about the file and download urls
        /// </summary>
        /// <param name="searchQuery">A string used to search files by name or description</param>
        /// <param name="withTags">A list of tags to search for (results must contain one or more of the supplied tags)</param>
        /// <returns>A list of file results</returns>      
        [HttpGet]
        public IEnumerable<FileRecord> Get(string searchQuery = null, string[] withTags = null)
        {
            List<FileRecord> list;

            if (searchQuery != null)
                list = db.Files.Include(x => x.TagLinks).ThenInclude(l => l.Tag).Where(x => x.Name == searchQuery || x.Description.Contains(searchQuery)).ToList();
            else if (withTags != null && withTags.Length > 0) {
                list = db.Files.Include(x => x.TagLinks).ThenInclude(l => l.Tag)
                    .Where(x => x.TagLinks.Select(s => s.Tag.Name).Intersect(withTags).Count() > 0)
                    .ToList();                
            }
            else
                list = db.Files.Include(x => x.TagLinks).ThenInclude(l => l.Tag).ToList();

            string baseUrl = Request.Scheme + "://" + (new Uri(Request.GetDisplayUrl())).Authority.TrimEnd('/') + "/filestorage/";
            foreach (var file in list)
            {
                file.Url = baseUrl + file.FilePath;
                file.Tags = file.TagLinks.Select(x => x.Tag.Name).ToArray();
            }
            return list;            
        }

        /// <summary>
        /// Get a single file record containing information about the file and download urls
        /// </summary>
        /// <param name="id">The id of the file</param>
        /// <returns>A file record</returns>
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var file = db.Files.Include(x=>x.TagLinks).ThenInclude(t=>t.Tag).FirstOrDefault(w=>w.Id==id);
            string baseUrl = Request.Scheme + "://" + (new Uri(Request.GetDisplayUrl())).Authority.TrimEnd('/') + "/filestorage/";
            file.Url = baseUrl + file.FilePath;
            file.Tags = file.TagLinks.Select(x => x.Tag.Name).ToArray();
            return Ok(file);
        }
        
        /// <summary>
        /// Delete a file record and its associated data along with all copies and mirrors of the physical file
        /// </summary>
        /// <param name="id">The id of the file</param>
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var file = db.Files.Find(id);

            if (file != null)
            {
                var fsroot = Path.Combine(env.WebRootPath, "filestorage");
                var fullFilePath = Path.Combine(fsroot, file.FilePath);

                System.IO.File.Delete(fullFilePath); // delete the physical file

                foreach (var link in db.TagLinks.Where(x => x.FileId == file.Id).ToList())
                {
                    db.TagLinks.Remove(link);
                }

                var image = db.Images.FirstOrDefault(x => x.FileId == file.Id);
                if (image != null)
                {
                    db.Images.Remove(image);
                }
                db.Files.Remove(file);

                db.SaveChanges();
            }
            else return NotFound();

            return Ok();
        }

        /// <summary>
        /// Post a file or set of files
        /// </summary>
        /// <param name="files">Multipart form data</param>
        /// <returns>A list of new file records</returns>
        [HttpPost]
        public async Task<IActionResult> UploadFile(List<IFormFile> files)
        {
            List<int> ids = new List<int>();

            var path = Path.Combine(env.WebRootPath, "filestorage");
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);

            try
            {
                foreach (var formFile in files)
                {
                    if (formFile.Length > 0)
                    {

                        string fileNameSrc = formFile.FileName;
                        if (string.IsNullOrEmpty(fileNameSrc) || !fileNameSrc.Contains(".")) continue;
                        fileNameSrc = fileNameSrc.Replace("\"", string.Empty);

                        if (string.IsNullOrWhiteSpace(fileNameSrc)) fileNameSrc = Guid.NewGuid().ToString();
                        if (fileNameSrc.StartsWith("\"") && fileNameSrc.EndsWith("\""))
                        {
                            fileNameSrc = fileNameSrc.Trim('"');
                        }
                        if (fileNameSrc.Contains(@"/") || fileNameSrc.Contains(@"\"))
                        {
                            fileNameSrc = Path.GetFileName(fileNameSrc);
                        }

                        var fileExt = Path.GetExtension(fileNameSrc).ToLower().Substring(1);
                        var fileName = Path.GetFileNameWithoutExtension(fileNameSrc).ToLower();
                        var filePath = fileName + "." + fileExt;
                        var fullFilePath = Path.Combine(path, filePath);


                        using (var stream = new FileStream(fullFilePath, FileMode.Create))
                        {
                            await formFile.CopyToAsync(stream);
                        }

                        var fileInfo = new FileInfo(fullFilePath);
                        var sizekb = fileInfo.Length * 0.001M;

                        // store the file metadata
                        var record = new FileRecord();
                        record.Name = fileName;
                        record.FilePath = filePath;
                        record.FileExt = fileExt;
                        record.SizeKb = sizekb;
                        record.Key = fileName + '.' + fileExt;
                        record.FileType = (fileExt == "png" || fileExt == "gif" || fileExt == "jpg" || fileExt == "jpeg") ? FileType.Image : FileType.Unknown;
                        db.Files.Add(record);
                        db.SaveChanges();
                        var fileId = record.Id;

                        if (record.FileType == FileType.Image)
                        {
                            //return ImageSetService.Build(fileExt, fileName, fullFilePath, path, fileId, record.Key);
                            ids.Add(ImageSetService.Build(db, fileExt, fileName, fullFilePath, path, fileId, record.Key));
                        }
                        else
                        {
                            ids.Add(fileId);
                            //return fileId;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
            return Ok(ids); 
        }


        /// <summary>
        /// Update a file record to add a description, tags, or other info
        /// </summary>
        /// <param name="id">The id of the file record</param>        
        /// <param name="name">The name for the file</param>
        /// <param name="description">A description for the file</param>
        /// <param name="tags">A list of tags to create/associate with the file</param>
        /// <returns>The Updated file record</returns>
        [HttpPut("{id}")]
        public IActionResult Update(int id, string name = null, string description = null, string[] tags = null)
        {
            var entity = db.Files.Include(i=>i.TagLinks).FirstOrDefault(x=>x.Id==id);
            if (entity != null)
            {
                if (name != null) entity.Name = name;
                if (description != null) entity.Description = description;
                //if (info.Tags != null) entity.Tags = info.Tags;

                if (tags != null)
                {
                    foreach (var link in entity.TagLinks)
                    {
                        db.TagLinks.Remove(link);
                    }
                    entity.TagLinks.Clear();
                    
                    foreach(var tag in tags)
                    {
                        var tagLink = new TagLink();
                        entity.TagLinks.Add(tagLink);

                        var existingTag= db.Tags.FirstOrDefault(x => x.Name == tag.ToLower());
                        if (existingTag == null)
                        {
                            existingTag = new Tag() { Name = tag.ToLower() };
                            tagLink.Tag = existingTag;
                        }
                        else
                        {
                            tagLink.TagId = existingTag.Id;
                        }
                    }
                    db.SaveChanges();
                }

                db.SaveChanges();
                return Get(entity.Id);
            }
            else return NotFound();
        }
    }
}
