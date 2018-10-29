using lazyimgservice.core.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;

namespace lazyimgservice.core.Controllers
{
    [Route("api/[controller]")]
    public class ImageController : Controller
    {
        public ImageController(EFContext db, IHostingEnvironment env) { this.db = db; this.env = env; }
        private readonly EFContext db;
        private readonly IHostingEnvironment env;

        /// <summary>
        /// Get a list of available image records
        /// </summary>
        /// <param name="searchQuery">A string used to search images by name or description</param>
        /// <param name="withTags">A list of tags to search for (results must contain one or more of the supplied tags)</param>
        /// <returns>A condensed list of results</returns>
        [HttpGet]
        public IEnumerable<SummaryResponse> Get(string searchQuery = null, string[] withTags = null)
        {
            List<ImageGroup> list;

            if (searchQuery != null)
                list = db.ImageGroups.Include(x => x.TagLinks).ThenInclude(l => l.Tag).Where(x => x.Active && (x.Name == searchQuery || x.Description.Contains(searchQuery))).ToList();
            else if (withTags != null && withTags.Length > 0)
            {                
                list = db.ImageGroups.Include(x=>x.TagLinks).ThenInclude(l=>l.Tag)
                    .Where(x => x.Active && x.TagLinks.Select(s=>s.Tag.Name).Intersect(withTags).Count() > 0)
                    .ToList();                
            }
            else
                list = db.ImageGroups.Include(x => x.TagLinks).ThenInclude(l => l.Tag).Where(x => x.Active).ToList();

            try
            {                
                try
                {
                    var results= list.Select(x => new SummaryResponse()
                    {
                        Id = x.Id,
                        Name = x.Name,
                        Description = x.Description,
                        Tags = x.TagLinks.Select(s=>s.Tag.Name).ToArray()
                    });
                    return results.ToList(); // materialize result set
                }
                catch (Exception ex1)
                {
                    Debugger.Break();
                    throw;
                }
                
            }
            catch (Exception ex)
            {
                Debugger.Break();
                throw;
            }
        }

        private string getImageUrl(ImageRecord img)
        {
            string baseUrl = Request.Scheme + "://" + (new Uri(Request.GetDisplayUrl())).Authority.TrimEnd('/') + "/filestorage/";
            return baseUrl + img.File.FilePath;
        }

        /// <summary>
        /// Get an image by id
        /// </summary>
        /// <param name="id">The id of the image record</param>
        /// <returns>An image record</returns>
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {

            var imgGroup = db.ImageGroups
                .Include(x => x.Images).ThenInclude(img=>img.File)
                .Include(x => x.TagLinks).ThenInclude(t=>t.Tag)
                .FirstOrDefault(x => x.Id == id);
            if (imgGroup == null) return NotFound();
            
            foreach (var img in imgGroup.Images)
            {
                img.Url = getImageUrl(img);
            }

            var imgSet = new ImageSet()
            {
                Id = imgGroup.Id,
                Active = imgGroup.Active,
                Description = imgGroup.Description,
                Name = imgGroup.Name,
                Updated = imgGroup.Updated,
                Images = imgGroup.Images,
                Image = imgGroup.Images.FirstOrDefault(x => x.ImageSize == ImageSize.src),
                Image_h2048 = imgGroup.Images.FirstOrDefault(x => x.ImageSize == ImageSize.h2048),
                Image_h1024 = imgGroup.Images.FirstOrDefault(x => x.ImageSize == ImageSize.h1024),
                Image_h720 = imgGroup.Images.FirstOrDefault(x => x.ImageSize == ImageSize.h720),
                Image_h480 = imgGroup.Images.FirstOrDefault(x => x.ImageSize == ImageSize.h480),
                Image_h240 = imgGroup.Images.FirstOrDefault(x => x.ImageSize == ImageSize.h240),
                Image_h128 = imgGroup.Images.FirstOrDefault(x => x.ImageSize == ImageSize.h128),
                Image_h64 = imgGroup.Images.FirstOrDefault(x => x.ImageSize == ImageSize.h64),
                Image_h32 = imgGroup.Images.FirstOrDefault(x => x.ImageSize == ImageSize.h32),
                Tags = imgGroup.TagLinks.Select(x => x.Tag.Name).ToArray()
            };                 
                
            return Ok(imgSet);
        }

        /// <summary>
        /// Delete image and all associated data and files
        /// </summary>
        /// <param name="id">The id of the image</param>
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            
            var entity = db.ImageGroups.Include(x=>x.Images).ThenInclude(img => img.File).FirstOrDefault(x=>x.Id==id);
            if (entity != null)
            {
                var fsroot = Path.Combine(env.WebRootPath, "filestorage");
                foreach (var image in entity.Images)
                {
                    if (image != null) { 
                        var file = image.File;
                        if (file != null)
                        {
                            foreach (var link in db.TagLinks.Where(x => x.FileId == entity.Id).ToList())
                            {
                                db.TagLinks.Remove(link);
                            }

                            var fullFilePath = Path.Combine(fsroot, file.FilePath);
                            System.IO.File.Delete(fullFilePath); // delete the physical file
                            db.Files.Remove(file);
                        }
                        db.Images.Remove(image);
                    }                    
                }

                foreach (var link in db.TagLinks.Where(x => x.ImageGroupId == entity.Id).ToList())
                {
                    db.TagLinks.Remove(link);
                }

                db.ImageGroups.Remove(entity);
                db.SaveChanges();
                return Ok();
            }
            else return NotFound();
        }

        /// <summary>
        /// Update an image record to add a description, tags, or other info
        /// </summary>
        /// <param name="id">The id of the image record</param>
        /// <param name="name">The name for the image record</param>
        /// <param name="description">A description for the image</param>
        /// <param name="tags">A list of tags to create/associate with the image</param>
        /// <returns>The Update image record</returns>
        [HttpPut("{id}")]
        public IActionResult Update(int id, string name = null, string description = null, string[] tags = null)
        {
            var entity = db.ImageGroups.Include(i => i.TagLinks).FirstOrDefault(x => x.Id == id);
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

                    foreach (var tag in tags)
                    {
                        var tagLink = new TagLink();
                        entity.TagLinks.Add(tagLink);

                        var existingTag = db.Tags.FirstOrDefault(x => x.Name == tag.ToLower());
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
                }

                db.SaveChanges();
                return Get(entity.Id);
            }
            else return NotFound();
        }
    }
}
